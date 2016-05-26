using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.Sql;
using Xunit;

namespace Woodpecker.Core.Tests
{
    public class AzureSqlDvmIntegrationTests
    {
        private string _connectionString;
        

        public AzureSqlDvmIntegrationTests()
        {
            _connectionString =
                Environment.GetEnvironmentVariable("AzureSqlDvmConnectionStringForTest", EnvironmentVariableTarget.Machine);

            if (string.IsNullOrEmpty(_connectionString))
                _connectionString = Environment.GetEnvironmentVariable("AzureSqlDvmConnectionStringForTest", EnvironmentVariableTarget.User);

            if(string.IsNullOrEmpty(_connectionString))
                throw new InvalidProgramException("Please set env var for the test");
        }

        [Fact]
        public void Test_PerfGetCaptured()
        {
            var pecker = new AzureSqlDvmPerfPecker();
            var entities = pecker.PeckAsync(new PeckSource()
            {
                SourceConnectionString = _connectionString
            }).Result.ToArray();

            Assert.NotEmpty(entities);
            var first = entities.First();
            foreach (var entity in entities)
            {
                Assert.NotNull(entity.PartitionKey);
                Assert.NotNull(entity.RowKey);
                Assert.StartsWith("25", entity.PartitionKey);
                Assert.EndsWith("999", entity.PartitionKey);
                Assert.Equal(first.PartitionKey, entity.PartitionKey);
            }

        }

        [Fact]
        public void Test_ResourceGetCaptured()
        {
            var pecker = new AzureSqlDvmResourcePecker();
            var entities = pecker.PeckAsync(new PeckSource()
            {
                SourceConnectionString = _connectionString
            }).Result.ToArray();

            Assert.NotEmpty(entities);
            var entity = (DynamicTableEntity) entities.Single();
            var builder = new SqlConnectionStringBuilder(_connectionString);
            Assert.Contains(entity.Properties["server_name"].StringValue, builder.DataSource);
            Assert.Equal(builder.InitialCatalog, entity.Properties["database_name"].StringValue);
            Assert.NotNull(entity.PartitionKey);
            Assert.NotNull(entity.RowKey);
            Assert.StartsWith("25", entity.PartitionKey);
            Assert.EndsWith("999", entity.PartitionKey);
        }

        [Fact]
        public void Test_ExecStatsGetCaptured()
        {
            var pecker = new AzureSqlDvmExecutionStatsPecker();
            var entities = pecker.PeckAsync(new PeckSource()
            {
                SourceConnectionString = _connectionString
            }).Result.ToArray();

            Assert.NotEmpty(entities);
            var first = entities.First();
            foreach (var entity in entities)
            {
                Assert.NotNull(entity.PartitionKey);
                Assert.NotNull(entity.RowKey);
                Assert.StartsWith("25", entity.PartitionKey);
                Assert.EndsWith("999", entity.PartitionKey);
                Assert.Equal(first.PartitionKey, entity.PartitionKey);
                Trace.WriteLine(((DynamicTableEntity)entity).Properties["sqldb_stats_plan_handle"].StringValue);
            }
        }

        [Fact]
        public void Test_WaitsGetCaptured()
        {
            var pecker = new AzureSqlDvmWaitPecker();
            var entities = pecker.PeckAsync(new PeckSource()
            {
                SourceConnectionString = _connectionString
            }).Result.ToArray();

            Assert.NotEmpty(entities);
            var first = entities.First();
            foreach (var entity in entities)
            {
                Assert.NotNull(entity.PartitionKey);
                Assert.NotNull(entity.RowKey);
                Assert.StartsWith("25", entity.PartitionKey);
                Assert.EndsWith("999", entity.PartitionKey);
                Assert.Equal(first.PartitionKey, entity.PartitionKey);
            }
        }
    }
}
