namespace OpenTelemetry.Trace;

/// <summary>
/// Contains extension methods to <see cref="TracerProviderBuilder"/> for enabling MassTransit traces instrumentation.
/// </summary>
public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddMassTransitInstrumentation(this TracerProviderBuilder builder)
    {
        builder.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);
        return builder;
    }
}
