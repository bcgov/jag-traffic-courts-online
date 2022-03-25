using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts
{
    public interface Message
    {
        Guid Id { get; set; }
        DateTimeOffset Timestamp { get; set; }
    }
}
