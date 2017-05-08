using System;
using System.Threading.Tasks;
using Woodpecker.Core.DocumentDb.Model;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public interface IMetricCollectionService
    {
        Task<MetricsResponse> CollectMetrics(DateTime startTimeUtc, DateTime endTimeUtc, IMetricsInfo metricsInfo);
    }
}