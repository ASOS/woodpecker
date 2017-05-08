using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.DocumentDb.Configuration;
using Woodpecker.Core.DocumentDb.Infrastructure;
using Woodpecker.Core.Internal;

namespace Woodpecker.Core.DocumentDb
{
    public class DocumentDbSourcePecker : ISourcePecker
    {
        private readonly IMetricCollectionService metricCollectionService;
        private readonly ConfigurationMapper configMapper;

        public DocumentDbSourcePecker()
            : this(null)

        {

        }

        public DocumentDbSourcePecker(IMetricCollectionService metricCollectionService)
        {
            this.metricCollectionService = metricCollectionService;
            this.configMapper = new ConfigurationMapper();
        }

        public async Task<IEnumerable<ITableEntity>> PeckAsync(PeckSource source)
        {
            var config = this.configMapper.Map(source.SourceConnectionString);

            var metricsInfo = new DocumentDbMetricsInfo(config.ResourceId);

            var startTimeUtc = source.LastOffset.DateTime;

            var metrics = await this.metricCollectionService.CollectMetrics(startTimeUtc, startTimeUtc.AddMinutes(source.IntervalMinutes), metricsInfo);

            return metrics.Select(m => m.ToEntity(source.Name, startTimeUtc));
        }
    }
}
