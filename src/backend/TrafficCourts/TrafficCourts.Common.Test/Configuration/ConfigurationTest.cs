using System.Text.Json.Serialization;
using TrafficCourts.Common.Configuration;
using Xunit;

namespace TrafficCourts.Common.Test.Configuration;

public class ConfigurationTest
{
    [Fact]
    public void ConfigureJsonOptions_should_add_JsonStringEnumConverter()
    {
        var options = new Microsoft.AspNetCore.Mvc.JsonOptions();
        Assert.Empty(options.JsonSerializerOptions.Converters); // precondition check

        ConfigureJsonOptions sut = new ConfigureJsonOptions();
        
        sut.Configure(options);

        Assert.Single(options.JsonSerializerOptions.Converters);
        Assert.IsType<JsonStringEnumConverter>(options.JsonSerializerOptions.Converters[0]);
    }
}
