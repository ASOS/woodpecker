using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json;
using Woodpecker.Core.Metrics.Infrastructure;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class MonitoringResourceClientShould
    {
        private readonly IHttpClient fakeHttpClient;
        private readonly ISecurityTokenProvider fakeSecurityTokenProvider;
        private readonly IMonitoringApiClient sut;
        private readonly Uri testUri;
        private readonly IMetricsRequest fakeMetricRequest;

        public MonitoringResourceClientShould()
        {
            this.testUri = new Uri("https://management.azure.com");
            this.fakeHttpClient = A.Fake<IHttpClient>();
            this.fakeSecurityTokenProvider = A.Fake<ISecurityTokenProvider>();
            this.fakeMetricRequest = BuildMetricRequest();
            SetupFakeHttpResponseMessage(new HttpResponseMessage(HttpStatusCode.OK) { Content = GetTestContent(new MetricsResponse() { Metrics = GetMetricValues() }) });

            this.sut = new MonitoringApiClient(this.fakeSecurityTokenProvider, this.fakeHttpClient);
        }

        private IMetricsRequest BuildMetricRequest()
        {
            var fake = A.Fake<IMetricsRequest>();
            A.CallTo(() => fake.MetricsToGather).Returns(new string[] { "Metric1" });
            A.CallTo(() => fake.ResourceId).Returns("ResourceId");
            A.CallTo(() => fake.StartTimeUtc).Returns(new DateTime(2016, 10, 5));
            A.CallTo(() => fake.EndTimeUtc).Returns(new DateTime(2016, 11, 5));
            return fake;
        }

        private HttpContent GetTestContent(MetricsResponse metricsResponse)
        {
            var a = JsonConvert.SerializeObject(metricsResponse);
            return new StringContent(a, Encoding.UTF8, "application/json");
        }

        private Metric[] GetMetricValues()
        {
            return new Metric[]
            {
                new Metric() {MetricValues = new MetricValue[] {new MetricValue()}}
            };
        }

        private void SetupFakeHttpResponseMessage(HttpResponseMessage responseMessage)
        {
            A.CallTo(() => this.fakeHttpClient.SendAsync(A<HttpRequestMessage>._)).Returns(responseMessage);
        }

        [Fact]
        public async Task Collaborate_With_The_Security_Token_Provider_Successfully()
        {
            //Arrange
            A.CallTo(() => this.fakeSecurityTokenProvider.GetSecurityTokenAsync()).Returns("1234accessToken");


            // Act
            await this.sut.FetchMetrics(this.fakeMetricRequest);


            // Assert
            A.CallTo(() => this.fakeHttpClient.SendAsync(A<HttpRequestMessage>.That.Matches(x => ShouldMatchAccessToken(x, "Bearer 1234accessToken")))).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => this.fakeSecurityTokenProvider.GetSecurityTokenAsync()).MustHaveHappened(Repeated.Exactly.Once);
        }


        [Fact]
        public async Task Collaborate_With_The_Http_Client_Successfully()
        {
            // Arrange
            var expectedHttpRequest = new HttpRequestMessage(HttpMethod.Get, this.testUri);
            var expectedResponse = new MetricsResponse() { Metrics = new Metric[] { new Metric() { MetricValues = new MetricValue[] { new MetricValue() { Average = 1, Count = 1, Maximum = 1, Minimum = 1, TimeStamp = DateTime.MaxValue, Total = 1 } } } } };
            SetupFakeHttpResponseMessage(new HttpResponseMessage() { Content = GetTestContent(expectedResponse) });


            // Act
            var actual = await this.sut.FetchMetrics(this.fakeMetricRequest).ConfigureAwait(false);


            // Assert
            A.CallTo(() => this.fakeHttpClient.SendAsync(A<HttpRequestMessage>.That.Matches(x => ShouldMatchHttpClient(x, expectedHttpRequest)))).MustHaveHappened(Repeated.Exactly.Once);
            actual.Should().BeEquivalentTo(expectedResponse);
        }

        private static bool ShouldMatchHttpClient(HttpRequestMessage actual, HttpRequestMessage expected)
        {
            try
            {
                return actual.Method.Equals(HttpMethod.Get) && actual.RequestUri.ToString().StartsWith(expected.RequestUri.ToString());
            }
            catch
            {
                return false;
            }
        }

        private static bool ShouldMatchAccessToken(HttpRequestMessage httpRequestMessage, string accessToken)
        {
            try
            {
                accessToken.Should().BeEquivalentTo(httpRequestMessage.Headers.Authorization.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}