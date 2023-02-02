using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using TrafficCourts.Common.Configuration.Validation;
using TrafficCourts.Common.OpenAPIs.VirusScan;
using TrafficCourts.Common.OpenAPIs.VirusScan.V1;
using Xunit;

namespace TrafficCourts.Common.Test.OpenAPIs;

public class AddVirusScanClientTest
{
    [Fact]
    public void AddVirusScanClient_adds_IVirusScanClient_as_injectable_service()
    {
        ServiceCollection services = new();

        Dictionary<string, string?> items = new()
        {
            { $"{VirusScanApiConfiguration.Section}:{nameof(VirusScanApiConfiguration.BaseUrl)}", "http://localhost/" }
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(items)
            .Build();

        // act
        _ = DependencyInjectionExtensions.AddVirusScanClient(services, configuration);

        // assert
        ServiceProvider provider = services.BuildServiceProvider();
        IVirusScanClient actual = provider.GetRequiredService<IVirusScanClient>();

        Assert.NotNull(actual);
    }

    [Fact]
    public void AddVirusScanClient_throws_SettingsValidationException_if_configuration_section_not_found()
    {
        ServiceCollection services = new();

        IConfiguration configuration = new ConfigurationBuilder()
            .Build();

        // act
        var actual = Assert.Throws<SettingsValidationException>(() => DependencyInjectionExtensions.AddVirusScanClient(services, configuration));
        Assert.NotNull(actual);
    }
}
