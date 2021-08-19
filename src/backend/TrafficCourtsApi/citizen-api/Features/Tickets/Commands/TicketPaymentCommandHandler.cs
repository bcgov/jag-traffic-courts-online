using AutoMapper;
using Gov.CitizenApi.Features.Tickets.DBModel;
using Gov.TicketSearch;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Gov.CitizenApi.Features.Tickets.Commands
{
    public class TicketPaymentCommandHandler : IRequestHandler<TicketPaymentCommand, TicketPaymentResponse>
    {
        private readonly ILogger _logger;
        private readonly ITicketsService _ticketsService;
        private readonly ITicketSearchClient _ticketSearchClient;

        private readonly IMapper _mapper;

        public TicketPaymentCommandHandler(
            ITicketSearchClient ticketSearchClient, 
            ILogger<TicketPaymentCommandHandler> logger, 
            ITicketsService ticketService,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ticketsService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _ticketSearchClient = ticketSearchClient ?? throw new ArgumentNullException(nameof(ticketSearchClient));
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
            string callbackUrl = $"{Keys.PaybcApi_CallbackBaseUrl}id={payment.Guid}";
            
            return new TicketPaymentResponse
            {
                ViolationTicketNumber = payment.ViolationTicketNumber,
                ViolationTime = payment.ViolationTime,
                Counts = payment.RequestedCounts,
                CallbackUrl = callbackUrl,
                RedirectUrl = $"{Keys.PaybcApi_BaseUrl}ticket?ticketNumber={payment.ViolationTicketNumber}&time={payment.ViolationTime}&counts={payment.RequestedCounts}&amount={String.Format("{0:0.00}",payment.RequestedAmount)}&callback={HttpUtility.UrlEncode(callbackUrl)}"
            };
        }
    }
}