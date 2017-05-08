using System.Net.Http;
using System.Threading.Tasks;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public  interface IHttpClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}