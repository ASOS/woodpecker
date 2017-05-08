using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Woodpecker.Core.DocumentDb.Infrastructure;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class MonitoringResourceClientShould
    {
        private readonly IHttpClient fakeHttpClient;
        private ISecurityTokenProvider fakeSecurityTokenProvider;
        private IMonitoringResourceClient sut;
        private Uri testUri;

        public MonitoringResourceClientShould()
        {
            this.testUri = new Uri("https://management.azure.com");
            this.fakeHttpClient = A.Fake<IHttpClient>();
            this.fakeSecurityTokenProvider = A.Fake<ISecurityTokenProvider>();
            //SetupFakeHttpResponseMessage(new HttpResponseMessage(HttpStatusCode.OK) {Content = new httpC});

            this.sut = new MonitoringResourceClient(this.fakeSecurityTokenProvider,this.fakeHttpClient);
        }

        private void SetupFakeHttpResponseMessage()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task Collaborate_With_The_Security_Token_Provider()
        {

            //Arrange
            A.CallTo(() => this.fakeSecurityTokenProvider.GetSecurityTokenAsync()).Returns("1234accessToken");
            A.CallTo(() => this.fakeHttpClient.SendAsync(A<HttpRequestMessage>._)).Returns(new HttpResponseMessage());

            // Act
            await this.sut.GetResponse(this.testUri).ConfigureAwait(false);


            // Assert
            A.CallTo(() => this.fakeHttpClient.SendAsync(A<HttpRequestMessage>.That.Matches(x => ShouldMatchAccessToken(x, "Bearer 1234accessToken")))).MustHaveHappened(Repeated.Exactly.Once);
        }

        private static bool ShouldMatchHttpClient(HttpRequestMessage x, Uri expectedRequestUri)
        {
            try
            {
                x.RequestUri.ShouldBeEquivalentTo(expectedRequestUri);
                return true;
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
                accessToken.ShouldBeEquivalentTo(httpRequestMessage.Headers.Authorization.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}