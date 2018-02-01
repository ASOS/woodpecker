using System;
using Woodpecker.Core.Metrics.Infrastructure;

namespace Woodpecker.Core.Metrics.DocumentDb
{
    public class DocumentDbMetricsRequest : IMetricsRequest
    {
        public DocumentDbMetricsRequest(string resourceId, DateTime startTimeUtc, DateTime endTimeUtc)
        {
            StartTimeUtc = startTimeUtc;
            EndTimeUtc = endTimeUtc;
            ResourceId = resourceId;
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
                    };
            }
        }
        public string ResourceId { get; private set; }
        public DateTime StartTimeUtc { get; private set; }
        public DateTime EndTimeUtc { get; private set; }
    }
}