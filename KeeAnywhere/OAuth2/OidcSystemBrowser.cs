using IdentityModel.OidcClient.Browser;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
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

        internal const string DefaultClosePageResponse =
    @"<html>
          <head><title>OAuth 2.0 Authentication Token Received</title></head>
          <body>
            Received verification code. You may now close this window.
            <script type='text/javascript'>
              // This doesn't work on every browser.
              window.setTimeout(function() {
                  this.focus();
                  window.opener = this;
                  window.open('', '_self', ''); 
                  window.close(); 
                }, 1000);
              //if (window.opener) { window.opener.checkToken(); }
            </script>
          </body>
        </html>";

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken)
        {
            using (var listener = _listener)
            {
                OpenBrowser(options.StartUrl);

                try
                {
                    var context = await listener.GetContextAsync();

                    string result;

                    if (options.ResponseMode == IdentityModel.OidcClient.OidcClientOptions.AuthorizeResponseMode.Redirect)
                    {
                        result = context.Request.Url.Query;
                    }
                    else
                    {
                        result = ProcessFormPost(context.Request);
                    }

                    await SendResponse(context.Response);


                    if (String.IsNullOrWhiteSpace(result))
                    {
                        return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = "Empty response." };
                    }

                    return new BrowserResult { Response = result, ResultType = BrowserResultType.Success };
                }
                catch (TaskCanceledException ex)
                {
                    return new BrowserResult { ResultType = BrowserResultType.Timeout, Error = ex.Message };
                }
                catch (Exception ex)
                {
                    return new BrowserResult { ResultType = BrowserResultType.UnknownError, Error = ex.Message };
                }
            }
        }

        public async Task<NameValueCollection> GetQueryStringAsync(string startUrl, CancellationToken cancellationToken)
        {
            using (var listener = _listener)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();


                    OpenBrowser(startUrl);

                    var context = await listener.GetContextAsync();

                    var result = context.Request.QueryString;

                    await SendResponse(context.Response);

                    return result;
                }
                catch (TaskCanceledException)
                {
                    return null;
                }
            }
        }

        private static async Task SendResponse(HttpListenerResponse response)
        {
            var buffer = Encoding.UTF8.GetBytes(DefaultClosePageResponse);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);
            responseOutput.Close();
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


