using System;

namespace TrafficCourts.Common.Contract
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class QueueNameAttribute : Attribute
    {
        public string QueueName { get; }
        public QueueNameAttribute(string queueName)
        {
            QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
        }
    }
}
