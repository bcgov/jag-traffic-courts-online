namespace System.Diagnostics.Metrics;

/// <summary>
/// Provides extension method for creating a timer based on a meter.
/// </summary>
/// <remarks>
/// Taken from https://github.com/open-telemetry/opentelemetry-specification/issues/464
/// </remarks>
public static class MeterExtension
{
    /// <summary>
    /// Create a timer to record  the number of milliseconds an operation takes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="meter"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    /// <remarks>
    /// Timer supports only the following generic parameter types: <see cref="int" /> and <see cref="double" />
    /// </remarks>
    static public Timer<T> CreateTimer<T>(this Meter meter, string name, string? description = null) where T : struct
    {
        return new Timer<T>(meter, name, "ms", description);
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
/// <remarks>
/// This class supports only the following generic parameter types: <see cref="int" /> and <see cref="double" />
/// </remarks>
public sealed class Timer<T> : Instrument<T> where T : struct
{
    private static KeyValuePair<string, object?>[] EmptyTags => Array.Empty<KeyValuePair<string, object?>>();
    private readonly Histogram<T> _histogram;

    public Timer(Meter meter, string name, string? unit, string? description)
        : base(meter, name, unit, description)
    {
        _histogram = meter.CreateHistogram<T>(name, unit, description);
    }

    public TimerMark<T> Start()
    {
        return Start(EmptyTags);
    }

    public TimerMark<T> Start(params KeyValuePair<string, object?>[] tags)
    {
        return new TimerMark<T>(this, tags);
    }

    public void Stop(TimerMark<T> mark)
    {
        Stop(mark, EmptyTags);
    }

    public void Stop(TimerMark<T> mark, params KeyValuePair<string, object?>[] tags)
    {
        mark._tags = tags;
        mark.Dispose();
    }

    internal void Record(TimeSpan elapsed, KeyValuePair<string, object?>[] tags)
    {
        Type type = typeof(T);
        T value;

        if (type == typeof(int))
        {
            int totalMilliseconds = (int)elapsed.TotalMilliseconds;
            value = (T)(object)totalMilliseconds;
        }
        else if (type == typeof(double))
        {
            value = (T)(object)elapsed.TotalMilliseconds;
        }
        else
        {
            throw new InvalidOperationException($"{type} is unsupported type for this operation. The only supported types are int and double.");
        }

        _histogram.Record(value, tags);
    }

    public class TimerMark<T1> : IDisposable where T1 : struct
    {
        private readonly Timer<T1> _timer;
        private readonly Stopwatch _sw;
        internal KeyValuePair<string, object?>[] _tags;

        public TimerMark(Timer<T1> timer, params KeyValuePair<string, object?>[] tags)
        {
            _timer = timer;
            _tags = tags;
            _sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _sw.Stop();
            _timer.Record(_sw.Elapsed, _tags);
        }
    }
}
