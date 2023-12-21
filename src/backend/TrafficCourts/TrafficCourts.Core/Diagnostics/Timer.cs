using Microsoft.Extensions.Diagnostics.Latency;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace TrafficCourts.Diagnostics;

/// <summary>
/// Wraps an instance of a <see cref="Histogram<double>"/> into 
/// that simplifieds timing certain operations.
/// </summary>
public sealed class Timer : Instrument<double>
{
    private readonly Histogram<double> _histogram;

    public Timer(Meter meter, string name, string unit, string description)
    : base(meter, name, unit, description)
    {
        _histogram = meter.CreateHistogram<double>(name, unit, description);
    }

    public ITimerOperation Start(string key, string value)
    {
        return new TimerMark(this, new KeyValuePair<string, object?>(key, value));
    }

    public ITimerOperation Start(TagList tagList)
    {
        return new TimerMark(this, tagList);
    }

    public ITimerOperation Start()
    {
        return new TimerMark(this);
    }

    public ITimerOperation Start(KeyValuePair<string, object?> tag)
    {
        return new TimerMark(this, tag);
    }

    public ITimerOperation Start(KeyValuePair<string, object?> tag1, KeyValuePair<string, object?> tag2)
    {
        return new TimerMark(this, tag1, tag2);
    }

    public ITimerOperation Start(IEnumerable<KeyValuePair<string, object?>> tags)
    {
        return new TimerMark(this, tags);
    }

    /// <summary>
    /// Record the elapsed time of an operation.
    /// </summary>
    /// <param name="elapsed"></param>
    /// <param name="tagList"></param>
    internal void Record(TimeSpan elapsed, TagList tagList)
    {
        _histogram.Record(elapsed.TotalMilliseconds, tagList);
    }

    public sealed class TimerMark : ITimerOperation
    {
        /// <summary>
        /// TODO: determine how to make this testable without compromising performance
        /// </summary>
        private static readonly TimeProvider _timeProvider = TimeProvider.System;

        /// <summary>
        /// The timer a
        /// </summary>
        private readonly Timer _timer;
        /// <summary>
        /// The timestamp this operation started
        /// </summary>
        private readonly long _startTimestamp;
        /// <summary>
        /// Was this operation successful?
        /// </summary>
        private bool _success = true;
        /// <summary>
        /// The list of tags. A <see cref="TagList"/> should work, but Add was not working.
        /// </summary>
        private List<KeyValuePair<string, object?>> _tags;

        public TimerMark(Timer timer)
        {
            _timer = timer;
            _tags = new List<KeyValuePair<string, object?>>(8);
            _startTimestamp = _timeProvider.GetTimestamp();
        }

        public TimerMark(Timer timer, TagList tags)
        {
            _timer = timer;
            _tags = new List<KeyValuePair<string, object?>>(tags);
            _startTimestamp = _timeProvider.GetTimestamp();
        }

        public TimerMark(Timer timer, KeyValuePair<string, object?> tag)
        {
            _timer = timer;
            _tags = [tag];
            _startTimestamp = _timeProvider.GetTimestamp();
        }

        public TimerMark(Timer timer, KeyValuePair<string, object?> tag1, KeyValuePair<string, object?> tag2)
        {
            _timer = timer;
            _tags = [tag1, tag2];
            _startTimestamp = _timeProvider.GetTimestamp();
        }

        public TimerMark(Timer timer, IEnumerable<KeyValuePair<string, object?>> tags)
        {
            _timer = timer;
            _tags = new List<KeyValuePair<string, object?>>(tags);
            _startTimestamp = _timeProvider.GetTimestamp();
        }

        public void Dispose()
        {
            var elapsed = _timeProvider.GetElapsedTime(_startTimestamp);

            AddTag("success", _success);
            _timer.Record(elapsed, GetTagList());
        }

        public void Error(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            AddTag("exception_type", exception.GetType().Name);
            _success = false;
        }

        public void AddTag(string key, object? value)
        {
            ArgumentNullException.ThrowIfNull(key);
            _tags.Add(new KeyValuePair<string, object?>(key, value));
        }

        public TagList Tags => GetTagList();

        private TagList GetTagList()
        {
            TagList tagList = new(new ReadOnlySpan<KeyValuePair<string, object?>>([.. _tags]));
            return tagList;
        }
    }
}
