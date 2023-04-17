using TrafficCourts.Messaging.MessageContracts;
using Xunit;

namespace TrafficCourts.Test.Messaging;

public class SearchDisputeResponseTest
{
    [Fact]
    public void default_constructor_should_not_set_is_error_or_is_not_found()
    {
        var sut = new SearchDisputeResponse();
        Assert.False(sut.IsError);
        Assert.False(sut.IsNotFound);
    }

    [Fact]
    public void error_instance_should_be_flagged_as_error_and_not_NotFound_and_all_data_properties_null()
    {
        var sut = SearchDisputeResponse.Error;
        Assert.NotNull(sut);

        Assert.True(sut.IsError);
        Assert.False(sut.IsNotFound);

        Assert.Null(sut.NoticeOfDisputeGuid);
        Assert.Null(sut.DisputeStatus);
        Assert.Null(sut.NoticeOfDisputeGuid);
        Assert.Null(sut.JJDisputeStatus);
    }

    [Fact]
    public void not_found_instance_should_be_flagged_as_error_and_not_IsError_and_all_data_properties_null()
    {
        var sut = SearchDisputeResponse.NotFound;
        Assert.NotNull(sut);

        Assert.True(sut.IsNotFound);
        Assert.False(sut.IsError);

        Assert.Null(sut.NoticeOfDisputeGuid);
        Assert.Null(sut.DisputeStatus);
        Assert.Null(sut.NoticeOfDisputeGuid);
        Assert.Null(sut.JJDisputeStatus);
    }
}
