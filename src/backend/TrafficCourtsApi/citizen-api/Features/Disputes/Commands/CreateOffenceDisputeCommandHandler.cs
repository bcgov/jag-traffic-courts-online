using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Gov.CitizenApi.Features.Disputes.DBModel;
using MediatR;
using Microsoft.Extensions.Logging;
using TrafficCourts.Common.Contract;

namespace Gov.CitizenApi.Features.Disputes.Commands
{
    public class
        CreateOffenceDisputeCommandHandler : IRequestHandler<CreateOffenceDisputeCommand, CreateOffenceDisputeResponse>
    {
        private readonly ILogger _logger;
        private readonly IDisputeService _disputeService;
        private readonly IMapper _mapper;

        public CreateOffenceDisputeCommandHandler(
            ILogger<CreateOffenceDisputeCommandHandler> logger,
            IDisputeService disputeService, 
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<CreateOffenceDisputeResponse> Handle(CreateOffenceDisputeCommand createOffenceDispute, CancellationToken cancellationToken)
        {
            Dispute dispute = await _disputeService.FindTicketDisputeAsync(createOffenceDispute.ViolationTicketNumber);
            if (dispute == null) return new CreateOffenceDisputeResponse { Id=0};

            //update existing to new additional
            dispute.LawyerPresent = createOffenceDispute.Additional?.LawyerPresent??false;
            dispute.InterpreterRequired = createOffenceDispute.Additional?.InterpreterRequired ?? false;
            dispute.InterpreterLanguage = createOffenceDispute.Additional?.InterpreterLanguage;
            dispute.WitnessPresent = createOffenceDispute.Additional?.WitnessPresent??false;

            //if offenceDispute exists, do update, else add new on
            var details = dispute.OffenceDisputeDetails.FirstOrDefault(m => m.OffenceNumber == createOffenceDispute.OffenceNumber);
            if (details != null)
            {
                details.OffenceNumber = createOffenceDispute.OffenceNumber;
                details.MoreTimeReason = createOffenceDispute.OffenceDisputeDetail?.MoreTimeReason;
                details.OffenceAgreementStatus = createOffenceDispute.OffenceDisputeDetail?.OffenceAgreementStatus;
                details.ReductionReason = createOffenceDispute.OffenceDisputeDetail?.ReductionReason;
                details.RequestMoreTime = createOffenceDispute.OffenceDisputeDetail?.RequestMoreTime ?? false;
                details.RequestReduction = createOffenceDispute.OffenceDisputeDetail?.RequestReduction ?? false;
                details.Status = DisputeStatus.Submitted;
            }
            else
            {
                var detail = _mapper.Map<OffenceDisputeDetail>(createOffenceDispute.OffenceDisputeDetail);
                detail.OffenceNumber = createOffenceDispute.OffenceNumber;
                detail.Status = DisputeStatus.Submitted;
                dispute.OffenceDisputeDetails.Add(detail);
            }
            _logger.LogInformation("Offence Dispute created. ");
            var result = await _disputeService.UpdateAsync(dispute);
            //temp remove: todo: uncomment
            //await SendToQueue(_mapper.Map<DisputeContract>(result));
            //temp

            return new CreateOffenceDisputeResponse {Id = result.Id};
        }

    }
}