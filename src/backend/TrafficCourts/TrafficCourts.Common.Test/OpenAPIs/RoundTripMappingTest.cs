using AutoFixture;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace TrafficCourts.Common.Test.OpenAPIs;

/// <summary>
/// Generic base class for testing if mapping to the destination model and back creates
/// the same values.
/// </summary>
/// <typeparam name="TSourceModel"></typeparam>
/// <typeparam name="TDestinationModel"></typeparam>
public abstract class RoundTripMappingTest<TSourceModel, TDestinationModel> : DomainModelMappingTest
{
    private readonly bool _debug;

    /// <summary>
    /// </summary>
    /// <param name="output"></param>
    /// <param name="debug">Set to true if you want the expected and actual values written to json files for troubleshooting.</param>
    protected RoundTripMappingTest(ITestOutputHelper output, bool debug = false) : base(output)
    {
        _debug = debug;
    }

    [Fact]
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

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.BoundingBox"/>
/// to <see cref="TrafficCourts.Domain.Models.BoundingBox"/> and back again create the same object
/// </summary>
public class BoundingBox_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.BoundingBox, TrafficCourts.Domain.Models.BoundingBox>
{
    public BoundingBox_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute"/>
/// to <see cref="TrafficCourts.Domain.Models.Dispute"/> and back again create the same object
/// </summary>
public class Dispute_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Dispute, TrafficCourts.Domain.Models.Dispute>
{
    public Dispute_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount"/>
/// to <see cref="TrafficCourts.Domain.Models.DisputeCount"/> and back again create the same object
/// </summary>
public class DisputeCount_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeCount, TrafficCourts.Domain.Models.DisputeCount>
{
    public DisputeCount_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeListItem"/>
/// to <see cref="TrafficCourts.Domain.Models.DisputeListItem"/> and back again create the same object
/// </summary>
public class DisputeListItem_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeListItem, TrafficCourts.Domain.Models.DisputeListItem>
{
    public DisputeListItem_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeResult"/>
/// to <see cref="TrafficCourts.Domain.Models.DisputeResult"/> and back again create the same object
/// </summary>
public class DisputeResult_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeResult, TrafficCourts.Domain.Models.DisputeResult>
{
    public DisputeResult_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest"/>
/// to <see cref="TrafficCourts.Domain.Models.DisputeUpdateRequest"/> and back again create the same object
/// </summary>
public class DisputeUpdateRequest_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest, TrafficCourts.Domain.Models.DisputeUpdateRequest>
{
    public DisputeUpdateRequest_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.EmailHistory"/>
/// to <see cref="TrafficCourts.Domain.Models.EmailHistory"/> and back again create the same object
/// </summary>
public class EmailHistory_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.EmailHistory, TrafficCourts.Domain.Models.EmailHistory>
{
    public EmailHistory_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Field"/>
/// to <see cref="TrafficCourts.Domain.Models.Field"/> and back again create the same object
/// </summary>
public class Field_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Field, TrafficCourts.Domain.Models.Field>
{
    public Field_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.FileHistory"/>
/// to <see cref="TrafficCourts.Domain.Models.FileHistory"/> and back again create the same object
/// </summary>
public class FileHistory_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.FileHistory, TrafficCourts.Domain.Models.FileHistory>
{
    public FileHistory_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.FileMetadata"/>
/// to <see cref="TrafficCourts.Domain.Models.FileMetadata"/> and back again create the same object
/// </summary>
public class FileMetadata_MappingTest : RoundTripMappingTest<TrafficCourts.Common.Models.FileMetadata, TrafficCourts.Domain.Models.FileMetadata>
{
    public FileMetadata_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

///// <summary>
///// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.FileResponse"/>
///// to <see cref="TrafficCourts.Domain.Models.FileResponse"/> and back again create the same object
///// </summary>
//public class FileResponse_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.FileResponse, TrafficCourts.Domain.Models.FileResponse>
//{
//    public FileResponse_MappingTest(ITestOutputHelper output) : base(output)
//    {
//    }
//}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute"/>
/// to <see cref="TrafficCourts.Domain.Models.JJDispute"/> and back again create the same object
/// </summary>
public class JJDispute_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDispute, TrafficCourts.Domain.Models.JJDispute>
{
    public JJDispute_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDisputeCourtAppearanceRoP"/>
/// to <see cref="TrafficCourts.Domain.Models.JJDisputeCourtAppearanceRoP"/> and back again create the same object
/// </summary>
public class JJDisputeCourtAppearanceRoP_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDisputeCourtAppearanceRoP, TrafficCourts.Domain.Models.JJDisputeCourtAppearanceRoP>
{
    public JJDisputeCourtAppearanceRoP_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDisputedCount"/>
/// to <see cref="TrafficCourts.Domain.Models.JJDisputedCount"/> and back again create the same object
/// </summary>
public class JJDisputedCount_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDisputedCount, TrafficCourts.Domain.Models.JJDisputedCount>
{
    public JJDisputedCount_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDisputedCountRoP"/>
/// to <see cref="TrafficCourts.Domain.Models.JJDisputedCountRoP"/> and back again create the same object
/// </summary>
public class JJDisputedCountRoP_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDisputedCountRoP, TrafficCourts.Domain.Models.JJDisputedCountRoP>
{
    public JJDisputedCountRoP_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDisputeRemark"/>
/// to <see cref="TrafficCourts.Domain.Models.JJDisputeRemark"/> and back again create the same object
/// </summary>
public class JJDisputeRemark_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.JJDisputeRemark, TrafficCourts.Domain.Models.JJDisputeRemark>
{
    public JJDisputeRemark_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.OcrViolationTicket"/>
/// to <see cref="TrafficCourts.Domain.Models.OcrViolationTicket"/> and back again create the same object
/// </summary>
public class OcrViolationTicket_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.OcrViolationTicket, TrafficCourts.Domain.Models.OcrViolationTicket>
{
    public OcrViolationTicket_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Point"/>
/// to <see cref="TrafficCourts.Domain.Models.Point"/> and back again create the same object
/// </summary>
public class Point_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.Point, TrafficCourts.Domain.Models.Point>
{
    public Point_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.TicketImageDataJustinDocument"/>
/// to <see cref="TrafficCourts.Domain.Models.TicketImageDataJustinDocument"/> and back again create the same object
/// </summary>
public class TicketImageDataJustinDocument_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.TicketImageDataJustinDocument, TrafficCourts.Domain.Models.TicketImageDataJustinDocument>
{
    public TicketImageDataJustinDocument_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicket"/>
/// to <see cref="TrafficCourts.Domain.Models.ViolationTicket"/> and back again create the same object
/// </summary>
public class ViolationTicket_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicket, TrafficCourts.Domain.Models.ViolationTicket>
{
    public ViolationTicket_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicketCount"/>
/// to <see cref="TrafficCourts.Domain.Models.ViolationTicketCount"/> and back again create the same object
/// </summary>
public class ViolationTicketCount_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicketCount, TrafficCourts.Domain.Models.ViolationTicketCount>
{
    public ViolationTicketCount_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicketImage"/>
/// to <see cref="TrafficCourts.Domain.Models.ViolationTicketImage"/> and back again create the same object
/// </summary>
public class ViolationTicketImage_MappingTest : RoundTripMappingTest<TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.ViolationTicketImage, TrafficCourts.Domain.Models.ViolationTicketImage>
{
    public ViolationTicketImage_MappingTest(ITestOutputHelper output) : base(output)
    {
    }
}

