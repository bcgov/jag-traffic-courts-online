using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Cdogs.Client;
using TrafficCourts.Common.Features.Lookups;
using TrafficCourts.Common.Models;
using TrafficCourts.Common.OpenAPIs.KeycloakAdminApi.v22_0;
using TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;
using TrafficCourts.Staff.Service.Services;
using Xunit;

namespace TrafficCourts.Staff.Service.Test.Services.PrintDigialCaseFile
{
    public class PrintDigitalCaseFileIntegrationTest
    {
        private PrintDigitalCaseFileService _sut;


        public PrintDigitalCaseFileIntegrationTest()
        {
            IConfiguration configuration = GetConfiguration();

            IOracleDataApiClient oracleDataApi = new OracleDataApiClient(new HttpClient
            {
                BaseAddress = configuration.GetValue<System.Uri>("OracleDataApi:BaseAddress")
            });

            var mock = new Mock<IStaffDocumentService>();
            mock.Setup(_ => _.FindFilesAsync(It.IsAny<DocumentProperties>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TrafficCourts.Domain.Models.FileMetadata>());

            var mockKeycloakService = new Mock<IKeycloakService>();
            // Add setup for the methods within IKeycloakService that are called internally by GetDigitalCaseFileAsync.
            mockKeycloakService.Setup(_ => _.UsersByIdirAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<UserRepresentation>());

            IJJDisputeService disputeService = new JJDisputeService(
                oracleDataApi,
                Mock.Of<IBus>(),
                mock.Object,
                Mock.Of<IKeycloakService>(),
                Mock.Of<IStatuteLookupService>(),
                Mock.Of<ILogger<JJDisputeService>>());

            _sut = new PrintDigitalCaseFileService(
                disputeService,
                oracleDataApi,
                Mock.Of<IProvinceLookupService>(),
                Mock.Of<IDocumentGenerationService>(),
                Mock.Of<ILogger<PrintDigitalCaseFileService>>());
        }

        [IntegrationTestFact]
        public async Task Print()
        {
            Models.DigitalCaseFiles.Print.DigitalCaseFile dcf = await _sut.GetDigitalCaseFileAsync("EA03148599", "Pacific Standard Time", CancellationToken.None);
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
