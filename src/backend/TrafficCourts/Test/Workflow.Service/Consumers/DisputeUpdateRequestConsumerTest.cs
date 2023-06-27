using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Features.Mail.Templates;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Workflow.Service.Consumers;
using TrafficCourts.Workflow.Service.Services;
using Xunit;
using DisputeUpdateRequest = TrafficCourts.Messaging.MessageContracts.DisputeUpdateRequest;

namespace TrafficCourts.Test.Workflow.Service.Consumers;

public class DisputeUpdateRequestConsumerTest
{
    private readonly DisputeUpdateRequest _message;
    private readonly Dispute _dispute;
    private Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest _updateRequest;
    private readonly Mock<ILogger<DisputeUpdateRequestConsumer>> _mockLogger;
    private readonly Mock<IOracleDataApiService> _oracleDataApiService;
    private readonly Mock<ConsumeContext<DisputeUpdateRequest>> _context;
    private readonly DisputeUpdateRequestConsumer _consumer;

    public DisputeUpdateRequestConsumerTest()
    {
        _dispute = new()
        {
            DisputeId = 1,
            NoticeOfDisputeGuid = new System.Guid("08dadc6b-c307-e6d4-0a58-0a6101000000").ToString(),
            EmailAddress = "someone@somewhere.com",
            EmailAddressVerified = true,
        };
        _message = new()
        {
            NoticeOfDisputeGuid = new System.Guid(_dispute.NoticeOfDisputeGuid),
        };
        _updateRequest = new();
        _mockLogger = new();
        _oracleDataApiService = new();
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _oracleDataApiService.Setup(_ => _.GetDisputeByNoticeOfDisputeGuidAsync(_message.NoticeOfDisputeGuid, It.IsAny<CancellationToken>())).Returns(Task.FromResult(_dispute));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
        _oracleDataApiService.Setup(_ => _.SaveDisputeUpdateRequestAsync(_message.NoticeOfDisputeGuid.ToString(), _updateRequest, It.IsAny<CancellationToken>())).Returns(Task.FromResult<long>(1));
        _oracleDataApiService.Setup(_ => _.ResetDisputeEmailAsync(_dispute.DisputeId, It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(_dispute));
        _context = new();
        _context.Setup(_ => _.Message).Returns(_message);
        _context.Setup(_ => _.CancellationToken).Returns(CancellationToken.None);
        _consumer = new(_mockLogger.Object, _oracleDataApiService.Object, new DisputeUpdateRequestReceivedTemplate());
    }

    [Fact]
    public async Task TestDisputeUpdateRequestConsumer_Blank()
    {
        // Arrange

        // Act
        await _consumer.Consume(_context.Object);

        // Assert the oracle service was never called.
        _oracleDataApiService.Verify(m => m.SaveDisputeUpdateRequestAsync(It.IsAny<string>(), It.IsAny<Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TestDisputeUpdateRequestConsumer_Email()
    {
        // Arrange
        _message.EmailAddress = "new-email@somewhere.com";

        // Act
        await _consumer.Consume(_context.Object);

        // Assert the oracle service was called to save the email address.
        _oracleDataApiService.Verify(m => m.ResetDisputeEmailAsync(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("fname1", "", "", "lname", DisputeContactTypeCd.INDIVIDUAL, "")]
    [InlineData("", "fname2", "", "lname", DisputeContactTypeCd.INDIVIDUAL, "")]
    [InlineData("", "", "fname3", "lname", DisputeContactTypeCd.INDIVIDUAL, "")]
    [InlineData("", "", "", "lname", DisputeContactTypeCd.INDIVIDUAL, "")]
    [InlineData("", "", "", "lname", DisputeContactTypeCd.INDIVIDUAL, "contactLawFirmName")]
    [InlineData("fname1", null, null, "lname", DisputeContactTypeCd.INDIVIDUAL, null)]
    [InlineData(null, "fname2", null, "lname", DisputeContactTypeCd.INDIVIDUAL, null)]
    [InlineData(null, null, "fname3", "lname", DisputeContactTypeCd.INDIVIDUAL, null)]
    [InlineData(null, null, null, "lname", DisputeContactTypeCd.INDIVIDUAL, "contactLawFirmName")]
    public async Task TestDisputeUpdateRequestConsumer_Name(string fname1, string fname2, string fname3, string lname, DisputeContactTypeCd contactType, string contactLawFirmName)
    {
        // Arrange
        _message.DisputantGivenName1 = fname1;
        _message.DisputantGivenName2 = fname2;
        _message.DisputantGivenName3 = fname3;
        _message.DisputantSurname = lname;
        _message.ContactGiven1Nm = fname1;
        _message.ContactGiven2Nm = fname2;
        _message.ContactGiven3Nm = fname3;
        _message.ContactSurnameNm = lname;
        _message.ContactType = contactType;
        _message.ContactLawFirmName = contactLawFirmName;

        // Act
        await _consumer.Consume(_context.Object);

        // Assert the oracle service was called once, INSERTing an update request of type DISPUTANT_NAME and status PENDING.
        _oracleDataApiService.Verify(m => m.SaveDisputeUpdateRequestAsync(_message.NoticeOfDisputeGuid.ToString(),
            It.Is<Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest>(a =>
                a.Status == DisputeUpdateRequestStatus2.PENDING &&
                a.UpdateType == DisputeUpdateRequestUpdateType.DISPUTANT_NAME
            ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("addr1", "", "", "", "", "", null, null, null)]
    [InlineData("addr1", "addr2", "", "", "", "", null, null, null)]
    [InlineData("addr1", "", "addr3", "", "", "", null, null, null)]
    [InlineData("addr1", "", "", "city", "", "", null, null, null)]
    [InlineData("addr1", "", "", "", "prov", "", null, null, null)]
    [InlineData("addr1", "", "", "", "", "postal", null, null, null)]
    [InlineData("addr1", "", "", "", "", "", 1, null, null)]
    [InlineData("addr1", "", "", "", "", "", null, 1, null)]
    [InlineData("addr1", "", "", "", "", "", null, null, 1)]
    [InlineData("addr1", null, null, null, null, null, null, null, null)]
    [InlineData("addr1", "addr2", null, null, null, null, null, null, null)]
    [InlineData("addr1", null, "addr3", null, null, null, null, null, null)]
    [InlineData("addr1", null, null, "city", null, null, null, null, null)]
    [InlineData("addr1", null, null, null, "prov", null, null, null, null)]
    [InlineData("addr1", null, null, null, null, "postal", null, null, null)]
    [InlineData("addr1", null, null, null, null, null, 1, null, null)]
    [InlineData("addr1", null, null, null, null, null, null, 1, null)]
    [InlineData("addr1", null, null, null, null, null, null, null, 1)]
    public async Task TestDisputeUpdateRequestConsumer_Address(string? addr1, string? addr2, string? addr3, string? city, string? prov, string? postal, int? pid, int? pno, int? aid)
    {
        // Arrange
        _message.AddressLine1 = addr1;
        _message.AddressLine2 = addr2;
        _message.AddressLine3 = addr3;
        _message.AddressCity = city;
        _message.AddressProvince = prov;
        _message.PostalCode = postal;
        _message.AddressProvinceCountryId = pid;
        _message.AddressProvinceSeqNo = pno;
        _message.AddressCountryId = aid;

        // Act
        await _consumer.Consume(_context.Object);

        // Assert the oracle service was called once, INSERTing an update request of type DISPUTANT_ADDRESS and status PENDING.
        _oracleDataApiService.Verify(m => m.SaveDisputeUpdateRequestAsync(_message.NoticeOfDisputeGuid.ToString(),
            It.Is<Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest>(a =>
                a.Status == DisputeUpdateRequestStatus2.PENDING &&
                a.UpdateType == DisputeUpdateRequestUpdateType.DISPUTANT_ADDRESS
            ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TestDisputeUpdateRequestConsumer_Phone()
    {
        // Arrange
        _message.HomePhoneNumber = "2505556666";

        // Act
        await _consumer.Consume(_context.Object);

        // Assert the oracle service was called once, INSERTing an update request of type DISPUTANT_PHONE and status PENDING.
        _oracleDataApiService.Verify(m => m.SaveDisputeUpdateRequestAsync(_message.NoticeOfDisputeGuid.ToString(),
            It.Is<Common.OpenAPIs.OracleDataApi.v1_0.DisputeUpdateRequest>(a =>
                a.Status == DisputeUpdateRequestStatus2.PENDING &&
                a.UpdateType == DisputeUpdateRequestUpdateType.DISPUTANT_PHONE
            ), It.IsAny<CancellationToken>()), Times.Once);
    }
}
