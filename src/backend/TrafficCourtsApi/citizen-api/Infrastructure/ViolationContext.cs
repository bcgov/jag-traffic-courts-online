using Gov.CitizenApi.Features.Disputes.DBModel;
using Microsoft.EntityFrameworkCore;


namespace Gov.CitizenApi.Infrastructure
{
    public class ViolationContext : DbContext
    {
        public ViolationContext(DbContextOptions<ViolationContext> options) : base(options)
        {
        }
        public DbSet<Gov.CitizenApi.Models.Ticket> Tickets { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<OffenceDisputeDetail> OffenceDisputeDetails { get; set; }
    }
}
