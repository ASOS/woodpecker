using System;
using System.Linq;

namespace Woodpecker.Core.Metrics.Infrastructure
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

            var metricsSelector = metricsRequest.MetricsToGather.Select(m => string.Format("name.value eq '{0}'", m));
            var metricsFilter = string.Join(" or ", metricsSelector);

            var filter = string.Format("({0}) and endTime eq {1} and startTime eq {2} and timeGrain eq duration'PT1M'", metricsFilter, end, start);

            return filter;
        }
    }
}