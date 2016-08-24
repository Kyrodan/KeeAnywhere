using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Box.V2;
using Box.V2.Config;
using Box.V2.Request;

namespace KeeAnywhere.StorageProviders.Box
{
    public class BoxHttpRequestHandler : IRequestHandler
    {
        public async Task<IBoxResponse<T>> ExecuteAsync<T>(IBoxRequest request)
            where T : class
        {
            // Need to account for special cases when the return type is a stream
            bool isStream = typeof(T) == typeof(Stream);

            HttpRequestMessage httpRequest = request.GetType() == typeof(BoxMultiPartRequest) ?
                BuildMultiPartRequest(request as BoxMultiPartRequest) :
                BuildRequest(request);

            // Add headers
            foreach (var kvp in request.HttpHeaders)
            {
                if (kvp.Key == Constants.RequestParameters.ContentMD5)
                {
                    httpRequest.Content.Headers.Add(kvp.Key, kvp.Value);
                }
                else
                {
                    httpRequest.Headers.TryAddWithoutValidation(kvp.Key, kvp.Value);
                }
            }


            // If we are retrieving a stream, we should return without reading the entire response
            HttpCompletionOption completionOption = isStream ?
                HttpCompletionOption.ResponseHeadersRead :
                HttpCompletionOption.ResponseContentRead;

            Debug.WriteLine(string.Format("RequestUri: {0}", httpRequest.RequestUri));//, RequestHeader: {1} , httpRequest.Headers.Select(i => string.Format("{0}:{1}", i.Key, i.Value)).Aggregate((i, j) => i + "," + j)));

            try
            {
                HttpClient client = CreateClient(request);

                HttpResponseMessage response = await client.SendAsync(httpRequest, completionOption).ConfigureAwait(false);

                BoxResponse<T> boxResponse = new BoxResponse<T>();
                boxResponse.Headers = response.Headers;

                // Translate the status codes that interest us 
                boxResponse.StatusCode = response.StatusCode;
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Created:
                    case HttpStatusCode.NoContent:
                    case HttpStatusCode.Found:
                        boxResponse.Status = ResponseStatus.Success;
                        break;
                    case HttpStatusCode.Accepted:
                        boxResponse.Status = ResponseStatus.Pending;
                        break;
                    case HttpStatusCode.Unauthorized:
                        boxResponse.Status = ResponseStatus.Unauthorized;
                        break;
                    case HttpStatusCode.Forbidden:
                        boxResponse.Status = ResponseStatus.Forbidden;
                        break;
                    default:
                        boxResponse.Status = ResponseStatus.Error;
                        break;
                }

                if (isStream && boxResponse.Status == ResponseStatus.Success)
                {
                    var resObj = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    boxResponse.ResponseObject = resObj as T;
                }
                else
                {
                    boxResponse.ContentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
                return boxResponse;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Exception: {0}", ex.Message));
                throw;
            }
        }

        private HttpClient CreateClient(IBoxRequest request)
        {
            HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip };
            handler.AllowAutoRedirect = request.FollowRedirect;
            handler.ApplyProxy();

            HttpClient client = new HttpClient(handler);

            if (request.Timeout.HasValue)
                client.Timeout = request.Timeout.Value;

            return client;
        }

        private HttpRequestMessage BuildRequest(IBoxRequest request)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage();
            httpRequest.RequestUri = request.AbsoluteUri;
            //httpRequest.Content = new StringContent(request.GetQueryString(), Encoding.UTF8, "application/x-www-form-urlencoded");

            switch (request.Method)
            {
                case RequestMethod.Get:
                    httpRequest.Method = HttpMethod.Get;
                    return httpRequest;
                case RequestMethod.Put:
                    httpRequest.Method = HttpMethod.Put;
                    break;
                case RequestMethod.Delete:
                    httpRequest.Method = HttpMethod.Delete;
                    break;
                case RequestMethod.Post:
                    httpRequest.Method = HttpMethod.Post;
                    break;
                case RequestMethod.Options:
                    httpRequest.Method = HttpMethod.Options;
                    break;
                default:
                    throw new InvalidOperationException("Http method not supported");
            }

            // Set request content to string or form-data
            httpRequest.Content = !string.IsNullOrWhiteSpace(request.Payload) ?
                string.IsNullOrEmpty(request.ContentType) ? // Check for custom content type
                    (HttpContent)new StringContent(request.Payload) :
                    (HttpContent)new StringContent(request.Payload, request.ContentEncoding, request.ContentType) :
                (HttpContent)new FormUrlEncodedContent(request.PayloadParameters);

            return httpRequest;
        }

        private HttpRequestMessage BuildMultiPartRequest(BoxMultiPartRequest request)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, request.AbsoluteUri);
            MultipartFormDataContent multiPart = new MultipartFormDataContent();

            // Break out the form parts from the request
            var filePart = request.Parts.Where(p => p.GetType() == typeof(BoxFileFormPart))
                .Select(p => p as BoxFileFormPart)
                .FirstOrDefault(); // Only single file upload is supported at this time
            var stringParts = request.Parts.Where(p => p.GetType() == typeof(BoxStringFormPart))
                .Select(p => p as BoxStringFormPart);

            // Create the file part
            StreamContent fileContent = new StreamContent(filePart.Value);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = ForceQuotesOnParam(filePart.Name),
                FileName = ForceQuotesOnParam(filePart.FileName)
            };
            multiPart.Add(fileContent);

            // Create the string part
            foreach (var sp in stringParts)
                multiPart.Add(new StringContent(sp.Value), ForceQuotesOnParam(sp.Name));

            httpRequest.Content = multiPart;

            return httpRequest;
        }

        /// <summary>
        /// Adds quotes around the named parameters
        /// This is required as the API will currently not take multi-part parameters without quotes
        /// </summary>
        /// <param name="name">The name parameter to add quotes to</param>
        /// <returns>The name parameter surrounded by quotes</returns>
        private string ForceQuotesOnParam(string name)
        {
            return string.Format("\"{0}\"", name);
        }
    }
}