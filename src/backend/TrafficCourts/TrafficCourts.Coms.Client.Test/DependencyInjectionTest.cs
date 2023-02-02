using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Coms.Client.Test;

using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrafficCourts.Coms.Client;

public class DependencyInjectionTest
{
    private readonly Fixture _fixture = new Fixture();

    [Fact]
    public void should_register_service_without_MemoryStream_factory()
    {
        register_service();
    }

    [Fact]
    public void should_register_service_with_MemoryStream_factory()
    {
        register_service(() => new MemoryStream());
    }

    private void register_service(Func<MemoryStream>? factory = null)
    {
        ServiceCollection services = new ServiceCollection();

        ObjectManagementServiceConfiguration serviceConfiguration = new()
        {
            BaseUrl = $"https://host-{Guid.NewGuid():n}",
            Username = _fixture.Create<string>(),
            Password = _fixture.Create<string>()
        };

        string section = "section-" + _fixture.Create<string>();

        IConfiguration configuration = BuildConfiguration(section, serviceConfiguration);
        services.AddSingleton(configuration);

        if (factory is null)
        {
            services.AddObjectManagementService(section);
        }
        else
        {
            services.AddObjectManagementService(section, factory);
        }

        // check the public service is created correctly
        var actual = services.SingleOrDefault(Exists<IObjectManagementService, ObjectManagementService>);
        Assert.NotNull(actual);
        Assert.Equal(ServiceLifetime.Transient, actual!.Lifetime);

        // make sure it can be resolved
        var serviceProvider = services.BuildServiceProvider();

        var actualService = serviceProvider.GetRequiredService<IObjectManagementService>();
        Assert.NotNull(actual);

        // check the internal client is created correctly
        var actualClientInterface = serviceProvider.GetRequiredService<IObjectManagementClient>();
        var actualClient = Assert.IsAssignableFrom<ObjectManagementClient>(actualClientInterface);

        // check base url

        // check client.DefaultRequestHeaders.Authorization is set
    }

    private IConfiguration BuildConfiguration(string section, ObjectManagementServiceConfiguration configuration)
    {
        var items = new Dictionary<string, string?>
            {
                { $"{section}:{nameof(ObjectManagementServiceConfiguration.BaseUrl)}", configuration.BaseUrl },
                { $"{section}:{nameof(ObjectManagementServiceConfiguration.Username)}", configuration.Username! },
                { $"{section}:{nameof(ObjectManagementServiceConfiguration.Password)}", configuration.Password! }
            };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(items)
            .Build();
    }

    private static bool Exists<TService, TImplementation>(ServiceDescriptor descriptor)
    {
        return descriptor.ServiceType == typeof(TService) && descriptor.ImplementationType == typeof(TImplementation);
    }

}