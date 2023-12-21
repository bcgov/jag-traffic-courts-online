using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;

namespace TrafficCourts.Diagnostics;

/// <summary>
/// Provides base class for creating timed operation metics.
/// </summary>
public abstract class OperationMetrics : IOperationMetrics
{
    private readonly Meter _meter;
    private readonly Timer _timer;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factory">The meter factory to create meter.</param>
    /// <param name="meterName">The name of the meter.</param>
    /// <param name="name">The name prefix of the timer and counter.</param>
    /// <param name="description">The short description of the timer and counter.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="factory"/>, <paramref name="meterName"/>, <paramref name="name"/> or <paramref name="description"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="meterName"/>, <paramref name="name"/> or <paramref name="description"/> is empty or whitespace.
    /// </exception>
    protected OperationMetrics(IMeterFactory factory, string meterName, string name, string description)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentException.ThrowIfNullOrWhiteSpace(meterName);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);

        _meter = factory.Create(new MeterOptions(meterName));
        _timer = new Timer(_meter, $"{name}.operation.duration", "ms", $"Elapsed time spent executing a {description} operation");
    }

    /// <summary>
    /// Begins a named operation
    /// </summary>
    /// <param name="operation">
    /// The name of the operation. If not supplied, the caller's method name will be used.
    /// If the name ends with &quot;Async&quot;, it will be trimmed off.
    /// </param>
    /// <returns>
    /// A disposable <see cref="ITimerOperation"/> that when disposed records the operation completion.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="operation"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="operation"/> is empty.</exception>
    public ITimerOperation BeginOperation([CallerMemberName] string operation = null!)
    {
        ArgumentException.ThrowIfNullOrEmpty(operation);

        // trim async if the operation name ends in that
        if (operation.EndsWith("Async", StringComparison.InvariantCulture))
        {
            operation = operation[..^"Async".Length];
        }

        return _timer.Start("operation", operation);
    }
}
