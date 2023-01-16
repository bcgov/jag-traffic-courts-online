using System.Buffers;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BCGov.VirusScan.Api.Network;
using BCGov.VirusScan.Api.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Logging;

namespace BCGov.VirusScan.Api.Services;

public sealed class VirusScanService : IVirusScanService
{
    private static byte[] ZeroLengthChunk => "\0\0\0\0"u8.ToArray();
    private static byte[] PingCommand => "zPING\0"u8.ToArray();
    private static byte[] VersionCommand => "zVERSION\0"u8.ToArray();
    private static byte[] InStreamCommand => "zINSTREAM\0"u8.ToArray();
    private static byte[] BeginSessionCommand => "zIDSESSION\0"u8.ToArray();
    private static byte[] EndSessionCommand => "zEND\0"u8.ToArray();

    private readonly string _server = "127.0.0.1";
    private readonly int _port = 3310;

    private readonly ITcpClient _client;
    private readonly ILogger<VirusScanService> _logger;

    public VirusScanService(ITcpClient client, IConfiguration configuration, ILogger<VirusScanService> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // TODO: use better strongly typed configuration
        IConfigurationSection? section = configuration.GetSection("ClamAV");
        if (section is not null)
        {
            var server = section.GetValue<string>("Server");
            if (server is not null)
            {
                _server = server;
            }


            var port = section.GetValue<int?>("Port");
            if (port is not null)
            {
                _port = port.Value;
            }
        }

        _logger.LogDebug("Using ClamAV server {Server}:{Port}", _server, _port);
    }

    public async Task<bool> PingAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Sending ping command to ClamAV");

        await SendCommandAsync(PingCommand, cancellationToken);

        var response = await ReadResponseAsync(cancellationToken);

        _logger.LogDebug("Ping response {Response}", response);

        response = ParsePingResponse(response);

