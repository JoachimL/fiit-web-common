using System;

namespace Fiit.Web.Common.Http.Clients
{
    public class HttpCallTiming
    {
        public TimeSpan Elapsed { get; set; }
        public string Method { get; set; }
        public string FullUri { get; set; }
        public string RelativeUri { get; set; }
    }
}