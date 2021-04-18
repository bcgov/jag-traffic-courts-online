using AutoMapper;
using DisputeApi.Web.Messaging.Configuration;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Contract;
using ContractDispute = TrafficCourts.Common.Contract.Dispute;

namespace DisputeApi.Web.Features.Disputes.Commands
{
    public class CreateDisputeCommandHandler : IRequestHandler<CreateDisputeCommand, CreateDisputeResponse>
    {
        private readonly ILogger _logger;
        private readonly IDisputeService _disputeService;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly RabbitMQConfiguration _rabbitMQConfig;
        private readonly IMapper _mapper;

        public CreateDisputeCommandHandler(ILogger<DisputesController> logger, IDisputeService disputeService,
            ISendEndpointProvider sendEndpointProvider, IOptions<RabbitMQConfiguration> rabbitMqOptions, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
            _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
            _rabbitMQConfig = rabbitMqOptions.Value ?? throw new ArgumentNullException(nameof(rabbitMqOptions));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CreateDisputeResponse> Handle(CreateDisputeCommand createDispute, CancellationToken cancellationToken)
        {
            
            var result = await _disputeService.CreateAsync(_mapper.Map<DBModel.Dispute>(createDispute));

            if (result == null)
            {
                return new CreateDisputeResponse { Id = 0 };

            }

            await SendToQueue(_mapper.Map<ContractDispute>(createDispute));
            return new CreateDisputeResponse { Id=result.Id};
        }

        private async Task SendToQueue(ContractDispute dispute)
        {
            ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"rabbitmq://{_rabbitMQConfig.Host}:{_rabbitMQConfig.Port}/{Constants.DisputeRequestedQueueName}"));

            await sendEndpoint.Send<ContractDispute>(dispute);

            return;
        }
    }
}
