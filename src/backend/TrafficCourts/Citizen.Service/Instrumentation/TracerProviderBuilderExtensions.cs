namespace OpenTelemetry.Trace;

/// <summary>
/// Contains extension methods to <see cref="TracerProviderBuilder"/> for enabling Citizen Service traces instrumentation.
/// </summary>
public static class TracerProviderBuilderExtensions
{
    public static TracerProviderBuilder AddCitizenServiceInstrumentation(this TracerProviderBuilder builder)
    {
        return builder;
    }
}
