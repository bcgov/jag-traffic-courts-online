namespace TrafficCourts.Cdogs.Client;

public interface IDocumentGenerationService
{
    Task<RenderedReport> UploadTemplateAndRenderReportAsync<T>(string template, ConvertTo convertTo, string reportName, T data, CancellationToken cancellationToken)
        where T : class;

    Task<RenderedReport> UploadTemplateAndRenderReportAsync<T>(Stream template, TemplateType templateType, ConvertTo convertTo, string reportName, T data, CancellationToken cancellationToken)
        where T : class;
}
