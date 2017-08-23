using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Woodpecker.Core.Metrics.Infrastructure;
using Woodpecker.Core.Metrics.Model;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class MetricCollectionServiceShould
    {
        private static readonly DateTime StartUtc = DateTime.Now;
        private static readonly DateTime EndUtc = DateTime.Now.AddHours(1);

        private readonly IMonitoringApiClient fakeMonitoringResource;
        private readonly MetricsAggregator fakeMetricsAggregator;
        private readonly IMetricCollectionService sut;

        private readonly IMetricsRequest _fakeMetricsRequest;

        public MetricCollectionServiceShould()
        {
            this.fakeMonitoringResource = A.Fake<IMonitoringApiClient>();
            this.fakeMetricsAggregator = A.Fake<MetricsAggregator>();
            this._fakeMetricsRequest = BuildMetricRequest();
            this.sut = new MetricCollectionService(this.fakeMonitoringResource, this.fakeMetricsAggregator);
        }

        private IMetricsRequest BuildMetricRequest()
        {
            var fake = A.Fake<IMetricsRequest>();
            A.CallTo(() => fake.StartTimeUtc).Returns(StartUtc);
            A.CallTo(() => fake.EndTimeUtc).Returns(EndUtc);
            return fake;
        }

        [Fact]
        public async Task Return_Aggregated_Metrics_From_MonitoringResourceService()
        {
            // Arrange
            var response = CreateResponse(nMetrics: 2, nValues: 2);
            A.CallTo(() => this.fakeMonitoringResource.FetchMetrics(this._fakeMetricsRequest)).Returns(response);

            var expectedMetrics = SetupFakeMetricsAggregator(response);

            // Act
            var actualMetrics = await this.sut.CollectMetrics(this._fakeMetricsRequest);

            // Assert
            Assert.Equal(expectedMetrics, actualMetrics);
        }

        [Fact]
        public async Task Not_Aggregate_Metrics_That_Dont_Have_Values()
        {
            var response = CreateResponse(nMetrics: 1, nValues: 0);
            A.CallTo(() => this.fakeMonitoringResource.FetchMetrics(this._fakeMetricsRequest)).Returns(response);

            // Act
            await this.sut.CollectMetrics(_fakeMetricsRequest);

            // assert
            A.CallTo(() => this.fakeMetricsAggregator.Aggregate(A<Metric>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Return_EmptyEnumerable_When_No_Metrics_Have_Values()
        {
            var response = CreateResponse(nMetrics: 1, nValues: 0);
            A.CallTo(() => this.fakeMonitoringResource.FetchMetrics(this._fakeMetricsRequest)).Returns(response);

            // Act
            var actualMetrics = await this.sut.CollectMetrics(_fakeMetricsRequest);

            // assert
            Assert.Equal(0, actualMetrics.Count());
        }

        private MetricsResponse CreateResponse(int nMetrics, int nValues)
        {
            return new MetricsResponse
            {
                Metrics = Enumerable.Range(1, nMetrics)
                    .Select(i => new Metric
                    {
                        Name = new LocalizedString
                        {
                            Value = string.Format("Metric {0}", i)
                        },
                        MetricValues = Enumerable.Range(1, nValues)
                                                 .Select(j => new MetricValue()).ToArray()
                    }).ToArray()
            };
        }

        private IEnumerable<MetricModel> SetupFakeMetricsAggregator(MetricsResponse response)
        {
            var models = new List<MetricModel>();

            foreach (var metric in response.Metrics)
            {
                var model = new MetricModel();
                A.CallTo(() => this.fakeMetricsAggregator.Aggregate(metric))
                    .Returns(model);

                models.Add(model);
            }

            return models;
        }
    }
}