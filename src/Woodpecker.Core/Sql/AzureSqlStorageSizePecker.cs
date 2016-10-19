using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker.Core.Sql
{
    public class AzureSqlStorageSizePecker : AzureSqlDmvPeckerBase
    {
        private const string DatabaseWaitInlineSqlNotSoBad = @"select @@servername [server_name]                        
-- varchar(128)  
     , db_name() [database_name]                         -- varchar(128) 
     , db_id() [database_id]                             -- int 
     , getutcdate() [collection_time_utc]                -- datetime
     , df.file_id                                        -- int 
     , df.type_desc [file_type_desc]                     -- varchar(60) 
     , convert(bigint, df.size) *8 [size_kb]             -- bigint 
     , convert(bigint, df.max_size) *8 [max_size_kb]     -- bigint 
from   sys.database_files df; ";

        protected override string GetQuery()
        {
            return DatabaseWaitInlineSqlNotSoBad;
        }

        protected override IEnumerable<string> GetRowKeyFieldNames()
        {
            return new[] { "server_name", "database_name", "file_id"};
        }

        protected override string GetUtcTimestampFieldName()
        {
            return "collection_time_utc";
        }
    }
}
