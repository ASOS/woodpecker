using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Woodpecker.Core.DocumentDb.Model;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class MonitoringApiClient : IMonitoringResourceService
    {
        private static readonly Uri MonitoringApiBaseUri = new Uri("https://management.azure.com");
        private readonly IMonitoringResourceClient monitoringResourceClient;

        public MonitoringApiClient(
            IMonitoringResourceClient monitoringResourceClient)
        {
            this.monitoringResourceClient = monitoringResourceClient;
        }
        public async Task<MetricsResponse> FetchMetrics(DateTime startTimeUtc, DateTime endTimeUtc, IMetricsInfo metricsInfo)
        {
            var filter = BuildFilter(startTimeUtc, endTimeUtc, metricsInfo);

            var uri = BuildUri(metricsInfo.ResourceId, filter);

            return await this.monitoringResourceClient.GetResponse(uri);
        }
        private Uri BuildUri(string resource, string filter)
        {
            var uri = String.Format("{0}/metrics?api-version=2014-04-01&$filter={1}", resource, filter);

            return new Uri(MonitoringApiBaseUri, uri);
        }
        private string BuildFilter(DateTime startUtc, DateTime endUtc, IMetricsInfo metricsInfo)
        {
            var start = startUtc.ToString("yyyy-MM-ddTHH:mm:00.000Z");
            var end = endUtc.ToString("yyyy-MM-ddTHH:mm:00.000Z");

            var metricsSelector = metricsInfo.MetricsToGather.Select(m => $"name.value eq '{m}'");
            var metricsFilter = $"{string.Join(" or ", metricsSelector)}";

            var filter = $"({metricsFilter}) and endTime eq {end} and startTime eq {start} and timeGrain eq duration'PT1M'";

            return filter;
        }
    }
}