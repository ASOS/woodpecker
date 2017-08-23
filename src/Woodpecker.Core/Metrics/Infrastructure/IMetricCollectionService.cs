using System.Collections.Generic;
using System.Threading.Tasks;
using Woodpecker.Core.Metrics.Model;

namespace Woodpecker.Core.Metrics.Infrastructure
{
    public interface IMetricCollectionService
    {
        Task<IEnumerable<MetricModel>> CollectMetrics(IMetricsRequest metricsRequest);
    }
}