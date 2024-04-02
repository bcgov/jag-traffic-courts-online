namespace OpenTelemetry.Metrics;

/// <summary>
/// Contains extension methods to <see cref="MeterProviderBuilder"/> for enabling Citizen Service metrics instrumentation.
/// </summary>
public static class MeterProviderBuilderExtensions
{
    public static MeterProviderBuilder AddCitizenServiceInstrumentation(this MeterProviderBuilder builder)
    {
        builder.AddMeter("CitizenService");
        return builder;
    }
}
