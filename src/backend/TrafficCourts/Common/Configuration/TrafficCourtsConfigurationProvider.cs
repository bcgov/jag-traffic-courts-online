using Microsoft.Extensions.Configuration;

namespace TrafficCourts.Common.Configuration;

public abstract class TrafficCourtsConfigurationProvider : ConfigurationProvider
{
    /// <summary>
    /// Gets the environment variable and maps it to app key if it has a value.
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="appKey"></param>
    protected void Add(string variable, string appKey)
    {
        if (variable == null) throw new ArgumentNullException(nameof(variable));
        if (appKey == null) throw new ArgumentNullException(nameof(appKey));

        var value = Environment.GetEnvironmentVariable(variable);
        if (value != null)
        {
            Data.Add(appKey, value);
        }
    }
}
