using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v18_0;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Services;

public class JJDisputeServiceTest
{

    [Fact]
    public void TestGetPartId_JJAssignedToNull()
    {
        // Arrange
        var _oracleDataApiClient = new Mock<IOracleDataApiClient>();
        var _bus = new Mock<IBus>();
        var _staffDocumentService = new Mock<IStaffDocumentService>();
        var _keycloakService = new Mock<IKeycloakService>();
        var _logger = new Mock<ILogger<JJDisputeService>>();
        var _httpContextAccessor = new Mock<IHttpContextAccessor>();
        JJDisputeService jJDisputeService = new(_oracleDataApiClient.Object, _bus.Object, _staffDocumentService.Object, _keycloakService.Object, _logger.Object, _httpContextAccessor.Object);
        JJDispute dispute = new();
        dispute.TicketNumber = "AJ201092461";

        _oracleDataApiClient.Setup(_ => _.GetJJDisputeAsync(dispute.TicketNumber, It.IsAny<bool>(), CancellationToken.None)).ReturnsAsync(dispute);

        // Assert/Act
        Assert.ThrowsAsync<ArgumentNullException>(() => jJDisputeService.GetPartIdAsync(dispute.TicketNumber, CancellationToken.None));
    }

    [Fact]
    public void TestGetPartId_HearingTypeNull()
    {
        // Arrange
        var _oracleDataApiClient = new Mock<IOracleDataApiClient>();
        var _bus = new Mock<IBus>();
        var _staffDocumentService = new Mock<IStaffDocumentService>();
        var _keycloakService = new Mock<IKeycloakService>();
        var _logger = new Mock<ILogger<JJDisputeService>>();
        var _httpContextAccessor = new Mock<IHttpContextAccessor>();
        JJDisputeService jJDisputeService = new(_oracleDataApiClient.Object, _bus.Object, _staffDocumentService.Object, _keycloakService.Object, _logger.Object, _httpContextAccessor.Object);
        JJDispute dispute = new();
        dispute.JjAssignedTo = "ckent";
        dispute.TicketNumber = "AJ201092461";

        _oracleDataApiClient.Setup(_ => _.GetJJDisputeAsync(dispute.TicketNumber, It.IsAny<bool>(), CancellationToken.None)).ReturnsAsync(dispute);

        // Assert/Act
        Assert.ThrowsAsync<ArgumentException>(() => jJDisputeService.GetPartIdAsync(dispute.TicketNumber, CancellationToken.None));
    }

    [Fact]
    public void TestGetPartId_EmptyKeycloak()
    {
        // Arrange
        var _oracleDataApiClient = new Mock<IOracleDataApiClient>();
        var _bus = new Mock<IBus>();
        var _staffDocumentService = new Mock<IStaffDocumentService>();
        var _keycloakService = new Mock<IKeycloakService>();
        var _logger = new Mock<ILogger<JJDisputeService>>();
        var _userReps = new Mock<ICollection<UserRepresentation>>();
        var _httpContextAccessor = new Mock<IHttpContextAccessor>();
        JJDisputeService jJDisputeService = new(_oracleDataApiClient.Object, _bus.Object, _staffDocumentService.Object, _keycloakService.Object, _logger.Object, _httpContextAccessor.Object);
        JJDispute dispute = new();
        dispute.JjAssignedTo = "ckent";
        dispute.TicketNumber = "AJ201092461";
        dispute.HearingType = JJDisputeHearingType.WRITTEN_REASONS;

        _oracleDataApiClient.Setup(_ => _.GetJJDisputeAsync(dispute.TicketNumber, It.IsAny<bool>(), CancellationToken.None)).ReturnsAsync(dispute);
        _keycloakService.Setup(_ => _.UsersByIdirAsync(dispute.JjAssignedTo, CancellationToken.None)).ReturnsAsync(_userReps.Object);

        // Assert/Act
        Assert.ThrowsAsync<ArgumentNullException>(() => jJDisputeService.GetPartIdAsync(dispute.TicketNumber, CancellationToken.None));
    }

    [Fact]
    public async void TestGetPartId()
    {
        // Arrange
        var _oracleDataApiClient = new Mock<IOracleDataApiClient>();
        var _bus = new Mock<IBus>();
        var _staffDocumentService = new Mock<IStaffDocumentService>();
        var _keycloakService = new Mock<IKeycloakService>();
        var _logger = new Mock<ILogger<JJDisputeService>>();
        var _userReps = new List<UserRepresentation>();
        var _userRep = new Mock<UserRepresentation>();
        var _expectedPartIds = new List<string>();
        var _httpContextAccessor = new Mock<IHttpContextAccessor>();
        JJDisputeService jJDisputeService = new(_oracleDataApiClient.Object, _bus.Object, _staffDocumentService.Object, _keycloakService.Object, _logger.Object, _httpContextAccessor.Object);
        JJDispute dispute = new();
        dispute.JjAssignedTo = "ckent";
        dispute.TicketNumber = "AJ201092461";
        dispute.HearingType = JJDisputeHearingType.WRITTEN_REASONS;
        _userRep.Object.Username = dispute.JjAssignedTo;
        _userReps.Add(_userRep.Object);
        _expectedPartIds.Add("1234.5678");

        _oracleDataApiClient.Setup(_ => _.GetJJDisputeAsync(dispute.TicketNumber, It.IsAny<bool>(), CancellationToken.None)).ReturnsAsync(dispute);
        _keycloakService.Setup(_ => _.UsersByIdirAsync(dispute.JjAssignedTo, CancellationToken.None)).ReturnsAsync(_userReps);
        _keycloakService.Setup(_ => _.TryGetPartIds(_userRep.Object)).Returns(_expectedPartIds);

        // Act
        string _actualPartId = await jJDisputeService.GetPartIdAsync(dispute.TicketNumber, CancellationToken.None);

        // Assert
        Assert.Equal(_expectedPartIds.First(), _actualPartId);
    }

}
