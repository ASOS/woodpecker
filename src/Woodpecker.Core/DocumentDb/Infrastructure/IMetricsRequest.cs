using System;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public interface IMetricsRequest
    {
        string[] MetricsToGather { get; }
        string ResourceId { get; }
        DateTime StartTimeUtc { get; }
        DateTime EndTimeUtc { get; }
    }
}