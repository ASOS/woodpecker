using System;
using Microsoft.ServiceBus.Messaging;

namespace Woodpecker.Core.Azure
{
    public static class QueueDescriptionsExtensions
    {
        public static BusPeckResult Peck(this QueueDescription queueDescription, PeckSource source)
        {
            var peckResult = new BusPeckResult()
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

        public static BusPeckResult Peck(this TopicDescription topicDescription, PeckSource source)
        {
            var peckResult = new BusPeckResult()
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

        public static BusPeckResult Peck(this SubscriptionDescription subscriptionDescription, PeckSource source)
        {
            var peckResult = new BusPeckResult()
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
