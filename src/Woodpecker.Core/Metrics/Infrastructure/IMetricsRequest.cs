using System;

namespace Woodpecker.Core.Metrics.Infrastructure
{
    public interface IMetricsRequest
    {
        string[] MetricsToGather { get; }
        string ResourceId { get; }
        DateTime StartTimeUtc { get; }
        DateTime EndTimeUtc { get; }
    }
}