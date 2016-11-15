using System;
using System.Collections.Generic;


namespace Woodpecker.Core.Sql
{
    public class AzureSqlDmvQueryStatsPecker : AzureSqlDmvPeckerBase
    {
        private const string _query = @"
select @@servername [collection_server_name] 
     , db_name() [collection_database_name] 
     , convert(datetime, rsi.[end_time]) [collection_time_utc] 
     , rs.[runtime_stats_id] 
     , convert(varchar, convert(datetime, rsi.[start_time]), 121) [interval_start_time] 
     , convert(varchar, convert(datetime, rsi.[end_time]), 121) [interval_end_time] 
     , qt.[query_sql_text] 
     , q.[query_id] 
     , convert(varchar, q.[query_hash], 1) [query_hash]
     , convert(varchar, convert(datetime, rs.[last_execution_time]), 121) [last_execution_time] 
     , rs.[count_executions] 
     , q.[object_id] 
     , coalesce(quotename(object_schema_name(q.[object_id])) + quotename(object_name(q.[object_id])), '') [object_name] 
     , floor(rs.[max_cpu_time] /1000.) [max_cpu_time_ms] 
     , floor(rs.[last_cpu_time] /1000.) [last_cpu_time_ms] 
     , floor(rs.[max_duration] /1000.) [max_duration_ms] 
     , floor(rs.[last_duration] /1000.) [last_duration_ms] 
     , rs.[max_physical_io_reads] 
     , rs.[last_physical_io_reads] 
     , rs.[max_logical_io_reads] 
     , rs.[last_logical_io_reads] 
     , rs.[max_logical_io_writes] 
     , rs.[last_logical_io_writes] 
     , rs.[max_rowcount] 
     , rs.[last_rowcount] 
     , p.[count_compiles] 
     , convert(varchar, convert(datetime, p.[last_compile_start_time]), 121) [last_compile_start_time] 
     , floor(q.[last_compile_duration] /1000.) [last_compile_duration_ms] 
from   sys.query_store_query q 
join   sys.query_store_query_text qt on qt.[query_text_id] = q.[query_text_id] 
join   sys.query_store_plan p on p.[query_id] = q.[query_id] 
join   sys.query_store_runtime_stats rs on rs.[plan_id] = p.[plan_id] 
join   sys.query_store_runtime_stats_interval rsi on rsi.[runtime_stats_interval_id] = rs.[runtime_stats_interval_id] 
where  q.[is_internal_query] | q.[is_clouddb_internal_query] = 'false'​
and    rs.[last_execution_time] > dateadd(mi, -2, getutcdate());"; 

        protected override string GetQuery()
        {
            return _query;
        }

        protected override IEnumerable<string> GetRowKeyFieldNames()
        {
           return new[] { "collection_server_name", "collection_database_name", "runtime_stats_id", "last_execution_time" };
        }

        protected override string GetUtcTimestampFieldName()
        {
            return "collection_time_utc";
        }
    }
}
