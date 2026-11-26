using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon.S3;
using KeePass;
using KeePass.UI;
using KeePassLib.Utility;

namespace KeeAnywhere
{
    // Shows errors in a user friendly way: a readable message instead of the complete exception,
    // the technical details (incl. stack trace) can be expanded and copied to the clipboard.
    public static class ErrorMessageService
    {
        private const int ButtonOk = 1;
        private const int ButtonCopy = 2;

        // Shows the error on KeePass' UI thread; doesn't block when called from a background thread
        public static void ShowError(string mainInstruction, string context, Exception ex)
        {
            var mainForm = Program.MainForm;
            if (mainForm != null && mainForm.InvokeRequired)
            {
                mainForm.BeginInvoke(new Action(() => ShowErrorDialog(mainInstruction, context, ex)));
                return;
            }

            ShowErrorDialog(mainInstruction, context, ex);
        }

        private static void ShowErrorDialog(string mainInstruction, string context, Exception ex)
        {
            var message = GetUserMessage(ex);
            var content = string.IsNullOrEmpty(context) ? message : context + Environment.NewLine + Environment.NewLine + message;
            var details = GetDetails(ex);

            var dlg = new VistaTaskDialog();
            dlg.WindowTitle = "KeeAnywhere";
            dlg.MainInstruction = mainInstruction;
            dlg.Content = content;
            dlg.SetIcon(VtdIcon.Warning);
            dlg.ExpandedInformation = details;
            dlg.CollapsedControlText = "Show details";
            dlg.ExpandedControlText = "Hide details";
            dlg.AddButton(ButtonCopy, "Copy details", null);
            dlg.AddButton(ButtonOk, "OK", null);
            dlg.DefaultButtonID = ButtonOk;

            if (!dlg.ShowDialog(Program.MainForm))
            {
                // Task dialogs are not supported (e.g. Mono/Wine): simple message box without details
                MessageService.ShowWarning(mainInstruction, context, message);
                return;
            }

            if (dlg.Result == ButtonCopy)
            {
                try
                {
                    Clipboard.SetText(mainInstruction + Environment.NewLine + content + Environment.NewLine + Environment.NewLine + details);
                }
                catch (Exception)
                {
                    // Clipboard may be locked by another application
                }
            }
        }

        // A readable message for the user, followed by a short technical summary (for issue reports)
        public static string GetUserMessage(Exception ex)
        {
            if (ex == null) return string.Empty;

            var exceptions = GetExceptionChain(ex).ToArray();

            string message = null;
            string summary = null;

            var s3Exception = exceptions.OfType<AmazonS3Exception>().FirstOrDefault();
            var googleException = exceptions.OfType<Google.GoogleApiException>().FirstOrDefault();
            var networkException = exceptions.FirstOrDefault(e => e is HttpRequestException || e is WebException || e is SocketException);
            // HttpClient reports a timeout as TaskCanceledException
            var timeoutException = exceptions.FirstOrDefault(e => e is TaskCanceledException || e is TimeoutException);
            var authException = exceptions.FirstOrDefault(IsAuthorizationError);
            var azureAuthException = exceptions.FirstOrDefault(IsAzureAuthenticationError);

            if (s3Exception != null)
            {
                message = GetS3Message(s3Exception);
                summary = string.Format("AmazonS3Exception, HTTP {0}, {1}", (int)s3Exception.StatusCode, s3Exception.ErrorCode);
            }
            else if (authException != null)
            {
                // e.g. expired or revoked token (#106)
                message = "The authorization of this account is no longer valid (e.g. expired, revoked or the password was changed)." +
                          Environment.NewLine + Environment.NewLine +
                          "Please re-authorize the account: go to 'Tools - KeeAnywhere Settings - Accounts', select the account and click 'Check'.";
                summary = authException.GetType().Name;
            }
            else if (azureAuthException != null)
            {
                message = "Azure denied the access - the access key or SAS token may be wrong or expired. Please check the account (remove it and add it again with a valid key or token)." +
                          Environment.NewLine + Environment.NewLine + azureAuthException.Message;
                summary = azureAuthException.GetType().Name;
            }
            else if (googleException != null)
            {
                message = GetGoogleMessage(googleException);
                summary = string.Format("GoogleApiException, HTTP {0}", (int)googleException.HttpStatusCode);
            }
            else if (networkException != null)
            {
                message = "The server could not be reached. Please check your internet connection and proxy settings." +
                          Environment.NewLine + Environment.NewLine + networkException.Message;
                summary = networkException.GetType().Name;
            }
            else if (timeoutException != null)
            {
                message = "The server did not respond in time. Please check your internet connection and try again.";
                summary = timeoutException.GetType().Name;
            }

            if (message == null)
            {
                // Unknown error: all distinct messages of the exception chain, without stack trace
                var root = exceptions.FirstOrDefault(e => !(e is AggregateException) && !(e is TargetInvocationException)) ?? ex;
                message = string.Join(Environment.NewLine,
                    GetExceptionChain(root).Select(e => e.Message).Where(m => !string.IsNullOrEmpty(m)).Distinct().ToArray());
                summary = root.GetType().Name;
            }

            return message + Environment.NewLine + Environment.NewLine + "(" + summary + ")";
        }

