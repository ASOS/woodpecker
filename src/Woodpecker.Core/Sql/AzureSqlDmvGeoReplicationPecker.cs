using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker.Core.Sql
{
    public class AzureSqlDmvGeoReplicationPecker : AzureSqlDmvPeckerBase
    {
        private const string _query = @"
select @@servername [collection_server_name]    
     , db_name() [collection_database_name]    
     , getutcdate() [collection_time_utc]    
     , [partner_server] [secondary_server_name]  
     , [partner_database] [secondary_database_name] 
     , convert(varchar(30), convert(datetime, [last_replication]), 121) [last_replication]  
     , [replication_lag_sec]  
     , [replication_state_desc]  
     , [role_desc]  
     , [secondary_allow_connections_desc]  
from   sys.dm_geo_replication_link_status";

        protected override string GetQuery()
        {
            return _query;
        }

        protected override IEnumerable<string> GetRowKeyFieldNames()
        {
           return new[] { "collection_server_name", "collection_database_name", "secondary_server_name", "secondary_database_name" };
        }

        protected override string GetUtcTimestampFieldName()
        {
            return "collection_time_utc";
        }
    }
}
