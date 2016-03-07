using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker.Core
{
    public class ConfigurationKeys
    {
        public const string ServiceBusConnectionString = "Woodpecker.Core.ServiceBusConnectionString";
        public const string TableStorageConnectionString = "Woodpecker.Core.TableStorageConnectionString";
        public const string SourceTableName = "Woodpecker.Core.SourceTableName";
        public const string ClusterName = "Woodpecker.Core.ClusterName";
    }
}
