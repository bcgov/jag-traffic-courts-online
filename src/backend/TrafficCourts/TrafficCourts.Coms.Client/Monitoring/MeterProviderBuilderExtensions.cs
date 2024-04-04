using TrafficCourts.Coms.Client.Monitoring;

namespace OpenTelemetry.Metrics;

/// <summary>
/// Contains extension methods to <see cref="MeterProviderBuilder"/> for enabling Coms Client metrics instrumentation.
/// </summary>
public static class MeterProviderBuilderExtensions
{
    public static MeterProviderBuilder AddComsClientInstrumentation(this MeterProviderBuilder builder)
    {
        builder.AddMeter(Instrumentation.MeterName);
        return builder;
    }
}
