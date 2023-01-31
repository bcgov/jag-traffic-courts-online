using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Common.OpenAPIs.VirusScan.V1;
using TrafficCourts.Workflow.Service.Services;

namespace TrafficCourts.Test;

public static class MockExtensions
{
    public static Moq.Language.Flow.ISetup<IVirusScanClient, Task<VirusScanResult>> VirusScanWithAnyParameters(this Mock<IVirusScanClient> mock)
    {
        return mock.Setup(_ => _.VirusScanAsync(It.IsAny<Common.OpenAPIs.VirusScan.V1.FileParameter>(), It.IsAny<CancellationToken>()));
    }

    public static Moq.Language.Flow.ISetup<IWorkflowDocumentService, Task<Coms.Client.File>> SetupGetFileWithAnyParameters(this Mock<IWorkflowDocumentService> mock)
    {
        return mock.Setup(_ => _.GetFileAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));
    }


    /// <summary>
    /// Verify ICitizenDocumentService.GetFileAsync was called once with the specified id
    /// </summary>
    public static void VerifyGetFile(this Mock<IWorkflowDocumentService> mock, Guid id)
    {
        mock.Verify(_ => _.GetFileAsync(
            It.Is<Guid>((actual) => actual == id),
            It.IsAny<CancellationToken>()), Times.Once());
    }

    public static void VerifyUpdateFile(this Mock<IWorkflowDocumentService> mock, Times times)
    {
        mock.Verify(_ => _.UpdateFileAsync(
            It.IsAny<Guid>(),
            It.IsAny<Coms.Client.File>(),
            It.IsAny<CancellationToken>()), times);
    }

    public static void VerifyVirusScan(this Mock<IVirusScanClient> mock, Times times)
    {
        mock.Verify(_ => _.VirusScanAsync(
            It.IsAny<Common.OpenAPIs.VirusScan.V1.FileParameter>(),
            It.IsAny<CancellationToken>()), times);
    }

    public static void VerifyVirusScan(this Mock<IVirusScanClient> mock, Stream stream)
    {
        mock.Verify(_ => _.VirusScanAsync(
            It.Is<Common.OpenAPIs.VirusScan.V1.FileParameter>(actual => actual.Data == stream),
            It.IsAny<CancellationToken>()), Times.Once());
    }
}
