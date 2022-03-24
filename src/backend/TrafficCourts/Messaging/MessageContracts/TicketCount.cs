using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts
{
    public interface TicketCount
    {
        string OffenceDeclaration { get; set; }
        bool TimeToPayRequest { get; set; }
        bool FineReductionRequest { get; set; }
    }
}
