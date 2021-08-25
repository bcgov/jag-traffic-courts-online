using System;

namespace TrafficCourts.Common.Contract
{
    public static class TypeExtensions
    {
        public static string GetQueueName(this Type type)
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attributes)
            {
                if (attr is QueueNameAttribute)
                {
                    return ((QueueNameAttribute)attr).QueueName;
                }
            }

            return null;
        }
    }
}
