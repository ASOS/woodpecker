using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker.Core.Persistence
{
    public interface IPeckResultStore
    {
        Task StoreAsync(BusSource source, IEnumerable<PeckResult> results);
    }
}
