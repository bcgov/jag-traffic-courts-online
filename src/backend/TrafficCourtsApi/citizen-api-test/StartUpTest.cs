using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;
using NSwag.Generation;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Gov.CitizenApi.Features.Tickets;
using Gov.CitizenApi;
using Xunit;

namespace Gov.CitizenApi.Test
{
    [ExcludeFromCodeCoverage]
    public class StartUpTest
    {
        private WebApplicationFactory<Startup> _webApplicationFactory;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private IConfiguration _configuration;

        public StartUpTest()
        {
            var configuration = new Dictionary<string, string>
            {
                {"Jwt:Authority", "http://localhost:8080/auth/realms/myrealm"},
                {"Jwt:Audience", "account"}
            };
            
            _webApplicationFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(
                    builder =>
                    {
                        builder.ConfigureTestServices(services => { });
                        builder.ConfigureAppConfiguration((context, config) =>
                        {
                            config.AddInMemoryCollection(configuration);
                        });
                    });
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            _webHostEnvironmentMock.Setup(m => m.EnvironmentName).Returns("Development");
            
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configuration)
                .Build();
        }

        [Fact]
        public void ConfigureOpenApi_does_not_throw_error()
        {
            var services = new ServiceCollection();

            var sut = new Startup(_webHostEnvironmentMock.Object, _configuration);
            sut.ConfigureOpenApi(services);
        }
        
        [Fact]
        public void ConfigureServices_does_not_throw_error()
        {
            var services = new ServiceCollection();

            var sut = new Startup(_webHostEnvironmentMock.Object, _configuration);
            sut.ConfigureServices(services);
        }

        //do not know why it fails on openshift.
        //[Fact]
        //public async Task Returns_ok_if_for_health_check()
        //{
        //using var httpClient = _webApplicationFactory.CreateClient();
        //var response = await httpClient.GetAsync("health");
        //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        //}

        /* TODO Add back once authentication is figured out
        [Fact]
        public async Task Returns_unauthorized_for_missing_token()
        {
            using var httpClient = WebAppFactoryObj.CreateClient();
            var response = await httpClient.GetAsync("api/Tickets/getTickets");
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
        */

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void missing_jwt_config()
#pragma warning restore IDE1006 // Naming Styles
        {
            Dictionary<string, string> emptyConfig = new Dictionary<string, string>
            {
                {"Jwt:Authority", ""}, {"Jwt:Audience", ""}
            };

            IConfiguration emptyConfiguration = new ConfigurationBuilder()
                    .AddInMemoryCollection(emptyConfig)
                    .Build();

            var target = new Startup(_webHostEnvironmentMock.Object, emptyConfiguration);
            Assert.Throws<ConfigurationErrorsException>(() => target.ConfigureJwtBearerAuthentication(new JwtBearerOptions()));
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void success_jwt_config()
#pragma warning restore IDE1006 // Naming Styles
        {
            var target = new Startup(_webHostEnvironmentMock.Object, _configuration);
            JwtBearerOptions options = new JwtBearerOptions();
            target.ConfigureJwtBearerAuthentication(options);
            Assert.Equal("http://localhost:8080/auth/realms/myrealm/", options.Authority);
            Assert.False(options.RequireHttpsMetadata);
            Assert.Equal("http://localhost:8080/auth/realms/myrealm/.well-known/uma2-configuration", options.MetadataAddress);
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void startup_should_registered_all_required_services()
#pragma warning restore IDE1006 // Naming Styles
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            Assert.NotNull(webHost);
            Assert.NotNull(webHost.Services.GetService<HealthCheckService>());
            Assert.NotNull(webHost.Services.GetService<ITicketsService>());
            Assert.NotNull(webHost.Services.GetService<IOpenApiDocumentGenerator>());
        }

        [Fact]
#pragma warning disable IDE1006 // Naming Styles
        public void configure_services_should_inject_services()
#pragma warning restore IDE1006 // Naming Styles
        {
            IServiceCollection services = new ServiceCollection();

            var target = new Startup(_webHostEnvironmentMock.Object, _configuration);
            target.ConfigureServices(services);
            
            var serviceProvider = services.BuildServiceProvider();
            Assert.NotNull(serviceProvider);
            Assert.True(services.Count > 0);
        }
    }
}
