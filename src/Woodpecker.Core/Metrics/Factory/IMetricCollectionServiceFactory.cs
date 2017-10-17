using Woodpecker.Core.Metrics.Infrastructure;

namespace Woodpecker.Core.Metrics.Factory
{
    public interface IMetricCollectionServiceFactory
    {
        IMetricCollectionService Create(string tenantId, string clientId, string clientSecrect);
    }
}