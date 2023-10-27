using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace TrafficCourts.Staff.Service.Test.Services.PrintDigialCaseFile;

public abstract class CommonDocumentGenerationServiceTest
{
    protected IContainer Container { get; init; }

    protected CommonDocumentGenerationServiceTest()
    {
        Container = new ContainerBuilder()
            .WithImage("bcgovimages/common-document-generation-service:latest")
            .WithPortBinding(3000, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(3000)))
            .WithVolumeMount("carbone-cache", "/tmp/carbone-files")
            .Build();
    }
}
