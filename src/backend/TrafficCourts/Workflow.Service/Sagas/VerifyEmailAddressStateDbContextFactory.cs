using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TrafficCourts.Workflow.Service.Sagas;

/// <summary>
/// IDesignTimeDbContextFactory used by EF Migrations to create the context.
/// </summary>
public class VerifyEmailAddressStateDbContextFactory : IDesignTimeDbContextFactory<VerifyEmailAddressStateDbContext>
{
    public VerifyEmailAddressStateDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VerifyEmailAddressStateDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=password;Database=postgres");

        return new VerifyEmailAddressStateDbContext(optionsBuilder.Options);
    }
}
