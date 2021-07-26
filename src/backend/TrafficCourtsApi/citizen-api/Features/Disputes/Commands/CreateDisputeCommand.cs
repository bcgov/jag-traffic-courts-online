using Gov.CitizenApi.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Gov.CitizenApi.Features.Disputes.Commands
{
    public class CreateDisputeCommand : TicketDispute,IRequest<CreateDisputeResponse>
    {

    }

 
}
