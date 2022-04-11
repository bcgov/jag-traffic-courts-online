using AutoFixture;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TrafficCourts.Staff.Service.Authentication;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Authentication;

public class ConfigureJwtBearerOptionsTest
{
    [Fact]
    public void constructor_checks_for_null()
    {
        Assert.Throws<ArgumentNullException>(() => new ConfigureJwtBearerOptions(null!));
    }

    [Fact]
    public void configures_Authority_and_Audience()
    {
        Fixture fixture = new Fixture();
        var authority = fixture.Create<string>();
        var audience = fixture.Create<string>();

        var values = new Dictionary<string, string>();
        values.Add("Jwt:Authority", authority);
        values.Add("Jwt:Audience", audience);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        var sut = new ConfigureJwtBearerOptions(configuration);

        var actual = new JwtBearerOptions();
        sut.Configure(actual);

        Assert.Equal(authority, actual.Authority);
        Assert.Equal(audience, actual.Audience);
    }

    [Fact]
    public void configures_Authority_and_Audience_to_null_if_not_configured()
    {
        Fixture fixture = new Fixture();

        var values = new Dictionary<string, string>();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        var sut = new ConfigureJwtBearerOptions(configuration);

        var actual = new JwtBearerOptions();
        sut.Configure(actual);

        Assert.Null(actual.Authority);
        Assert.Null(actual.Audience);
    }
}
