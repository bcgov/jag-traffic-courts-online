using Gov.CitizenApi.Models;
using MediatR;

namespace Gov.CitizenApi.Features.Tickets.Commands
{
    public class CreateShellTicketCommand : ShellTicket, IRequest<CreateShellTicketResponse>
    {
    }
}
