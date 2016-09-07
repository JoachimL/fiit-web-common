using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fiit.Web.Common.Http.Clients
{
    public abstract class HttpClientDecorator : IHttpClient
    {
        private IHttpClient _httpClient;

        public HttpClientDecorator(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected IHttpClient HttpClient { get { return _httpClient; } }

        public virtual Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return _httpClient.DeleteAsync(requestUri);
        }

        public virtual Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return _httpClient.GetAsync(requestUri);
        }

        public virtual Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
        {
            return _httpClient.PostAsJsonAsync(requestUri, value);
        }

        public virtual Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
        {
            return _httpClient.PutAsJsonAsync(requestUri, value);
        }

        public void SetBearerToken(string token)
        {
            _httpClient.SetBearerToken(token);
        }

        public Func<string> GetAccessToken { get; set; }
    }
}