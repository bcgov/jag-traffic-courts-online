using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace TrafficCourts.Workflow.Service.Sagas;

public class VerifyEmailAddressStateDbContext : SagaDbContext
{
    public VerifyEmailAddressStateDbContext(DbContextOptions<VerifyEmailAddressStateDbContext> options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new VerifyEmailAddressStateMap(); }
    }
}