        return response == "PONG";
    }

    public async Task<VirusScanResult> ScanFileAsync(MemoryStream source, CancellationToken cancellationToken)
    {
        await SendCommandAsync(InStreamCommand, cancellationToken);

        NetworkStream stream = _client.GetStream();

        await StreamChunkAsync(stream, source, cancellationToken);

        // write zero length chunk 
        await stream.WriteAsync(ZeroLengthChunk, 0, ZeroLengthChunk.Length, cancellationToken);

        string response = await ReadResponseAsync(cancellationToken);

        VirusScanResult result = ParseScanResponse(response);
        return result;
    }

    public async Task<VirusScanResult> ScanFileAsync(IAsyncEnumerable<MemoryStream> sourceSections, CancellationToken cancellationToken)
    {
        await SendCommandAsync(InStreamCommand, cancellationToken);
        NetworkStream stream = _client.GetStream();

        await foreach (var section in sourceSections)
        {
            await StreamChunkAsync(stream, section, cancellationToken);
        }

        // write zero length chunk 
        await stream.WriteAsync(ZeroLengthChunk, 0, ZeroLengthChunk.Length, cancellationToken);

        string response = await ReadResponseAsync(cancellationToken);

        var result = ParseScanResponse(response);
        return result;

    }

    public async Task<VirusScannerVersion> GetVersionAsync(CancellationToken cancellationToken)
    {
        await SendCommandAsync(VersionCommand, cancellationToken);
        string response = await ReadResponseAsync(cancellationToken);

        VirusScannerVersion version = ParseVersionResponse(response);
        return version;
    }


    private string ParsePingResponse(string response)
    {
        // See: https://github.com/Cisco-Talos/clamav/blob/main/clamd/session.c
        // Response will be one of the following:
        //
        // PONG
        // <connection-id>: PONG
        //
        // The connection-id will be returned if the ping command
        // was issued as part of a IDSESSION group command
        if (response == "PONG" || response.EndsWith(": PONG"))
        {
            return response;
        }

        _logger.LogInformation("Ping response does not end in PONG, actual response is {Response} {Length}", response, response.Length);

        return string.Empty;
    }

    private VirusScanResult ParseScanResponse(string response)
    {
        // see: https://github.com/Cisco-Talos/clamav/blob/main/clamd/scanner.c
        // Response will be one of the following:
        //
        // stream: OK
        // stream: <error> ERROR
        // stream: <virus-name> FOUND
        // stream: <virus-name>(<virus-hash>:<virus-size>) FOUND
        //
        // The virus hash and size will be returned if ExtendedDetectionInfo is enabled,
        // otherwise just the virus name is returned with FOUND.

        // handle the no virus response quickly
        if (response == "stream: OK")
        {
            return new VirusScanResult { Status = VirusScanStatus.NotInfected };
        }

        if (response.EndsWith(" FOUND", StringComparison.OrdinalIgnoreCase))
        {

            return new VirusScanResult { Status = VirusScanStatus.Infected, VirusName = GetVirusName(response) };
        }

        if (response.EndsWith(" ERROR", StringComparison.OrdinalIgnoreCase))
        {
            return new VirusScanResult { Status = VirusScanStatus.Error, Error = GetErrorMessage(response) };
        }

        // unexpected case
        return new VirusScanResult { Status = VirusScanStatus.Error, Error = "Unexpeced response from ClamAV" };
    }

    private VirusScannerVersion ParseVersionResponse(string response)
    {
        // If the engine version is not available, the first response will be returned.
        //
        // ClamAV 1.0.0
        // ClamAV 1.0.0/26778/Wed Jan 11 09:38:05 2023
        var parts = response.Split('/', StringSplitOptions.RemoveEmptyEntries);

        var release = parts[0];
        if (release.StartsWith("ClamAV "))
        {
            release = release[7..];
        }

        string? engine = null;
        DateTime? engineDate = null;

        if (parts.Length > 1)
        {
            engine = parts[1];

            if (parts.Length > 2)
            {
                IFormatProvider? provider = CultureInfo.InvariantCulture;
                DateTimeStyles style = DateTimeStyles.None;

                if (DateTime.TryParseExact(parts[2], "ddd MMM dd HH:mm:ss yyyy", provider, style, out DateTime date))
                {
                    engineDate = date;
                }
            }
        }

        return new VirusScannerVersion(release, engine, engineDate);
    }
    private string GetErrorMessage(string response)
    {
        // stream: <error> ERROR
        var start = "stream: ".Length;
        var fromEnd = " ERROR".Length;

        var error = response[start..-fromEnd];
        return error;
    }

    private string GetVirusName(string response)
    {
        // stream: <virus-name> FOUND
        // stream: <virus-name>(<virus-hash>:<virus-size>) FOUND

        // extract the part between "stream: " and " FOUND"
        var start = "stream: ".Length;
        var fromEnd = " FOUND".Length;

        var virus = response[start..-fromEnd];

        var hashSizeStart = virus.IndexOf('(');
        if (hashSizeStart > 0)
        {
            var end = hashSizeStart - 1;
            return virus[0..end];
        }

        return virus;
    }

    private async ValueTask SendCommandAsync(byte[] command, CancellationToken cancellationToken)
    {
        // client could already be connected if consumer of this service makes multiple calls
        // like scan file and then check for virus scanner version.
        // However, if use case does request doing multiple commands against ClamAV, 
        // The session command should be implemented.
        if (!_client.Connected)
        {
            await _client.ConnectAsync(_server, _port, cancellationToken).ConfigureAwait(false);
        }

        NetworkStream stream = _client.GetStream();
        await stream.WriteAsync(command, 0, command.Length, cancellationToken);
    }

    private async Task<string> ReadResponseAsync(CancellationToken cancellationToken)
    {
        int bufferSize = 1024; // response sizes are very small, 1kb should be enough for all response types

        // get a buffer to read the response into
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

        try
        {
            NetworkStream stream = _client.GetStream();

            int bytes = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

            var length = bytes;

            if (length == buffer.Length)
            {
                // our buffer is too small, this shouldn't happen
                _logger.LogWarning("Command response filled the response buffer, response should be less than {BufferSize} bytes", bufferSize);
            }
            else if (length > 1)
            {
                length--; // remove trailing null that ClamAV adds cause we are using 'z' commands
            }

            string response = Encoding.UTF8.GetString(buffer, 0, length);
            return response;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Sends the <paramref name="source"/> as a ClamAV chunk to the target <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="source"></param>
    /// <param name="cancellationToken"></param>
    private async Task StreamChunkAsync(NetworkStream stream, MemoryStream source, CancellationToken cancellationToken)
    {
        // send the file in 8k chunks (todo: determine if 8k is correct size)
        int bufferSize = 8 * 1024;
        byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);

        try
        {
            // Send the file to be scanned in chunks
            // The stream is sent to clamd in chunks, after INSTREAM,
            // on the same socket on which the command was sent.
            // The format of the chunk is: '<length><data>' where <length>
            // is the size of the following data in bytes expressed as a
            // 4 byte unsigned integer in network byte order and <data> is
            // the actual chunk. Streaming is terminated by sending a zero-length chunk.
            // Note: do not exceed StreamMaxLength as defined in clamd.conf,
            // otherwise clamd will reply with INSTREAM size limit exceeded and close the connection.
            // StreamMaxLength defaults to 10M
            int bytes;
            while ((bytes = await source.ReadAsync(buffer, 0, bufferSize, cancellationToken).ConfigureAwait(false)) > 0)
            {
                _logger.LogDebug("Read: {Bytes} from input stream", bytes);

                // write chunk size
                _logger.LogDebug("Writing chunk size: {ChunkSize}", bytes);
                byte[] chunkSize = HostToNetworkOrder(bytes);
                await stream.WriteAsync(chunkSize, 0, chunkSize.Length, cancellationToken).ConfigureAwait(false);

                // write chunk contents
                _logger.LogDebug("Writing buffer if length: {ChunkSize}", bytes);
                await stream.WriteAsync(buffer, 0, bytes, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <summary>
    /// Gets the bytes of the integer in network byte order.
    /// </summary>
    private static byte[] HostToNetworkOrder(int bytes) => BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bytes));
}
