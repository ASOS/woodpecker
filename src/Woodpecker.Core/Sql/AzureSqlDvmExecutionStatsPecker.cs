using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker.Core.Sql
{
    public class AzureSqlDvmExecutionStatsPecker : AzureSqlDvmPeckerBase
    {
        private const string DatabaseGeoReplicationInlineSqlNotSoBad = @"
SELECT
    GETUTCDATE() AS collection_time_utc, -- datetime
    @@SERVERNAME AS server_name, -- string
    DB_NAME() AS database_name, -- string
    CONCAT(@@SERVERNAME,'/',DB_NAME()) AS [full_name], -- string
    'procedure' AS sqldb_stats_type, -- string
    COALESCE(OBJECT_NAME(object_id),'') AS sqldb_proc_object_name, -- string
    type AS sqldb_stats_proc_type, -- string
    '' AS sqldb_stats_querystore_id,
    execution_count AS sqldb_stats_total_execution_count, -- string
    FLOOR(total_worker_time / 1000.0) AS sqldb_stats_total_cpu_time_ms, -- int64
    total_elapsed_time AS sqldb_stats_total_elapsed_time_ms, -- int64
    total_physical_reads AS sqldb_stats_total_physical_reads_ms, -- int64
    total_logical_reads AS sqldb_stats_total_logical_reads_ms, -- int64
    total_logical_writes AS sqldb_stats_total_logical_writes_ms, -- int64
    CONVERT(VARCHAR(MAX),plan_handle,1) AS sqldb_stats_plan_handle, -- string
    '' AS sqldb_stats_query_hash
FROM sys.dm_exec_procedure_stats
WHERE OBJECT_NAME(object_id) IS NOT NULL
UNION ALL
SELECT
    GETUTCDATE() AS collection_time_utc, -- datetime
    @@SERVERNAME AS server_name, -- string
    DB_NAME() AS database_name, -- string
    CONCAT(@@SERVERNAME,'/',DB_NAME()) AS [full_name], -- string
    'query' AS sqldb_stats_type, -- string
    '' AS sqldb_proc_object_name, -- string
    '' AS sqldb_stats_proc_type, -- string
    COALESCE(qry.query_id,0) AS sqldb_query_query_store_query_id, -- int64
    execution_count AS sqldb_stats_total_execution_count, -- string
    FLOOR(total_worker_time / 1000.0) AS sqldb_stats_total_cpu_time_ms, -- int64
    total_elapsed_time AS sqldb_stats_total_elapsed_time_ms, -- int64
    total_physical_reads AS sqldb_stats_total_physical_reads_ms, -- int64
    total_logical_reads AS sqldb_stats_total_logical_reads_ms, -- int64
    total_logical_writes AS sqldb_stats_total_logical_writes_ms, -- int64
    CONVERT(VARCHAR(MAX),plan_handle,1) AS sqldb_stats_plan_handle, -- string
    CONVERT(VARCHAR(MAX),qs.query_hash,1) AS sqldb_stats_query_hash
FROM sys.dm_exec_query_stats qs
LEFT JOIN sys.query_store_query qry
    ON qry.query_hash = qs.query_hash
";

        protected override string GetQuery()
        {
            return DatabaseGeoReplicationInlineSqlNotSoBad;
        }

        protected override IEnumerable<string> GetRowKeyFieldNames()
        {
            return new string[0];
        }

        protected override string GetUtcTimestampFieldName()
        {
            return "collection_time_utc";
        }
    }
}
