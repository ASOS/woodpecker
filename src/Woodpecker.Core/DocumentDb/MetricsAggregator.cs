using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Woodpecker.Core.DocumentDb.Infrastructure;
using Woodpecker.Core.DocumentDb.Model;

namespace Woodpecker.Core.DocumentDb
{
    public class MetricsAggregator
    {
        public virtual MetricModel Aggregate(string name, MetricValue[] values)
        {
            if (!values.Any())
            {
                throw new ArgumentException("values cannot be empty", "values");
            }

            var metric = new MetricModel
            {
                Name = name,
                TimeStamp = values.Max(v => v.TimeStamp),
                Count = GetLongValueOrNull(values.Select(v => v.Count), Enumerable.Sum),
                Total = GetDoubleValueOrNull(values.Select(v => v.Total), Enumerable.Max),
                Average = GetDoubleValueOrNull(values.Select(v => v.Average), Enumerable.Average),
                Maximum = GetDoubleValueOrNull(values.Select(v => v.Maximum), Enumerable.Max),
                Minimum = GetDoubleValueOrNull(values.Select(v => v.Minimum), Enumerable.Min),
            };

            return metric;
        }

        private long? GetLongValueOrNull(IEnumerable<long?> values, Func<IEnumerable<long?>, long?> aggregate)
        {
            var candidates = values.Where(v => v.HasValue).ToArray();

            if (candidates.Length == 0)
            {
                return null;
            }

            return aggregate(candidates);
        }

        private double? GetDoubleValueOrNull(IEnumerable<double?> values, Func<IEnumerable<double?>, double?> aggregate)
        {
            var candidates = values.Where(v => v.HasValue).ToArray();

            if (candidates.Length == 0)
            {
                return null;
            }

            return aggregate(candidates);
        }
    }
}
