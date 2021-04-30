using DisputeApi.Web.Models;
using MediatR;

namespace DisputeApi.Web.Features.Disputes.Commands
{
    public class CreateOffenceDisputeCommand : OffenceDispute, IRequest<CreateOffenceDisputeResponse>
    {
    }
}
