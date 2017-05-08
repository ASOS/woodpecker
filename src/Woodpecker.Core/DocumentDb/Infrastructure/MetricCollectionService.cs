using System;
using System.Linq;
using System.Threading.Tasks;
using Woodpecker.Core.DocumentDb.Model;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class MetricCollectionService : IMetricCollectionService
    {
        private readonly IMonitoringResourceService monitoringResourceClient;

        public MetricCollectionService(IMonitoringResourceService monitoringResourceClient)
        {
            this.monitoringResourceClient = monitoringResourceClient;
        }

        public Task<MetricsResponse> CollectMetrics(DateTime startTimeUtc, DateTime endTimeUtc, IMetricsInfo metricsInfo)
        {
            // aggreate
            return this.monitoringResourceClient.FetchMetrics(startTimeUtc, endTimeUtc, metricsInfo);
        }
    }
}