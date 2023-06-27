using Serilog.Core;
using Serilog.Events;

namespace BCGov.VirusScan.Api.Logging;

public class EnvironmentVariableEnricher : ILogEventEnricher
{
    private string? _value;
    private string _propertyName;
    private LogEventProperty? _property;

    public EnvironmentVariableEnricher(string variable, string? propertyName = null)
    {
        ArgumentNullException.ThrowIfNull(variable);

        if (propertyName is null)
        {
            propertyName = variable;
        }

        _propertyName = propertyName;
        _value = Environment.GetEnvironmentVariable(variable);
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (_value is null)
        {
            return; // nothing to log
        }

        if (_property is null)
        {
            _property = propertyFactory.CreateProperty(_propertyName, _value);
        }

        logEvent.AddPropertyIfAbsent(_property);
    }
}
