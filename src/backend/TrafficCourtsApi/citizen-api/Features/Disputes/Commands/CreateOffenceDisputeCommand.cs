using Gov.CitizenApi.Models;
using MediatR;

namespace Gov.CitizenApi.Features.Disputes.Commands
{
    public class CreateOffenceDisputeCommand : OffenceDispute, IRequest<CreateOffenceDisputeResponse>
    {
    }
}
