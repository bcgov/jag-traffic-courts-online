using Microsoft.AspNetCore.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TrafficCourts.Common.Configuration;
using Xunit;

namespace TrafficCourts.Common.Test.Configuration;

public class ConfigurationTest
{
    [Fact]
    public void ConfigureJsonOptions_should_add_JsonStringEnumConverter()
    {
        var options = new Microsoft.AspNetCore.Mvc.JsonOptions();
        Assert.Equal(0, options.JsonSerializerOptions.Converters.Count); // precondition check

        ConfigureJsonOptions sut = new ConfigureJsonOptions();
        
        sut.Configure(options);

        Assert.Equal(1, options.JsonSerializerOptions.Converters.Count);
        Assert.IsType<JsonStringEnumConverter>(options.JsonSerializerOptions.Converters[0]);
    }
}
