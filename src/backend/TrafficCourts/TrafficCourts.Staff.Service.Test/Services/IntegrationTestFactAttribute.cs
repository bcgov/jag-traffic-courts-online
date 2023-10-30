using Microsoft.Extensions.Configuration;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Services;

public sealed class IntegrationTestFactAttribute : FactAttribute
{
    private static readonly Mode _mode;

    static IntegrationTestFactAttribute()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<IntegrationTestFactAttribute>()
            .Build();

        _mode = configuration.GetSection("IntegrationTest:Mode").Get<Mode>();
    }
    public IntegrationTestFactAttribute()
    {
        if (_mode == Mode.Skip)
        {
            Skip = "Integration test not configured, set IntegrationTest:Mode=Container to user secret to enable";
        }
    }

    public enum Mode
    {
        Skip = 0,
        Container
    }
}
