using FastEndpoints;
using System.Runtime.CompilerServices;
using BCGov.VirusScan.Api.Contracts.Scan;
using BCGov.VirusScan.Api.Contracts.Version;
using BCGov.VirusScan.Api.Security;
using BCGov.VirusScan.Api.Services;

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
        Post("scan");
        AllowFileUploads(dontAutoBindFormData: true); // turns off buffering

        if (AuthenticationConfiguration.AllowAnonymous)
        {
            AllowAnonymous();
        }
    }

    public override async Task HandleAsync(ScanRequest req, CancellationToken cancellationToken)
    {
        // the the virus scanner version
        var version = await _virusScanService.GetVersionAsync(cancellationToken);
        var versionResponse = new VersionResponse(version.SoftwareVersion, version.DatabaseVersion, version.DatabaseDate);

        IAsyncEnumerable<MemoryStream> streams = GetFormFileStreams(cancellationToken);
        await _virusScanService.ScanFileAsync(streams, cancellationToken);

       await SendOkAsync(cancellationToken);
    }

    private async IAsyncEnumerable<MemoryStream> GetFormFileStreams([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var section in FormFileSectionsAsync(cancellationToken))
        {
            if (section is not null)
            {
                MemoryStream buffer = _streamManager.GetStream("Scan File Section", 1024 * 1024); // by default, get 1MB buffer
                await section.Section.Body.CopyToAsync(buffer, 1024 * 64, cancellationToken);
                buffer.Position = 0; // rewind this stream position
                yield return buffer;
            }
        }
    }
}
