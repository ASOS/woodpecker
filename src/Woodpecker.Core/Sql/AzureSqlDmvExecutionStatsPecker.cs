using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker.Core.Sql
{
    public class AzureSqlDmvExecutionStatsPecker : AzureSqlDmvPeckerBase
    {
        private const string ExecutionStatsInlineSqlNotSoBad = @"
SELECT
    GETUTCDATE() AS collection_time_utc, -- datetime
    @@SERVERNAME AS server_name, -- string
    DB_NAME() AS database_name, -- string
    CONCAT(@@SERVERNAME,'/',DB_NAME()) AS [full_name], -- string
    COALESCE(OBJECT_NAME(object_id),'') AS sqldb_proc_object_name, -- string
    type_desc AS sqldb_proc_type_description, -- string
    execution_count AS sqldb_proc_total_execution_count, -- string
    FLOOR(total_worker_time / 1000.0) AS sqldb_proc_total_cpu_time_microsec, -- int64
    total_elapsed_time AS sqldb_proc_total_elapsed_time_microsec, -- int64
    total_physical_reads AS sqldb_proc_total_physical_reads, -- int64
    total_logical_reads AS sqldb_proc_total_logical_reads, -- int64
    total_logical_writes AS sqldb_proc_total_logical_writes, -- int64
    CONVERT(VARCHAR(MAX),plan_handle,1) AS sqldb_proc_plan_handle -- string
FROM sys.dm_exec_procedure_stats
WHERE OBJECT_NAME(object_id) IS NOT NULL";

        protected override string GetQuery()
        {
            return ExecutionStatsInlineSqlNotSoBad;
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
