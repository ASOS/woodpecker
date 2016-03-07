using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woodpecker.Core
{
    public class PeckResult
    {

        /// <summary>
        /// If pub-sub, it will appear as topic-subscription
        /// </summary>
        public string QueueName { get; set; }

        public DateTimeOffset TimeCaptured { get; set; }

        public string SourceName { get; set; }

        public long ActiveMessageCount { get; set; }

        public long DeadLetterMessageCount { get; set; }
        
        public long ScheduledMessageCount { get; set; }

        public long SizeInMB { get; set; }

        public long MaxSizeInMB { get; set; }

        public string QueueType { get; set; }


    }
}
