using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.WindowsAzure.Storage.Table;

namespace Woodpecker.Core.Sql
{
    public class AzureSqlDvmResourcePecker : ISourcePecker
    {
        public async Task<IEnumerable<ITableEntity>> PeckAsync(PeckSource source)
        {
            var results = new List<ITableEntity>();
            var connection = new SqlConnection(source.SourceConnectionString);
            try
            {
                await connection.OpenAsync();
                var dtu = (await connection.QueryAsync(DatabaseResourceInlineSqlNotSoBad)).FirstOrDefault();
                if (dtu != null)
                    results.Add(BuildDtuResult(dtu));
                
                return results;
            }
            finally
            {
                connection.Close();
            }
        }

        private const string DatabaseResourceInlineSqlNotSoBad = @"
SELECT
     GETUTCDATE() as collection_time_utc,
     @@SERVERNAME as server_name,
     DB_NAME() as database_name,
     MAX(avg_cpu_percent) as avg_cpu_percent,
     MAX(avg_data_io_percent) as avg_data_io_percent,
     MAX(avg_log_write_percent) as avg_log_write_percent,
     MAX(avg_memory_usage_percent) as avg_memory_usage_percent,
     MAX(xtp_storage_percent) as xtp_storage_percent,
     MAX(max_worker_percent) as max_worker_percent,
     MAX(max_session_percent) as max_session_percent,
     MAX(dtu_limit) as  dtu_limit
FROM sys.dm_db_resource_stats
WHERE end_time > DATEADD(MINUTE,-1,GETUTCDATE())"; // TODO: In the future we might need to pass the variable to DATEADD

        private ITableEntity BuildDtuResult(dynamic record)
        {
            var ofsted = new DateTimeOffset(record.collection_time_utc, TimeSpan.Zero);
            var minuteOffset = new DateTimeOffset(DateTime.Parse(ofsted.UtcDateTime.ToString("yyyy-MM-dd HH:mm:00")), TimeSpan.Zero);
            var shardKey = (DateTimeOffset.MaxValue.Ticks - minuteOffset.Ticks).ToString("D19");
            var dtuResult = new DynamicTableEntity(shardKey, string.Format("{0}_{1}", record.server_name, record.database_name));

            dtuResult.Properties.Add("collection_time_utc",
                EntityProperty.GeneratePropertyForDateTimeOffset(ofsted));
            dtuResult.Properties.Add("server_name", EntityProperty.GeneratePropertyForString(record.server_name));
            dtuResult.Properties.Add("database_name", EntityProperty.GeneratePropertyForString(record.database_name));
            dtuResult.Properties.Add("avg_cpu_percent", EntityProperty.GeneratePropertyForDouble((double)record.avg_cpu_percent));
            dtuResult.Properties.Add("avg_log_write_percent", EntityProperty.GeneratePropertyForDouble((double)record.avg_log_write_percent));
            dtuResult.Properties.Add("avg_memory_usage_percent", EntityProperty.GeneratePropertyForDouble((double)record.avg_memory_usage_percent));
            dtuResult.Properties.Add("xtp_storage_percent", EntityProperty.GeneratePropertyForDouble((double)record.xtp_storage_percent));
            dtuResult.Properties.Add("max_worker_percent", EntityProperty.GeneratePropertyForDouble((double)record.max_worker_percent));
            dtuResult.Properties.Add("max_session_percent", EntityProperty.GeneratePropertyForDouble((double)record.max_session_percent));
            dtuResult.Properties.Add("dtu_limit", EntityProperty.GeneratePropertyForInt(record.dtu_limit));

            return dtuResult;
        }
    }
}
