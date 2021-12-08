using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using MediatR;
using TrafficCourts.Ticket.Search.Service.Features.Search.RoadSafetyTicketSearch;
using TrafficCourts.Ticket.Search.Service.Models;

namespace TrafficCourts.Ticket.Search.Service.Features.Search
{
    public static class TicketSearch
    {
        public class Request : IRequest<Response>
        {
            /// <summary>
            /// The traffic violation ticket number
            /// </summary>
            [FromQuery(Name = "ticketNumber")]
            [Required]
            [RegularExpression("^[A-Z]{2}[0-9]{6,}$", ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")]
            public string TicketNumber { get; set; } = null!;

            /// <summary>
            /// The time of traffic violation
            /// </summary>
            [FromQuery(Name = "time")]
            [Required]
            [RegularExpression("^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessage = "time must be properly formatted 24 hour clock")]
            public string Time { get; set; } = null!;
        }

        public class Response
        {
            public static readonly Response NotFound = new();

            public Response()
            {
                Offences = new List<Offence>();
            }

            /// <summary>
            /// The traffic violation ticket number of the searching result
            /// </summary>
            public string? ViolationTicketNumber { get; set; }

            /// <summary>
            /// The traffic violation time of the searching result in format of hh:mm
            /// </summary>
            public string? ViolationTime { get; set; }

            /// <summary>
            /// The traffic violation date of the searching result in format of yyyy-mm-dd
            /// </summary>
            public string? ViolationDate { get; set; }

            /// <summary>
            /// The list of all offences of the searching ticket
            /// </summary>
            public List<Offence> Offences { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly ITicketSearchService _service;
            private readonly ILogger<Handler> _logger;

            public Handler(ITicketSearchService service, ILogger<Handler> logger)
            {
                _service = service ?? throw new ArgumentNullException(nameof(service));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    IEnumerable<Invoice> invoices = await _service.SearchTicketAsync(request.TicketNumber, request.Time, cancellationToken);

                    Response response = CreateResponse(invoices);

                    return response;
                }
                catch (Exception exception)
                {
                    _logger.LogInformation(exception, "Error searching for ticket");
                    throw;
                }
            }

            private Response CreateResponse(IEnumerable<Invoice> invoices)
            {
                var items = invoices.ToList();

                if (items.Count == 0)
                {
                    return Response.NotFound;
                }

                Response response = new();
 
                Invoice firstInvoice = items[0];
                if (firstInvoice != null)
                {
                    if (!string.IsNullOrEmpty(firstInvoice.InvoiceNumber))
                    {
                        response.ViolationTicketNumber = firstInvoice.InvoiceNumber.Remove(firstInvoice.InvoiceNumber.Length - 1);
                    }

                    if (!string.IsNullOrEmpty(firstInvoice.ViolationDateTime))
                    {
                        if (DateTime.TryParse(firstInvoice.ViolationDateTime, out DateTime violationDateTime))
                        {
                            response.ViolationDate = violationDateTime.ToString("yyyy-MM-dd");
                            response.ViolationTime = violationDateTime.ToString("HH:mm");
                        }
                    }
                }

                response.Offences.AddRange(items.Where(item => item is not null).Select(CreateOffence));

                return response;
            }

            private Offence CreateOffence(Invoice item)
            {
                var offence = new Offence();
                if (!string.IsNullOrEmpty(item.InvoiceNumber))
                {
                    offence.OffenceNumber = (int)char.GetNumericValue(item.InvoiceNumber[^1]);
                }
                offence.AmountDue = item.AmountDue;
                offence.OffenceDescription = item.OffenceDescription;
                offence.InvoiceType = item.InvoiceType;
                offence.VehicleDescription = item.VehicleDescription;
                offence.ViolationDateTime = item.ViolationDateTime;
                offence.TicketedAmount = item.TicketedAmount;

                if (item.DiscountAmount == "n/a")
                {
                    offence.DiscountAmount = 0;
                    offence.DiscountDueDate = null;
                }
                else
                {
                    offence.DiscountAmount = Convert.ToDecimal(item.DiscountAmount);

                    if (!string.IsNullOrEmpty(item.ViolationDateTime))
                    {
                        if (DateTime.TryParse(item.ViolationDateTime, out DateTime violationDateTime))
                        {
                            offence.DiscountDueDate = violationDateTime.AddDays(30).ToString("yyyy-MM-dd");
                        }
                    }
                }

                return offence;
            }
        }
    }
}
