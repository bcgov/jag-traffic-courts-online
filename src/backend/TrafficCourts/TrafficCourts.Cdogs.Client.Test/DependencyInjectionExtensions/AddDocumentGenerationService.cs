using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrafficCourts.Core.Http;

namespace TrafficCourts.Cdogs.Client.Test.DependencyInjectionExtensions;

public class AddDocumentGenerationService
{
    [Fact(Skip = "Requires User Secrets")]
    public void adding_documement_generation_service_can_resolve_service()
    {
        var services = new ServiceCollection();

        Fixture fixture = new Fixture();
        var clientConfiguration = fixture.Create<OidcConfidentialClientConfiguration>();
        var section = fixture.Create<string>();

        IConfiguration configuration = new ConfigurationBuilder()
            .AddUserSecrets<AddDocumentGenerationService>()
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        Extensions.AddDocumentGenerationService(services, "Cdogs");

        var serviceProvider = services.BuildServiceProvider();

        var actual = serviceProvider.GetRequiredService<IDocumentGenerationService>();
        Assert.IsType<Client.DocumentGenerationService>(actual);
    }
}
