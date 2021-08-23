using Gov.CitizenApi.Models;

namespace Gov.CitizenApi.Features.Tickets.Commands
{
    public class TicketPaymentConfirmResponse 
    {
        public string TicketNumber { get; set; }
        public string Time { get; set; }
    }
}
