using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Woodpecker.Core.Sql
{
    public abstract class AzureSqlDvmPeckerBase : ISourcePecker
    {

        private string _query;
        private List<string> _fieldNames; 

        public async Task<IEnumerable<ITableEntity>> PeckAsync(PeckSource source)
        {
            if (_query == null)
                _query = GetQuery();


            var results = new List<ITableEntity>();
            var connection = new SqlConnection(source.SourceConnectionString);
            try
            {
                await connection.OpenAsync();
                var command = new SqlCommand(_query, connection);
                var reader = await command.ExecuteReaderAsync();
                var columnNames = Enumerable.Range(0, reader.FieldCount).Select(x => reader.GetName(x)).ToDictionary(s => s);

                while (reader.Read())
                {
                    // row key
                    var rowKeyParams = GetRowKeyFieldNames().Select(x => columnNames.ContainsKey(x) ? reader[x].ToString() : x).ToArray();
                    var rowKey = rowKeyParams.Length == 0 ? Guid.NewGuid().ToString("N") : string.Join("_", rowKeyParams);
                    
                    // par key
                    var ofsted = new DateTimeOffset(reader.GetDateTime(reader.GetOrdinal(GetUtcTimestampFieldName())), TimeSpan.Zero);
                    var minuteOffset = new DateTimeOffset(DateTime.Parse(ofsted.UtcDateTime.ToString("yyyy-MM-dd HH:mm:00")), TimeSpan.Zero);
                    var shardKey = (DateTimeOffset.MaxValue.Ticks - minuteOffset.Ticks).ToString("D19");
                    
                    var entity = new DynamicTableEntity(shardKey, rowKey);
                    foreach (var columnName in columnNames.Keys)
                        entity.Properties.Add(columnName, EntityProperty.CreateEntityPropertyFromObject(reader[columnName]));

                    results.Add(entity);                    
                }
                

                return results;
            }
            finally
            {
                connection.Close();
            }


        }

        
        protected abstract string GetQuery();


        /// <summary>
        /// 
        /// </summary>
        /// <returns>if field does not exist then it is simply a CONSTANT. If empty it means one needs to be generated (GUID)</returns>
        protected abstract IEnumerable<string> GetRowKeyFieldNames();

        protected abstract string GetUtcTimestampFieldName();
    }
}
