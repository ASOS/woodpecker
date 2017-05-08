using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Woodpecker.Core.DocumentDb.Infrastructure;
using Woodpecker.Core.DocumentDb.Model;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class MonitoringResourceServiceShould
    {
        private readonly MonitoringApiClient sut;
        private readonly ISecurityTokenProvider fakeSecurityTokenProvider;
        private readonly IMonitoringResourceClient fakemonitoringResourceClient;
        private readonly Uri fakeMonitoringApiBaseUri = new Uri("https://management.azure.com");

        public MonitoringResourceServiceShould()
        {
            fakeSecurityTokenProvider = A.Fake<ISecurityTokenProvider>();
            fakemonitoringResourceClient = A.Fake<IMonitoringResourceClient>();
            // this.sut = new MonitoringApiClient(fakeSecurityTokenProvider, fakemonitoringResourceClient);
        }


        //    [Fact]
        //    public async Task Collaborate_With_The_Security_TokenProvider_When_GettingMetrics()
        //    {
        //        //Act
        //        await this.sut.GetMetrics(string.Empty, string.Empty);

        //        //Assert
        //        A.CallTo(() => fakeSecurityTokenProvider.GetSecurityTokenAsync()).MustHaveHappened(Repeated.Exactly.Once);
        //    }

        //    [Fact]
        //    public async Task Collaborate_With_The_HttpClient_When_Resource_Is_Passed_In()
        //    {
        //        // Arrange
        //        var resource = "Someuri";
        //        var filter = string.Empty;

        //        // Act
        //        await this.sut.GetMetrics(resource, filter);


        //        // Assert
        //        var expectedRequestUri = BuildUri(resource, filter);

        //        A.CallTo(
        //            () => this.fakemonitoringResourceClient.GetResponse<MetricsResponse>(A<HttpRequestMessage>.That.Matches(x => ShouldMatchHttpClient(x, expectedRequestUri)))).MustHaveHappened(Repeated.Exactly.Once);
        //    }

        //    [Fact]
        //    public async Task Collaborate_With_The_HttpClient_When_Filter_Is_Passed_In()
        //    {
        //        // Arrange
        //        var resource = "Someuri";
        //        var filter = "SomeFilter";

        //        // Act
        //        await sut.GetMetrics(resource, filter);

        //        // Assert
        //        var expectedRequestUri = BuildUri(resource, filter);

        //        A.CallTo(
        //            () => this.fakemonitoringResourceClient.GetResponse<MetricsResponse>(A<HttpRequestMessage>.That.Matches(x => ShouldMatchHttpClient(x, expectedRequestUri)))).MustHaveHappened(Repeated.Exactly.Once);
        //    }

        //    [Fact]
        //    public async Task Return_Response_When_HttpClient_Is_Valid()
        //    {
        //        // Arrange
        //        var resource = "Someuri";
        //        var filter = "SomeFilter";
        //        var metricResponse = new MetricsResponse();
        //        var fakeResouceClient = A.Fake<IMonitoringResourceClient>();
        //        A.CallTo(() => fakeResouceClient.GetResponse<MetricsResponse>(A<HttpRequestMessage>._)).Returns(metricResponse);


        //        // Act
        //        await sut.GetMetrics(resource, filter);

        //        // Assert
        //        var expectedRequestUri = BuildUri(resource, filter);

        //        A.CallTo(
        //            () => this.fakemonitoringResourceClient.GetResponse<MetricsResponse>(A<HttpRequestMessage>.That.Matches(x => ShouldMatchHttpClient(x, expectedRequestUri)))).MustHaveHappened(Repeated.Exactly.Once);
        //    }


        //    private static bool ShouldMatchHttpClient(HttpRequestMessage x, Uri expectedRequestUri)
        //    {
        //        try
        //        {
        //            x.RequestUri.ShouldBeEquivalentTo(expectedRequestUri);
        //            return true;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }

        //    private Uri BuildUri(string resource, string filter)
        //    {
        //        var uri = string.Format("{0}/metrics?api-version=2014-04-01&$filter={1}", resource, filter);

        //        return new Uri(fakeMonitoringApiBaseUri, uri);
        //    }
        //}
    }
}
