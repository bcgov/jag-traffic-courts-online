using Grpc.Core;
using System.Diagnostics;
using System.Globalization;
using TrafficCourts.Ticket.Search.Service.Features.Search;

namespace TrafficCourts.Ticket.Search.Service.Services
{
    public class TicketSearchService : TicketSearch.TicketSearchBase
    {
        private readonly ITicketSearchService _ticketSearchService;
        private readonly ILogger<TicketSearchService> _logger;

        public TicketSearchService(ITicketSearchService ticketSearchService, ILogger<TicketSearchService> logger)
        {
            _ticketSearchService = ticketSearchService ?? throw new ArgumentNullException(nameof(ticketSearchService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<SearchReply> Search(SearchRequest request, ServerCallContext context)
        {
            ArgumentNullException.ThrowIfNull(request);

            var time = $"{request.Time.Hour:d2}:{request.Time.Minute:d2}";

            using var scope = _logger.BeginScope(new Dictionary<string, string> { { "TicketNumber", request.Number }, { "Time", time } });

            try
            {
                _logger.LogDebug("Searching for violation ticket");

                // call the ticket service
                IEnumerable<Invoice>? response = await _ticketSearchService.SearchTicketAsync(request.Number, time, context.CancellationToken);
                var invoices = response.ToList();

                if (invoices.Count != 0)
                {
                    _logger.LogDebug("Found violation ticket with {OffenceCount} offence", invoices.Count);
                    var reply = CreateReply(request, invoices);

                    using var replyScope = _logger.BeginScope(new Dictionary<string, object> { { "SearchReply", reply } });
                    _logger.LogDebug("Search complete");

                    return reply;
                }

                _logger.LogDebug("Violation ticket not found");
                Status status = new Status(StatusCode.NotFound, "Violation ticket not found");
                throw new RpcException(status);
            }
            catch (Exception exception)
            {
                _logger.LogInformation(exception, "Error finding violation ticket");
                Status status = new Status(StatusCode.Internal, "Error finding violation ticket");
                throw new RpcException(status);
            }
        }

        private SearchReply CreateReply(SearchRequest request, List<Invoice> invoices)
        {
            Debug.Assert(invoices.Count != 0);

            if (!string.IsNullOrEmpty(invoices[0].ViolationDateTime))
            {
                _logger.LogInformation("Violation date and time is empty");
            }

            SearchReply reply = new SearchReply();

            reply.ViolationTicketNumber = request.Number;

            if (DateTime.TryParseExact(invoices[0].ViolationDateTime, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime violationDateTime))
            {
                reply.ViolationDate = new Date
                {
                    Year = violationDateTime.Year,
                    Month = violationDateTime.Month,
                    Day = violationDateTime.Day,
                };

                reply.ViolationTime = new TimeOfDay
                {
                    Hour = violationDateTime.Hour,
                    Minute = violationDateTime.Minute
                };
            }

            foreach (var invoice in invoices)
            {
                var offence = CreateOffence(invoice);
                reply.Offences.Add(offence);
            }

            return reply;
        }

        private Offence CreateOffence(Invoice item)
        {
            var offence = new Offence();
            if (!string.IsNullOrEmpty(item.InvoiceNumber))
            {
                offence.OffenceNumber = (int)char.GetNumericValue(item.InvoiceNumber[^1]);
            }
            offence.AmountDue = (int)item.AmountDue * 100;
            offence.OffenceDescription = item.OffenceDescription;
            offence.InvoiceType = item.InvoiceType;
            offence.VehicleDescription = item.VehicleDescription ?? String.Empty;
            offence.TicketedAmount = (int)item.TicketedAmount * 100;

            if (item.DiscountAmount == "n/a")
            {
                offence.DiscountAmount = 0;
                offence.DiscountDueDate = null;
            }
            else
            {
                offence.DiscountAmount = (int)(Convert.ToDecimal(item.DiscountAmount) * 100);

                if (string.IsNullOrEmpty(item.ViolationDateTime))
                {

                }
                else
                {
                    if (DateTime.TryParse(item.ViolationDateTime, out DateTime violationDateTime))
                    {
                        var plus30days = violationDateTime.AddDays(30);
                        offence.DiscountDueDate.Year = plus30days.Year;
                        offence.DiscountDueDate.Month = plus30days.Month;
                        offence.DiscountDueDate.Day = plus30days.Day;
                    }
                }
            }

            return offence;
        }
    }
}