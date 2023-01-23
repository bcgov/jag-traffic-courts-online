using FastEndpoints;
using System.Runtime.CompilerServices;
using BCGov.VirusScan.Api.Contracts.Scan;
using BCGov.VirusScan.Api.Contracts.Version;
using BCGov.VirusScan.Api.Security;
using BCGov.VirusScan.Api.Services;
using Microsoft.AspNetCore.WebUtilities;
using BCGov.VirusScan.Api.Models;

namespace BCGov.VirusScan.Api.Endpoints;

public class ScanEndpoint : Endpoint<ScanRequest, ScanResponse>
{
    private readonly IRecyclableMemoryStreamManager _streamManager;
    private readonly IVirusScanService _virusScanService;

    public ScanEndpoint(IRecyclableMemoryStreamManager streamManager, IVirusScanService virusScanService)
    {
        _streamManager = streamManager ?? throw new ArgumentNullException(nameof(streamManager));
        _virusScanService = virusScanService ?? throw new ArgumentNullException(nameof(virusScanService));
    }

    public override void Configure()
    {
        Post("v1/clamav/scan");
        AllowFileUploads(dontAutoBindFormData: true); // turns off buffering
        Description(x => x.WithName("scanFile"));

        if (AuthenticationConfiguration.AllowAnonymous)
        {
            AllowAnonymous();
        }
    }

    public override async Task HandleAsync(ScanRequest req, CancellationToken cancellationToken)
    {
        IAsyncEnumerable<MemoryStream> streams = GetFormFileStreams(cancellationToken);
        var scanResponse = await _virusScanService.ScanFileAsync(streams, cancellationToken);

        var response = new ScanResponse
        {
            Status = scanResponse.Status,
            VirusName = scanResponse.VirusName
        };

        await SendOkAsync(response, cancellationToken);
    }

    /// <summary>
    /// Gets the enumerator that gets the multi-part sections of the uploaded file. This prevents buffering the entire
    /// file into memory.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async IAsyncEnumerable<MemoryStream> GetFormFileStreams([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (FileMultipartSection? section in FormFileSectionsAsync(cancellationToken))
        {
            if (section is not null)
            {
                MemoryStream buffer = _streamManager.GetStream("Scan File Section", 1024 * 1024); // by default, get 1MB buffer
                await section.Section.Body.CopyToAsync(buffer, cancellationToken);
                buffer.Position = 0; // rewind this stream position
                yield return buffer;
            }
        }
    }
}
