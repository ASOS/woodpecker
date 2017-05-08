using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Woodpecker.Core.DocumentDb;
using Woodpecker.Core.DocumentDb.Infrastructure;
using Woodpecker.Core.DocumentDb.Model;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class MetricCollectionServiceShould
    {
        private static readonly DateTime StartUtc = DateTime.Now;
        private static readonly DateTime EndUtc = DateTime.Now.AddHours(1);

        private readonly IMonitoringResourceService fakeMonitoringResource;
        private readonly MetricsAggregator fakeMetricsAggregator;
        private readonly IMetricCollectionService sut;

        private readonly IMetricsInfo fakeMetricsInfo;

        public MetricCollectionServiceShould()
        {
            this.fakeMonitoringResource = A.Fake<IMonitoringResourceService>();
            this.fakeMetricsAggregator = A.Fake<MetricsAggregator>();
            this.fakeMetricsInfo = A.Fake<IMetricsInfo>();
            this.sut = new MetricCollectionService(this.fakeMonitoringResource, this.fakeMetricsAggregator);
        }

        [Fact]
        public async Task Return_Aggregated_Metrics_From_MonitoringResourceService()
        {
            // Arrange
            var response = CreateResponse(nMetrics: 2, nValues: 2);
            A.CallTo(() => this.fakeMonitoringResource.FetchMetrics(StartUtc, EndUtc, this.fakeMetricsInfo)).Returns(response);

            var expectedMetrics = SetupFakeMetricsAggregator(response);

            // Act
            var actualMetrics = await this.sut.CollectMetrics(StartUtc, EndUtc, this.fakeMetricsInfo);

            // Assert
            Assert.Equal(expectedMetrics, actualMetrics);
        }

        [Fact]
        public async Task Not_Aggregate_Metrics_That_Dont_Have_Values()
        {
            var response = CreateResponse(nMetrics: 1, nValues: 0);
            A.CallTo(() => this.fakeMonitoringResource.FetchMetrics(StartUtc, EndUtc, this.fakeMetricsInfo)).Returns(response);

            // Act
            await this.sut.CollectMetrics(StartUtc, EndUtc, fakeMetricsInfo);

            // assert
            A.CallTo(() => this.fakeMetricsAggregator.Aggregate(A<string>.Ignored, A<MetricValue[]>.Ignored)).MustNotHaveHappened();
        }

        [Fact]
        public async Task Return_EmptyEnumerable_When_No_Metrics_Have_Values()
        {
            var response = CreateResponse(nMetrics: 1, nValues: 0);
            A.CallTo(() => this.fakeMonitoringResource.FetchMetrics(StartUtc, EndUtc, this.fakeMetricsInfo)).Returns(response);

            // Act
            var actualMetrics = await this.sut.CollectMetrics(StartUtc, EndUtc, fakeMetricsInfo);

            // assert
            Assert.Equal(0, actualMetrics.Count());
        }

        [Fact]
        public async void Collaborate_With_The_Monitoring_Resource_Service_SuccessFully()
        {
            //// Arrange
            //var expectedResourceUri = "DocumentDbResourceUri";
            //var expectedFilter = "(name.value eq 'Metric1' or name.value eq 'Metric2') and endTime eq 0001-01-01T00:00:00.000Z and startTime eq 0001-01-01T00:00:00.000Z and timeGrain eq duration'PT1M'";
            //var expectedMetricResponse = new MetricsResponse();
            //var metrics = new string[] {"Metric1", "Metric2"};


            //A.CallTo(() => this.fakeMetricCollectionInfo.MetricsToGather).Returns(metrics);
            //A.CallTo(() => this.fakeMetricCollectionInfo.ResourceId).Returns(expectedResourceUri);
            //A.CallTo(() => this.fakeMonitoringResource.GetMetrics(A<string>._, A<string>._)).Returns(expectedMetricResponse);


            //// Act
            //var actual = await this.sut.CollectMetrics(new DateTime(), new DateTime()).ConfigureAwait(false);

            //// Assert
            //A.CallTo(() => this.fakeMonitoringResource.GetMetrics(expectedResourceUri, expectedFilter)).MustHaveHappened(Repeated.Exactly.Once);
            //actual.ShouldBeEquivalentTo(expectedMetricResponse);
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
                A.CallTo(() => this.fakeMetricsAggregator.Aggregate(metric.Name.Value, metric.MetricValues))
                    .Returns(model);

                models.Add(model);
            }

            return models;
        }
    }
}