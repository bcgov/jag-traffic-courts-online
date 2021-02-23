using Microsoft.EntityFrameworkCore;
using DisputeApi.Web.Features.TicketService.Models;

namespace DisputeApi.Web.Features.TicketService.DBContexts
{
    public class TicketContext : DbContext
    {
        public TicketContext(DbContextOptions<TicketContext> options) : base(options) { }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>().HasData(
                new Ticket
                {
                    TicketNumber = 11234,
                    Name = "John Doe",
                    DateOfIssue = "11-12-2002",
                    TimeOfIssue = "12:23",
                    DriversLicence = "L2323G7"
                }
            );
        }
    }


}

