namespace TrafficCourts.Citizen.Service.Configuration;

public class EnvironmentVariablesConfigurationProvider : ConfigurationProvider
{
    public override void Load()
    {
        //Add("SPLUNK_URL", $"{nameof(CsrsConfiguration.Splunk)}:{nameof(SplunkConfiguration.Url)}");
        //Add("SPLUNK_TOKEN", $"{nameof(CsrsConfiguration.Splunk)}:{nameof(SplunkConfiguration.Token)}");
        //Add("SPLUNK_VALIDATE_SERVER_CERTIFICATE", $"{nameof(CsrsConfiguration.Splunk)}:{nameof(SplunkConfiguration.ValidatServerCertificate)}");
        //Add("SPLUNK_MINIMUMLEVEL", $"{nameof(CsrsConfiguration.Splunk)}:{nameof(SplunkConfiguration.MinimumLevel)}");
        //Add("ZIPKIN_URL", $"{nameof(CsrsConfiguration.Tracing)}:{nameof(TracingConfiguration.Zipkin)}:{nameof(ZipkinConfiguration.Url)}");
        //Add("JAEGER_HOST", $"{nameof(CsrsConfiguration.Tracing)}:{nameof(TracingConfiguration.Jaeger)}:{nameof(JaegerConfiguration.Host)}");
        //Add("JAEGER_PORT", $"{nameof(CsrsConfiguration.Tracing)}:{nameof(TracingConfiguration.Jaeger)}:{nameof(JaegerConfiguration.Port)}");
    }

    /// <summary>
    /// Gets the environment variable and maps it to app key if it has a value.
    /// </summary>
    /// <param name="variable"></param>
    /// <param name="appKey"></param>
    private void Add(string variable, string appKey)
    {
        ArgumentNullException.ThrowIfNull(variable);
        ArgumentNullException.ThrowIfNull(appKey);

        // note the Load method may be called multiple times if before the
        // application builder Build method is called, the configuration is requested
        // so check if the key already exists before adding it
        if (!Data.ContainsKey(appKey))
        {
            var value = Environment.GetEnvironmentVariable(variable);
            if (value is not null)
            {
                Data.Add(appKey, value);
            }
        }
    }
}
