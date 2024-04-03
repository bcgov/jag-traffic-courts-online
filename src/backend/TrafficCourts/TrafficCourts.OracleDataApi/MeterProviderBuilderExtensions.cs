namespace OpenTelemetry.Metrics;

/// <summary>
/// Contains extension methods to <see cref="MeterProviderBuilder"/> for enabling Oracle Data API metrics instrumentation.
/// </summary>
public static class MeterProviderBuilderExtensions
{
    public static MeterProviderBuilder AddOracleDataApiInstrumentation(this MeterProviderBuilder builder)
    {
        builder.AddMeter(TrafficCourts.OracleDataApi.OracleDataApiOperationMetrics.MeterName);
        return builder;
    }
}
