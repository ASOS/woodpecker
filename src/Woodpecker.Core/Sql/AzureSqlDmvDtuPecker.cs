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
    public class AzureSqlDmvDtuPecker : AzureSqlDmvPeckerBase
    {

        private const string _query = @"
select @@servername [collection_server_name] 
     , db_name() [collection_database_name]  
     , getutcdate() [collection_time_utc] 
     , convert(varchar(30), [end_time], 121) [end_time]  
     , [cpu_percent]  
     , [physical_data_read_percent]    
     , [log_write_percent]    
     , [memory_usage_percent] 
     , [xtp_storage_percent]  
     , [workers_percent]  
     , [session_percent]  
     , [dtu_used_percent] 
     , [dtu_limit] * [dtu_used_percent] /100. [dtu_used] 
     , [dtu_limit]
from   ( 
       select end_time  
            , avg_cpu_percent [cpu_percent] 
            , avg_data_io_percent [physical_data_read_percent] 
            , avg_log_write_percent [log_write_percent] 
            , avg_memory_usage_percent [memory_usage_percent] 
            , xtp_storage_percent [xtp_storage_percent] 
            , max_worker_percent [workers_percent] 
            , max_session_percent [session_percent] 
            , dtu_limit 
            , (select max(v) from (values (avg_cpu_percent), (avg_data_io_percent), (avg_log_write_percent)) t(v)) [dtu_used_percent] 
       from   sys.dm_db_resource_stats 
       ) t​ ​
where  [end_time] >= dateadd(mi, -2, getutcdate());
;";

        protected override string GetQuery()
        {
            return _query;
        }

        protected override IEnumerable<string> GetRowKeyFieldNames()
        {
           return new[] { "collection_server_name", "collection_database_name", "end_time" };
        }

        protected override string GetUtcTimestampFieldName()
        {
            return "collection_time_utc";
        }
    }
}