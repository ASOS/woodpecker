using System;
using System.Collections.Generic;


namespace Woodpecker.Core.Sql
{
    public class AzureSqlDmvProcedureStatsPecker : AzureSqlDmvPeckerBase
    {
        private const string _query = @"
select @@servername [collection_server_name]     
     , db_name() [collection_database_name]  
     , getutcdate() [collection_time_utc]  
     , ps.[type_desc]
     , ps.[object_id]    
     , coalesce(quotename(object_schema_name(ps.[object_id])) + quotename(object_name(ps.[object_id])), '') [procedure_name] 
     , convert(varchar(30), ps.[last_execution_time], 121) [last_execution_time] 
     , ps.[execution_count] 
     , floor(ps.[max_worker_time] /1000.) [max_cpu_time_ms] 
     , floor(ps.[last_worker_time] /1000.) [last_cpu_time_ms] 
     , floor(ps.[max_elapsed_time] /1000.) [max_elapsed_time_ms] 
     , floor(ps.[last_elapsed_time] /1000.) [last_elapsed_time_ms] 
     , convert(varchar(1024), ps.[plan_handle], 1) [plan_handle]  
from   sys.dm_exec_procedure_stats ps 
where  ps.[type] in ('P', 'PC') 
and    ps.[last_execution_time] > dateadd(ss, -80, getutcdate());"; 


        protected override string GetQuery()
        {
            return _query;
        }

        protected override IEnumerable<string> GetRowKeyFieldNames()
        {
           return new[] { "collection_server_name", "collection_database_name", "object_id", "last_execution_time" };
        }

        protected override string GetUtcTimestampFieldName()
        {
            return "collection_time_utc";
        }
    }
}
