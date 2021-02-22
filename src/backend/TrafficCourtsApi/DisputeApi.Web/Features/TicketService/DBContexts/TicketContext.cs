using Microsoft.EntityFrameworkCore;
using DisputeApi.Web.Features.TicketService.Models;

namespace DisputeApi.Web.Features.TicketService.DBContexts
{
    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions<TicketContext> options) : base(options) { }
        public DbSet<Ticket> Tickets { get; set; }

    }


}

