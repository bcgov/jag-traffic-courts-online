using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Common.Features.FilePersistence;
using TrafficCourts.Staff.Service.Configuration;
using TrafficCourts.Staff.Service.OpenAPIs.OracleDataApi.v1_0;
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
        //Given
        DisputeService service = new(new OracleDataApiConfiguration(), new Mock<IBus>().Object, new Mock<IFilePersistenceService>().Object, new Mock<ILogger<DisputeService>>().Object);
        Dispute dispute = new();
        dispute.Id = Guid.NewGuid();
        dispute.ViolationTicket = new();
        dispute.ViolationTicket.OcrViolationTicket = json;

        //When
        string? filename = service.GetViolationTicketImageFilename(dispute);

        //Then
        Assert.Equal(expectedFilename, filename);
    }
}
