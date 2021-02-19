using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Net;
using DisputeApi.Web.Features.TicketService;

namespace DisputeApi.Web.Test
{
    public class StartUpTest
    {
        private WebApplicationFactory<Startup> WebAppFactoryObj;

        [SetUp]
        public void SetUp()
        {
            WebAppFactoryObj = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(
                    builder =>
                    {
                        builder.ConfigureTestServices(services => { });
                    });
        }

        [Test]
        public async Task Returns_ok_if_for_health_check()
        {
            using var httpClient = WebAppFactoryObj.CreateClient();
            var response = await httpClient.GetAsync("health");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void startup_should_registered_all_required_services()
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            Assert.IsNotNull(webHost);
            Assert.IsNotNull(webHost.Services.GetService<HealthCheckService>());
            Assert.IsNotNull(webHost.Services.GetService<ITicketService>());
            Assert.IsNotNull(webHost.Services.GetService<IOpenApiDocumentGenerator>());
           
        }
        [Test]
        public void configure_services_should_inject_services()
        {
            IServiceCollection services = new ServiceCollection();
            var target = new Startup();
            target.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            Assert.IsNotNull(serviceProvider);
            Assert.IsTrue(services.Count > 0);

        }
    }

   
}
