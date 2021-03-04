using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DisputeApi.Web.Features.TicketService.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSwag.Generation;
using NUnit.Framework;

namespace DisputeApi.Web.Test
{
    public class StartUpTest
    {
        private WebApplicationFactory<Startup> WebAppFactoryObj;

        private Dictionary<string, string> inMemorySettings = new Dictionary<string, string> {
                {"Jwt.Authority", "http://localhost:8080/auth/realms/myrealm/"}, {"Jwt.Audience", "account"}
        };

        [SetUp]
        public void SetUp()
        {
            WebAppFactoryObj = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(
                    builder =>
                    {
                        builder.ConfigureTestServices(services => { });
                        builder.ConfigureAppConfiguration((context, config) =>
                        {
                            config.AddInMemoryCollection(inMemorySettings);
                        });
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
        public async Task Returns_unauthorized_for_missing_token()
        {
            using var httpClient = WebAppFactoryObj.CreateClient();
            var response = await httpClient.GetAsync("api/Tickets/getTickets");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void startup_should_registered_all_required_services()
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            Assert.IsNotNull(webHost);
            Assert.IsNotNull(webHost.Services.GetService<HealthCheckService>());
            Assert.IsNotNull(webHost.Services.GetService<ITicketsService>());
            Assert.IsNotNull(webHost.Services.GetService<IOpenApiDocumentGenerator>());

        }
        [Test]
        public void configure_services_should_inject_services()
        {
            IConfiguration configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(inMemorySettings)
               .Build();
            IServiceCollection services = new ServiceCollection();
            var target = new Startup(configuration);
            target.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            Assert.IsNotNull(serviceProvider);
            Assert.IsTrue(services.Count > 0);

        }
    }


}
