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
                    Id = 1,
                    UserId = "User123",
                    ViolationTicketNumber = "AX87878888",
                    CourtLocation = "Victoria",
                    ViolationDate = "11-12-2002 12:23",
                    SurName = "Smith",
                    GivenNames = "John",
                    Mailing = "Mailing",
                    Postal = "V0W0A0",
                    City = "Victoria",
                    Province = "BC",
                    Licence = "L2323G7",
                    ProvLicense = "L34343G64",
                    HomePhone = "2434332233",
                    WorkPhone = "3345553344",
                    Birthdate = "12-12-2002",
                    LawyerPresent = true,
                    InterpreterRequired = false,
                    CallWitness = false
                }
            );
        }
    }


}

