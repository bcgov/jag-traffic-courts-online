﻿using BCGov.VirusScan.Api.Models;

namespace BCGov.VirusScan.Api.Services;

public interface IVirusScanService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="PingException"></exception>
    Task<bool> PingAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Scans file contained in a single memory stream.
    /// </summary>
    /// <param name="sourceStream"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<VirusScanResult> ScanFileAsync(Stream sourceStream, CancellationToken cancellationToken);

    /// <summary>
    /// Scans a file contained in a chunked file. Used for large files. Prevents buffering 
    /// the entire file into memory. Each chunk is streamed to the ClamAV server
    /// </summary>
    /// <param name="sourceSection"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<VirusScanResult> ScanFileAsync(IAsyncEnumerable<Stream> sourceSection, CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="VersionException"></exception>
    /// <returns></returns>
    Task<GetVersionResult> GetVersionAsync(CancellationToken cancellationToken);
}
