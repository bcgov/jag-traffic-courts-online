using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DisputeApi.Web.Features.TicketLookup
{
    public static class TicketLookup
    {
        public class Query : IRequest<Response>
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

        public class Response
        {
            public string ViolationTicketNumber { get; set; }
            public string ViolationTime { get; set; }
            public string ViolationDate { get; set; }

            public List<Offence> Offences { get; set; }

            /// <summary>
            /// Gets or sets the raw response returned from the RSI Pay BC API.
            /// Used only for troubleshooting during development. Will be removed
            /// once the API usage is understood.
            /// </summary>
            public RawTicketSearchResponse RawResponse { get; set; }
        }

        public class Offence
        {
            public int OffenceNumber { get; set; }
            public decimal TicketAmount { get; set; }
            public decimal AmountDue { get; set; }
            public string DueDate { get; set; }
            public string Description { get; set; }
            public OffenceStatus Status { get; set; }

            ///will change to Dispute class later
            public string Dispute { get; set; }
        }

        public enum OffenceStatus
        {
            OutstandingBalance,
            Paid,
            DisputeInProgress,
            DisputeSettled,
            DisputeSubmitted,
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            IRsiRestApi _rsiApi;
            public Handler(IRsiRestApi rsiApi )
            {
                _rsiApi = rsiApi;
            }
            public async Task<Response> Handle(Query query, CancellationToken cancellationToken)
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

            private async Task<Response> GetResponseFromRsi(string ticketNumber, string time)
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

                return rawResponse.ConvertToResponse();
            }

            private async Task<Response> GetFakeResponseFromFile(string ticketNumber, string time)
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
                return rawResponse.ConvertToResponse();
            }

        }

    }
}
