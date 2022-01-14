using Grpc.Core;
using Grpc.Net.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TrafficCourts.Ticket.Search.Service;

namespace TrafficCourts.Citizen.Service.Features.Tickets
{
    public static class Search
    {
        public class Request : IRequest<Response>
        {
            [FromQuery(Name = "ticketNumber")]
            [Required]
            [RegularExpression("^[A-Z]{2}[0-9]{6,}$", ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")]
            public string TicketNumber { get; set; }

            [FromQuery(Name = "time")]
            [Required]
            [RegularExpression("^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$",
                ErrorMessage = "time must be properly formatted 24 hour clock")]
            public string Time { get; set; }
        }

        public class Response
        {
            private Response()
            {
            }

            public Response(SearchReply reply)
            {
            }

            public Response(Exception exception)
            {
            }

            public object Ticket { get; }

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
                TicketSearch.TicketSearchClient client = new TicketSearch.TicketSearchClient(_grpcChannel);

                // 24 hour clock
                var time = int.Parse(request.Time);

                var searchResult = new SearchRequest
                {
                    Number = request.TicketNumber,
                    Time = new TimeOfDay {
                        Hour = time / 100,
                        Minute = time % 100
                    }
                };

                try
                {
                    SearchReply searchReply = await client.SearchAsync(searchResult, cancellationToken: cancellationToken);
                    return new Response(searchReply);
                }
                catch (RpcException exception) when (exception.StatusCode is StatusCode.NotFound)
                {
                    return Response.Empty;
                }
                catch (RpcException exception)
                {
                    return Response.Empty;
                }
            }
        }
    }
}
