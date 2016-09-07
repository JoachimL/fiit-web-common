using Newtonsoft.Json;
using Serilog;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fiit.Web.Common.Http.Clients
{
    public class TimeOutAwareHttpClientDecorator : HttpClientDecorator
    {
        public TimeOutAwareHttpClientDecorator(IHttpClient httpClient) : base(httpClient)
        {
        }

        public override Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            Func<Task<HttpResponseMessage>> apiOperation = () => base.GetAsync(requestUri);
            string errorMessage = $"TaskCancelledException occured when attempting to GET from {requestUri}.";
            return GetResponseOrThrowTimeoutException(apiOperation, errorMessage);
        }

        public override Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            Func<Task<HttpResponseMessage>> apiOperation = () => base.DeleteAsync(requestUri);
            string errorMessage = $"TaskCancelledException occured when attempting to DELETE {requestUri}.";
            return GetResponseOrThrowTimeoutException(apiOperation, errorMessage);
        }

        public override Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
        {
            Func<Task<HttpResponseMessage>> apiOperation = () => base.PostAsJsonAsync(requestUri, value);
            string errorMessage = $"TaskCancelledException occured when attempting to POST to {requestUri}. Payload: {JsonConvert.SerializeObject(value)}";
            return GetResponseOrThrowTimeoutException(apiOperation, errorMessage);
        }

        public override Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
        {
            Func<Task<HttpResponseMessage>> apiOperation = () => base.PutAsJsonAsync(requestUri, value);
            string errorMessage = $"TaskCancelledException occured when attempting to PUT to {requestUri}. Payload: {JsonConvert.SerializeObject(value)}";
            return GetResponseOrThrowTimeoutException(apiOperation, errorMessage);
        }

        private static async Task<HttpResponseMessage> GetResponseOrThrowTimeoutException(Func<Task<HttpResponseMessage>> apiOperation, string errorMessage)
        {
            try
            {
                return await apiOperation();
            }
            catch (TaskCanceledException ex)
            {
                Log.Error(errorMessage, ex);
                throw new TimeoutException("Unable to get a response in a timely manner.");
            }
        }
    }
}