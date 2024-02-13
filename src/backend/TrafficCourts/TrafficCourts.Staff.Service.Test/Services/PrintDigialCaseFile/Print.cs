using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TrafficCourts.Cdogs.Client;
using TrafficCourts.Staff.Service.Models.DigitalCaseFiles.Print;
using Xunit;
using Xunit.Abstractions;

namespace TrafficCourts.Staff.Service.Test.Services.PrintDigialCaseFile;

public class PrintTest : CommonDocumentGenerationServiceTest
{
    private readonly ITestOutputHelper _output;

    public PrintTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [IntegrationTestFact]
    public async Task render_digital_case_file()
    {
        // Start the container.
        await Container.StartAsync();

        var template = GetTemplate("digital_case_file.template.docx");
        Assert.NotNull(template);

        var data = GetData<DigitalCaseFile>("digital_case_file.data.json");

        if (data is null)
        {
            data = CreateDigitalCaseFile();
        }

        var service = CreateDocumentGenerationService();
        RenderedReport report = await service.UploadTemplateAndRenderReportAsync(template, TemplateType.Word, ConvertTo.Pdf, "DigitalCaseFile.pdf", data, CancellationToken.None);

        SaveReport(report, data);
    }

    [IntegrationTestFact]
    public async Task render_watermark_footer_header_example()
    {
        // Start the container
        await Container.StartAsync();

        var template = GetTemplate("watermark_footer_header.template.odt");
        var data = GetData<TestCases.watermark_footer_header.Data>("watermark_footer_header.data.json");

        Assert.NotNull(template);
        Assert.NotNull(data);

        var service = CreateDocumentGenerationService();
        RenderedReport report = await service.UploadTemplateAndRenderReportAsync(template, TemplateType.Word, ConvertTo.Pdf, "DigitalCaseFile.pdf", data, CancellationToken.None);

        SaveReport(report, data);
    }

    [IntegrationTestFact]
    public async Task render_carbone_2_example()
    {
        // Start the container
        await Container.StartAsync();

        var template = GetTemplate("carbone_2.template.docx");
        var data = GetData<TestCases.carbone_2.Data>("carbone_2.data.json");

        Assert.NotNull(template);
        Assert.NotNull(data);

        var service = CreateDocumentGenerationService();
        RenderedReport report = await service.UploadTemplateAndRenderReportAsync(template, TemplateType.Word, ConvertTo.Pdf, "DigitalCaseFile.pdf", data, CancellationToken.None);

        SaveReport(report, data);
    }

    /// <summary>
    /// Save the report and data file for validation
    /// </summary>
    private void SaveReport(RenderedReport report, object data)
    {
        // get a temp file name
        string tempFileName = Path.GetTempFileName();

        // create the pdf filename and save file
        string fileName = Path.ChangeExtension(tempFileName, ".pdf");
        using var target = File.OpenWrite(fileName);
        report.Content.CopyTo(target);
        target.Flush();
        target.Close();

        // create the json filename and save file
        string jsonFileName = Path.ChangeExtension(tempFileName, ".json");
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented);

        File.WriteAllText(jsonFileName, json);

        _output.WriteLine($"Rendered report written to {fileName} and the data {jsonFileName}");

    }

    /// <summary>
    /// Creates an instance of the document generation service that calls
    /// a local container based instance of the service.
    /// </summary>
    private IDocumentGenerationService CreateDocumentGenerationService()
    {
        HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri($"http://{Container.Hostname}:{Container.GetMappedPublicPort(3000)}")
        };

        DocumentGenerationClient client = new DocumentGenerationClient(httpClient);
        client.ReadResponseAsString = true; // help debugging
        DocumentGenerationService service = new DocumentGenerationService(client, Moq.Mock.Of<ILogger<DocumentGenerationService>>());

        return service;
    }

    /// <summary>
    /// Creates an instance of the document generation service that calls
    /// the show case application. Requires configuration to be in user secrets.
    /// </summary>
    private IDocumentGenerationService CreateShowcaseDocumentGenerationService()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<PrintTest>()
            //.AddInMemoryCollection(myConfiguration)
            .Build();

        ServiceCollection services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(configuration);

        services.AddDocumentGenerationService("cdogs");
        var provider = services.BuildServiceProvider();

        var service = provider.GetRequiredService<IDocumentGenerationService>();

        return service;
    }

    /// <summary>
    /// Gets optional embedded test case file. A test case file could be a template or data file.
    /// </summary>
    private MemoryStream? GetTestCaseFile(string name, bool optional = false)
    {
        // get the template
        IFileProvider fileProvder = new EmbeddedFileProvider(typeof(PrintTest).Assembly);
        string path = $"Services.PrintDigialCaseFile.TestCases.{name}";
        var fileInfo = fileProvder.GetFileInfo(path);
        if (optional && !fileInfo.Exists)
        {
            // template is optional and doesn't exist
            return null;
        }

        var stream = fileInfo.CreateReadStream();

        MemoryStream template = new MemoryStream();
        stream.CopyTo(template);
        template.Position = 0;

        return template;
    }

    /// <summary>
    /// Gets optional embedded template
    /// </summary>
    private MemoryStream? GetTemplate(string name)
    {
        return GetTestCaseFile(name);
    }

    /// <summary>
    /// Gets optional embedded data file
    /// </summary>
    private T? GetData<T>(string name)
    {
        // get the template
        var stream = GetTestCaseFile(name, true);

        if (stream is null)
        {
            // get random data
            return default(T);
        }

        var data = System.Text.Json.JsonSerializer.Deserialize<T>(stream);
        return data;
    }

    /// <summary>
    /// Create a random digital case file.
    /// </summary>
    private DigitalCaseFile CreateDigitalCaseFile()
    {
        var faker = new DigitalCaseFileFaker();
        DigitalCaseFile file = faker.Generate();

        for (int i = 0; i < file.Counts.Count; i++)
        {
            file.Counts[i].Count = (i + 1).ToString(); // fix up the count numbers
        }

        return file;
    }
}
