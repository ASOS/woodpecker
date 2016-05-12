using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.Internal;

namespace Woodpecker.Core.Sql
{
    public class AzureSqlDvmPerfPecker : ISourcePecker
    {

        public async Task<IEnumerable<ITableEntity>> PeckAsync(PeckSource source)
        {
            var results = new List<ITableEntity>();
            var connection = new SqlConnection(source.SourceConnectionString);
            try
            {
                await connection.OpenAsync();
                results.AddRange(BuildPerfResults(await connection.QueryAsync(DatabasePerfCountersInlineSqlNotSoBad)));
                return results;
            }
            finally
            {
                connection.Close();
            }
        }

        private const string DatabasePerfCountersInlineSqlNotSoBad = @"SELECT
     GETUTCDATE() AS collection_time_utc,
     @@SERVERNAME as server_name,
     DB_NAME() as database_name,
     LTRIM(RTRIM(SUBSTRING([object_name], PATINDEX('%:%',object_name)+1,LEN([object_name])))) AS [object_name],
     LTRIM(RTRIM(counter_name)) AS counter_name,
     LTRIM(RTRIM(instance_name)) AS instance_name,
     cntr_value,
     cntr_type
FROM sys.dm_os_performance_counters
WHERE LTRIM(RTRIM(counter_name)) IN (
     'Batch Requests/sec',
     'User Connections',
     'Buffer cache hit ratio',
     'Buffer cache hit ratio base'
)";

        private IEnumerable<ITableEntity> BuildPerfResults(IEnumerable<dynamic> records)
        {
            var results = new List<DynamicTableEntity>();
            foreach (var record in records)
            {
                var ofsted = new DateTimeOffset(record.collection_time_utc, TimeSpan.Zero);
                var minuteOffset = new DateTimeOffset(DateTime.Parse(ofsted.UtcDateTime.ToString("yyyy-MM-dd HH:mm:00")), TimeSpan.Zero);
                var shardKey = (DateTimeOffset.MaxValue.Ticks - minuteOffset.Ticks).ToString("D19");
                var result = new DynamicTableEntity(shardKey, string.Format("{0}_{1}_{2}", record.server_name, record.database_name, record.cntr_type.ToString()));

                result.Properties.Add("collection_time_utc",
                    EntityProperty.GeneratePropertyForDateTimeOffset(new DateTimeOffset?(new DateTimeOffset(record.collection_time_utc, TimeSpan.Zero))));
                result.Properties.Add("server_name", EntityProperty.GeneratePropertyForString(record.server_name));
                result.Properties.Add("object_name", EntityProperty.GeneratePropertyForString(record.object_name));
                result.Properties.Add("counter_name", EntityProperty.GeneratePropertyForString(record.counter_name));
                result.Properties.Add("instance_name", EntityProperty.GeneratePropertyForString(record.instance_name));
                result.Properties.Add("cntr_value", EntityProperty.GeneratePropertyForLong(record.cntr_value));
                result.Properties.Add("cntr_type", EntityProperty.GeneratePropertyForInt(record.cntr_type));                
                results.Add(result);
            }

            return results;
        }

    }
}