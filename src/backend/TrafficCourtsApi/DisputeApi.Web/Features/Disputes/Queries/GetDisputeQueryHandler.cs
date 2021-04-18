using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.Disputes.Queries
{
    public class GetDisputeQueryHandler : IRequestHandler<GetDisputeQuery, GetDisputeResponse>
    {
        private readonly ILogger _logger;
        private readonly IDisputeService _disputeService;
        private readonly IMapper _mapper;
        public GetDisputeQueryHandler(ILogger<DisputesController> logger, IDisputeService disputeService, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GetDisputeResponse> Handle(GetDisputeQuery request, CancellationToken cancellationToken)
        {
            DBModel.Dispute dispute = await _disputeService.GetAsync(request.DisputeId);
            
            return _mapper.Map<GetDisputeResponse>(dispute);
        }
    }
}
