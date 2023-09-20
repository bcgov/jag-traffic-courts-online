using Microsoft.Extensions.Logging;
using System.Text;

namespace TrafficCourts.Cdogs.Client;

public class DocumentGenerationService : IDocumentGenerationService
{
    private readonly IDocumentGenerationClient _client;
    private readonly ILogger<DocumentGenerationService> _logger;

    public DocumentGenerationService(IDocumentGenerationClient client, ILogger<DocumentGenerationService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<RenderedReport> UploadTemplateAndRenderReportAsync<T>(
        Stream template,
        TemplateType templateType,
        ConvertTo convertTo,
        string reportName,
        T data,
        CancellationToken cancellationToken) where T : class
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(reportName);
        ArgumentNullException.ThrowIfNull(data);

        TemplateRenderObject<T> renderObject = new TemplateRenderObject<T>
        {
            Data = data,
            Options = CreateOptions(convertTo, reportName),
            Template = CreateTemplate(templateType, template)
        };

        FileResponse response = await _client
            .UploadTemplateAndRenderReportAsync<T>(renderObject, cancellationToken)
            .ConfigureAwait(false);

        var report = await GetRenderedReportAsync(response);
        return report;
    }

    public async Task<RenderedReport> UploadTemplateAndRenderReportAsync<T>(
        string template,
        ConvertTo convertTo,
        string reportName,
        T data,
        CancellationToken cancellationToken) where T : class
    {
        ArgumentNullException.ThrowIfNull(template);
        ArgumentNullException.ThrowIfNull(reportName);
        ArgumentNullException.ThrowIfNull(data);

        TemplateRenderObject<T> renderObject = new TemplateRenderObject<T>
        {
            Data = data,
            Options = CreateOptions(convertTo, reportName),
            Template = CreateTemplate(Client.TemplateType.Text, template)
        };

        FileResponse response = await _client
            .UploadTemplateAndRenderReportAsync<T>(renderObject, cancellationToken)
            .ConfigureAwait(false);

        var report = await GetRenderedReportAsync(response);
        return report;
    }

    private static Options CreateOptions(ConvertTo convertTo, string reportName)
    {
        return new Options
        {
            CacheReport = false,
            ConvertTo = ConvertTo(convertTo),
            Overwrite = true,
            ReportName = reportName
        };
    }

    private static Template CreateTemplate(TemplateType templateType, string template)
    {
        return new Template
        {
            EncodingType = InlineTemplateObjectEncodingType.Base64,
            FileType = TemplateType(templateType),
            Content = Convert.ToBase64String(Encoding.ASCII.GetBytes(template))
        };
    }

    private static Template CreateTemplate(TemplateType templateType, Stream template)
    {
        MemoryStream memoryStream = new MemoryStream();
        template.CopyTo(memoryStream);

        return new Template
        {
            EncodingType = InlineTemplateObjectEncodingType.Base64,
            FileType = TemplateType(templateType),
            Content = Convert.ToBase64String(memoryStream.ToArray())
        };
    }

    private async Task<RenderedReport> GetRenderedReportAsync(FileResponse response)
    {
        string reportName = GetHeaderValue(response, Headers.ReportName);
        string contentType = GetHeaderValue(response, Headers.ContentType);

        MemoryStream content = new MemoryStream();
        await response.Stream.CopyToAsync(content).ConfigureAwait(false);

        var report = new RenderedReport(reportName, contentType, content);
        return report;

    }

    private static string ConvertTo(ConvertTo value)
    {
        switch (value)
        {
            case Client.ConvertTo.Pdf:
                return "pdf";
            case Client.ConvertTo.Word:
                return "docx";
            case Client.ConvertTo.Html:
                return "html";
            default:
                return "pdf";
        }
    }

    private static string TemplateType(TemplateType value)
    {
        switch (value)
        {
            case Client.TemplateType.Text:
                return "txt";
            case Client.TemplateType.Word:
                return "docx";
            case Client.TemplateType.Html:
                return "html";
            default:
                return "txt";
        }
    }

    private string GetHeaderValue(FileResponse response, string header)
    {
        if (response.Headers.TryGetValue(header, out IEnumerable<string>? values) && values.Any())
        {
            return values.First();
        }

        return string.Empty;
    }

    private static class Headers
    {
        public const string ReportName = "x-report-name";
        public const string ContentType = "Content-Type";
    }
}
