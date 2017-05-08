using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Woodpecker.Core.DocumentDb.Model;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class MetricCollectionService : IMetricCollectionService
    {
        private readonly IMonitoringResourceService monitoringResourceClient;
        private readonly MetricsAggregator metricsAggregator;

        public MetricCollectionService(IMonitoringResourceService monitoringResourceClient, MetricsAggregator metricsAggregator)
        {
            this.monitoringResourceClient = monitoringResourceClient;
            this.metricsAggregator = metricsAggregator;
        }

        public async Task<IEnumerable<MetricModel>> CollectMetrics(DateTime startTimeUtc, DateTime endTimeUtc, IMetricsInfo metricsInfo)
        {
            var response = await this.monitoringResourceClient.FetchMetrics(startTimeUtc, endTimeUtc, metricsInfo).ConfigureAwait(false);

            var aggregatedMetrics = response.Metrics.Where(m => m.MetricValues != null && m.MetricValues.Any())
                .Select(m => metricsAggregator.Aggregate(m.Name.Value, m.MetricValues))
                .ToList();

            return aggregatedMetrics;
        }
    }
}