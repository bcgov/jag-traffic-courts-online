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
using System.Collections.Generic;

namespace DisputeApi.Web.Test.Features.Disputes.Queries
{
    [ExcludeFromCodeCoverage]
    public class GetAllDisputesQueryHandlerTest
    {
        private Mock<ILogger<GetAllDisputesQueryHandler>> _loggerMock;
        private Mock<IDisputeService> _disputeServiceMock;
        private Mock<IMapper> _mapperMock;
        private GetAllDisputesQueryHandler _sut;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = LoggerServiceMock.LoggerMock<GetAllDisputesQueryHandler>();
            _disputeServiceMock = new Mock<IDisputeService>();
            _mapperMock = new Mock<IMapper>();

            _sut = new GetAllDisputesQueryHandler(_loggerMock.Object, _disputeServiceMock.Object, _mapperMock.Object);
        }

        [Test, AutoData]
        public async Task GetAllDisputesQueryHandler_handle_will_call_service(GetAllDisputesQuery getAllDisputesQuery,
            IEnumerable<Dispute> createdDisputes, IEnumerable<GetDisputeResponse> responseDisputes)
        {
            _mapperMock.Setup(m => m.Map<IEnumerable<GetDisputeResponse>>(It.IsAny<IEnumerable<Dispute>>()))
                .Returns(responseDisputes);
            _disputeServiceMock.Setup(m => m.GetAllAsync())
                .Returns(Task.FromResult<IEnumerable<Dispute>>(createdDisputes));

            var result = await _sut.Handle(getAllDisputesQuery, CancellationToken.None);
            _disputeServiceMock.Verify(x => x.GetAllAsync(), Times.Once);
            Assert.AreEqual(responseDisputes, result);
        }
    }
}
