using System.Net.Http;
using System.Threading.Tasks;

namespace Fiit.Web.Common.Http.Clients
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
        Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value);
        Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value);
        Task<HttpResponseMessage> DeleteAsync(string requestUri);
        void SetBearerToken(string token);
    }
}