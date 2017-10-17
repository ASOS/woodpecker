using System.Threading.Tasks;

namespace Woodpecker.Core.Metrics.Infrastructure
{
    public interface IMonitoringApiClient
    {
        Task<MetricsResponse> FetchMetrics(IMetricsRequest metricsRequest);
    }
}