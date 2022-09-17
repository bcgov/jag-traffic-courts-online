using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Messaging.MessageContracts
{
    /// <summary>
    /// Interface message contract for saving file history
    /// </summary>
    public class FileHistoryRecord : IMessage
    {
        public string Description { get; set; } = String.Empty;
        public string TicketNumber { get; set; } = String.Empty;
    }
}
