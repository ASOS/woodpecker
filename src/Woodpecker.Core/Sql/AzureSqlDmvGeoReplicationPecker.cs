using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker.Core.Sql
{
    public class AzureSqlDmvGeoReplicationPecker : AzureSqlDmvPeckerBase
    {
        private const string DatabaseGeoReplicationInlineSqlNotSoBad = @"
SELECT
    GETUTCDATE() as collection_time_utc, -- datetime
    @@SERVERNAME as server_name, -- string
    DB_NAME() as database_name, -- string
    CONCAT(@@SERVERNAME,'/',DB_NAME()) AS [full_name], -- string
    partner_server AS sqldb_geo_partner_server, -- string
    partner_database AS sqldb_geo_partner_database, -- string
    CONCAT(partner_server,'/',partner_database) AS sqldb_geo_partner_full_name, -- string
    replication_lag_sec AS sqldb_geo_lag_sec, -- int32
    role_desc AS sqldb_geo_role_desc -- string
FROM sys.dm_geo_replication_link_status
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
