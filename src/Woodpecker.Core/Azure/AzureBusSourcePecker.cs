using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus;

namespace Woodpecker.Core.Azure
{
    public class AzureServiceBusSourcePecker : IBusSourcePecker
    {
        public async Task<IEnumerable<PeckResult>> PeckAsync(BusSource source)
        {
            var peckResults = new List<PeckResult>();
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

            return peckResults;
        }
    }
}
