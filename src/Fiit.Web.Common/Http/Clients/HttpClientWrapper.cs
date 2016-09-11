using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Fiit.Web.Common.Http.Clients
{
    public class HttpClientWrapper : IHttpClient
    {
        private readonly HttpClient _httpClient;

        public Uri BaseAddress
        {
            get
            {
                return _httpClient.BaseAddress;
            }
        }

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return _httpClient.GetAsync(requestUri);
        }

        public Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
        {
            return _httpClient.PostAsJsonAsync<T>(requestUri, value);
        }

        public Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
        {
            return _httpClient.PutAsJsonAsync(requestUri, value);
        }

        public Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return _httpClient.DeleteAsync(requestUri);
        }

        public void SetBearerToken(string token)
        {
            _httpClient.SetBearerToken(token);
        }
    }
}