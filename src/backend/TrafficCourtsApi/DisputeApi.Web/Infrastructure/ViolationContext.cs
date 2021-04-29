using DisputeApi.Web.Features.Disputes.DBModel;
using Microsoft.EntityFrameworkCore;


namespace DisputeApi.Web.Infrastructure
{
    public class ViolationContext : DbContext
    {
        public ViolationContext(DbContextOptions<ViolationContext> options) : base(options)
        {
        }
        public DbSet<DisputeApi.Web.Models.Ticket> Tickets { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
        public DbSet<OffenceDisputeDetail> OffenceDisputeDetails { get; set; }
    }
}
