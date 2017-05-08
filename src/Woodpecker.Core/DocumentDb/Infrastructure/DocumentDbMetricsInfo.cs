using System;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class DocumentDbMetricsInfo : IMetricsInfo
    {
        private readonly string resourceId;

        public DocumentDbMetricsInfo(string resourceId)
        {
            this.resourceId = resourceId;
        }

        public string[] MetricsToGather
        {
            get
            {
                return
                    new[]
                    {
                        "Available Storage",
                        "Average Requests per Second",
                        "Data Size",
                        "Document Count",
                        "Index Size",
                        "Max RUs Per Second",
                        "Observed Read Latency",
                        "Observed Write Latency",
                        "Throttled Requests",
                        "Total Request Units",
                        "Total Requests",
                    };}
        }
        public string ResourceId { get { return this.resourceId; } }
    }
}