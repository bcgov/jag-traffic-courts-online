using Gov.CitizenApi.Features.Tickets.DBModel;
using Gov.CitizenApi.Features.Disputes.DBModel;
using Microsoft.EntityFrameworkCore;


namespace Gov.CitizenApi.Infrastructure
{
    public class ViolationContext : DbContext
    {
        public ViolationContext(DbContextOptions<ViolationContext> options) : base(options)
        {
        }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Offence> Offences { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<OffenceDisputeDetail> OffenceDisputeDetails { get; set; }
    }
}
