using BCGov.VirusScan.Api.Logging;
using Serilog.Configuration;

namespace Serilog;

public static class EnvironmentVariableLoggerConfigurationExtensions
{
    public static LoggerConfiguration WithEnvironmentVariable(this LoggerEnrichmentConfiguration enrichmentConfiguration, string variable, string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(enrichmentConfiguration);
        ArgumentNullException.ThrowIfNull(variable);
        return enrichmentConfiguration.With(new EnvironmentVariableEnricher(variable, propertyName));
    }
}