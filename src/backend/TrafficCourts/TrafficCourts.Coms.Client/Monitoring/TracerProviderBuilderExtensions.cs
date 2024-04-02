namespace OpenTelemetry.Trace;

/// <summary>
/// Contains extension methods to <see cref="TracerProviderBuilder"/> for enabling Coms Client traces instrumentation.
/// </summary>
public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddComsClientInstrumentation(this TracerProviderBuilder builder)
    {
        builder.AddSource(TrafficCourts.Coms.Client.Monitoring.Diagnostics.Source.Name);
        return builder;
    }
}
