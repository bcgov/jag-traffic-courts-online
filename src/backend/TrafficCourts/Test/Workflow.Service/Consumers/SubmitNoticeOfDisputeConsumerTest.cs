using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Domain.Models;
using TrafficCourts.Interfaces;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Consumers;
using TrafficCourts.Workflow.Service.Mappings;
using Xunit;

namespace TrafficCourts.Test.Workflow.Service.Consumers;

public class SubmitNoticeOfDisputeConsumerTest
{
    [Fact]
    public async Task Consume_SavesDisputeWithCitizenPortalSource()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Register<DateOnly>(() => new(2000, 1, 31));
        var message = fixture.Create<SubmitNoticeOfDispute>();

        var mapper = new MapperConfiguration(cfg => cfg.AddProfile(new MessageContractToDisputeMappingProfile())).CreateMapper();

        Dispute? savedDispute = null;
        var oracleDataApi = new Mock<IOracleDataApiService>();
        oracleDataApi
            .Setup(_ => _.SaveDisputeAsync(It.IsAny<Dispute>(), It.IsAny<CancellationToken>()))
            .Callback<Dispute, CancellationToken>((dispute, _) => savedDispute = dispute)
            .ReturnsAsync(1L);

        var context = new Mock<ConsumeContext<SubmitNoticeOfDispute>>();
        context.Setup(_ => _.Message).Returns(message);
        context.Setup(_ => _.CancellationToken).Returns(CancellationToken.None);

        var consumer = new SubmitNoticeOfDisputeConsumer(Mock.Of<ILogger<SubmitNoticeOfDisputeConsumer>>(), oracleDataApi.Object, mapper);

        // Act
        await consumer.Consume(context.Object);

        // Assert
        Assert.NotNull(savedDispute);
        Assert.Equal(DisputeDisputeSourceType.CITZ, savedDispute!.DisputeSourceType);
    }
}
