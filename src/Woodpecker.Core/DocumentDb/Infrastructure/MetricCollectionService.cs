using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Woodpecker.Core.DocumentDb.Model;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class MetricCollectionService : IMetricCollectionService
    {
        private readonly IMonitoringApiClient monitoringResourceClient;
        private readonly MetricsAggregator metricsAggregator;

        public MetricCollectionService(IMonitoringApiClient monitoringResourceClient, MetricsAggregator metricsAggregator)
        {
            this.monitoringResourceClient = monitoringResourceClient;
            this.metricsAggregator = metricsAggregator;
        }

        public async Task<IEnumerable<MetricModel>> CollectMetrics(IMetricsRequest metricsRequest)
        {
            var response = await this.monitoringResourceClient.FetchMetrics(metricsRequest).ConfigureAwait(false);

            var aggregatedMetrics = response.Metrics.Where(m => m.MetricValues != null && m.MetricValues.Any())
                .Select(m => metricsAggregator.Aggregate(m))
                .ToList();

            return aggregatedMetrics;
        }
    }
}