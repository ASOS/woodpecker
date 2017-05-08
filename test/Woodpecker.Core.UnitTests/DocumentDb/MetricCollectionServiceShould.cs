using System;
using FakeItEasy;
using FluentAssertions;
using Woodpecker.Core.DocumentDb.Infrastructure;
using Woodpecker.Core.DocumentDb.Model;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class MetricCollectionServiceShould
    {
        private readonly IMetricsInfo fakeMetricCollectionInfo;
        private readonly IMonitoringResourceService fakeMonitoringResource;
        private readonly IMetricCollectionService sut;


        public MetricCollectionServiceShould()
        {
            this.fakeMonitoringResource = A.Fake<IMonitoringResourceService>();
            this.fakeMetricCollectionInfo = A.Fake<IMetricsInfo>();
            this.sut = new MetricCollectionService(this.fakeMonitoringResource);
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
    }
}