using System.Collections.Generic;
using Newtonsoft.Json;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class MetricsResponse
    {
        [JsonProperty("value")]
        public Metric[] Metrics { get; set; }
    }
}