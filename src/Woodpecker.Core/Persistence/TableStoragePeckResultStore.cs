using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeHive.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.Internal;

namespace Woodpecker.Core.Persistence
{
    public class TableStoragePeckResultStore : IPeckResultStore
    {
        public async Task StoreAsync(BusSource source, IEnumerable<PeckResult> results)
        {
            var account = CloudStorageAccount.Parse(source.DestinationConnectionString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference(source.DestinationTableName);
            await table.CreateIfNotExistsAsync();
            var copy = results.ToArray(); // have to do this because of batching
            var groups = copy.GroupBy(x => x.TimeCaptured.UtcDateTime.ToString("dd/MM/yyyy HH:mm:00 +00:00"));

            foreach (var g in groups)
            {
                var minuteOffset = DateTimeOffset.Parse(g.Key);
                var shardKey = (DateTimeOffset.MaxValue.Ticks - minuteOffset.Ticks).ToString("D19");
                var list = new List<TableBatchOperation>();
                var batchOperation = new TableBatchOperation();
                foreach (var result in g)
                {
                    if (batchOperation.Count >= 40)
                    {
                        list.Add(batchOperation);
                        batchOperation = new TableBatchOperation();
                    }

                    batchOperation.Add(TableOperation.InsertOrReplace(result.ToEntity(shardKey)));
                }

                list.Add(batchOperation);
                foreach (var batch in list)
                {
                    await table.ExecuteBatchAsync(batch);
                }
            }
        }
    }
}
