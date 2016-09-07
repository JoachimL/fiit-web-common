using Microsoft.Azure;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fiit.Web.Common.Http.Clients
{
    public class HttpClientFactory
    {
        public IHttpClient Build()
        {
            return BuildFor(null);
        }

        public static IHttpClient BuildFor(Uri baseUri)
        {
            return BuildFor(baseUri, null);
        }

        public static IHttpClient BuildFor(Uri baseUri, Func<Task<string>> getAccessToken)
        {
            var client = new HttpClient()
            {
                Timeout = GetTimeout()
            };

            if (baseUri != null)
                client.BaseAddress = baseUri;

            //client.DefaultRequestHeaders.Add("X-Nsc-Username", HttpContext.Current?.User?.Identity?.Name ?? string.Empty);
            return new TimeOutAwareHttpClientDecorator(
                new TimingHttpClientDecorator(
                    new TokenDecoratingHttpClient(
                        getAccessToken,
                        new HttpClientWrapper(client))));
        }

        private static TimeSpan GetTimeout()
        {
            var setting = CloudConfigurationManager.GetSetting("ApiTimeout");
            if (setting == null)
                return TimeSpan.FromSeconds(5);
            return TimeSpan.FromSeconds(int.Parse(setting));
        }
    }
}