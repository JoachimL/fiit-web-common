using System.Net.Http;

namespace Fiit.Web.Common.Http.Clients
{
    public class HttpClientErrorResponse
    {
        public string Method { get; set; }
        public object Payload { get; set; }
        public string RequestUri { get; set; }
        public HttpResponseMessage Response { get; set; }
    }
}
