using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts
{
    public class DisputeCancelled : IMessage
    {
        public long Id { get; set; } = -1;
        public string? Email { get; set; }
    }
}
