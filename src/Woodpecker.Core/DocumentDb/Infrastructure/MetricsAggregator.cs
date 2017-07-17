using System;
using System.Collections.Generic;
using System.Linq;
using Woodpecker.Core.DocumentDb.Model;

namespace Woodpecker.Core.DocumentDb.Infrastructure
{
    public class MetricsAggregator
    {
        private static readonly Func<IEnumerable<long?>, long?> Count_Sum = Enumerable.Sum;
        private static readonly Func<IEnumerable<long?>, long?> Count_Max = Enumerable.Max;
        private static readonly Func<IEnumerable<double?>, double?> Total_Sum = Enumerable.Sum;
        private static readonly Func<IEnumerable<double?>, double?> Total_Max = Enumerable.Max;

        public virtual MetricModel Aggregate(Metric source)
        {
            if (!source.MetricValues.Any())
            {
                throw new ArgumentException("values cannot be empty", "values");
            }

            var metric = new MetricModel
            {
                Name = source.Name.Value,
                TimeStamp = source.MetricValues.Max(v => v.TimeStamp),
                Count = GetLongValueOrNull(source.MetricValues.Select(v => v.Count), source.IsCumulative ? Count_Max : Count_Sum),
                Total = GetDoubleValueOrNull(source.MetricValues.Select(v => v.Total), source.IsCumulative ? Total_Max : Total_Sum),
                Average = GetDoubleValueOrNull(source.MetricValues.Select(v => v.Average), Enumerable.Average),
                Maximum = GetDoubleValueOrNull(source.MetricValues.Select(v => v.Maximum), Enumerable.Max),
                Minimum = GetDoubleValueOrNull(source.MetricValues.Select(v => v.Minimum), Enumerable.Min),
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
