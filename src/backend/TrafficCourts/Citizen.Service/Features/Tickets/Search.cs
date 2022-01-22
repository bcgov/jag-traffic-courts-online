using Grpc.Core;
using Grpc.Net.Client;
using MediatR;
using OneOf;
using System.Text.RegularExpressions;
using TrafficCourts.Citizen.Service.Models.Search;
using TrafficCourts.Ticket.Search.Service;
using TicketSearchResult = TrafficCourts.Citizen.Service.Models.Search.TicketSearchResult;

namespace TrafficCourts.Citizen.Service.Features.Tickets
{
    public static class Search
    {
        public class Request : IRequest<Response>
        {
            public const string TicketNumberRegex = "^[A-Z]{2}[0-9]{8}$";
            public const string TimeRegex = "^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$";

            public string TicketNumber { get; }

            /// <summary>
            /// The 24 hour clock
            /// </summary>
            public int Hour { get; }
            public int Minute { get; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="ticketNumber"></param>
            /// <param name="time"></param>
            /// <exception cref="ArgumentNullException"></exception>
            /// <exception cref="ArgumentException"></exception>
            public Request(string ticketNumber, string time)
            {
                ArgumentNullException.ThrowIfNull(ticketNumber);
                ArgumentNullException.ThrowIfNull(time);

                if (!Regex.IsMatch(ticketNumber, TicketNumberRegex))
                {
                    throw new ArgumentException("ticketNumber must start with two upper case letters and 6 or more numbers", nameof(ticketNumber));
                }

                if (!Regex.IsMatch(time, TimeRegex))
                {
                    throw new ArgumentException("time must be properly formatted 24 hour clock", nameof(time));
                }

                TicketNumber = ticketNumber;
                Hour = int.Parse(time[0..2]);
                Minute = int.Parse(time[3..5]);
            }
        }

        public class Response
        {
            private Response()
            {
            }

            public Response(SearchReply reply)
            {
                ArgumentNullException.ThrowIfNull(reply);
                if (reply.ViolationTicketNumber is null) throw new ArgumentException("Search reply does not contain a voilation ticket", nameof(reply));

                TicketSearchResult ticketSearchResult = new()
                {
                    ViolationTicketNumber = reply.ViolationTicketNumber,
                    ViolationDate = new DateTime(reply.ViolationDate.Year, reply.ViolationDate.Month, reply.ViolationDate.Day),
                    ViolationTime = $"{reply.ViolationTime.Hour:d2}:{reply.ViolationTime.Minute:d2}"
                };

                foreach (Offence offence in reply.Offences)
                {
                    Date discountDueDate = offence.DiscountDueDate;

                    TicketOffence ticketOffence = new()
                    {
                        AmountDue = offence.AmountDue / 100m,
                        DiscountAmount = offence.DiscountAmount / 100m,
                        DiscountDueDate = new DateTime(discountDueDate.Year, discountDueDate.Month, discountDueDate.Day),
                        OffenceDescription = offence.OffenceDescription,
                        VehicleDescription = offence.VehicleDescription,
                        OffenceNumber = offence.OffenceNumber,
                        TicketedAmount = offence.TicketedAmount / 100m,
                        InvoiceType = offence.InvoiceType
                    };

                    ticketSearchResult.Offences.Add(ticketOffence);
                }

                Result = ticketSearchResult;
            }

            public Response(Exception exception)
            {
                ArgumentNullException.ThrowIfNull(exception);
                Result = exception;
            }

            /// <summary>
            /// The result value.
            /// </summary>
            public OneOf<TicketSearchResult, Exception> Result { get; }

            public TicketSearchResult? Ticket { get; }

            public static readonly Response Empty = new();
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            private readonly GrpcChannel _grpcChannel;
            private readonly ILogger<Handler> _logger;

            public Handler(GrpcChannel grpcChannel, ILogger<Handler> logger)
            {
                _grpcChannel = grpcChannel ?? throw new ArgumentNullException(nameof(logger));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                ArgumentNullException.ThrowIfNull(request);

                using IDisposable? requestScope = _logger.BeginScope(new Dictionary<string, object> { { "Request", request } });
                _logger.LogTrace("Begin handler");

                TicketSearch.TicketSearchClient client = new(_grpcChannel);

                _logger.LogDebug("Creating SearchRequest");

                SearchRequest? searchResult = new()
                {
                    Number = request.TicketNumber,
                    Time = new TimeOfDay
                    {
                        Hour = request.Hour,
                        Minute = request.Minute
                    }
                };

                try
                {

                    _logger.LogDebug("Searching for ticket");
                    SearchReply searchReply = await client.SearchAsync(searchResult, cancellationToken: cancellationToken);
                    _logger.LogDebug("Search complete");
                    return new Response(searchReply);
                }
                catch (RpcException exception) when (exception.StatusCode is StatusCode.NotFound)
                {
                    _logger.LogInformation(exception, "Not found");
                    return Response.Empty;
                }
                catch (RpcException exception)
                {
                    _logger.LogInformation(exception, "Error");
                    return Response.Empty;
                }
            }
        }
    }
}
