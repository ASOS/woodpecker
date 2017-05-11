using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.DocumentDb;
using Woodpecker.Core.DocumentDb.Infrastructure;
using Woodpecker.Core.DocumentDb.Model;
using Woodpecker.Core.Factory;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class DocumentDbSourcePeckerShould
    {
        private const string ConnectionString = "TenantId=MyTestTenant1.onmicrosoft.com;ClientId=B6BC8DAF-C802-457B-B906-4E9A9483A36C;ClientSecret=cfrWa1+UAHSdas23hsdlkjf8hdiashdasd=;ResourceId=subscriptions/mySub/resourceGroups/myRG/providers/Microsoft.DocumentDB/databaseAccounts/myDBAcc/databases/myDB12==/collections/myColl234=";

        private const string ResourceId = "subscriptions/mySub/resourceGroups/myRG/providers/Microsoft.DocumentDB/databaseAccounts/myDBAcc/databases/myDB12==/collections/myColl234=";

        private readonly IMetricCollectionServiceFactory fakemetricCollectionServiceFactory;
        private ISourcePecker sut;
        private IMetricCollectionService fakemetricCollectionService;

        public DocumentDbSourcePeckerShould()
        {
            this.fakemetricCollectionService = A.Fake<IMetricCollectionService>();
            this.fakemetricCollectionServiceFactory = A.Fake<IMetricCollectionServiceFactory>();
            A.CallTo(() => this.fakemetricCollectionServiceFactory.Create(A<string>.Ignored, A<string>.Ignored, A<string>.Ignored)).Returns(this.fakemetricCollectionService);

            this.sut = new DocumentDbSourcePecker(this.fakemetricCollectionServiceFactory);
        }

        [Fact]
        public async void Return_TableEntities_For_Collected_Metrics()
        {
            // Arrange
            var startUtc = DateTime.Now;
            var intervalInMinutes = 30;
            var peckSource = GetPeckSource(ConnectionString, intervalInMinutes, "azure-stk-documentdb", startUtc);

            var expectedMetrics = SetupExpectedMetrics();

            // Act
            var actualEntities = await this.sut.PeckAsync(peckSource);

            // Assert
            AssertEntitiesMatchExpectedMetrics(actualEntities.ToArray(), expectedMetrics.ToArray());
        }

        [Fact]
        public async void Return_All_Entities_With_Same_PartitionKey()
        {
            // Arrange
            var startUtc = DateTime.Now;
            var intervalInMinutes = 30;
            var peckSource = GetPeckSource(ConnectionString, intervalInMinutes, "azure-stk-documentdb", startUtc);

            SetupExpectedMetrics();

            // Act
            var entities = await this.sut.PeckAsync(peckSource);

            // Assert
            AssertEntitiesHaveSamePartitionKey(entities);
        }

        public IEnumerable<MetricModel> SetupExpectedMetrics(int n = 10)
        {
            var startUtc = DateTime.Now;
            var intervalInMinutes = 30;
            var expectedMetrics = GetMetricModels(n);

            var expectedMetricsRequest = new DocumentDbMetricsRequest(ResourceId, startUtc, startUtc.AddMinutes(intervalInMinutes));

            A.CallTo(
                () => this.fakemetricCollectionService.CollectMetrics(A<IMetricsRequest>.That.Matches(m => IsEqualMetricsRequest(expectedMetricsRequest, (DocumentDbMetricsRequest)m))))
                .Returns(expectedMetrics);

            return expectedMetrics;
        }

        private void AssertEntitiesMatchExpectedMetrics(ITableEntity[] actualEntities, MetricModel[] expectedMetrics)
        {
            Assert.Equal(expectedMetrics.Count(), actualEntities.Count());

            for (var i = 0; i < actualEntities.Count(); ++i)
            {
                var metric = expectedMetrics[i];
                var entity = actualEntities[i] as DynamicTableEntity;

                AssertEntityMatchesExpectedMetric(entity, metric);
            }
        }

        private void AssertEntityMatchesExpectedMetric(DynamicTableEntity entity, MetricModel metric)
        {
            Assert.NotNull(entity.PartitionKey);
            Assert.NotNull(entity.RowKey);

            var metricProperties = metric.GetType().GetProperties();
            Assert.Equal(entity.Properties.Count, metricProperties.Length);

            foreach (var property in metricProperties)
            {
                Assert.True(entity.Properties.ContainsKey(property.Name));

                var metricValue = property.GetValue(metric);
                Assert.Equal(metricValue, entity.Properties[property.Name].PropertyAsObject);
            }

        }

        private void AssertEntitiesHaveSamePartitionKey(IEnumerable<ITableEntity> entities)
        {
            Assert.NotEmpty(entities);

            var first = entities.First();
            Assert.NotNull(first.PartitionKey);

            foreach (var entity in entities)
            {
                Assert.Equal(first.PartitionKey, entity.PartitionKey);
            }
        }

        private bool IsEqualMetricsRequest(DocumentDbMetricsRequest expected, DocumentDbMetricsRequest actual)
        {
            try
            {
                actual.ShouldBeEquivalentTo(expected);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static IEnumerable<MetricModel> GetMetricModels(int n)
        {
            var metrics = Enumerable.Range(0, n)
                                    .Select(i => new MetricModel
                                    {
                                        Name = string.Format("Metric {0}", i),
                                        Total = 4,
                                        TimeStamp = DateTime.UtcNow,
                                        Minimum = 1,
                                        Count = 3,
                                        Average = 2,
                                        Maximum = 5
                                    }).ToList();

            return metrics;
        }

        private static PeckSource GetPeckSource(string connectionString, int intervalInMinutes, string name, DateTimeOffset dateTimeOffset)
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