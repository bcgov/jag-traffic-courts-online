using MediatR;

namespace Gov.CitizenApi.Features.Disputes.Queries
{
    public class GetDisputeQuery : IRequest<GetDisputeResponse>
    {
        public int DisputeId { get; set; }
    }
}
