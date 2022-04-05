using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using TrafficCourts.Common.Configuration;
using Xunit;

namespace TrafficCourts.Common.Test.Configuration;

public class SwaggerConfigurationTests
{
    [Fact]
    public void should_default_to_disabled()
    {
        var sut = new SwaggerConfiguration();
        Assert.False(sut.Enabled);
    }

    [Fact]
    public void should_default_to_disabled_when_value_not_in_configuration()
    {
        var values = new Dictionary<string, string>();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(Enumerable.Empty<KeyValuePair<string, string>>())
            .Build();

        var sut = SwaggerConfiguration.Get(configuration);
        Assert.False(sut.Enabled);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void should_use_value_from_configuration(string value, bool expected)
    {
        var values = new Dictionary<string, string> { { $"{SwaggerConfiguration.Section}:Enabled", value } };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        var sut = SwaggerConfiguration.Get(configuration);
        Assert.NotNull(sut);
        Assert.Equal(expected, sut.Enabled);
    }
}
