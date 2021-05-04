using AutoFixture.NUnit3;
using AutoMapper;
using DisputeApi.Web.Features.Disputes;
using DisputeApi.Web.Test.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DisputeApi.Web.Features.Disputes.DBModel;
using System.Threading;
using DisputeApi.Web.Features.Disputes.Queries;

namespace DisputeApi.Web.Test.Features.Disputes.Queries
{
    [ExcludeFromCodeCoverage]
    public class GetDisputeQueryHandlerTest
    {
        private Mock<ILogger<GetDisputeQueryHandler>> _loggerMock;
        private Mock<IDisputeService> _disputeServiceMock;
        private Mock<IMapper> _mapperMock;
        private GetDisputeQueryHandler _sut;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<GetDisputeQueryHandler>();
            _disputeServiceMock = new Mock<IDisputeService>();
            _mapperMock = new Mock<IMapper>();

            _sut = new GetDisputeQueryHandler(_loggerMock.Object, _disputeServiceMock.Object, _mapperMock.Object);
        }

        [Test, AllowCirculationAutoData]
        public async Task GetDisputeQueryHandler_handle_will_call_service(GetDisputeQuery getDisputesQuery,
            Dispute dispute, GetDisputeResponse responseDispute)
        {
            _mapperMock.Setup(m => m.Map<GetDisputeResponse>(It.IsAny<Dispute>())).Returns(responseDispute);
            _disputeServiceMock.Setup(m => m.GetAsync(It.IsAny<int>())).Returns(Task.FromResult<Dispute>(dispute));

            var result = await _sut.Handle(getDisputesQuery, CancellationToken.None);
            _disputeServiceMock.Verify(x => x.GetAsync(It.IsAny<int>()), Times.Once);
            Assert.AreEqual(responseDispute, result);
        }
    }
}

