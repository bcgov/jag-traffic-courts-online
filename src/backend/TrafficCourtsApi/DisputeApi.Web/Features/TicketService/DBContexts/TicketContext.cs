using Microsoft.EntityFrameworkCore;
using DisputeApi.Web.Features.TicketService.Models;
using DisputeApi.Web.Features.TcoDispute.Models;

namespace DisputeApi.Web.Features.TicketService.DBContexts
{
    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions<TicketContext> options) : base(options) { }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Dispute> Disputes { get; set; }

    }


}

