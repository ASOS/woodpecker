using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Woodpecker.Core
{
    public interface IBusSourcePecker
    {
        Task<IEnumerable<PeckResult>> PeckAsync(BusSource source);
    }
}
