using System;
using System.Linq;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class UriFactory
    {
        private static readonly Uri MonitoringApiBaseUri = new Uri("https://management.azure.com");
        public static Uri CreateMonitoringUriWithMetricFilter(IMetricsRequest metricsRequest)
        {
            return BuildUri(metricsRequest.ResourceId, BuildFilter(metricsRequest));
        }

        private static Uri BuildUri(string resource, string filter)
        {
            var uri = String.Format("{0}/metrics?api-version=2014-04-01&$filter={1}", resource, filter);

            return new Uri(MonitoringApiBaseUri, uri);
        }
        private static string BuildFilter(IMetricsRequest metricsRequest)
        {
            var start = metricsRequest.StartTimeUtc.ToString("yyyy-MM-ddTHH:mm:00.000Z");
            var end = metricsRequest.EndTimeUtc.ToString("yyyy-MM-ddTHH:mm:00.000Z");

            var metricsSelector = metricsRequest.MetricsToGather.Select(m => $"name.value eq '{m}'");
            var metricsFilter = $"{string.Join(" or ", metricsSelector)}";

            var filter = $"({metricsFilter}) and endTime eq {end} and startTime eq {start} and timeGrain eq duration'PT1M'";

            return filter;
        }
    }
}