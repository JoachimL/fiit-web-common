using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Fiit.Web.Common.Http.Clients
{
    public class ErrorHandlingHttpClient : HttpClientDecorator
    {
        private readonly Func<HttpClientErrorResponse, Task> _errorHandler;

        public ErrorHandlingHttpClient(
            IHttpClient httpClient,
            Func<HttpClientErrorResponse, Task> errorHandler) : base(httpClient)
        {
            _errorHandler = errorHandler;
        }


        public override async Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
        {
            var result = await base.PutAsJsonAsync<T>(requestUri, value);
            if (!result.IsSuccessStatusCode)
                await HandleErrorAsync(requestUri, result, "PUT", value);
            return result;
        }

        public override async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            var result = await base.GetAsync(requestUri);
            if (!result.IsSuccessStatusCode)
                await HandleErrorAsync(requestUri, result, "GET", null);
            return result;
        }

        public override async Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            var result = await base.DeleteAsync(requestUri);
            if (!result.IsSuccessStatusCode)
                await HandleErrorAsync(requestUri, result, "DELETE", null);
            return result;
        }

        public override async Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
        {
            var result = await base.PostAsJsonAsync<T>(requestUri, value);
            if (!result.IsSuccessStatusCode)
                await HandleErrorAsync(requestUri, result, "POST", value);
            return result;
        }

        private Task HandleErrorAsync(string requestUri, HttpResponseMessage result, string method, object payload)
        {
            if (_errorHandler != null)
            {
                var error = new HttpClientErrorResponse
                {
                    RequestUri = requestUri,
                    Response = result,
                    Method = method,
                    Payload = payload
                };
                return this._errorHandler(error);
            }
            return Task.FromResult(0);
        }
    }
}
