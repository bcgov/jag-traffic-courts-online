using AutoFixture;
using Newtonsoft.Json;
using TrafficCourts.OracleDataApi.Test;
using Xunit.Abstractions;

namespace TrafficCourts.Common.Test.OpenAPIs;

/// <summary>
/// Generic base class for testing if mapping to the destination model and back creates
/// the same values.
/// </summary>
/// <typeparam name="TSourceModel"></typeparam>
/// <typeparam name="TDestinationModel"></typeparam>
public class RoundTripMappingTest<TSourceModel, TDestinationModel> : DomainModelMappingTest
{
    private readonly bool _debug;

    /// <summary>
    /// </summary>
    /// <param name="output"></param>
    /// <param name="debug">Set to true if you want the expected and actual values written to json files for troubleshooting.</param>
    public RoundTripMappingTest(ITestOutputHelper output, bool debug = false) : base(output)
    {
        _debug = debug;
    }

    public void can_map_and_reverse_map()
    {
        var expected = _fixture.Create<TSourceModel>();

        var mapped = _sut.Map<TDestinationModel>(expected);

        var actual = _sut.Map<TSourceModel>(mapped);

        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        var actualJson = JsonConvert.SerializeObject(expected, Formatting.Indented);

        // uncomment and change path to help diagnose differences
        if (_debug)
        {
            WriteAllTextToTempPath($"{typeof(TSourceModel).Name}-expected.json", expectedJson);
            string tempPath = WriteAllTextToTempPath($"{typeof(TSourceModel).Name}-actual.json", actualJson);
            _output.WriteLine($"Wrote expected and actual json files to {tempPath}");
        }

        Assert.Equivalent(expected, actual);
        Assert.Equal(expectedJson, actualJson);
    }

    private string WriteAllTextToTempPath(string filename, string content)
    {
        string tempPath = System.IO.Path.GetTempPath();

        System.IO.File.WriteAllText(System.IO.Path.Combine(tempPath, filename), content);
        return tempPath;
    }
}

// -------------------------------------------------------------------------------------------------
// Generated code from DomainModelMappingTestGenerator test class generate_round_trip_unit_tests()

