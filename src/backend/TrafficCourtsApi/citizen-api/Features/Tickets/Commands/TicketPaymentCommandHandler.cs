using AutoMapper;
using Gov.CitizenApi.Features.Tickets.DBModel;
using Gov.CitizenApi.Models;
using Gov.TicketSearch;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Gov.CitizenApi.Features.Tickets.Commands
{
    public class TicketPaymentCommand : IRequest<TicketPaymentResponse>
    {
        [FromQuery(Name = "ticketNumber")]
        [Required]
        [RegularExpression("^[A-Z]{2}[0-9]{6,}$", ErrorMessage = "ticketNumber must start with two upper case letters and 6 or more numbers")]
        public string TicketNumber { get; set; }

        [FromQuery(Name = "time")]
        [Required]
        [RegularExpression("^(2[0-3]|[01]?[0-9]):([0-5]?[0-9])$", ErrorMessage = "time must be properly formatted 24 hour clock")]
        public string Time { get; set; }

        [FromQuery(Name = "counts")]
        [Required]
        [RegularExpression("^[1-3]+(,[1-3]+)*$", ErrorMessage = "counts must be properly formatted, user , as seperatoer")]
        public string Counts { get; set; }

        [FromQuery(Name = "amount")]
        [Required]
        [RegularExpression(@"^\d*\.?\d*$", ErrorMessage = "amount needs to be a valid decimal")]
        public string Amount { get; set; }
    }

    public class TicketPaymentResponse : RedirectPay
    {
    }

    public class TicketPaymentCommandHandler : IRequestHandler<TicketPaymentCommand, TicketPaymentResponse>
    {
        private readonly ILogger _logger;
        private readonly ITicketsService _ticketsService;

        private readonly IMapper _mapper;

        public TicketPaymentCommandHandler(
            ITicketSearchClient ticketSearchClient, 
            ILogger<TicketPaymentCommandHandler> logger, 
            ITicketsService ticketService,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ticketsService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<TicketPaymentResponse> Handle(TicketPaymentCommand ticketPaymentCommand,
            CancellationToken cancellationToken)
        {
            Payment payment = await _ticketsService.CreatePaymentAsync(_mapper.Map<DBModel.Payment>(ticketPaymentCommand));
            return BuildTicketPaymentResponse(payment);
        }

        private TicketPaymentResponse BuildTicketPaymentResponse(Payment payment)
        {
            string callbackUrl = $"{Keys.PaybcApi_CallbackBaseUrl}?id={payment.Guid}";
            NameValueCollection redirectUrlQueryParam = new NameValueCollection() 
            {
                {"ticketNumber", payment.ViolationTicketNumber },
                {"time", payment.ViolationTime },
                {"counts", payment.RequestedCounts },
                {"amount", $"{payment.RequestedAmount:0.00}" },
                {"callback", callbackUrl }
            };
            UriBuilder uriBuilder = new UriBuilder(Keys.PaybcApi_BaseUrl);
            uriBuilder.Query = ToQueryString(redirectUrlQueryParam);

            return new TicketPaymentResponse
            {
                ViolationTicketNumber = payment.ViolationTicketNumber,
                ViolationTime = payment.ViolationTime,
                Counts = payment.RequestedCounts,
                CallbackUrl = callbackUrl,
                RedirectUrl = uriBuilder.Uri.ToString()
            };
        }

        private string ToQueryString(NameValueCollection nvc)
        {
            var array = (
                from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format(
                    "{0}={1}",
                    HttpUtility.UrlEncode(key),
                    HttpUtility.UrlEncode(value))
                        ).ToArray();
            return "?" + string.Join("&", array);
        }
    }
}