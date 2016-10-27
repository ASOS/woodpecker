using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker.Core.Sql
{
    public class AzureSqlDmvConnectionsAndDeadlocksPecker : AzureSqlDmvPeckerBase
    {
        private const string _query = @"
select @@servername [collection_server_name]  
     , db_name() [collection_database_name] 
     , getutcdate() [collection_time_utc] 
     , el.[database_name]        
     , convert(varchar(30), el.[start_time], 121) [start_time] 
     , convert(varchar(30), el.[end_time], 121) [end_time] 
     , el.[event_category]     
     , el.[event_type]     
     , el.[event_subtype_desc]        
     , el.[severity]    
     , el.[event_count]      
     , el.[description]      
from   sys.event_log el 
where  el.[event_category] in ('connectivity', 'engine') 
and    el.[event_type] in ('connection_successful', 'connection_failed', 'deadlock') 
and    [end_time] > dateadd(mi, -30, getutcdate()) 
order  by  
       [start_time] desc;";

        protected override string GetQuery()
        {
            return _query;
        }

        protected override IEnumerable<string> GetRowKeyFieldNames()
        {
            return new[] { "collection_server_name", "collection_database_name", "end_time", "database_name", "event_subtype_desc"} ;
        }

        protected override string GetUtcTimestampFieldName()
        {
            return "collection_time_utc";
        }
    }
}
