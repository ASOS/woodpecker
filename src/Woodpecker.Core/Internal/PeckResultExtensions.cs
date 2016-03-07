using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Woodpecker.Core.Internal
{
    internal static class PeckResultExtensions
    {
        public static DynamicTableEntity ToEntity(this PeckResult result, string shardKey)
        {
            var entity = new DynamicTableEntity(shardKey,
                string.Format("{0}_{1}", result.SourceName, result.QueueName));

            entity.Properties.Add("ActiveMessageCount", EntityProperty.GeneratePropertyForLong(result.ActiveMessageCount));
            entity.Properties.Add("DeadLetterMessageCount", EntityProperty.GeneratePropertyForLong(result.DeadLetterMessageCount));
            entity.Properties.Add("MaxSizeInMB", EntityProperty.GeneratePropertyForLong(result.MaxSizeInMB));
            entity.Properties.Add("ScheduledMessageCount", EntityProperty.GeneratePropertyForLong(result.ScheduledMessageCount));
            entity.Properties.Add("SizeInMB", EntityProperty.GeneratePropertyForLong(result.SizeInMB));
            entity.Properties.Add("QueueName", EntityProperty.GeneratePropertyForString(result.QueueName));
            entity.Properties.Add("SourceName", EntityProperty.GeneratePropertyForString(result.SourceName));
            entity.Properties.Add("TimeCaptured", EntityProperty.GeneratePropertyForDateTimeOffset(result.TimeCaptured));
            entity.Properties.Add("QueueType", EntityProperty.GeneratePropertyForString(result.QueueType));

            return entity;
        }
    }
}
