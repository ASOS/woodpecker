using System;
using System.Collections.Generic;

using Woodpecker.Core.DocumentDb.Model;

namespace Woodpecker.Core.DocumentDb
{
    public interface IDocumentDbMetricsCollector
    {
        IEnumerable<MetricModel> GetMetrics(string resourceUri, DateTime startUtc, DateTime endUtc);
    }
}
