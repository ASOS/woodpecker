using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.Internal;

namespace Woodpecker.Core.Sql
{
    public class AzureSqlDmvPerfPecker : AzureSqlDmvPeckerBase
    {

        private const string DatabasePerfCountersInlineSqlNotSoBad = @"DECLARE @counters TABLE (
    obj_name varchar(max) not null,
    cntr_name varchar(max) not null,
    instance_name varchar(max) not null
);

INSERT INTO @counters 
VALUES 
     ('Buffer Manager','Buffer cache hit ratio','')
    ,('Buffer Manager','Buffer cache hit ratio base','')
    ,('Buffer Manager','Page life expectancy','')
    ,('General Statistics','Logins/sec','')
    ,('General Statistics','Logouts/sec','')
    ,('Locks','Number of Deadlocks/sec','_Total')
    ,('Databases','Transactions/sec','_Total')
    ,('Access Methods','Full Scans/sec','')
    ,('SQL Statistics','Batch Requests/sec','')
    ,('SQL Statistics','SQL Compilations/sec','')
    ,('SQL Statistics','SQL Re-Compilations/sec','')
    ,('Plan Cache','Cache Hit Ratio','_Total')
    ,('Plan Cache','Cache Hit Ratio Base','_Total')
    ,('HTTP Storage','Reads/Sec','_Total')
    ,('HTTP Storage','Writes/Sec','_Total')
    ,('Query Store','Query Store CPU usage','_Total')
;

WITH c AS (
    SELECT
        GETUTCDATE() as collection_time_utc, -- datetime
        @@SERVERNAME as server_name, -- string
        DB_NAME() as database_name, -- string
        CONCAT(@@SERVERNAME,'/',DB_NAME()) AS [full_name], -- string
        LTRIM(RTRIM(SUBSTRING(pc.[object_name], PATINDEX('%:%',object_name)+1,LEN(pc.[object_name])))) AS [sqldb_perf_object_name], -- string
        LTRIM(RTRIM(pc.counter_name)) AS sqldb_perf_counter_name, -- string
        LTRIM(RTRIM(pc.instance_name)) AS sqldb_perf_instance_name, -- string
        pc.cntr_value AS sqldb_perf_counter_value, -- int64
        pc.cntr_type AS sqldb_perf_counter_type -- int32
    FROM sys.dm_os_performance_counters pc
    JOIN @counters c
        ON c.cntr_name = pc.counter_name
)
SELECT
    c.*
FROM c
JOIN @counters cntrs
    ON cntrs.obj_name = c.sqldb_perf_object_name
    AND cntrs.cntr_name = c.sqldb_perf_counter_name
    AND cntrs.instance_name = c.sqldb_perf_instance_name
;";

        protected override string GetQuery()
        {
            return DatabasePerfCountersInlineSqlNotSoBad;
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