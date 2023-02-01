using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Consumers;
using TrafficCourts.Workflow.Service.Services;
using Xunit;
using DisputantUpdateRequest = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputantUpdateRequest;

namespace TrafficCourts.Test.Workflow.Service.Consumers;

public class DisputantUpdateRequestAcceptedConsumerTest
{
    private readonly DisputantUpdateRequestAccepted _message;
    private readonly Dispute _dispute;
    private readonly DisputantUpdateRequest _updateRequest;
    private readonly Mock<ILogger<DisputantUpdateRequestAcceptedConsumer>> _mockLogger;
    private readonly Mock<IOracleDataApiService> _oracleDataApiService;
    private readonly Mock<ConsumeContext<DisputantUpdateRequestAccepted>> _context;
    private readonly DisputantUpdateRequestAcceptedConsumer _consumer;

    public DisputantUpdateRequestAcceptedConsumerTest()
    {
        _message = new(1);
        _dispute = new()
        {
            DisputeId = 1,
        };
        _updateRequest = new()
        {
            DisputantUpdateRequestId = 1,
            DisputeId = 1
        };

        _mockLogger = new();
        _oracleDataApiService = new();
        _oracleDataApiService.Setup(_ => _.UpdateDisputantUpdateRequestStatusAsync(1, DisputantUpdateRequestStatus.ACCEPTED, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_updateRequest));
        _oracleDataApiService.Setup(_ => _.GetDisputeByIdAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_dispute));
        _oracleDataApiService.Setup(_ => _.UpdateDisputeAsync(1, _dispute, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_dispute));
        _context = new();
        _context.Setup(_ => _.Message).Returns(_message);
        _context.Setup(_ => _.CancellationToken).Returns(CancellationToken.None);

        _consumer = new(_mockLogger.Object, _oracleDataApiService.Object, new DisputantUpdateRequestAcceptedTemplate());
    }

    [Fact]
    public async Task TestDisputantUpdateRequestAcceptedConsumer_AddressUpdates()
    {
        // Arrange
        _updateRequest.Status = DisputantUpdateRequestStatus2.ACCEPTED;
        _updateRequest.UpdateType = DisputantUpdateRequestUpdateType.DISPUTANT_ADDRESS;
        _updateRequest.UpdateJson = "{ \"addressLine1\": \"addr1\", \"addressLine2\": \"addr2\", \"addressLine3\": \"addr3\", \"addressCity\": \"city\", \"addressProvince\": \"BC\", \"postalCode\": \"A1B2C3\"}";

        // Act
        await _consumer.Consume(_context.Object);

        // Assert
        Assert.Equal("addr1", _dispute.AddressLine1);
        Assert.Equal("addr2", _dispute.AddressLine2);
        Assert.Equal("addr3", _dispute.AddressLine3);
        Assert.Equal("city", _dispute.AddressCity);
        Assert.Equal("BC", _dispute.AddressProvince);
        Assert.Equal("A1B2C3", _dispute.PostalCode);
    }

    [Fact]
    public async Task TestDisputantUpdateRequestAcceptedConsumer_NameUpdates()
    {
        // Arrange
        _updateRequest.Status = DisputantUpdateRequestStatus2.ACCEPTED;
        _updateRequest.UpdateType = DisputantUpdateRequestUpdateType.DISPUTANT_NAME;
        _updateRequest.UpdateJson = "{ \"contactGivenName1\": \"fname1\", \"contactGivenName2\": \"fname2\", \"contactGivenName3\": \"fname3\", \"contactSurname\": \"lname\", \"contactLawFirmName\":\"lawFirmname\", \"contactType\":\"I\" }";

        // Act
        await _consumer.Consume(_context.Object);

        // Assert
        Assert.Equal("fname1", _dispute.ContactGiven1Nm);
        Assert.Equal("fname2", _dispute.ContactGiven2Nm);
        Assert.Equal("fname3", _dispute.ContactGiven3Nm);
        Assert.Equal("lname", _dispute.ContactSurnameNm);
        Assert.Equal("contactLawFirmName", _dispute.ContactLawFirmNm);
        Assert.Equal("contactType", _dispute.ContactTypeCd.ToString());
    }

    [Fact]
    public async Task TestDisputantUpdateRequestAcceptedConsumer_PhoneUpdates()
    {
        // Arrange
        _updateRequest.Status = DisputantUpdateRequestStatus2.ACCEPTED;
        _updateRequest.UpdateType = DisputantUpdateRequestUpdateType.DISPUTANT_PHONE;
        _updateRequest.UpdateJson = "{ \"homePhoneNumber\": \"2505556666\" }";

        // Act
        await _consumer.Consume(_context.Object);

        // Assert
        Assert.Equal("2505556666", _dispute.HomePhoneNumber);
    }
}
