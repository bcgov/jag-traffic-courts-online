using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace TrafficCourts.Common.Configuration;

public static class ConfigurationManagerExtensions
{
    /// <summary>
    /// Adds the specified <see cref="ConfigurationProvider"/> as a configuration source.
    /// </summary>
    /// <typeparam name="TConfigurationProvider">Type of the configuration provider</typeparam>
    /// <param name="configurationManager"></param>
    public static void Add<TConfigurationProvider>(this ConfigurationManager configurationManager) 
        where TConfigurationProvider : TrafficCourtsConfigurationProvider, new()
    {
        ArgumentNullException.ThrowIfNull(configurationManager);
        Debug.Assert(configurationManager is IConfigurationBuilder);

        ((IConfigurationBuilder)configurationManager).Add(new TrafficCourtsConfigurationSource<TConfigurationProvider>());
    }
}
