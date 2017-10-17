using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Woodpecker.Core.Metrics.Infrastructure;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class UriFactoryShould
    {
        private readonly Uri testUri;

        public UriFactoryShould()
        {
             this.testUri = new Uri("https://management.azure.com");
        }

        [Fact]
        public void Collaborate_With_The_Monitoring_Resource_Client_Successfully()
        {
            // Arrange
            var startTime = new DateTime(2016, 10, 5);
            var endTime = new DateTime(2016, 11, 5);
            Uri expectedUri = new Uri("https://management.azure.com//metrics?api-version=2014-04-01&$filter=(name.value eq 'Metric1' or name.value eq 'Metric2') and endTime eq 2016-11-05T00:00:00.000Z and startTime eq 2016-10-05T00:00:00.000Z and timeGrain eq duration'PT1M'");

            var fakeMetricsInfo = A.Fake<IMetricsRequest>();
            A.CallTo(() => fakeMetricsInfo.MetricsToGather).Returns(new[] {"Metric1", "Metric2"});
            A.CallTo(() => fakeMetricsInfo.ResourceId).Returns(this.testUri.ToString());
            A.CallTo(() => fakeMetricsInfo.StartTimeUtc).Returns(startTime);
            A.CallTo(() => fakeMetricsInfo.EndTimeUtc).Returns(endTime);


            // Act
            var actualUri = UriFactory.CreateMonitoringUriWithMetricFilter(fakeMetricsInfo);


            // Assert
            actualUri.ShouldBeEquivalentTo(expectedUri);
        }
    }
}
