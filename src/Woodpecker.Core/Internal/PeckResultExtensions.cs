﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Woodpecker.Core.Metrics.Model;

namespace Woodpecker.Core.Internal
{
    internal static class PeckResultExtensions
    {
        public static DynamicTableEntity ToEntity(this BusPeckResult result)
        {
            var minuteOffset = new DateTimeOffset(DateTime.Parse(result.TimeCaptured.UtcDateTime.ToString("yyyy-MM-dd HH:mm:00")), TimeSpan.Zero);
            var shardKey = (DateTimeOffset.MaxValue.Ticks - minuteOffset.Ticks).ToString("D19");

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

        public static DynamicTableEntity ToEntity(this MetricModel metric, string sourceName, DateTimeOffset timeCaptured)
        {
            var minuteOffset = new DateTimeOffset(DateTime.Parse(timeCaptured.UtcDateTime.ToString("yyyy-MM-dd HH:mm:00")), TimeSpan.Zero);
            var shardKey = (DateTimeOffset.MaxValue.Ticks - minuteOffset.Ticks).ToString("D19");

            var entity = new DynamicTableEntity(shardKey,
                string.Format("{0}_{1}", sourceName, metric.Name));

            entity.Properties.Add("Name", EntityProperty.GeneratePropertyForString(metric.Name));
            entity.Properties.Add("TimeStamp", EntityProperty.GeneratePropertyForDateTimeOffset(metric.TimeStamp));
            entity.Properties.Add("Average", EntityProperty.GeneratePropertyForDouble(metric.Average));
            entity.Properties.Add("Count", EntityProperty.GeneratePropertyForLong(metric.Count));
            entity.Properties.Add("Maximum", EntityProperty.GeneratePropertyForDouble(metric.Maximum));
            entity.Properties.Add("Minimum", EntityProperty.GeneratePropertyForDouble(metric.Minimum));
            entity.Properties.Add("Total", EntityProperty.GeneratePropertyForDouble(metric.Total));

            return entity;
        }
    }
}
