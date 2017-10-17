using Newtonsoft.Json;

namespace Woodpecker.Core.Metrics.Infrastructure
{
    public class MetricsResponse
    {
        [JsonProperty("value")]
        public Metric[] Metrics { get; set; }
    }
}