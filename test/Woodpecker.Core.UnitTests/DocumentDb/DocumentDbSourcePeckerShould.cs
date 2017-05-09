using System;
using System.Collections.Generic;
using System.Globalization;
using FakeItEasy;
using Woodpecker.Core.DocumentDb;
using Woodpecker.Core.DocumentDb.Infrastructure;
using Woodpecker.Core.DocumentDb.Model;
using Woodpecker.Core.Factory;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class DocumentDbSourcePeckerShould
    {
        private readonly IMetricCollectionServiceFactory fakemetricCollectionServiceFactory;
        private ISourcePecker sut;
        private IMetricCollectionService fakemetricCollectionService;

        public DocumentDbSourcePeckerShould()
        {
            this.fakemetricCollectionService = A.Fake<IMetricCollectionService>();
            this.fakemetricCollectionServiceFactory = A.Fake<IMetricCollectionServiceFactory>();
            this.sut = new DocumentDbSourcePecker(this.fakemetricCollectionServiceFactory);
        }

        [Fact]
        public async void Collaborate_With_MetricCollectionServiceFactory_Successfully()
        {
            //// Arrange
            //var startDateTime = new DateTimeOffset(new DateTime(2016, 10, 5));
            //var endDateTime = new DateTimeOffset(new DateTime(2016, 10, 5 ,0,30,0));
            //var peckSource = GetPeckSource("TenantId=MyTestTenant1.onmicrosoft.com;ClientId=B6BC8DAF-C802-457B-B906-4E9A9483A36C;ClientSecret=cfrWa1+UAHSdas23hsdlkjf8hdiashdasd=;ResourceId=subscriptions/mySub/resourceGroups/myRG/providers/Microsoft.DocumentDB/databaseAccounts/myDBAcc/databases/myDB12==/collections/myColl234=", 
            //                               30, 
            //                               "azure-stk-documentdb", 
            //                               startDateTime);
            //var expectedMetrics = GetMetricModels();
            //SetUpMetricCollectionServiceToReturnResults(expectedMetrics);


            //// Act
            //var actual = await this.sut.PeckAsync(peckSource).ConfigureAwait(false);


            //// Assert
            //A.CallTo(() => this.fakemetricCollectionServiceFactory.Create("subscriptions/mySub/resourceGroups/myRG/providers/Microsoft.DocumentDB/databaseAccounts/myDBAcc/databases/myDB12==/collections/myColl234=", "MyTestTenant1.onmicrosoft.com", "B6BC8DAF-C802-457B-B906-4E9A9483A36C", "cfrWa1+UAHSdas23hsdlkjf8hdiashdasd=")).MustHaveHappened(Repeated.Exactly.Once);
            //A.CallTo(() => this.fakemetricCollectionService.CollectMetrics(A<DateTime>.That.Matches(x => IsDateTimeMatched(x,startDateTime)),A<DateTime>.That.Matches(x => IsDateTimeMatched(x,endDateTime)), A<DocumentDbMetricsRequest>.That.Matches(x => )))
        }

        private static IEnumerable<MetricModel> GetMetricModels()
        {
            return new List<MetricModel>()
            {
                new MetricModel()
                {
                    Name = "doc db",
                    Total = 4,
                    TimeStamp = new DateTime(2016,10,5),
                    Minimum = 1,
                    Count = 3,
                    Average = 2,
                    Maximum = 5
                }
            };
        }

        private void SetUpMetricCollectionServiceToReturnResults(IEnumerable<MetricModel> metricModels)
        {
            A.CallTo(() => this.fakemetricCollectionService.CollectMetrics(A<IMetricsRequest>._)).Returns(metricModels);
        }


        private static PeckSource GetPeckSource(string connectionString, int intervalInMinutes, string name,DateTimeOffset dateTimeOffset)
        {
            return new PeckSource()
            {
                Name = name,
                SourceConnectionString = connectionString,
                IntervalMinutes = intervalInMinutes,
                LastOffset = dateTimeOffset
            };
        }
    }
}