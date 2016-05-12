using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeeHive.DataStructures;
using Microsoft.WindowsAzure.Storage.Table;

namespace Woodpecker.Core.Persistence
{
    public interface IPeckResultStore
    {
        Task StoreAsync(PeckSource source, IEnumerable<ITableEntity> results);
    }
}
