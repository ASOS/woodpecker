using System;
using System.Collections.Generic;


namespace Woodpecker.Core.Sql
{
    public class AzureSqlDmvQueryStoreConfigPecker : AzureSqlDmvPeckerBase
    {
        private const string _query = @"
select @@servername [collection_server_name]         
     , db_name() [collection_database_name] 
     , getutcdate() [collection_time_utc]          
     , qso.[actual_state_desc]                      
     , qso.[desired_state_desc]                     
     , qso.[current_storage_size_mb]             
     , qso.[max_storage_size_mb]                 
     , qso.[readonly_reason]                     
     , qso.[interval_length_minutes]             
     , qso.[stale_query_threshold_days]             
     , qso.[query_capture_mode_desc]                
     , qso.[size_based_cleanup_mode_desc]           
     , qso.[flush_interval_seconds]              
     , qso.[max_plans_per_query]                 
from   [sys].[database_query_store_options] qso;"; 

        protected override string GetQuery()
        {
            return _query;
        }

        protected override IEnumerable<string> GetRowKeyFieldNames()
        {
           return new[] { "collection_server_name", "collection_database_name" };
        }

        protected override string GetUtcTimestampFieldName()
        {
            return "collection_time_utc";
        }
    }
}
