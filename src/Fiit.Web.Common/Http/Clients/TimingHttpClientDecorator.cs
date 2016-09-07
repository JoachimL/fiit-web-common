using Serilog;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fiit.Web.Common.Http.Clients
{
    public class TimingHttpClientDecorator : HttpClientDecorator
    {
        public TimingHttpClientDecorator(IHttpClient httpClient) : base(httpClient)
        {
        }

        public override Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            Func<Task<HttpResponseMessage>> getResponse = () => base.GetAsync(requestUri);
            return GetAndTimeResponseAsync(requestUri, "GET", getResponse);
        }

        public override Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            Func<Task<HttpResponseMessage>> getResponse = () => base.DeleteAsync(requestUri);
            return GetAndTimeResponseAsync(requestUri, "DELETE", getResponse);
        }

        public override Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
        {
            Func<Task<HttpResponseMessage>> apiOperation = () => base.PostAsJsonAsync(requestUri, value);
            return GetAndTimeResponseAsync(requestUri, "POST", apiOperation);
        }

        public override Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
        {
            Func<Task<HttpResponseMessage>> apiOperation = () => base.PutAsJsonAsync(requestUri, value);
            return GetAndTimeResponseAsync(requestUri, "PUT", apiOperation);
        }

        private static async Task<HttpResponseMessage> GetAndTimeResponseAsync(string requestUri, string method, Func<Task<HttpResponseMessage>> getResponse)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await getResponse();
                stopwatch.Stop();
                return response;
            }
            catch (Exception)
            {
                stopwatch.Stop();
                throw;
            }
            finally
            {
                Log.Information("Method:{Method};Uri:{RequestUri};Elapsed:{Elapsed}",
                    method, requestUri, stopwatch.Elapsed);
            }
        }
    }
}