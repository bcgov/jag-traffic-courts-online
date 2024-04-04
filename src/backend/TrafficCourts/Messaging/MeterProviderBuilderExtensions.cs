namespace OpenTelemetry.Metrics;

/// <summary>
/// Contains extension methods to <see cref="MeterProviderBuilder"/> for enabling MassTransit metrics instrumentation.
/// </summary>
public static class MeterProviderBuilderExtensions
{
    public static MeterProviderBuilder AddMassTransitInstrumentation(this MeterProviderBuilder builder)
    {
        builder.AddMeter("MassTransit");
        return builder;
    }
}
