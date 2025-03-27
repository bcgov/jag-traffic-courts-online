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

        // Reverse map
        var actual = _sut.Map<TSourceModel>(mapped);

        // Exclude IssuedTs from comparison
        RemoveIssuedTs(expected!);
        RemoveIssuedTs(actual!);

        var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented);
        var actualJson = JsonConvert.SerializeObject(actual, Formatting.Indented);

        if (_debug)
        {
            WriteAllTextToTempPath($"{typeof(TSourceModel).Name}-expected.json", expectedJson);
            string tempPath = WriteAllTextToTempPath($"{typeof(TSourceModel).Name}-actual.json", actualJson);
            _output.WriteLine($"Wrote expected and actual json files to {tempPath}");
        }

        Assert.Equivalent(expected, actual);
        Assert.Equal(expectedJson, actualJson);
    }

    private static void RemoveIssuedTs(object obj)
    {
        if (obj == null) return;

        // Check if the object has an IssuedTs property
        var issuedTsProperty = obj.GetType().GetProperty("IssuedTs");
        if (issuedTsProperty != null && issuedTsProperty.PropertyType == typeof(DateTimeOffset?))
        {
            // Set IssuedTs to null
            issuedTsProperty.SetValue(obj, null);
        }

        // Recursively handle nested objects
        foreach (var property in obj.GetType().GetProperties())
        {
            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
            {
                var nestedObject = property.GetValue(obj);
                if (nestedObject != null)
                {
                    if (nestedObject is IEnumerable<object> enumerable)
                    {
                        foreach (var item in enumerable)
                        {
                            RemoveIssuedTs(item);
                        }
                    }
                    else
                    {
                        RemoveIssuedTs(nestedObject);
                    }
                }
            }
        }
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

