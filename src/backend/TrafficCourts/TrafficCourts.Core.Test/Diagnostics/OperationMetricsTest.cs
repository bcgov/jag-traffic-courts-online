using AutoFixture;
using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using System.Diagnostics.Metrics;
using System.Xml.Linq;
using TrafficCourts.Diagnostics;

namespace TrafficCourts.Core.Test.Diagnostics;

public class OperationMetricsTest
{
    private readonly string _meterName;

    public OperationMetricsTest()
    {
        Fixture fixture = new Fixture();

        _meterName = fixture.Create<string>();
    }

    [Fact]
    public void should_create_meter()
    {
        var factory = new TestMeterFactory();

        new TestOperationMetrics(factory, _meterName, "x", "y");

        // assert meter is created
        var meter = Assert.Single(factory.Meters);
        Assert.Equal(_meterName, meter.Name);
    }

    [Fact]
    public void begin_should_create_timer_operation_with_callers_method_name()
    {
        var factory = new TestMeterFactory();

        var sut = new TestOperationMetrics(factory, _meterName, "x", "y");

        // act
        var actual = sut.BeginOperation();

        // assert
        var tag = Assert.Single(actual.Tags);
        Assert.Equal("operation", tag.Key);
        Assert.Equal(nameof(begin_should_create_timer_operation_with_callers_method_name), tag.Value);
    }

    [Fact]
    public void begin_should_create_timer_operation_with_callers_method_name_without_Async()
    {
        var factory = new TestMeterFactory();

        var sut = new TestOperationMetrics(factory, _meterName, "x", "y");

        // act
        var actual = sut.BeginOperation();

        // assert
        var tag = Assert.Single(actual.Tags);
        Assert.Equal("operation", tag.Key);
        Assert.Equal(nameof(begin_should_create_timer_operation_with_callers_method_name_without_Async)[..^5], tag.Value);
    }

    [Fact]
    public void operation_should_record_operation_duration()
    {
        var meterFactory = new TestMeterFactory();
        var sut = new TestOperationMetrics(meterFactory, _meterName, "x", "y");

        // act
        using (var operation = sut.BeginOperation())
        {
        }

        // assert
        var measurement = Assert.Single(sut.Measurements);

        // operation tag should be correct
        var tag = Assert.Single(measurement.Tags, tag => tag.Key == "operation");
        Assert.Equal(nameof(operation_should_record_operation_duration), tag.Value);
        Assert.True(0d <= measurement.Value);

        // success tag should be correct
        tag = Assert.Single(measurement.Tags, tag => tag.Key == "success");
        Assert.Equal(true, tag.Value);
    }

    [Fact]
    public void operation_with_error_should_record_operation_error()
    {
        var meterFactory = new TestMeterFactory();
        var sut = new TestOperationMetrics(meterFactory, _meterName, "x", "y");

        // act
        using (var operation = sut.BeginOperation())
        {
            operation.Error(new ArgumentNullException(nameof(meterFactory)));
        }

        // assert
        var measurement = Assert.Single(sut.Measurements);

        // operation tag should be correct
        var tag = Assert.Single(measurement.Tags, tag => tag.Key == "operation");
        Assert.Equal(nameof(operation_with_error_should_record_operation_error), tag.Value);

        // success tag should be correct
        tag = Assert.Single(measurement.Tags, tag => tag.Key == "success");
        Assert.Equal(false, tag.Value);

        // exception_type tag should be correct
        tag = Assert.Single(measurement.Tags, tag => tag.Key == "exception_type");
        Assert.Equal(typeof(ArgumentNullException).Name, tag.Value);
    }

    class TestOperationMetrics : OperationMetrics
    {
        private readonly MetricCollector<double> _measurements;

        public TestOperationMetrics(IMeterFactory meterFactory, string meterName, string name, string description) :
            base(meterFactory, meterName, name, description)
        {
            var factory = (TestMeterFactory)meterFactory;
            _measurements = new MetricCollector<double>(factory.Meters[0], "x.operation.duration");
        }

        public IReadOnlyList<CollectedMeasurement<double>> Measurements => _measurements.GetMeasurementSnapshot();
    }

    class TestMeterFactory : IMeterFactory
    {
        /// <summary>
        /// Stores the created meters.
        /// </summary>
        public readonly List<Meter> Meters = new();

        public Meter Create(MeterOptions options)
        {
            var meter = new Meter(options);
            Meters.Add(meter);
            return meter;
        }
       
        public void Dispose()
        {
            foreach (var meter in Meters) 
            {
                meter.Dispose();
            }
        }
    }
}

