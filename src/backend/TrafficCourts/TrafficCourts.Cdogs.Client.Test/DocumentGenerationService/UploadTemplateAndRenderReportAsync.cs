using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TrafficCourts.Cdogs.Client.Test.DocumentGenerationService;

public class UploadTemplateAndRenderReportAsync : DocumentGenerationServiceTests
{
    [Fact(Skip = "Requires User Secrets")]
    public async Task should_render_template()
    {
        var services = new ServiceCollection();

        IConfiguration configuration = new ConfigurationBuilder()
            .AddUserSecrets<UploadTemplateAndRenderReportAsync>()
            .Build();

        Extensions.AddDocumentGenerationService(services, "Cdogs", configuration);

        var serviceProvider = services.BuildServiceProvider();

        var sut = serviceProvider.GetRequiredService<IDocumentGenerationService>();

        string template = "Hello {d.firstName} {d.lastName}!";

        RenderedReport actual = await sut.UploadTemplateAndRenderReportAsync(
            template, 
            ConvertTo.Pdf, 
            "unit-test", 
            new { firstName = "Phil", lastName = "Bolduc"}, 
            CancellationToken.None);

        Assert.NotNull(actual);

        Assert.Equal("unit-test.pdf", actual.ReportName);
        Assert.Equal("application/pdf", actual.ContentType);
        Assert.NotNull(actual.Content);
    }
}
