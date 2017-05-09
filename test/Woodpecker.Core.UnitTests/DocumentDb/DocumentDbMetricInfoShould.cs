using System;
using Woodpecker.Core.DocumentDb.Infrastructure;
using Xunit;

namespace Woodpecker.Core.UnitTests.DocumentDb
{
    public class DocumentDbMetricInfoShould
    {
        private string subscriptionId;
        private string resourceGroupName;
        private string documentDbAccount;
        private string databaseId;
        private string collectionId;

        public DocumentDbMetricInfoShould()
        {
            this.subscriptionId = "mytestsubscription";
            this.resourceGroupName = "testresourcegroup";
            this.documentDbAccount = "testdocdbaccount";
            this.databaseId = "myDatabase";
            this.collectionId = "myCollection";
        }

        [Fact]
        public void Return_The_Resource_Uri()
        {
            // Arrange
            this.subscriptionId = "mytestsubscription";
            this.resourceGroupName = "testresourcegroup";
            this.documentDbAccount = "testdocdbaccount";
            this.databaseId = "myDatabase";
            this.collectionId = "myCollection";

            var expectedResourceId = "subscriptions/mytestsubscription/resourceGroups/testresourcegroup/providers/Microsoft.DocumentDB/databaseAccounts/testdocdbaccount/databases/myDatabase/collections/myCollection";


            // Act
            var sut = this.Sut(expectedResourceId);
            var actual = sut.ResourceId;

            // Assert
            Assert.Equal(actual, expectedResourceId);
        }

        [Fact]
        public void Gather_A_Pre_Determined_Set_Of_Metrics()
        {
            // Arrange
            var expectedResourceId = "subscriptions/mytestsubscription/resourceGroups/testresourcegroup/providers/Microsoft.DocumentDB/databaseAccounts/testdocdbaccount/databases/myDatabase/collections/myCollection";
            var expected = new string[]
            {
                "Available Storage",
                "Average Requests per Second",
                "Data Size",
                "Document Count",
                "Index Size",
                "Max RUs Per Second",
                "Observed Read Latency",
                "Observed Write Latency",
                "Throttled Requests",
                "Total Request Units",
                "Total Requests"
            };


            // Act
            var sut = this.Sut(expectedResourceId);

            var actual = sut.MetricsToGather;

            // Assert
            Assert.Equal(actual, expected);

        }

        [Fact]
        public void Set_The_Start_Time_And_End_Time_On_The_Object()
        {
            // Arrange
            var startTime = new DateTime(2016,10,5);
            var endTime = new DateTime(2016,11,5);


            // Act
            var sut = this.Sut("",startTime,endTime);
            

            // Assert
            Assert.Equal(sut.StartTimeUtc, startTime);
            Assert.Equal(sut.EndTimeUtc, endTime);
        }

        private IMetricsRequest Sut(string resourceId,DateTime startTimeUtc = default(DateTime),DateTime endTimeUtc = default(DateTime))
        {
            return new DocumentDbMetricsRequest(resourceId,startTimeUtc, endTimeUtc);
        }
    }
}