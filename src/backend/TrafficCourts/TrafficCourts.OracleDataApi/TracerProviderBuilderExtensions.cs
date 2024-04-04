namespace OpenTelemetry.Trace;

/// <summary>
/// Contains extension methods to <see cref="TracerProviderBuilder"/> for enabling Oracle Data API traces instrumentation.
/// </summary>
public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddOracleDataApiInstrumentation(this TracerProviderBuilder builder)
    {
        //builder.AddSource("...");
        return builder;
    }
}
