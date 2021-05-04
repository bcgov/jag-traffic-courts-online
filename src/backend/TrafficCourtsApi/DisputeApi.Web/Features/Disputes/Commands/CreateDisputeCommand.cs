using DisputeApi.Web.Models;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace DisputeApi.Web.Features.Disputes.Commands
{
    public class CreateDisputeCommand : TicketDispute,IRequest<CreateDisputeResponse>
    {

    }

 
}
