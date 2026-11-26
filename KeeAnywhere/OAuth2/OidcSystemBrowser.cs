using IdentityModel.OidcClient.Browser;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeeAnywhere.OAuth2
{
    public class OidcSystemBrowser : IBrowser
    {
        private readonly string _redirectUri;
        private readonly HttpListener _listener;

        public OidcSystemBrowser(IEnumerable<int> ports = null)
        {
            if (!CreateListener(out _redirectUri, out _listener, ports))
            {
                throw new Exception("No unused port found!");
            }
        }

        private static bool CreateListener(out string redirectUri, out HttpListener listener, IEnumerable<int> ports = null)
        {
            if (ports == null)
            {
                // IANA suggested range for dynamic or private ports
                //const int MinPort = 49215;
                //const int MaxPort = 65535;
                ports = Enumerable.Range(49215, 16321);
            }

            string[] hosts = { "127.0.0.1", "localhost" };
            foreach (var port in ports)
            {
                foreach (var host in hosts)
                {
                    redirectUri = CreateRedirectUri(host, port);
                    listener = new HttpListener();
                    listener.Prefixes.Add(redirectUri);
                    try
                    {
                        listener.Start();

                        return true;
                    }
                    catch
                    {
                        // nothing to do here -- the listener disposes itself when Start throws
                    }
                }
            }

            redirectUri = null;
            listener = null;

            return false;
        }

        private static string CreateRedirectUri(string host, int port)
        {
            return "http://" + host + ":" + port + "/";
        }

        public string RedirectUri
        {
            get
            {
                return _redirectUri;
            }
        }

        // Page shown in the browser after the redirect: {0} = title, {1} = message (both HTML encoded),
        // {2} = favicon tag, {3} = logo tag (images are embedded, nothing is loaded from external sources)
        private const string ResponsePageTemplate =
@"<!DOCTYPE html>
<html>
<head>
<meta charset=""utf-8"">
<meta name=""color-scheme"" content=""light dark"">
<title>KeeAnywhere - {0}</title>
{2}
</head>
<body style=""font-family: sans-serif; max-width: 36em; margin: 4em auto; padding: 0 1em"">
{3}
<h1>{0}</h1>
<p>{1}</p>
<p>You can close this window and return to KeePass.</p>
</body>
</html>";

        private static string CreateResponsePage(NameValueCollection query)
        {
            var error = query != null ? query["error"] : null;
            var code = query != null ? query["code"] : null;

            string title;
            string message;

            if (error == "access_denied")
            {
                title = "Access denied";
                message = "KeeAnywhere was not authorized - no account has been added.";
            }
            else if (!string.IsNullOrEmpty(error))
            {
                title = "Authorization failed";
                message = query["error_description"] ?? error;
            }
            else if (!string.IsNullOrEmpty(code))
            {
                title = "Authorization successful";
                message = "KeeAnywhere has been authorized.";
            }
            else
            {
                title = "Authorization failed";
                message = "No authorization code received.";
            }

            var favicon = GetPngDataUri(PluginResources.KeeAnywhere_16x16);
            var logo = GetPngDataUri(PluginResources.KeeAnywhere_48x48);

            return string.Format(ResponsePageTemplate,
                WebUtility.HtmlEncode(title),
                WebUtility.HtmlEncode(message),
                favicon != null ? "<link rel=\"icon\" type=\"image/png\" href=\"" + favicon + "\">" : "",
                logo != null ? "<img src=\"" + logo + "\" width=\"48\" height=\"48\" alt=\"KeeAnywhere\">" : "");
        }

        // Returns null on error: a missing image must never break the authorization
        private static string GetPngDataUri(Image image)
        {
            try
            {
                using (image)
                using (var stream = new MemoryStream())
                {
                    image.Save(stream, ImageFormat.Png);
                    return "data:image/png;base64," + Convert.ToBase64String(stream.ToArray());
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken)
        {
            using (var listener = _listener)
            using (cancellationToken.Register(StopListener, listener))
            {
                if (cancellationToken.IsCancellationRequested)
                    return new BrowserResult { ResultType = BrowserResultType.UserCancel, Error = "Cancelled by user." };

                OpenBrowser(options.StartUrl);

                try
                {
                    var context = await listener.GetContextAsync();

                    string result;

                    //if (options.ResponseMode == IdentityModel.OidcClient.OidcClientOptions.AuthorizeResponseMode.Redirect)
                    //{
                        result = context.Request.Url.Query;
                    //}
                    //else
                    //{
                    //    result = ProcessFormPost(context.Request);
                    //}

                    await SendResponse(context.Response, context.Request.QueryString);


                    if (String.IsNullOrWhiteSpace(result))
                    {
                        return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = "Empty response." };
                    }

                    return new BrowserResult { Response = result, ResultType = BrowserResultType.Success };
                }
                catch (Exception ex)
                {
                    // Cancelling stops the listener, so GetContextAsync fails
                    if (cancellationToken.IsCancellationRequested)
                        return new BrowserResult { ResultType = BrowserResultType.UserCancel, Error = "Cancelled by user." };

                    return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = ex.Message };
                }
            }
        }

        // GetContextAsync doesn't support a CancellationToken: stopping the listener ends the waiting
        private static void StopListener(object listener)
        {
            try
            {
                ((HttpListener)listener).Stop();
            }
            catch (Exception)
            {
                // Listener already closed
            }
        }

        public async Task<NameValueCollection> GetQueryStringAsync(string startUrl, CancellationToken cancellationToken)
        {
            using (var listener = _listener)
            using (cancellationToken.Register(StopListener, listener))
            {
                if (cancellationToken.IsCancellationRequested)
                    return null;

                OpenBrowser(startUrl);

                try
                {
                    var context = await listener.GetContextAsync();

                    var result = context.Request.QueryString;

                    await SendResponse(context.Response, result);

                    return result;
                }
                catch (Exception)
                {
                    // Cancelling stops the listener, so GetContextAsync fails
                    if (cancellationToken.IsCancellationRequested)
                        return null;

                    throw;
                }
            }
        }

        private static async Task SendResponse(HttpListenerResponse response, NameValueCollection query)
        {
            var buffer = Encoding.UTF8.GetBytes(CreateResponsePage(query));
            response.ContentType = "text/html; charset=utf-8";
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);
            responseOutput.Close();
            // Close the response explicitly, so it is completely sent to the browser
            // before the listener gets disposed.
            response.Close();
        }

        private static string ProcessFormPost(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }

            using (var body = request.InputStream)
            {
                using (var reader = new StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                //// hack because of this: https://github.com/dotnet/corefx/issues/10361
                //if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                //{
                //    url = url.Replace("&", "^&");
                //    Process.Start(new ProcessStartInfo("cmd", "/c start " + url) { CreateNoWindow = true });
                //}
                //else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                //{
                //    Process.Start("xdg-open", url);
                //}
                //else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                //{
                //    Process.Start("open", url);
                //}
                //else
                //{
                throw;
                //}
            }
        }
    }
}


