using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.Features.DisputeCreation;
using TrafficCourts.Coms.Client;
using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;
using TrafficCourts.Staff.Service.Services;
using TrafficCourts.TicketSearch;
using Xunit;
using ZiggyCreatures.Caching.Fusion;

namespace TrafficCourts.Staff.Service.Test.Services;

public class DisputeServiceTest
{
    [Theory]
    [InlineData(null)]
    [InlineData(DisputeDisputeSourceType.CITZ)]
    [InlineData(DisputeDisputeSourceType.JSTN)]
    public async Task CreateDisputeAsync_AlwaysSavesStaffPortalSource(DisputeDisputeSourceType? sourceFromClient)
    {
        // Arrange
        var dispute = new Dispute { TicketNumber = "AJ201092461", DisputeSourceType = sourceFromClient };
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "DAVEKAY") }, "test"));

        var disputeCreationService = new Mock<IDisputeCreationService>();
        disputeCreationService.Setup(_ => _.CanCreateDispute(dispute.TicketNumber, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        Dispute? savedDispute = null;
        var oracleDataApi = new Mock<IOracleDataApiService>();
        oracleDataApi
            .Setup(_ => _.SaveDisputeAsync(It.IsAny<Dispute>(), It.IsAny<CancellationToken>()))
            .Callback<Dispute, CancellationToken>((d, _) => savedDispute = d)
            .ReturnsAsync(1L);
        oracleDataApi
            .Setup(_ => _.GetDisputeAsync(1L, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => savedDispute!);

        var service = new DisputeService(
            oracleDataApi.Object,
            Mock.Of<IBus>(),
            Mock.Of<IObjectManagementService>(),
            Mock.Of<IAgencyLookupService>(),
            Mock.Of<IProvinceLookupService>(),
            Mock.Of<IStaffDocumentService>(),
            Mock.Of<ITicketSearchService>(),
            disputeCreationService.Object,
            Mock.Of<IFusionCache>(),
            Mock.Of<ILogger<DisputeService>>());

        // Act
        await service.CreateDisputeAsync(user, dispute, TimeZoneInfo.Utc, CancellationToken.None);

        // Assert
        Assert.NotNull(savedDispute);
        Assert.Equal(DisputeDisputeSourceType.STAF, savedDispute!.DisputeSourceType);
        Assert.Equal("DAVEKAY", savedDispute.CreatedBy);
    }
}
