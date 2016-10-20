using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker.Core.Sql
{
    public class AzureSqlStorageSizePecker : AzureSqlDmvPeckerBase
    {
       private const string _query = @"
select @@servername [collection_server_name]  
     , db_name() [collection_database_name]
     , getutcdate() [collection_time_utc]  
     , df.file_id  
     , df.type_desc [file_type_desc]  
     , convert(bigint, df.size) *8 [size_kb] 
     , convert(bigint, df.max_size) *8 [max_size_kb] 
from   sys.database_files df;";

        protected override string GetQuery()
        {
            return _query;
        }

        protected override IEnumerable<string> GetRowKeyFieldNames()
        {
           return new[] { "collection_server_name", "collection_database_name", "file_id" };
        }

        protected override string GetUtcTimestampFieldName()
        {
            return "collection_time_utc";
        }
    }
}
