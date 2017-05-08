namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public interface IMetricsInfo
    {
        string[] MetricsToGather { get; }
        string ResourceId { get; }
    }
}