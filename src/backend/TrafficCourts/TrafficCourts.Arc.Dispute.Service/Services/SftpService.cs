using Renci.SshNet;

namespace TrafficCourts.Arc.Dispute.Service.Services;

public class SftpService : ISftpService
{
    private readonly ILogger<SftpService> _logger;
    private readonly SftpClient _client;

    public SftpService(ILogger<SftpService> logger, SftpClient client)
    {
        _logger = logger;
        _client = client;
    }

    public void Dispose()
    {
        _client.Dispose();
    }

    public void UploadFile(MemoryStream data, string path, string filename)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(filename);

        string remoteFilePath = Path.Combine(path, filename);

        try
        {
            _client.Connect();

            if (_client.IsConnected)
            {
                _client.BufferSize = 32 * 1024;
        
                if (!_client.Exists(path))
                {
                    _logger.LogInformation("Remote {Directory} does not exist, creating", path);
                    _client.CreateDirectory(path);
                }

                _client.ChangeDirectory(path);
                _client.UploadFile(data, filename);
                _logger.LogDebug("Finished uploading file to [{RemoteFilePath}]", remoteFilePath);
            }                
        }
        catch (Renci.SshNet.Common.SftpPermissionDeniedException exception)
        {
            _logger.LogError(exception, "Operation permission denied for uploading file to [{RemoteFilePath}]", remoteFilePath);
            throw new FileUploadFailedException($"Operation permission denied for uploading file to [{remoteFilePath}]. Please check if you have write access.", exception);
        }
        catch (Renci.SshNet.Common.SshAuthenticationException exception)
        {
            _logger.LogError(exception, "Authentication failed for uploading file to [{RemoteFilePath}]", remoteFilePath);
            throw new FileUploadFailedException($"Authentication failed for uploading file to [{remoteFilePath}]", exception);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed in uploading file to [{RemoteFilePath}]", remoteFilePath);
            throw new FileUploadFailedException($"Failed in uploading file to [{remoteFilePath}]", exception);
        }
        finally
        {
            _client.Disconnect();
        }
    }
}
