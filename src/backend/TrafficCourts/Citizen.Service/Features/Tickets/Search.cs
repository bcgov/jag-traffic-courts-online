using Grpc.Core;
using Grpc.Net.Client;
using MediatR;
using OneOf;
using System.Text.RegularExpressions;
using TrafficCourts.Ticket.Search.Service;
using TicketSearchResult = TrafficCourts.Citizen.Service.Models.Search.TicketSearchResult;

namespace TrafficCourts.Citizen.Service.Features.Tickets
{
    public static class Search
    {
        public class Request : IRequest<Response>
        {
            public const string TicketNumberRegex = "^[A-Z]{2}[0-9]{6,}$";
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
                    throw new ArgumentException(nameof(ticketNumber));
                }

                if (!Regex.IsMatch(time, TimeRegex))
                {
                    throw new ArgumentException(nameof(time));
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
                TicketSearchResult ticketSearchResult = new TicketSearchResult
                {
                    // set properties
                };

                Result = ticketSearchResult;
            }

            public Response(Exception exception)
            {
                Result = exception;
            }

            public OneOf<TicketSearchResult, Exception> Result { get; }

            public TicketSearchResult? Ticket { get; }

            public static readonly Response Empty = new Response();
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

                using var requestScope = _logger.BeginScope(new Dictionary<string, object> { { "Request", request } });
                _logger.LogTrace("Begin handler");

                TicketSearch.TicketSearchClient client = new TicketSearch.TicketSearchClient(_grpcChannel);

                _logger.LogDebug("Creating SearchRequest");

                var searchResult = new SearchRequest
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
