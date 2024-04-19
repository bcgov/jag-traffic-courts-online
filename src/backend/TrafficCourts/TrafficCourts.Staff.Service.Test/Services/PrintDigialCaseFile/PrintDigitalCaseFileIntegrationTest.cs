using AutoFixture;
using AutoMapper;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Cdogs.Client;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v22_0;
using TrafficCourts.Coms.Client;
using TrafficCourts.Domain.Models;
using TrafficCourts.OracleDataApi;
using TrafficCourts.Staff.Service.Services;
using Xunit;
using ZiggyCreatures.Caching.Fusion;
using Oracle = TrafficCourts.OracleDataApi.Client.V1;

namespace TrafficCourts.Staff.Service.Test.Services.PrintDigialCaseFile
{
    public class PrintDigitalCaseFileIntegrationTest
    {
        private PrintDigitalCaseFileService _sut;
        private readonly Oracle.IOracleDataApiClient _client = Substitute.For<Oracle.IOracleDataApiClient>();
        private readonly IMapper _mapper = Substitute.For<IMapper>();
        private readonly IMediator _mediator = Substitute.For<IMediator>();
        private readonly ILogger<OracleDataApiService> _logger = Substitute.For<ILogger<OracleDataApiService>>();
        private readonly Fixture _fixture = new Fixture();

        public PrintDigitalCaseFileIntegrationTest()
        {
            IConfiguration configuration = GetConfiguration();

            OracleDataApiService oracleDataApi = new(_client, _mapper, _mediator, _logger);

            var mock = new Mock<IStaffDocumentService>();
            mock.Setup(_ => _.FindFilesAsync(It.IsAny<Domain.Models.DocumentProperties>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrafficCourts.Domain.Models.FileMetadata>());

            var mockKeycloakService = new Mock<IKeycloakService>();
            // Add setup for the methods within IKeycloakService that are called internally by GetDigitalCaseFileAsync.
            mockKeycloakService.Setup(_ => _.UsersByIdirAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UserRepresentation>());

            IJJDisputeService jjDisputeService = new JJDisputeService(
                oracleDataApi,
                Mock.Of<IBus>(),
                mock.Object,
                Mock.Of<IKeycloakService>(),
                Mock.Of<IStatuteLookupService>(),
                Mock.Of<ILogger<JJDisputeService>>());

            IDisputeService disputeService = new DisputeService(
                oracleDataApi,
                Mock.Of<IBus>(),
                Mock.Of<IObjectManagementService>(),
                Mock.Of<IAgencyLookupService>(),
                Mock.Of<IProvinceLookupService>(),
                mock.Object,
                Mock.Of<IFusionCache>(),
                Mock.Of<ILogger<DisputeService>>());

            _sut = new PrintDigitalCaseFileService(
                jjDisputeService,
                oracleDataApi,
                Mock.Of<IProvinceLookupService>(),
                Mock.Of<ICountryLookupService>(),
                Mock.Of<IDocumentGenerationService>(),
                disputeService,
                Mock.Of<ILogger<PrintDigitalCaseFileService>>());
        }

        [IntegrationTestFact]
        public async Task Print()
        {
            // create oracle object that should be returned from the client
            var oracle = _fixture.Create<Oracle.JJDispute>();
            var jjDispute = _fixture.Create<JJDispute>();

            _client.GetJJDisputeAsync(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
                .Returns(_ => Task.FromResult(oracle));

            _mapper.Map<JJDispute>(oracle).Returns(jjDispute);

            Models.DigitalCaseFiles.Print.DigitalCaseFile dcf = await _sut.GetDigitalCaseFileAsync("EA03148599", "Pacific Standard Time", CancellationToken.None);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(dcf);

            Assert.True(true);
        }

        [IntegrationTestFact]
        public async Task PrintTicketValidationView()
        {
            // create oracle object that should be returned from the client
            var oracle = _fixture.Create<Oracle.Dispute>();
            var dispute = _fixture.Create<Dispute>();

            _client.GetDisputeAsync(Arg.Any<long>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
                .Returns(_ => Task.FromResult(oracle));

            _mapper.Map<Dispute>(oracle).Returns(dispute);

            Models.DigitalCaseFiles.Print.DigitalCaseFile dcf = await _sut.GetDigitalCaseFileAsync(3354, "Pacific Standard Time", CancellationToken.None);
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(dcf);

            Assert.True(true);
        }


        private IConfiguration GetConfiguration()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets<PrintDigitalCaseFileIntegrationTest>()
                .Build();

            return configuration;
        }
    }
}
