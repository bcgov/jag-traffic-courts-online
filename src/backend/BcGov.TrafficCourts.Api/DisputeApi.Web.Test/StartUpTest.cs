using DisputeApi.Web.Features.Tickets.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation;
using NUnit.Framework;

namespace DisputeApi.Web.Test
{
    public class StartUpTest
    {

        [Test]
        public void startup_should_registered_all_required_services()
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            Assert.IsNotNull(webHost);
            Assert.IsNotNull(webHost.Services.GetService<HealthCheckService>());
            Assert.IsNotNull(webHost.Services.GetService<ITicketService>());
            Assert.IsNotNull(webHost.Services.GetService<IOpenApiDocumentGenerator>());
           
        }
    }
}
