using System;
using Microsoft.ServiceBus.Messaging;

namespace Woodpecker.Core.Azure
{
    public static class QueueDescriptionsExtensions
    {
        public static PeckResult Peck(this QueueDescription queueDescription, BusSource source)
        {
            var peckResult = new PeckResult()
            {
                ActiveMessageCount = queueDescription.MessageCountDetails.ActiveMessageCount,
                DeadLetterMessageCount = queueDescription.MessageCountDetails.DeadLetterMessageCount,
                ScheduledMessageCount = queueDescription.MessageCountDetails.ScheduledMessageCount,
                MaxSizeInMB = queueDescription.MaxSizeInMegabytes,
                SizeInMB = queueDescription.SizeInBytes / (1024 * 1024),
                QueueName = queueDescription.Path,
                SourceName = source.Name,
                TimeCaptured = DateTimeOffset.UtcNow,
                QueueType = "Q"
            };

            return peckResult;
        }

        public static PeckResult Peck(this TopicDescription topicDescription, BusSource source)
        {
            var peckResult = new PeckResult()
            {
                ActiveMessageCount = topicDescription.MessageCountDetails.ActiveMessageCount,
                DeadLetterMessageCount = topicDescription.MessageCountDetails.DeadLetterMessageCount,
                ScheduledMessageCount = topicDescription.MessageCountDetails.ScheduledMessageCount,
                MaxSizeInMB = topicDescription.MaxSizeInMegabytes,
                SizeInMB = topicDescription.SizeInBytes / (1024 * 1024),
                QueueName = topicDescription.Path,
                SourceName = source.Name,
                TimeCaptured = DateTimeOffset.UtcNow,
                QueueType = "T"
            };

            return peckResult;
        }

        public static PeckResult Peck(this SubscriptionDescription subscriptionDescription, BusSource source)
        {
            var peckResult = new PeckResult()
            {
                ActiveMessageCount = subscriptionDescription.MessageCountDetails.ActiveMessageCount,
                DeadLetterMessageCount = subscriptionDescription.MessageCountDetails.DeadLetterMessageCount,
                ScheduledMessageCount = subscriptionDescription.MessageCountDetails.ScheduledMessageCount,
                MaxSizeInMB = 0, // N/A
                SizeInMB = 0, // N/A
                QueueName = string.Format("{0}_{1}", subscriptionDescription.TopicPath, subscriptionDescription.Name),
                SourceName = source.Name,
                TimeCaptured = DateTimeOffset.UtcNow,
                QueueType = "S"
            };

            return peckResult;
        }
    }
}
