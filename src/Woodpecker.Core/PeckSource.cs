using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Woodpecker.Core
{
    public class PeckSource : TableEntity
    {
        public static readonly string PartiotionKeyValue = "Woody";

        /// <summary>
        /// Name should contain these:
        ///     Platform
        ///     Service/Product
        ///     Environment
        ///     (Component if source has a one-to-one relationship with)
        /// </summary>
        public string Name {
            get { return RowKey; }
            set { RowKey = value; } 
        }

        /// <summary>
        /// Bus connection string (e.g. Azure namespace connection string)
        /// </summary>
        public string SourceConnectionString { get; set; }

        /// <summary>
        /// Destination connection string
        /// </summary>
        public string DestinationConnectionString { get; set; }

        /// <summary>
        /// Destination table name (Table Storage, SQL Table, Elasticsearch Index+Type, etc). Depends on the destination
        /// </summary>
        public string DestinationTableName { get; set; }

        /// <summary>
        /// Actual type of the class implementing ISourcePecker
        /// </summary>
        public string StoreType { get; set; }

        /// <summary>
        /// Actual type of the class implementing ISourcePecker
        /// </summary>
        public string PeckerType { get; set; }

        /// <summary>
        /// This is intentionally NOT defined as seconds
        /// </summary>
        public int IntervalMinutes { get; set; }

        /// <summary>
        /// Last time source was pecked
        /// </summary>
        public DateTimeOffset LastOffset { get; set; }

        /// <summary>
        /// A field that can be used for custom uses cases per pecker
        /// </summary>
        public string CustomConfig { get; set; }

        public bool IsActive { get; set; }
    }
}
