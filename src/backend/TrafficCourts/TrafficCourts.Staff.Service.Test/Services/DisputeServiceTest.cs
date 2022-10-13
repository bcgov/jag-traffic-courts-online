using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Services;

public class DisputeServiceTest
{

    [Theory]
    [InlineData("{\"ImageFilename\":\"2022-05-09/5d6811cbae4f4dd297151a27ab9f0707.xwd\",\"GlobalConfidence\":0.88}", "2022-05-09/5d6811cbae4f4dd297151a27ab9f0707.xwd")]
    [InlineData("{\"BadProperty\":\"2022-05-09/5d6811cbae4f4dd297151a27ab9f0707.xwd\",\"GlobalConfidence\":0.88}", null)]
    [InlineData("", null)]
    [InlineData(null, null)]
    public void TestGetViolationTicketImageFilename(string json, string? expectedFilename)
    {
        var mockOracleDataApi = new Mock<IOracleDataApiClient>();
        //Given
        DisputeService service = new(
            mockOracleDataApi.Object, 
            new Mock<IBus>().Object, new Mock<IFilePersistenceService>().Object, 
            new Mock<ILogger<DisputeService>>().Object,
            Mock.Of<ICancelledDisputeEmailTemplate>(),
            Mock.Of<IRejectedDisputeEmailTemplate>());
        Dispute dispute = new();
        dispute.DisputeId = 1;
        dispute.ViolationTicket = new();
        dispute.OcrViolationTicket = json;

        //When
        string? filename = service.GetViolationTicketImageFilename(dispute);

        //Then
        Assert.Equal(expectedFilename, filename);
    }
}
