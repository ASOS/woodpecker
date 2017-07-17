using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class Metric
    {
        public LocalizedString Name { get; set; }
        public MetricValue[] MetricValues { get; set; }

        [JsonIgnore]
        public bool IsCumulative
        {
            get
            {
                return MetricValues.All(v => v.Average.HasValue &&
                                             v.Count.HasValue &&
                                             v.Total.HasValue &&
                                             !v.Minimum.HasValue &&
                                             !v.Maximum.HasValue);
            }
        }
    }

    public class MetricValue
    {
        public DateTime TimeStamp { get; set; }

        public double? Average { get; set; }
        [JsonProperty("_count")]
        public long? Count { get; set; }
        public double? Maximum { get; set; }
        public double? Minimum { get; set; }
        public double? Total { get; set; }
    }

    public class LocalizedString
    {
        public string Value { get; set; }
    }
}
