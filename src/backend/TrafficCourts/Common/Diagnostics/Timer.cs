using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace TrafficCourts.Common.Diagnostics;

public sealed class Timer : Instrument<double>
{
    private readonly Histogram<double> _histogram;

    public Timer(Meter meter, string name, string unit, string description)
    : base(meter, name, unit, description)
    {
        _histogram = meter.CreateHistogram<double>(name, unit, description);
    }

    public ITimerOperation Start(TagList tagList)
    {
        return new TimerMark(this, tagList);
    }

    internal void Record(TimeSpan elapsed, TagList tagList)
    {
        _histogram.Record(elapsed.TotalMilliseconds, tagList);
    }

    public sealed class TimerMark : ITimerOperation
    {
        private readonly Timer _timer;
        private readonly ValueStopwatch _valueStopwatch;
        private readonly TagList _tagList;

        public TimerMark(Timer timer, TagList tags)
        {
            _timer = timer;
            _tagList = tags;
            _valueStopwatch = ValueStopwatch.StartNew();
        }

        public void Dispose()
        {
            var elapsed = _valueStopwatch.GetElapsedTime();
            _timer.Record(elapsed, _tagList);
        }

        public void Error(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            _tagList.Add("exception_type", exception.GetType().Name);
        }

        public TagList Tags
        {
            get
            {
                var tags = new TagList();
                foreach (var tag in _tagList)
                {
                    tags.Add(tag);
                }
                return tags;
            }
        }
    }
}
