using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.Internal;

namespace Woodpecker.Core.Azure
{
    public class AzureServiceBusSourcePecker : ISourcePecker
    {
        public async Task<IEnumerable<ITableEntity>> PeckAsync(PeckSource source)
        {
            var peckResults = new List<BusPeckResult>();
            var namespaceManager = NamespaceManager.CreateFromConnectionString(source.SourceConnectionString);
            foreach (var topic in await namespaceManager.GetTopicsAsync())
            {
                peckResults.Add(topic.Peck(source));
                foreach (var s in await namespaceManager.GetSubscriptionsAsync(topic.Path))
                {
                    peckResults.Add(s.Peck(source));
                }
            }

            foreach (var q in await namespaceManager.GetQueuesAsync())
            {
                peckResults.Add(q.Peck(source));
            }

            return peckResults.Select(x => x.ToEntity());
        }
    }
}