        // OAuth based providers: the (refresh) token is invalid, so the account has to be re-authorized.
        // Detected by type name, OAuth error code or HTTP 401, so no direct dependency on every provider SDK is needed.
        private static bool IsAuthorizationError(Exception ex)
        {
            var typeName = ex.GetType().Name;

            // Dropbox
            if (typeName == "AuthException" || typeName == "OAuth2Exception")
                return true;

            // OAuth error codes of a failed token refresh (Google: TokenResponseException, OneDrive: ServiceException)
            var message = ex.Message ?? string.Empty;
            if (message.IndexOf("invalid_grant", StringComparison.OrdinalIgnoreCase) >= 0 ||
                message.IndexOf("unauthorized_client", StringComparison.OrdinalIgnoreCase) >= 0 ||
                message.IndexOf("deleted_client", StringComparison.OrdinalIgnoreCase) >= 0)
                return true;

            // HTTP 401 (e.g. OneDrive, Google Drive, Box, HiDrive) - not for S3 and Azure, which use access keys
            return !(ex is AmazonS3Exception) && typeName != "StorageException" && GetHttpStatusCode(ex) == 401;
        }

        // Azure (Blob/File storage): wrong or expired access key or SAS token
        private static bool IsAzureAuthenticationError(Exception ex)
        {
            if (ex.GetType().Name != "StorageException") return false;

            var statusCode = GetHttpStatusCode(ex);
            return statusCode == 401 || statusCode == 403;
        }

        // Reads the HTTP status code of provider specific exceptions (property names differ between the SDKs)
        private static int GetHttpStatusCode(Exception ex)
        {
            var webException = ex as WebException;
            if (webException != null)
            {
                var response = webException.Response as HttpWebResponse;
                return response != null ? (int)response.StatusCode : 0;
            }

            // Azure: StorageException.RequestInformation.HttpStatusCode
            var requestInformation = GetPropertyValue(ex, "RequestInformation");
            if (requestInformation != null)
                return ToStatusCode(GetPropertyValue(requestInformation, "HttpStatusCode"));

            foreach (var name in new[] { "HttpStatusCode", "StatusCode", "ResponseStatusCode" })
            {
                var statusCode = ToStatusCode(GetPropertyValue(ex, name));
                if (statusCode != 0) return statusCode;
            }

            return 0;
        }

        private static object GetPropertyValue(object obj, string name)
        {
            try
            {
                var property = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                return property != null ? property.GetValue(obj, null) : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static int ToStatusCode(object value)
        {
            if (value == null) return 0;
            if (value is HttpStatusCode) return (int)(HttpStatusCode)value;
            if (value is int) return (int)value;
            return 0;
        }

        // Complete technical details incl. stack trace
        public static string GetDetails(Exception ex)
        {
            return ex == null ? string.Empty : ex.ToString();
        }

        private static string GetS3Message(AmazonS3Exception ex)
        {
            switch (ex.ErrorCode)
            {
                case "AccessDenied":
                    return "Access denied by Amazon S3 - the user has no permission for this action. Please check the IAM policy and the bucket name." +
                           Environment.NewLine + Environment.NewLine + ex.Message;
                case "NoSuchBucket":
                    return "The bucket does not exist. Please check the bucket name.";
                case "NoSuchKey":
                    return "The file does not exist.";
                case "InvalidAccessKeyId":
                case "SignatureDoesNotMatch":
                    return "The access key or the secret key is wrong. Please check the account settings.";
                default:
                    return ex.Message;
            }
        }

        private static string GetGoogleMessage(Google.GoogleApiException ex)
        {
            var message = ex.Error != null && !string.IsNullOrEmpty(ex.Error.Message) ? ex.Error.Message : ex.Message;

            if (message != null && message.IndexOf("insufficient authentication scopes", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Access to Google Drive was not granted. Please remove the account and add it again - and enable the checkbox for Google Drive access on Google's consent page.";
            }

            return message;
        }

        private static IEnumerable<Exception> GetExceptionChain(Exception ex)
        {
            var visited = new HashSet<Exception>();
            var queue = new Queue<Exception>();
            queue.Enqueue(ex);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == null || !visited.Add(current)) continue;

                yield return current;

                var aggregate = current as AggregateException;
                if (aggregate != null)
                {
                    foreach (var inner in aggregate.InnerExceptions)
                        queue.Enqueue(inner);
                }
                else
                {
                    queue.Enqueue(current.InnerException);
                }
            }
        }
    }
}
