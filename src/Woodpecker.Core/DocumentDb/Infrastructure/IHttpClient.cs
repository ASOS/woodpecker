using System.Net.Http;
using System.Threading.Tasks;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public  interface IHttpClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }

    public class HttpClient : IHttpClient
    {
        private readonly System.Net.Http.HttpClient httpClient;

        public HttpClient()
        {
            this.httpClient = new System.Net.Http.HttpClient();
        }
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return this.httpClient.SendAsync(request);
        }
    }
}