using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DisputeApi.Web.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DisputeApi.Web.Features.TicketLookup
{
    public static class TicketLookup
    {
        public class Query : IRequest<TicketDispute>
        {
            [FromQuery(Name = "ticketNumber")]
            [Required]
            [RegularExpression("^[A-Z]{2}[0-9]{6,}$", ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")]
            public string TicketNumber { get; set; }

            [FromQuery(Name = "time")]
            [Required]
            [RegularExpression("^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessage = "time must be properly formatted 24 hour clock")]
            public string Time { get; set; }
        }

 

        public class Handler : IRequestHandler<Query, TicketDispute>
        {
            IRsiRestApi _rsiApi;
            public Handler(IRsiRestApi rsiApi )
            {
                _rsiApi = rsiApi;
            }
            public async Task<TicketDispute> Handle(Query query, CancellationToken cancellationToken)
            {
                string ticketNumber = query.TicketNumber;
                string time = query.Time;
                if (Keys.RSI_OPERATION_MODE != "FAKE")
                {
                    return await GetResponseFromRsi(ticketNumber, time);
                }
                else
                {
                    return await GetFakeResponseFromFile(ticketNumber, time);

 
                }
            }

            private async Task<TicketDispute> GetResponseFromRsi(string ticketNumber, string time)
            {
                RawTicketSearchResponse rawResponse = await _rsiApi.GetTicket(
                        new GetTicketParams { TicketNumber = ticketNumber, PRN = "10006", IssuedTime = time.Replace(":", "") }
                    );
                if (rawResponse == null || rawResponse.Items == null || rawResponse.Items.Count == 0) return null;

                foreach (Item item in rawResponse.Items)
                {
                    if (item.SelectedInvoice?.Reference != null)
                    {
                        int lastSlash = item.SelectedInvoice.Reference.LastIndexOf('/');
                        if (lastSlash > 0)
                        {
                            string invoiceNumber = item.SelectedInvoice.Reference.Substring(lastSlash + 1);
                            Invoice invoice = await _rsiApi.GetInvoice(invoiceNumber);
                            item.SelectedInvoice.Invoice = invoice;
                        }
                    }
                }

                return rawResponse.ConvertToTicketDispute();
            }

            private async Task<TicketDispute> GetFakeResponseFromFile(string ticketNumber, string time)
            {
                using FileStream openStream = File.OpenRead($"Features/TicketLookup/ticket-{ticketNumber}.json");
                RawTicketSearchResponse rawResponse = await JsonSerializer.DeserializeAsync<RawTicketSearchResponse>(openStream);

                if (rawResponse == null)
                {
                    return null;
                }

                for (int i = 0; i < rawResponse.Items.Count; i++)
                {
                    using FileStream itemStream = File.OpenRead($"Features/TicketLookup/invoice-{ticketNumber}{i + 1}.json");
                    var invoice = await JsonSerializer.DeserializeAsync<Invoice>(itemStream);
                    rawResponse.Items[i].SelectedInvoice.Invoice = invoice;
                }
                return rawResponse.ConvertToTicketDispute();
            }

        }

    }
}
