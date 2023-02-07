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
using DisputeUpdateRequest = TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest;

namespace TrafficCourts.Test.Workflow.Service.Consumers;

public class DisputeUpdateRequestAcceptedConsumerTest
{
    private readonly DisputeUpdateRequestAccepted _message;
    private readonly Dispute _dispute;
    private readonly DisputeUpdateRequest _updateRequest;
    private readonly Mock<ILogger<DisputeUpdateRequestAcceptedConsumer>> _mockLogger;
    private readonly Mock<IOracleDataApiService> _oracleDataApiService;
    private readonly Mock<ConsumeContext<DisputeUpdateRequestAccepted>> _context;
    private readonly DisputeUpdateRequestAcceptedConsumer _consumer;

    public DisputeUpdateRequestAcceptedConsumerTest()
    {
        _message = new(1);
        _dispute = new()
        {
            DisputeId = 1,
        };
        _updateRequest = new()
        {
            DisputeUpdateRequestId = 1,
            DisputeId = 1
        };

        _mockLogger = new();
        _oracleDataApiService = new();
        _oracleDataApiService.Setup(_ => _.UpdateDisputeUpdateRequestStatusAsync(1, DisputeUpdateRequestStatus.ACCEPTED, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_updateRequest));
        _oracleDataApiService.Setup(_ => _.GetDisputeByIdAsync(1, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_dispute));
        _oracleDataApiService.Setup(_ => _.UpdateDisputeAsync(1, _dispute, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_dispute));
        _context = new();
        _context.Setup(_ => _.Message).Returns(_message);
        _context.Setup(_ => _.CancellationToken).Returns(CancellationToken.None);

        _consumer = new(_mockLogger.Object, _oracleDataApiService.Object, new DisputeUpdateRequestAcceptedTemplate());
    }

    [Fact]
    public async Task TestDisputeUpdateRequestAcceptedConsumer_AddressUpdates()
    {
        // Arrange
        _updateRequest.Status = DisputeUpdateRequestStatus2.ACCEPTED;
        _updateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS;
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
    public async Task TestDisputeUpdateRequestAcceptedConsumer_NameUpdates()
    {
        // Arrange
        _updateRequest.Status = DisputeUpdateRequestStatus2.ACCEPTED;
        _updateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_NAME;
        _updateRequest.UpdateJson = "{ \"disputantGivenName1\": \"fname1\", \"disputantGivenName2\": \"fname2\", \"disputantGivenName3\": \"fname3\", \"disputantSurname\": \"lname\" }";

        // Act
        await _consumer.Consume(_context.Object);

        // Assert
        Assert.Equal("fname1", _dispute.DisputantGivenName1);
        Assert.Equal("fname2", _dispute.DisputantGivenName2);
        Assert.Equal("fname3", _dispute.DisputantGivenName3);
        Assert.Equal("lname", _dispute.DisputantSurname);
    }

    [Fact]
    public async Task TestDisputeUpdateRequestAcceptedConsumer_PhoneUpdates()
    {
        // Arrange
        _updateRequest.Status = DisputeUpdateRequestStatus2.ACCEPTED;
        _updateRequest.UpdateType = DisputeUpdateRequestUpdateType.DISPUTANT_PHONE;
        _updateRequest.UpdateJson = "{ \"homePhoneNumber\": \"2505556666\" }";

        // Act
        await _consumer.Consume(_context.Object);

        // Assert
        Assert.Equal("2505556666", _dispute.HomePhoneNumber);
    }
}
