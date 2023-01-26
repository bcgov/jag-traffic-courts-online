using MailKit.Net.Smtp;
using TrafficCourts.Common.OpenAPIs.VirusScan.V1;

namespace TrafficCourts.Workflow.Service.Services;

public interface IVirusScanService
{
    /// <summary>
    /// Scans a document for viruses
    /// </summary>
    /// <param name="cancellationToken">Stream of the uploaded file for scanning</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Document scan completed <see cref="ScanResponse"/></returns>
    /// <exception cref="ApiException">A server side error occurred.</exception>
    Task<ScanResponse> ScanDocumentAsync(Stream file, CancellationToken cancellationToken);
}
