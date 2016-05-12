using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Woodpecker.Core
{
    public interface ISourcePecker
    {
        Task<IEnumerable<ITableEntity>> PeckAsync(PeckSource source);
    }
}
