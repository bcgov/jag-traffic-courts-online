using DisputeApi.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace DisputeApi.Web.Infrastructure
{
    public class ViolationContext : DbContext
    {
        public ViolationContext(DbContextOptions<ViolationContext> options) : base(options)
        {
        }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Dispute> Disputes { get; set; }
    }
}
