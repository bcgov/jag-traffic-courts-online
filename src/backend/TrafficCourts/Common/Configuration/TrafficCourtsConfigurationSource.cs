using Microsoft.Extensions.Configuration;

namespace TrafficCourts.Common.Configuration;

/// <summary>
/// Provides ability to map custom environment variables to application keys
/// </summary>
/// <typeparam name="TConfigurationProvider"></typeparam>
public class TrafficCourtsConfigurationSource<TConfigurationProvider> : IConfigurationSource
    where TConfigurationProvider : TrafficCourtsConfigurationProvider, new()
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new TConfigurationProvider();
    }
}
