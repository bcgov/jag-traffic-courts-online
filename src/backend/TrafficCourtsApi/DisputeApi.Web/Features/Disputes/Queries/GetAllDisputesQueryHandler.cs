using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DisputeApi.Web.Features.Disputes.Queries
{
    public class GetAllDisputesQueryHandler : IRequestHandler<GetAllDisputesQuery, IEnumerable<GetDisputeResponse>>
    {
        private readonly ILogger _logger;
        private readonly IDisputeService _disputeService;
        private readonly IMapper _mapper;
        public GetAllDisputesQueryHandler(ILogger<DisputesController> logger, IDisputeService disputeService, IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _disputeService = disputeService ?? throw new ArgumentNullException(nameof(disputeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<GetDisputeResponse>> Handle(GetAllDisputesQuery request, CancellationToken cancellationToken)
        {
            var disputes = await _disputeService.GetAllAsync();
            return _mapper.Map<IEnumerable<GetDisputeResponse>>(disputes);
        }
    }
}
