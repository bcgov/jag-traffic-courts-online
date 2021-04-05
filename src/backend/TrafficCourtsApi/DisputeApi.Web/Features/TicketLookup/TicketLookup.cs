using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            // TO DO: why one the query it is ticketNumber and time but on the response it is violation_number / violation_time?
            [JsonPropertyName("violation_number")]
            public string ViolationNumber { get; set; }
            [JsonPropertyName("violation_time")]

            public string ViolationTime { get; set; }
            [JsonPropertyName("violation_date")]
            public DateTime ViolationDate { get; set; }

            public List<ViolationCount> Counts { get; set; }

            /// <summary>
            /// Gets or sets the raw response returned from the RSI Pay BC API.
            /// Used only for troubleshooting during development. Will be removed
            /// once the API usage is understood.
            /// </summary>
            public RawTicketSearchResponse RawResponse { get; set; }
        }

        public class Handler : IRequestHandler<Query, Response>
        {
            IRSIRestApi _rsiApi;
            public Handler(IRSIRestApi rsiApi )
            {
                _rsiApi = rsiApi;
            }
            public async Task<Response> Handle(Query query, CancellationToken cancellationToken)
            {
                string ticketNumber = query.TicketNumber;
                string time = query.Time;
                if (Keys.RSI_OPERATION_MODE != "FAKE")
                {
                    RawTicketSearchResponse rawResponse = await _rsiApi.GetTicket(
                        new GetTicketParams { TicketNumebr = "EZ02000460", PRN = "10006", IssuedTime = "0954" }
                    );
                    if (rawResponse == null || rawResponse.Items == null || rawResponse.Items.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        foreach(Item item in rawResponse.Items)
                        {
                            if(item.SelectedInvoice?.Reference != null)
                            {
                                int lastSlash = item.SelectedInvoice.Reference.LastIndexOf('/');
                                if (lastSlash > 0)
                                {
                                    string invoiceNumber = item.SelectedInvoice.Reference.Substring(lastSlash+1);
                                    Invoice invoice = await _rsiApi.GetInvoice(invoiceNumber);
                                    item.SelectedInvoice.Invoice = invoice;
                                }
                            }
                        }
                    }
                    Response response = new Response();
                    response.RawResponse = rawResponse;
                    return response;
                }
                else
                {
                    return await GetFakeResponseFromFile(ticketNumber, time);

 
                }
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


                Response response = new Response();
                response.RawResponse = rawResponse;

                var firstInvoice = rawResponse.Items.First().SelectedInvoice.Invoice;

                response.ViolationTime = firstInvoice.term_due_date.Substring(11, 5);
                if (time != response.ViolationTime)
                {
                    // time does not match
                    return null;
                }

                response.ViolationDate = DateTime.Parse(firstInvoice.term_due_date);
                response.ViolationNumber = ticketNumber;

                response.Counts = rawResponse.Items
                    .Select((_, i) => new ViolationCount
                    {
                        CountNumber = i + 1,
                        AmountDue = _.SelectedInvoice.Invoice.amount_due,
                        Description = _.SelectedInvoice.Invoice.attribute1,
                        DueDate = _.SelectedInvoice.Invoice.term_due_date,
                        TicketAmount = _.SelectedInvoice.Invoice.total
                    })
                    .ToList();

                return response;
            }

        }

    }
}
