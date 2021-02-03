using IdentityModel.OidcClient.Browser;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KeeAnywhere.OAuth2
{
    public class SystemBrowser : IBrowser
    {
        public int Port { get; }
        private readonly string _path;

        public SystemBrowser(int? port = null, string path = null)
        {
            _path = path;

            if (!port.HasValue)
            {
                Port = GetRandomUnusedPort();
            }
            else
            {
                Port = port.Value;
            }
        }

        private int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public string RedirectUri
        {
            get
            {
                return $"http://127.0.0.1:{Port}/{_path}";
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
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add(RedirectUri);
                listener.Start();

                OpenBrowser(options.StartUrl);

                try
                {
                    var context = await listener.GetContextAsync();

                    var response = context.Response;
                    var buffer = Encoding.UTF8.GetBytes(DefaultClosePageResponse);
                    response.ContentLength64 = buffer.Length;
                    var responseOutput = response.OutputStream;
                    await responseOutput.WriteAsync(buffer, 0, buffer.Length);
                    responseOutput.Close();

                    //var result = GetRequestPostData(context.Request);
                    var result = context.Request.Url.Query;


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

        private static string GetRequestPostData(HttpListenerRequest request)
        {
            string code = null;

            foreach (var pair in request.Url.Query.TrimStart('?').Split('&'))
            {
                var elements = pair.Split('=');
                if (elements.Length != 2)
                {
                    continue;
                }

                switch (elements[0])
                {
                    case "code":
                        code = Uri.UnescapeDataString(elements[1]);
                        break;
                }
            }

            return code;

            //if (!request.HasEntityBody)
            //{
            //    return null;
            //}

            //using (var body = request.InputStream)
            //{
            //    using (var reader = new StreamReader(body, request.ContentEncoding))
            //    {
            //        return reader.ReadToEnd();
            //    }
            //}
        }

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }

    //    public class LoopbackHttpListener : IDisposable
    //    {
    //        internal const string DefaultClosePageResponse =
    //@"<html>
    //  <head><title>OAuth 2.0 Authentication Token Received</title></head>
    //  <body>
    //    Received verification code. You may now close this window.
    //    <script type='text/javascript'>
    //      // This doesn't work on every browser.
    //      window.setTimeout(function() {
    //          this.focus();
    //          window.opener = this;
    //          window.open('', '_self', ''); 
    //          window.close(); 
    //        }, 1000);
    //      //if (window.opener) { window.opener.checkToken(); }
    //    </script>
    //  </body>
    //</html>";



    //        private HttpListener StartListener()
    //        {
    //            try
    //            {
    //                var listener = new HttpListener();
    //                listener.Prefixes.Add(RedirectUri);
    //                listener.Start();
    //                return listener;
    //            }
    //            catch
    //            {
    //                CallbackUriChooser.Default.ReportFailure(_callbackUriTemplate);
    //                throw;
    //            }
    //        }


    //        private async Task HandleOAuth2Redirect(HttpListener http)
    //        {
    //            var context = await http.GetContextAsync();

    //            // We only care about request to RedirectUri endpoint.
    //            while (context.Request.Url.AbsolutePath != RedirectUri.AbsolutePath)
    //            {
    //                context = await http.GetContextAsync();
    //            }

    //            context.Response.ContentType = "text/html";

    //            // Respond with a page which runs JS and sends URL fragment as query string
    //            // to TokenRedirectUri.
    //            using (var file = File.OpenRead("index.html"))
    //            {
    //                file.CopyTo(context.Response.OutputStream);
    //            }

    //            context.Response.OutputStream.Close();
    //        }





    //        private async Task<AuthorizationCodeResponseUrl> GetResponseFromListener(HttpListener listener, CancellationToken ct)
    //        {
    //            HttpListenerContext context;
    //            // Set up cancellation. HttpListener.GetContextAsync() doesn't accept a cancellation token,
    //            // the HttpListener needs to be stopped which immediately aborts the GetContextAsync() call.
    //            using (ct.Register(listener.Stop))
    //            {
    //                // Wait to get the authorization code response.
    //                try
    //                {
    //                    context = await listener.GetContextAsync().ConfigureAwait(false);
    //                }
    //                catch (Exception) when (ct.IsCancellationRequested)
    //                {
    //                    ct.ThrowIfCancellationRequested();
    //                    // Next line will never be reached because cancellation will always have been requested in this catch block.
    //                    // But it's required to satisfy compiler.
    //                    throw new InvalidOperationException();
    //                }
    //                catch
    //                {
    //                    CallbackUriChooser.Default.ReportFailure(_callbackUriTemplate);
    //                    throw;
    //                }
    //                CallbackUriChooser.Default.ReportSuccess(_callbackUriTemplate);
    //            }
    //            NameValueCollection coll = context.Request.QueryString;

    //            // Write a "close" response.
    //            var bytes = Encoding.UTF8.GetBytes(_closePageResponse);
    //            context.Response.ContentLength64 = bytes.Length;
    //            context.Response.SendChunked = false;
    //            context.Response.KeepAlive = false;
    //            var output = context.Response.OutputStream;
    //            await output.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
    //            await output.FlushAsync().ConfigureAwait(false);
    //            output.Close();
    //            context.Response.Close();

    //            // Create a new response URL with a dictionary that contains all the response query parameters.
    //            return new AuthorizationCodeResponseUrl(coll.AllKeys.ToDictionary(k => k, k => coll[k]));
    //        }



    //        const int DefaultTimeout = 60 * 5; // 5 mins (in seconds)

    //        IWebHost _host;
    //        TaskCompletionSource<string> _source = new TaskCompletionSource<string>();
    //        string _url;

    //        public string Url => _url;




    //        public LoopbackHttpListener(int port, string path = null)
    //        {
    //            path = path ?? String.Empty;
    //            if (path.StartsWith("/")) path = path.Substring(1);

    //            _url = $"http://127.0.0.1:{port}/{path}";

    //            _host = new WebHostBuilder()
    //                .UseKestrel()
    //                .UseUrls(_url)
    //                .Configure(Configure)
    //                .Build();
    //            _host.Start();
    //        }

    //        public void Dispose()
    //        {
    //            Task.Run(async () =>
    //            {
    //                await Task.Delay(500);
    //                _host.Dispose();
    //            });
    //        }

    //        void Configure(IApplicationBuilder app)
    //        {
    //            app.Run(async ctx =>
    //            {
    //                if (ctx.Request.Method == "GET")
    //                {
    //                    SetResult(ctx.Request.QueryString.Value, ctx);
    //                }
    //                else if (ctx.Request.Method == "POST")
    //                {
    //                    if (!ctx.Request.ContentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
    //                    {
    //                        ctx.Response.StatusCode = 415;
    //                    }
    //                    else
    //                    {
    //                        using (var sr = new StreamReader(ctx.Request.Body, Encoding.UTF8))
    //                        {
    //                            var body = await sr.ReadToEndAsync();
    //                            SetResult(body, ctx);
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    ctx.Response.StatusCode = 405;
    //                }
    //            });
    //        }

    //        private void SetResult(string value, HttpContext ctx)
    //        {
    //            try
    //            {
    //                ctx.Response.StatusCode = 200;
    //                ctx.Response.ContentType = "text/html";
    //                ctx.Response.WriteAsync("<h1>You can now return to the application.</h1>");
    //                ctx.Response.Body.Flush();

    //                _source.TrySetResult(value);
    //            }
    //            catch
    //            {
    //                ctx.Response.StatusCode = 400;
    //                ctx.Response.ContentType = "text/html";
    //                ctx.Response.WriteAsync("<h1>Invalid request.</h1>");
    //                ctx.Response.Body.Flush();
    //            }
    //        }

    //        public Task<string> WaitForCallbackAsync(int timeoutInSeconds = DefaultTimeout)
    //        {
    //            Task.Run(async () =>
    //            {
    //                await Task.Delay(timeoutInSeconds * 1000);
    //                _source.TrySetCanceled();
    //            });

    //            return _source.Task;
    //        }
    //    }
}


