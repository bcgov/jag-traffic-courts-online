using Microsoft.Extensions.Configuration;

namespace TrafficCourts.Common.Configuration;

public static class ConfigurationManagerExtensions
{
    public static void Add<TConfigurationProvider>(this ConfigurationManager configurationManager) 
        where TConfigurationProvider : TrafficCourtsConfigurationProvider, new()
    {
        ((IConfigurationBuilder)configurationManager).Add(new TrafficCourtsConfigurationSource<TConfigurationProvider>());
    }
}
