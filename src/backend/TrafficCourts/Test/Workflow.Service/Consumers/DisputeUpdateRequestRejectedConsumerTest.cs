using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Consumers;
using TrafficCourts.Workflow.Service.Services;
using Xunit;
using DisputeUpdateRequest = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest;

namespace TrafficCourts.Test.Workflow.Service.Consumers;

public class DisputeUpdateRequestRejectedConsumerTest
{
    private readonly DisputeUpdateRequestRejected _message;
    private readonly Dispute _dispute;
    private readonly DisputeUpdateRequest _updateRequest;
    private readonly Mock<ILogger<DisputeUpdateRequestRejectedConsumer>> _mockLogger;
    private readonly Mock<IOracleDataApiService> _oracleDataApiService;
    private readonly Mock<ConsumeContext<DisputeUpdateRequestRejected>> _context;
    private readonly DisputeUpdateRequestRejectedConsumer _consumer;

    public DisputeUpdateRequestRejectedConsumerTest()
    {
        _message = new(1, "user");
        _dispute = new()
        {
            DisputeId = 1,
        };
        _updateRequest = new()
        {
            DisputeUpdateRequestId = 1,
            DisputeId = 1,
            Status = DisputeUpdateRequestStatus2.REJECTED
        };

        _mockLogger = new();
        _oracleDataApiService = new();
        _oracleDataApiService.Setup(_ => _.UpdateDisputeUpdateRequestStatusAsync(1, DisputeUpdateRequestStatus.REJECTED, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_updateRequest));
        _oracleDataApiService.Setup(_ => _.GetDisputeByIdAsync(1, false, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_dispute));
        _context = new();
        _context.Setup(_ => _.Message).Returns(_message);
        _context.Setup(_ => _.CancellationToken).Returns(CancellationToken.None);

        _consumer = new(_mockLogger.Object, _oracleDataApiService.Object, new DisputeUpdateRequestRejectedTemplate());
    }

    [Fact]
    public async Task TestDisputeUpdateRequestRejectedConsumer_AddressUpdates()
    {
        // Arrange
        _updateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS;
        _updateRequest.UpdateJson = "{ \"addressLine1\": \"addr1\", \"addressLine2\": \"addr2\", \"addressLine3\": \"addr3\", \"addressCity\": \"city\", \"addressProvince\": \"BC\", \"postalCode\": \"A1B2C3\"}";

        // Act
        await _consumer.Consume(_context.Object);

        // Assert
        Assert.Equal(DisputeUpdateRequestStatus2.REJECTED, _updateRequest.Status);
    }

    [Fact]
    public async Task TestDisputeUpdateRequestRejectedConsumer_NameUpdates()
    {
        // Arrange
        _updateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_NAME;
        _updateRequest.UpdateJson = "{ \"contactGiven1Nm\": \"fname1\", \"contactGiven2Nm\": \"fname2\", \"contactGiven3Nm\": \"fname3\", \"contactSurnameNm\": \"lname\", \"disputantGivenName1\": \"fname1\", \"disputantGivenName2\": \"fname2\", \"disputantGivenName3\": \"fname3\", \"disputantSurname\": \"lname\"\"contactType\":\"I\", \"contactLawFirmName\":\"contactLawFirmName\" }";

        // Act
        await _consumer.Consume(_context.Object);

        // Assert
        Assert.Equal(DisputeUpdateRequestStatus2.REJECTED, _updateRequest.Status);
    }

    [Fact]
    public async Task TestDisputeUpdateRequestRejectedConsumer_PhoneUpdates()
    {
        // Arrange
        _updateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_PHONE;
        _updateRequest.UpdateJson = "{ \"homePhoneNumber\": \"2505556666\" }";

        // Act
        await _consumer.Consume(_context.Object);

        // Assert
        Assert.Equal(DisputeUpdateRequestStatus2.REJECTED, _updateRequest.Status);
    }
}
