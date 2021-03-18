using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

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
            [JsonProperty("violation_number")]
            public string ViolationNumber { get; set; }
            [JsonProperty("violation_time")]

            public string ViolationTime { get; set; }
            [JsonProperty("violation_date")]
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
            public async Task<Response> Handle(Query query, CancellationToken cancellationToken)
            {
                string ticketNumber = query.TicketNumber;
                string time = query.Time;

                RawTicketSearchResponse rawResponse = await DeserializeAsync<RawTicketSearchResponse>($"Features/TicketLookup/ticket-{ticketNumber}.json");

                if (rawResponse == null)
                {
                    return null;
                }

                for (int i = 0; i < rawResponse.Items.Count; i++)
                {
                    var invoice = await DeserializeAsync<Invoice>($"Features/TicketLookup/invoice-{ticketNumber}{i + 1}.json");
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

            private async Task<T> DeserializeAsync<T>(string resourcePath) where T : class
            {
                var provider = new ManifestEmbeddedFileProvider(GetType().Assembly);

                var fileInfo = provider.GetFileInfo(resourcePath);
                if (!fileInfo.Exists)
                {
                    return null;
                }

                using var streamReader = new StreamReader(fileInfo.CreateReadStream());
                string json = await streamReader.ReadToEndAsync();

                var settings = new JsonSerializerSettings();
                settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;

                T data = JsonConvert.DeserializeObject<T>(json, settings);
                return data;
            }
        }

        #region Raw RSI data for troubleshooting
        public class ViolationCount
        {
            [JsonProperty("count_number")]
            public int CountNumber { get; set; }

            [JsonProperty("ticket_amount")]
            public decimal TicketAmount { get; set; }

            [JsonProperty("amount_due")]
            public decimal AmountDue { get; set; }

            [JsonProperty("due_date")]
            public string DueDate { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }

        }


        public class RawTicketSearchResponse
        {
            [JsonProperty("items")]
            public List<Item> Items { get; set; }
        }

        public class Item
        {
            [JsonProperty("selected_invoice")]
            public SelectedInvoice SelectedInvoice { get; set; }

            [JsonProperty("open_invoices_for_site")]
            public object OpenInvoicesForSite { get; set; }
        }

        public class SelectedInvoice
        {
            [JsonProperty("$ref")]
            public string Reference { get; set; }

            [JsonProperty("invoice")]
            public Invoice Invoice { get; set; }
        }

        public class Invoice
        {
            [JsonProperty("invoice_number")]
            public string invoice_number { get; set; }

            [JsonProperty("pbc_ref_number")]
            public string pbc_ref_number { get; set; }

            [JsonProperty("party_number")]
            public string party_number { get; set; }

            [JsonProperty("party_name")]
            public string party_name { get; set; }

            [JsonProperty("account_number")]
            public string account_number { get; set; }

            [JsonProperty("site_number")]
            public string site_number { get; set; }

            [JsonProperty("cust_trx_type")]
            public string cust_trx_type { get; set; }

            [JsonProperty("term_due_date")]
            public string term_due_date { get; set; }

            [JsonProperty("total")]
            public decimal total { get; set; }

            [JsonProperty("amount_due")]
            public decimal amount_due { get; set; }

            [JsonProperty("attribute1")]
            public string attribute1 { get; set; }

            [JsonProperty("attribute2")]
            public string attribute2 { get; set; }

            [JsonProperty("attribute3")]
            public string attribute3 { get; set; }

            [JsonProperty("attribute4")]
            public string attribute4 { get; set; }
        }


        #endregion
    }
}
