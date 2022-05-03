using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts
{
    public class DisputeSubmitted : IMessage
    {
        Guid DisputeId { get; set; }
    }
}
