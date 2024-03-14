using TrafficCourts.Common.Test.OpenAPIs;
using Xunit.Abstractions;

namespace TrafficCourts.OracleDataApi.Test;

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.Dispute"/>
/// to <see cref="TrafficCourts.Domain.Models.Dispute"/> and back again create the same object
/// </summary>
public class Dispute_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.Dispute, TrafficCourts.Domain.Models.Dispute> _test;
    public Dispute_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.DisputeCount"/>
/// to <see cref="TrafficCourts.Domain.Models.DisputeCount"/> and back again create the same object
/// </summary>
public class DisputeCount_MappingTest
{
    private readonly RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.DisputeCount, TrafficCourts.Domain.Models.DisputeCount> _test;
    public DisputeCount_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.DisputeListItem"/>
/// to <see cref="TrafficCourts.Domain.Models.DisputeListItem"/> and back again create the same object
/// </summary>
public class DisputeListItem_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.DisputeListItem, TrafficCourts.Domain.Models.DisputeListItem> _test;
    public DisputeListItem_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.DisputeResult"/>
/// to <see cref="TrafficCourts.Domain.Models.DisputeResult"/> and back again create the same object
/// </summary>
public class DisputeResult_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.DisputeResult, TrafficCourts.Domain.Models.DisputeResult> _test;
    public DisputeResult_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.DisputeUpdateRequest"/>
/// to <see cref="TrafficCourts.Domain.Models.DisputeUpdateRequest"/> and back again create the same object
/// </summary>
public class DisputeUpdateRequest_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.DisputeUpdateRequest, TrafficCourts.Domain.Models.DisputeUpdateRequest> _test;
    public DisputeUpdateRequest_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.EmailHistory"/>
/// to <see cref="TrafficCourts.Domain.Models.EmailHistory"/> and back again create the same object
/// </summary>
public class EmailHistory_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.EmailHistory, TrafficCourts.Domain.Models.EmailHistory> _test;
    public EmailHistory_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.FileHistory"/>
/// to <see cref="TrafficCourts.Domain.Models.FileHistory"/> and back again create the same object
/// </summary>
public class FileHistory_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.FileHistory, TrafficCourts.Domain.Models.FileHistory> _test;
    public FileHistory_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.JJDispute"/>
/// to <see cref="TrafficCourts.Domain.Models.JJDispute"/> and back again create the same object
/// </summary>
public class JJDispute_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.JJDispute, TrafficCourts.Domain.Models.JJDispute> _test;
    public JJDispute_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.JJDisputeCourtAppearanceRoP"/>
/// to <see cref="TrafficCourts.Domain.Models.JJDisputeCourtAppearanceRoP"/> and back again create the same object
/// </summary>
public class JJDisputeCourtAppearanceRoP_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.JJDisputeCourtAppearanceRoP, TrafficCourts.Domain.Models.JJDisputeCourtAppearanceRoP> _test;
    public JJDisputeCourtAppearanceRoP_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.JJDisputedCount"/>
/// to <see cref="TrafficCourts.Domain.Models.JJDisputedCount"/> and back again create the same object
/// </summary>
public class JJDisputedCount_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.JJDisputedCount, TrafficCourts.Domain.Models.JJDisputedCount> _test;
    public JJDisputedCount_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.JJDisputedCountRoP"/>
/// to <see cref="TrafficCourts.Domain.Models.JJDisputedCountRoP"/> and back again create the same object
/// </summary>
public class JJDisputedCountRoP_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.JJDisputedCountRoP, TrafficCourts.Domain.Models.JJDisputedCountRoP> _test;
    public JJDisputedCountRoP_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.JJDisputeRemark"/>
/// to <see cref="TrafficCourts.Domain.Models.JJDisputeRemark"/> and back again create the same object
/// </summary>
public class JJDisputeRemark_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.JJDisputeRemark, TrafficCourts.Domain.Models.JJDisputeRemark> _test;
    public JJDisputeRemark_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.TicketImageDataJustinDocument"/>
/// to <see cref="TrafficCourts.Domain.Models.TicketImageDataJustinDocument"/> and back again create the same object
/// </summary>
public class TicketImageDataJustinDocument_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.TicketImageDataJustinDocument, TrafficCourts.Domain.Models.TicketImageDataJustinDocument> _test;
    public TicketImageDataJustinDocument_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.ViolationTicket"/>
/// to <see cref="TrafficCourts.Domain.Models.ViolationTicket"/> and back again create the same object
/// </summary>
public class ViolationTicket_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.ViolationTicket, TrafficCourts.Domain.Models.ViolationTicket> _test;
    public ViolationTicket_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

/// <summary>
/// Tests to ensure mapping from <see cref="TrafficCourts.OracleDataApi.ViolationTicketCount"/>
/// to <see cref="TrafficCourts.Domain.Models.ViolationTicketCount"/> and back again create the same object
/// </summary>
public class ViolationTicketCount_MappingTest
{
    private RoundTripMappingTest<TrafficCourts.OracleDataApi.Client.V1.ViolationTicketCount, TrafficCourts.Domain.Models.ViolationTicketCount> _test;
    public ViolationTicketCount_MappingTest(ITestOutputHelper output)
    {
        _test = new(output);
    }

    [Fact]
    public void can_map_and_reverse_map()
    {
        _test.can_map_and_reverse_map();
    }
}

