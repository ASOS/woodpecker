using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.Internal;
using Woodpecker.Core.Metrics.Configuration;
using Woodpecker.Core.Metrics.DocumentDb;
using Woodpecker.Core.Metrics.Factory;
using Woodpecker.Core.Metrics.Infrastructure;

namespace Woodpecker.Core.Metrics
{
    public class AzureMetricsApiSourcePecker : ISourcePecker
    {
        private readonly IMetricCollectionServiceFactory metricCollectionServiceFactory;

        public AzureMetricsApiSourcePecker()
            : this(new MetricCollectionServiceFactory())
        {
        }

        public AzureMetricsApiSourcePecker(IMetricCollectionServiceFactory metricCollectionServiceFactory)
        {
            this.metricCollectionServiceFactory = metricCollectionServiceFactory;
        }

        public async Task<IEnumerable<ITableEntity>> PeckAsync(PeckSource source)
        {
            var config = new ConfigurationMapper().Map(source.SourceConnectionString);

            var startTimeUtc = source.LastOffset.DateTime;

            var metricCollectionService = CreateMetricCollectionService(config);
            var request = CreateRequest(source.CustomConfig, config.ResourceId, startTimeUtc, source.IntervalMinutes);

            var metrics = await metricCollectionService.CollectMetrics(
                                    request);

            var timeCapturedUtc = DateTimeOffset.UtcNow;

            return metrics.Select(m => PeckResultExtensions.ToEntity(m, source.Name, timeCapturedUtc)).ToArray();
        }

        private IMetricsRequest CreateRequest(string provider, string resourceId, DateTime startTimeUtc, int intervalMinutes)
        {
            // Only DocumentDB supported for now
            if (provider != ProviderNames.DocumentDB)
            {
                throw new ArgumentException(string.Format("Only {0} provider is supported", ProviderNames.DocumentDB), "provider");
            }

            return new DocumentDbMetricsRequest(resourceId, startTimeUtc, startTimeUtc.AddMinutes(intervalMinutes));
        }

        private IMetricCollectionService CreateMetricCollectionService(IConfiguration configuration)
        {
            return this.metricCollectionServiceFactory.Create(configuration.TenantId, configuration.ClientId, configuration.ClientSecret);
        }
    }
}
