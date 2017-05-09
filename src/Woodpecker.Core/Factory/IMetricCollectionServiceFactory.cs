using Woodpecker.Core.DocumentDb.Infrastructure;

namespace Woodpecker.Core.Factory
{
    public interface IMetricCollectionServiceFactory
    {
        IMetricCollectionService Create(string tenantId, string clientId, string clientSecrect);
    }
}