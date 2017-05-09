using System;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class DocumentDbMetricsRequest : IMetricsRequest
    {
        public DocumentDbMetricsRequest(string resourceId,DateTime startTimeUtc, DateTime endTimeUtc)
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
                    };}
        }
        public string ResourceId { get; }
        public DateTime StartTimeUtc { get; }
        public DateTime EndTimeUtc { get; }
    }
}