using System;
using System.Threading.Tasks;
using Woodpecker.Core.DocumentDb.Model;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public interface IMonitoringResourceService
    {
        Task<MetricsResponse> FetchMetrics(DateTime startTimeUtc, DateTime endTimeUtc, IMetricsInfo metricsInfo);
    }
}