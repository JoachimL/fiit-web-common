using Microsoft.Azure;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Fiit.Web.Common.Http.Clients
{
    public class TokenDecoratingHttpClient : HttpClientDecorator
    {
        private Func<Task<string>> _getAccessToken;

        public TokenDecoratingHttpClient(Func<Task<string>> getAccessToken, IHttpClient httpClient) 
            : base(httpClient)
        {
            _getAccessToken = getAccessToken;
        }

        public override Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            Func<Task<HttpResponseMessage>> getResponse = () => base.GetAsync(requestUri);
            return DecorateWithTokenOnUnauthorizedResponseAsync(requestUri, "GET", getResponse);
        }

        public override Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            Func<Task<HttpResponseMessage>> getResponse = () => base.DeleteAsync(requestUri);
            return DecorateWithTokenOnUnauthorizedResponseAsync(requestUri, "DELETE", getResponse);
        }

        public override Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
        {
            Func<Task<HttpResponseMessage>> apiOperation = () => base.PostAsJsonAsync(requestUri, value);
            return DecorateWithTokenOnUnauthorizedResponseAsync(requestUri, "POST", apiOperation);
        }

        public override Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
        {
            Func<Task<HttpResponseMessage>> apiOperation = () => base.PutAsJsonAsync(requestUri, value);
            return DecorateWithTokenOnUnauthorizedResponseAsync(requestUri, "PUT", apiOperation);
        }

        private async Task<HttpResponseMessage> DecorateWithTokenOnUnauthorizedResponseAsync(string requestUri, string method, Func<Task<HttpResponseMessage>> getResponse)
        {
            var result = await getResponse();
            if ((!result.IsSuccessStatusCode) &&
                    result.StatusCode == HttpStatusCode.Unauthorized)
                return await RetryWithAccessTokenAsync(getResponse);
            return result;
        }

        private async Task<HttpResponseMessage> RetryWithAccessTokenAsync(Func<Task<HttpResponseMessage>> getResponse)
        {
            string token = await _getAccessToken();
            HttpClient.SetBearerToken(token);
            return await getResponse();
        }
    }
}
