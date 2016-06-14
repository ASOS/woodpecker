using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker.Core.Sql
{
    public class AzureSqlDmvWaitPecker : AzureSqlDmvPeckerBase
    {
        private const string DatabaseWaitInlineSqlNotSoBad = @"SELECT    
    GETUTCDATE() as collection_time_utc, -- datetime
    @@SERVERNAME as server_name, -- string
    DB_NAME() as database_name, -- string
    CONCAT(@@SERVERNAME,'/',DB_NAME()) AS [full_name], -- string
    wait_type AS sqldb_waits_wait_type, -- string
    waiting_tasks_count AS sqldb_waits_task_count, -- int32
    wait_time_ms AS sqldb_waits_total_wait_time_ms, -- int64
    max_wait_time_ms AS sqldb_waits_max_wait_time_ms, -- int64
    signal_wait_time_ms AS sqldb_waits_signal_wait_time_ms, -- int64
    wait_time_ms - signal_wait_time_ms AS sqldb_waits_resource_wait_time_ms -- int64
FROM sys.dm_db_wait_stats";

        protected override string GetQuery()
        {
            return DatabaseWaitInlineSqlNotSoBad;
        }

        protected override IEnumerable<string> GetRowKeyFieldNames()
        {
            return new[] { "server_name", "database_name", "sqdb_waits_wait_type" };
        }

        protected override string GetUtcTimestampFieldName()
        {
            return "collection_time_utc";
        }
    }
}
