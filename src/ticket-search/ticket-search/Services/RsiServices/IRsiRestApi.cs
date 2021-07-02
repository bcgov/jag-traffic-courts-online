using Gov.TicketSearch.Models;
using Gov.TicketSearch.Services.RsiServices.Models;
using Refit;
using System.Threading;
using System.Threading.Tasks;

namespace Gov.TicketSearch.Services.RsiServices
{
    public interface IRsiRestApi
    {
        [Get("/paybc/vph/rest/PSSGVPHPAYBC/vph/")]
        Task<RawTicketSearchResponse> GetTicket(GetTicketParams ticketParams, CancellationToken cancellationToken);

        [Get("/paybc/vph/rest/PSSGVPHPAYBC/vph/invs/{invoiceNumber}")]
        Task<Invoice> GetInvoice(string invoiceNumber, CancellationToken cancellationToken);
    }

    public class GetTicketParams
    {
        [AliasAs("in")]
        public string TicketNumber { get; set; }

        [AliasAs("prn")]
        public string Prn { get; set; }

        [AliasAs("time")]
        public string IssuedTime { get; set; }
    }

}
