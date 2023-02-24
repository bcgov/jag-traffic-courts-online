using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using TrafficCourts.Common.OpenAPIs.VirusScan.V1;
using TrafficCourts.Coms.Client;
using TrafficCourts.Messaging.MessageContracts;
using TrafficCourts.Workflow.Service.Consumers;
using TrafficCourts.Workflow.Service.Services;
using Xunit;
using ApiException = TrafficCourts.Common.OpenAPIs.VirusScan.V1.ApiException;

namespace TrafficCourts.Test.Workflow.Service.Consumers;

public class ScanUploadedDocumentForVirusesConsumerTest
{
    [Fact(Skip = "Failing in githut actions but not locally")]
    public async Task TestVirusScanDocumentConsumer_ConfirmScanResultClean()
    {
        Mock<IVirusScanClient> virusScanClient = new();
        Mock<IWorkflowDocumentService> comsService = new();

        // Arrange
        var file = CreateFile();
        //file.Metadata[FileProperty.VirusScanStatus] = Guid.NewGuid().ToString("n");
        //file.Metadata[FileProperty.VirusName] = Guid.NewGuid().ToString("n");

        comsService
            .SetupGetFileWithAnyParameters()
            .Returns(Task.FromResult(file));

        virusScanClient
            .VirusScanWithAnyParameters()
            .Returns(Task.FromResult(new VirusScanResult { Status = VirusScanStatus.NotInfected }));

        // comsService.UpdateFileAsync ...

        await using var provider = GetServiceProvider(virusScanClient.Object, comsService.Object);

        // Act
        var harness = await PublishAsync(provider, new DocumentUploaded { Id = file.Id!.Value });
        // note the consumer will not execute until we this completes
        var consumed = await harness.Consumed.Any<DocumentUploaded>();
        
        // Assert
        Assert.True(consumed);
        //Assert.True(file.VirusScanIsClean());
        //Assert.False(file.Metadata.ContainsKey(FileProperty.VirusName));

        comsService.VerifyGetFile(file.Id!.Value);

        virusScanClient.VerifyVirusScan(file.Data);

        // verify UpdateFile meta data
    }

    [Fact(Skip = "Failing in githut actions but not locally")]
    public async Task TestVirusScanDocumentConsumer_ConfirmScanResultInfected()
    {
        Mock<IVirusScanClient> virusScanClient = new();
        Mock<IWorkflowDocumentService> comsService = new();

        // Arrange
        var file = CreateFile();

        comsService
            .SetupGetFileWithAnyParameters()
            .Returns(Task.FromResult(file));

        virusScanClient
            .VirusScanWithAnyParameters()
            .Returns(Task.FromResult(new VirusScanResult { Status = VirusScanStatus.Infected, VirusName = "cryptolocker" }));

        // comsService.UpdateFileAsync ...

        await using var provider = GetServiceProvider(virusScanClient.Object, comsService.Object);

        // Act
        var harness = await PublishAsync(provider, new DocumentUploaded { Id = file.Id!.Value });

        // note the consumer will not execute until we this completes
        var consumed = await harness.Consumed.Any<DocumentUploaded>();

        // Assert
        Assert.True(consumed);
        //Assert.True(file.VirusScanIsInfected());
        //Assert.Equal("cryptolocker", file.GetVirusScanStatus());

        comsService.VerifyGetFile(file.Id!.Value);

        virusScanClient.VerifyVirusScan(file.Data);

        // verify UpdateFile meta data

    }

    [Fact(Skip = "Failing in githut actions but not locally")]
    public async Task TestVirusScanDocumentConsumer_ConfirmScanResultUnknown()
    {
        Mock<IVirusScanClient> virusScanClient = new();
        Mock<IWorkflowDocumentService> comsService = new();

        // Arrange
        var file = CreateFile();

        comsService
            .SetupGetFileWithAnyParameters()
            .Returns(Task.FromResult(file));

        virusScanClient
            .VirusScanWithAnyParameters()
            .Returns(Task.FromResult(new VirusScanResult { Status = VirusScanStatus.Error }));

        await using var provider = GetServiceProvider(virusScanClient.Object, comsService.Object);

        // Act
        var harness = await PublishAsync(provider, new DocumentUploaded { Id = file.Id!.Value });
        // note the consumer will not execute until we this completes
        var consumed = await harness.Consumed.Any<DocumentUploaded>();

        // Assert
        Assert.True(consumed);
        //Assert.True(file.VirusScanIsError());

        comsService.VerifyGetFile(file.Id!.Value);

        virusScanClient.VerifyVirusScan(file.Data);

        // verify UpdateFile meta data
    }


    [Fact(Skip = "Failing in githut actions but not locally")]
    public async Task TestVirusScanDocumentConsumer_ThrowsApiException()
    {
        Mock<IWorkflowDocumentService> comsService = new();
        Mock<IVirusScanClient> virusScanClient = new();

        // Arrange
        var file = CreateFile();

        comsService
            .SetupGetFileWithAnyParameters()
            .Returns(Task.FromResult(file));

        var exception = new ApiException("[Unit Test Expected]: There was an internal error virus scanning the file.", StatusCodes.Status500InternalServerError, It.IsAny<string>(), null, null);
        virusScanClient
            .VirusScanWithAnyParameters()
            .Throws(exception);

        await using var provider = GetServiceProvider(virusScanClient.Object, comsService.Object);

        // Act
        var harness = await PublishAsync(provider, new DocumentUploaded { Id = file.Id!.Value });
        // note the consumer will not execute until we this completes
        var consumed = await harness.Consumed.Any<DocumentUploaded>();
        var published = await harness.Published.SelectAsync<Fault<DocumentUploaded>>().Count();

        // Assert
        Assert.True(consumed);
        Assert.Equal(1, published);

        comsService.VerifyGetFile(file.Id!.Value);

        virusScanClient.VerifyVirusScan(file.Data);

        comsService.VerifySaveDocumentProperties(Times.Never());
    }

    /// <summary>
    /// Gets the ServiceProvider with all the registered services 
    /// </summary>
    /// <param name="virusScanClient"></param>
    /// <param name="comsService"></param>
    /// <returns></returns>
    private static ServiceProvider GetServiceProvider(IVirusScanClient virusScanClient, IWorkflowDocumentService comsService)
    {
        return new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<ScanUploadedDocumentForVirusesConsumer>();
                cfg.UsingInMemory((context, inMemoryConfig) =>
                {
                    inMemoryConfig.ConfigureEndpoints(context);
                });
            })
            .AddScoped(sp => comsService)
            .AddScoped(sp => virusScanClient)
            .AddScoped(sp => Mock.Of<ILogger<ScanUploadedDocumentForVirusesConsumer>>())
            .BuildServiceProvider(true);
    }

    /// <summary>
    /// Gets and starts the harness and published the message.
    /// </summary>
    private async Task<ITestHarness> PublishAsync<T>(ServiceProvider provider, T message) where T : class
    {
        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();
        await harness.Bus.Publish(message);

        return harness;
    }

    /// <summary>
    /// Creates a random file
    /// </summary>
    private Coms.Client.File CreateFile()
    {
        Stream stream = new MemoryStream(Guid.NewGuid().ToByteArray());
        return new Coms.Client.File (Guid.NewGuid(), stream, "sample_file", null, null, null);
    }
}
