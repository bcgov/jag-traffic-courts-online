using AutoMapper;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Contract;

namespace Gov.CitizenApi.Features.Disputes.Commands
{
    public class CreateDisputeCommandHandler : IRequestHandler<CreateDisputeCommand, CreateDisputeResponse>
    {
        private readonly ILogger _logger;
        private readonly IDisputeService _disputeService;
        private readonly IMapper _mapper;
        private readonly IBus _bus;

        public CreateDisputeCommandHandler(ILogger<CreateDisputeCommandHandler> logger, IDisputeService disputeService, IMapper mapper, IBus bus)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
        }

        public async Task<CreateDisputeResponse> Handle(CreateDisputeCommand createDispute,
            CancellationToken cancellationToken)
        {
            var result = await _disputeService.CreateAsync(_mapper.Map<DBModel.Dispute>(createDispute));
            if(result.Id == 0)
            {
                return new CreateDisputeResponse { Id = 0 };
            }
            else
            {
                _logger.LogInformation("Dispute created. ");
                await _bus.Send(_mapper.Map<DisputeContract>(result));

                //send notification
                NotificationContract notificationContract = new NotificationContract
                {
                    NotificationMethod = NotificationMethod.Email,
                    NotificationType = NotificationType.Dispute,
                    TicketDisputeContract = _mapper.Map<TicketDisputeContract>(createDispute)               
                };
                notificationContract.TicketDisputeContract.ConfirmationNumber = result.ConfirmationNumber;
                await _bus.Send(notificationContract);
                return new CreateDisputeResponse { Id = result.Id };
            }
        }
    }
}