using Woodpecker.Core.DocumentDb.Infrastructure;

namespace Woodpecker.Core.Factory
{
    public class MetricCollectionServiceFactory :  IMetricCollectionServiceFactory
    {
        public IMetricCollectionService Create(string tenantId, string clientId, string clientSecrect)
        {
            return new MetricCollectionService(CreateMonitoringApiClient(tenantId, clientId, clientSecrect), new MetricsAggregator());
        }

        private static IMonitoringApiClient CreateMonitoringApiClient(string tenantId, string clientId, string clientSecrect)
        {
            return new MonitoringApiClient(new MonitoringSecurityProvider(tenantId, clientId, clientSecrect), new DocumentDb.Infrastructure.HttpClient());
        }
    }
}