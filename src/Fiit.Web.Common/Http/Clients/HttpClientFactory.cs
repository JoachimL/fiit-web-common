using Microsoft.Azure;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fiit.Web.Common.Http.Clients
{
    public class HttpClientFactory
    {
        private readonly Uri _baseUri;

        public HttpClientFactory() : this(null)
        {

        }

        public HttpClientFactory(Uri baseUri)
        {
            _baseUri = baseUri;
        }

        public Func<HttpClientErrorResponse, Task> ErrorHandler { get; private set; }
        public Func<Task<string>> AccessTokenRetriever { get; private set; }
        public Func<HttpCallTiming, Task> TimingsLoggedTo { get; private set; }

        public HttpClientFactory WithAccessTokenFrom(Func<Task<string>> getAccessToken)
        {
            AccessTokenRetriever = getAccessToken;
            return this;
        }
        public HttpClientFactory WithTimingsLoggedTo(Func<HttpCallTiming, Task> logTimingsTo)
        {
            TimingsLoggedTo = logTimingsTo;
            return this;
        }

        public HttpClientFactory WithErrorHandler(Func<HttpClientErrorResponse, Task> errorHandler)
        {
            ErrorHandler = errorHandler;
            return this;
        }


        public IHttpClient Build()
        {
            var innerClient = new HttpClient()
            {
                Timeout = GetTimeout()
            };

            if (_baseUri != null)
                innerClient.BaseAddress = _baseUri;

            //client.DefaultRequestHeaders.Add("X-Nsc-Username", HttpContext.Current?.User?.Identity?.Name ?? string.Empty);

            IHttpClient client = new HttpClientWrapper(innerClient);
            if (TimingsLoggedTo != null)
                client = new TimingHttpClientDecorator(client, TimingsLoggedTo);
            if (AccessTokenRetriever != null)
                client = new TokenDecoratingHttpClient(AccessTokenRetriever, client);
            if (ErrorHandler != null)
                client = new ErrorHandlingHttpClient(client, ErrorHandler);
            //if(CorrelationIdProvider != null)
            //    client = new CorrelationIdDecorator()
            return new TimeOutAwareHttpClientDecorator(client);
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