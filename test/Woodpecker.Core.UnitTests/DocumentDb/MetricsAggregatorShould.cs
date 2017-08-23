using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Woodpecker.Core.Metrics.Infrastructure;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class MetricsAggregatorShould
    {
        private readonly MetricsAggregator sut = new MetricsAggregator();

        private static readonly LocalizedString MetricName = new LocalizedString { Value = "The metric!" };

        [Fact]
        public void Throw_ArgumentException_If_Empty_Values()
        {
            // arrange
            var values = new MetricValue[0];
            var metric = new Metric { Name = MetricName, MetricValues = values };

            // act & assert
            Assert.Throws<ArgumentException>(() => sut.Aggregate(metric));
        }

        [Fact]
        public void Return_Metric_With_Given_Name()
        {
            // arrange
            var expectedName = MetricName;
            var values = new MetricValue[] { new MetricValue() };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedName.Value, metric.Name);
        }

        [Fact]
        public void Return_Metric_With_Latest_Timestamp()
        {
            // arrange
            var firstTimeStamp = DateTime.Now;
            var latestTimeStamp = DateTime.Now.AddMinutes(1);

            var values = new MetricValue[]
            {
                new MetricValue { TimeStamp = firstTimeStamp },
                new MetricValue { TimeStamp =  latestTimeStamp}
            };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(latestTimeStamp, metric.TimeStamp);
        }

        [Fact]
        public void Return_Metric_With_Max_Of_Counts_For_Cumulative_MetricValues()
        {
            // arrange
            var values = Enumerable.Range(0, 10).Select(i => new MetricValue { Count = i, Total = 1, Average = 1 }).ToArray();

            var expectedCount = values.Max(v => v.Count);

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedCount, metric.Count);
        }

        [Fact]
        public void Return_Metric_With_Sum_Of_Counts_For_Non_Cumulative_MetricValues()
        {
            // arrange
            var values = Enumerable.Range(0, 10).Select(i => new MetricValue { Count = i }).ToArray();

            var expectedCount = values.Sum(v => v.Count);

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedCount, metric.Count);
        }

        [Fact]
        public void Ignore_Nulls_Count_When_Aggregating()
        {
            // arrange
            var expectedCount = 5;

            var values = new MetricValue[] { new MetricValue { Count = 5 }, new MetricValue { Count = null } };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedCount, metric.Count);
        }

        [Fact]
        public void Return_Metric_With_Null_Count_If_All_Values_Have_Null_Count()
        {
            // arrange
            var values = new MetricValue[] { new MetricValue { Count = null }, new MetricValue { Count = null } };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Null(metric.Count);
        }

        [Fact]
        public void Return_Metric_With_Max_Of_Maximums()
        {
            // arrange
            var values = Enumerable.Range(0, 10).Select(i => new MetricValue { Maximum = i }).ToArray();

            var expectedMaximum = values.Max(v => v.Maximum);

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedMaximum, metric.Maximum);
        }

        [Fact]
        public void Ignore_Nulls_Maximum_When_Aggregating()
        {
            // arrange
            var expectedMaximum = 5;

            var values = new MetricValue[] { new MetricValue { Maximum = 5 }, new MetricValue { Maximum = null } };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedMaximum, metric.Maximum);
        }

        [Fact]
        public void Return_Metric_With_Null_Maximum_If_All_Values_Have_Null_Maximum()
        {
            // arrange
            var values = new MetricValue[] { new MetricValue { Maximum = null }, new MetricValue { Maximum = null } };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Null(metric.Maximum);
        }

        [Fact]
        public void Return_Metric_With_Min_Of_Minimums()
        {
            // arrange
            var values = Enumerable.Range(0, 10).Select(i => new MetricValue { Minimum = i }).ToArray();

            var expectedMinimum = values.Min(v => v.Minimum);

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedMinimum, metric.Minimum);
        }

        [Fact]
        public void Ignore_Nulls_Minimum_When_Aggregating()
        {
            // arrange
            var expectedMinimum = 5;

            var values = new MetricValue[] { new MetricValue { Minimum = 5 }, new MetricValue { Minimum = null } };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedMinimum, metric.Minimum);
        }

        [Fact]
        public void Return_Metric_With_Null_Minimum_If_All_Values_Have_Null_Minimum()
        {
            // arrange
            var values = new MetricValue[] { new MetricValue { Minimum = null }, new MetricValue { Minimum = null } };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Null(metric.Minimum);
        }

        [Fact]
        public void Return_Metric_With_Avg_Of_Averages()
        {
            // arrange
            var values = Enumerable.Range(0, 10).Select(i => new MetricValue { Average = i }).ToArray();

            var expectedAverage = values.Average(v => v.Average);

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedAverage, metric.Average);
        }

        [Fact]
        public void Ignore_Nulls_Average_When_Aggregating()
        {
            // arrange
            var expectedAverage = 5;

            var values = new MetricValue[] { new MetricValue { Average = 5 }, new MetricValue { Average = null } };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedAverage, metric.Average);
        }

        [Fact]
        public void Return_Metric_With_Null_Average_If_All_Values_Have_Null_Average()
        {
            // arrange
            var values = new MetricValue[] { new MetricValue { Average = null }, new MetricValue { Average = null } };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Null(metric.Average);
        }

        [Fact]
        public void Return_Metric_With_Max_Of_Totals_For_Cumulative_MetricValues()
        {
            // arrange
            var values = Enumerable.Range(0, 10).Select(i => new MetricValue { Total = i, Average = 1, Count = 1 }).ToArray();

            var expectedTotal = values.Max(v => v.Total);

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedTotal, metric.Total);
        }

        [Fact]
        public void Return_Metric_With_Sum_Of_Totals_For_NonCumulative_MetricValues()
        {
            // arrange
            var values = Enumerable.Range(0, 10).Select(i => new MetricValue { Total = i }).ToArray();
            var expectedTotal = values.Sum(v => v.Total);

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedTotal, metric.Total);
        }

        [Fact]
        public void Ignore_Nulls_Total_When_Aggregating()
        {
            // arrange
            var expectedTotal = 5;

            var values = new MetricValue[] { new MetricValue { Total = 5 }, new MetricValue { Total = null } };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Equal(expectedTotal, metric.Total);
        }

        [Fact]
        public void Return_Metric_With_Null_Total_If_All_Values_Have_Null_Total()
        {
            // arrange
            var values = new MetricValue[] { new MetricValue { Total = null }, new MetricValue { Total = null } };

            // act
            var metric = sut.Aggregate(new Metric { Name = MetricName, MetricValues = values });

            // assert
            Assert.Null(metric.Total);
        }
    }
}
