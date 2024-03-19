using AutoFixture;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.Models;
using TrafficCourts.Common.OpenAPIs.Keycloak;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v22_0;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Services;

public class JJDisputeServiceTest
{

    [Fact]
    public async Task TestGetPartId_JJAssignedToNull()
    {
        // Arrange
        var _oracleDataApiClient = new Mock<IOracleDataApiClient>();
        var _bus = new Mock<IBus>();
        var _staffDocumentService = new Mock<IStaffDocumentService>();
        var _keycloakService = new Mock<IKeycloakService>();
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        var _logger = new Mock<ILogger<JJDisputeService>>();
        JJDisputeService jJDisputeService = new(_oracleDataApiClient.Object, _bus.Object, _staffDocumentService.Object, _keycloakService.Object, _statuteLookupService.Object, _logger.Object);
        JJDispute dispute = new();
        dispute.TicketNumber = "AJ201092461";

        _oracleDataApiClient.Setup(_ => _.GetJJDisputeAsync(dispute.TicketNumber, It.IsAny<bool>(), CancellationToken.None)).ReturnsAsync(dispute);

        // Assert/Act
        //await Assert.ThrowsAsync<ArgumentNullException>(() => jJDisputeService.GetPartIdAsync(dispute.TicketNumber, CancellationToken.None));
    }

    [Fact]
    public async Task TestGetPartId_HearingTypeNull()
    {
        // Arrange
        var _oracleDataApiClient = new Mock<IOracleDataApiClient>();
        var _bus = new Mock<IBus>();
        var _staffDocumentService = new Mock<IStaffDocumentService>();
        var _keycloakService = new Mock<IKeycloakService>();
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        var _logger = new Mock<ILogger<JJDisputeService>>();
        JJDisputeService jJDisputeService = new(_oracleDataApiClient.Object, _bus.Object, _staffDocumentService.Object, _keycloakService.Object, _statuteLookupService.Object, _logger.Object);
        JJDispute dispute = new();
        dispute.JjAssignedTo = "ckent";
        dispute.TicketNumber = "AJ201092461";

        _oracleDataApiClient.Setup(_ => _.GetJJDisputeAsync(dispute.TicketNumber, It.IsAny<bool>(), CancellationToken.None)).ReturnsAsync(dispute);

        // Assert/Act
        //await Assert.ThrowsAsync<ArgumentException>(() => jJDisputeService.GetPartIdAsync(dispute.TicketNumber, CancellationToken.None));
    }

    [Fact]
    public async Task TestGetPartId_EmptyKeycloak()
    {
        // Arrange
        var _oracleDataApiClient = new Mock<IOracleDataApiClient>();
        var _bus = new Mock<IBus>();
        var _staffDocumentService = new Mock<IStaffDocumentService>();
        var _keycloakService = new Mock<IKeycloakService>();
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        var _logger = new Mock<ILogger<JJDisputeService>>();
        var _userReps = new Mock<ICollection<UserRepresentation>>();
        JJDisputeService jJDisputeService = new(_oracleDataApiClient.Object, _bus.Object, _staffDocumentService.Object, _keycloakService.Object, _statuteLookupService.Object, _logger.Object);
        JJDispute dispute = new();
        dispute.JjAssignedTo = "ckent";
        dispute.TicketNumber = "AJ201092461";
        dispute.HearingType = JJDisputeHearingType.WRITTEN_REASONS;

        _oracleDataApiClient.Setup(_ => _.GetJJDisputeAsync(dispute.TicketNumber, It.IsAny<bool>(), CancellationToken.None)).ReturnsAsync(dispute);
        _keycloakService.Setup(_ => _.UsersByIdirAsync(dispute.JjAssignedTo, CancellationToken.None)).ReturnsAsync(_userReps.Object);

        // Assert/Act
        //await Assert.ThrowsAsync<ArgumentNullException>(() => jJDisputeService.GetPartIdAsync(dispute.TicketNumber, CancellationToken.None));
    }

    [Fact]
    public async Task TestGetDisputeAssignToPartId()
    {
        // Arrange

        // create a random dispute
        Fixture fix = new Fixture();
        JJDispute dispute = fix.Create<JJDispute>();

        var oracleDataApiClient = Substitute.For<IOracleDataApiClient>();

        oracleDataApiClient
            .GetJJDisputeAsync(dispute.TicketNumber, Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(dispute);

        // create UserRepresentation collection that UsersByIdirAsync will return
        var expectedPartId = System.Guid.NewGuid().ToString();
        List<UserRepresentation> representations = [
            new UserRepresentation()
                {
                    Attributes = new Dictionary<string, IList<string>>
                    {
                        { UserAttributes.PartId, [expectedPartId] }
                    }
                }
        ];

        var keycloakService = Substitute.For<IKeycloakService>();
        keycloakService
            .UsersByIdirAsync(dispute.JjAssignedTo, Arg.Any<CancellationToken>())
            .Returns(representations);

        // create the subject under test
        JJDisputeService sut = new JJDisputeService(
            oracleDataApiClient,
            Substitute.For<IBus>(),
            Substitute.For<IStaffDocumentService>(),
            keycloakService,
            Substitute.For<IStatuteLookupService>(),
            Substitute.For<ILogger<JJDisputeService>>());

        // Act
        string? actual = await sut.GetDisputeAssignToPartIdAsync(dispute.TicketNumber, CancellationToken.None);

        // Assert
        Assert.Equal(expectedPartId, actual);
    }

    [Fact]
    public async Task TestGetStatuteDescription()
    {
        // Arrange
        var _oracleDataApiClient = new Mock<IOracleDataApiClient>();
        var _bus = new Mock<IBus>();
        var _staffDocumentService = new Mock<IStaffDocumentService>();
        var _keycloakService = new Mock<IKeycloakService>();
        var _statuteLookupService = new Mock<IStatuteLookupService>();
        var _logger = new Mock<ILogger<JJDisputeService>>();

        JJDisputeService jJDisputeService = new(_oracleDataApiClient.Object, _bus.Object, _staffDocumentService.Object, _keycloakService.Object, _statuteLookupService.Object, _logger.Object);

        var counts = new List<JJDisputedCount>();
        JJDisputedCount count = new();
        count.Description = "19588";
        counts.Add(count);

        JJDispute dispute = new();
        dispute.Id = 5L;
        dispute.TicketNumber = "AJ201092461";
        dispute.JjDisputedCounts = counts;

        Domain.Models.Statute expected = new("19588", "MVA", "100", "1", "a", "i", "100(1)(a)i", "Fail to stop/police pursuit", "Fail to stop/police pursuit");
        string expectedDescription = "MVA 100(1)(a)i Fail to stop/police pursuit";

        _oracleDataApiClient.Setup(_ => _.GetJJDisputeAsync(dispute.TicketNumber, It.IsAny<bool>(), CancellationToken.None)).ReturnsAsync(dispute);
        _statuteLookupService.Setup(_ => _.GetByIdAsync(dispute.JjDisputedCounts.First().Description)).ReturnsAsync(expected);

        // Act
        JJDispute _jjDispute = await jJDisputeService.GetJJDisputeAsync(dispute.TicketNumber, false, CancellationToken.None);

        // Assert
        var expectedCount = Assert.Single(_jjDispute.JjDisputedCounts);
        Assert.Equal(expectedDescription, expectedCount.Description);
    }

}
