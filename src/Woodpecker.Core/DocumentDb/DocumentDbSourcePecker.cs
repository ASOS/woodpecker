using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.DocumentDb.Configuration;
using Woodpecker.Core.DocumentDb.Infrastructure;
using Woodpecker.Core.DocumentDb.Model;
using Woodpecker.Core.Factory;
using Woodpecker.Core.Internal;

namespace Woodpecker.Core.DocumentDb
{
    public class DocumentDbSourcePecker : ISourcePecker
    {
        private readonly IMetricCollectionServiceFactory metricCollectionServiceFactory;
        public DocumentDbSourcePecker()
            : this(new MetricCollectionServiceFactory())
        {
        }

        public DocumentDbSourcePecker(IMetricCollectionServiceFactory metricCollectionServiceFactory)
        {
            this.metricCollectionServiceFactory = metricCollectionServiceFactory;
        }

        public async Task<IEnumerable<ITableEntity>> PeckAsync(PeckSource source)
        {
            var config = new ConfigurationMapper().Map(source.SourceConnectionString);

            var startTimeUtc = source.LastOffset.DateTime;
            
            var metrics = await CreateMetricCollectionService(config).CollectMetrics(
                                    new DocumentDbMetricsRequest(config.ResourceId, startTimeUtc, startTimeUtc.AddMinutes(source.IntervalMinutes)));


            var timeCapturedUtc = DateTimeOffset.UtcNow;

            return metrics.Select(m => m.ToEntity(source.Name, timeCapturedUtc));
        }

        private IMetricCollectionService CreateMetricCollectionService(IConfiguration configuration)
        {
            return this.metricCollectionServiceFactory.Create(configuration.TenantId,configuration.ClientId,configuration.ClientSecret);
        }
    }
}
