using MediatR;

namespace DisputeApi.Web.Features.Disputes.Queries
{
    public class GetDisputeQuery : IRequest<GetDisputeResponse>
    {
        public int DisputeId { get; set; }
    }
}
