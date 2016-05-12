using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeHive;
using BeeHive.Configuration;
using BeeHive.DataStructures;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.Events;

namespace Woodpecker.Core
{
    public class MasterScheduler
    {
        private IEventQueueOperator _eventQueueOperator;
        private ILockStore _lockStore;
        private CloudTable _table;
        private string _clustername;

        public MasterScheduler(IEventQueueOperator eventQueueOperator,
            IConfigurationValueProvider configurationValueProvider,
            ILockStore lockStore)
        {
            _lockStore = lockStore;
            _eventQueueOperator = eventQueueOperator;
            var tscn = configurationValueProvider.GetValue(ConfigurationKeys.TableStorageConnectionString);
            var account = CloudStorageAccount.Parse(tscn);
            var client = account.CreateCloudTableClient();
            _table = client.GetTableReference(configurationValueProvider.GetValue(ConfigurationKeys.SourceTableName));
            _table.CreateIfNotExistsAsync();
            _clustername = configurationValueProvider.GetValue(ConfigurationKeys.ClusterName);
        }

        public async Task ScheduleSourcesAsync()
        {
            var token = new LockToken(_clustername);
            if (await _lockStore.TryLockAsync(token, tries: 1, timeoutMilliseconds: 30000))
            {
                foreach (var source in _table.ExecuteQuery(new TableQuery<PeckSource>()))
                {
                    if (source.IsActive && DateTimeOffset.Now > source.LastOffset.AddMinutes(source.IntervalMinutes))
                        await DoScheduleAsync(source);
                }
                await _lockStore.ReleaseLockAsync(token);
            }
        }

        private async Task DoScheduleAsync(PeckSource source)
        {
            var oldTimestamp = source.Timestamp;
            try
            {
                await _eventQueueOperator.PushAsync(new Event(new PeckSourceScheduled()
                {
                    Source = source
                })
                {
                    QueueName = QueueName.FromTopicName("PeckSourceScheduled").ToString()
                });

                source.LastOffset = DateTimeOffset.UtcNow;
                await _table.ExecuteAsync(TableOperation.InsertOrMerge(source));
            }
            catch (Exception e)
            {
                source.LastOffset = oldTimestamp;
                TheTrace.TraceError("Error scheduling source {0} : {1}", source.Name, e);
                try
                {
                    _table.Execute(TableOperation.InsertOrMerge(source));
                }
                catch (Exception ex)
                {
                    TheTrace.TraceError("Error saving error (!) in table. Source {0} : {1}", source.Name, ex);                                        
                }
            }
            
        }
    }
}
