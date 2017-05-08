using System;
using System.Net.Http;
using System.Threading.Tasks;
using Woodpecker.Core.DocumentDb.Model;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class MonitoringResourceClient : IMonitoringResourceClient
    {
        private readonly IHttpClient httpClient;
        private readonly ISecurityTokenProvider securityTokenProvider;

        public MonitoringResourceClient(ISecurityTokenProvider securityTokenProvider, IHttpClient httpClient)
        {
            this.securityTokenProvider = securityTokenProvider;
            this.httpClient = httpClient;
        }

        public async Task<MetricsResponse> GetResponse(Uri uri)
        {
            var token = await this.securityTokenProvider.GetSecurityTokenAsync();

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await this.httpClient.SendAsync(request).ConfigureAwait(false);

            return await response.Content.ReadAsAsync<MetricsResponse>().ConfigureAwait(false);
        }
    }
}