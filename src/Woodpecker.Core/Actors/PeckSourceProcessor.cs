using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeHive;
using Woodpecker.Core.Azure;
using Woodpecker.Core.Events;
using Woodpecker.Core.Internal;
using Woodpecker.Core.Persistence;

namespace Woodpecker.Core.Actors
{
    [ActorDescription("PeckSourceScheduled-Process", 10)]
    public class PeckSourceProcessor : IProcessorActor
    {
        public void Dispose()
        {
        }

        public async Task<IEnumerable<Event>> ProcessAsync(Event evnt)
        {
            var sourceScheduled = evnt.GetBody<PeckSourceScheduled>();
            TheTrace.TraceInformation("Got busSourceScheduled for {0}", sourceScheduled.Source.Name);
            var source = sourceScheduled.Source;
            var pecker = FactoryHelper.Create<ISourcePecker>(source.PeckerType, typeof(AzureServiceBusSourcePecker));
            var results = (await pecker.PeckAsync(source)).ToArray();
            TheTrace.TraceInformation("Got busSourceScheduled results: {0} items  for {1}", results.Length, sourceScheduled.Source.Name);
            var store = FactoryHelper.Create<IPeckResultStore>(source.StoreType, typeof(TableStoragePeckResultStore));
            await store.StoreAsync(source, results);
            TheTrace.TraceInformation("Stored away results: {0} items  for {1}", results.Length, sourceScheduled.Source.Name);
            return Enumerable.Empty<Event>();
        }
    }
}
