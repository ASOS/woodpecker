using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Woodpecker.Core.DocumentDb;
using Woodpecker.Core.DocumentDb.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Woodpecker.Core.Tests
{
    public class DocumentDbIntegrationTests
    {
        private readonly string _connectionString;

        public DocumentDbIntegrationTests()
        {
            _connectionString =
                Environment.GetEnvironmentVariable("DocumentDbMetricsConnectionStringForTest", EnvironmentVariableTarget.Machine);

            if (string.IsNullOrEmpty(_connectionString))
                _connectionString = Environment.GetEnvironmentVariable("DocumentDbMetricsConnectionStringForTest", EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidProgramException("Please set env var for the test");
        }

        [Fact]
        public async Task CanReadDocumentDbMetrics()
        {
            // arrange
            var pecker = new DocumentDbSourcePecker();
            var source = new PeckSource
            {
                SourceConnectionString = _connectionString,
                LastOffset = DateTimeOffset.UtcNow.AddHours(-1),
                IntervalMinutes = 30,
                Name = "TheSourceName"
            };

            // act
            var entities = await pecker.PeckAsync(source);

            // assert
            Assert.NotEmpty(entities);

            foreach (var entity in entities)
            {
                Assert.NotNull(entity.PartitionKey);
                Assert.NotNull(entity.RowKey);
            }
        }
    }
}
