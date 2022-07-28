using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

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

                // Iterates directory levels, testing each level using SftpClient.GetAttributes and create the levels that do not exist.
                // The code has been taken from the following resource:
                // https://stackoverflow.com/questions/36564941/renci-ssh-net-is-it-possible-to-create-a-folder-containing-a-subfolder-that-doe
                if (!_client.Exists(path))
                {
                    _logger.LogInformation("Remote {Directory} does not exist, creating", path);

                    string current = "";

                    if (path[0] == '/')
                    {
                        path = path.Substring(1);
                    }

                    while (!string.IsNullOrEmpty(path))
                    {
                        int p = path.IndexOf('/');
                        current += '/';
                        if (p >= 0)
                        {
                            current += path.Substring(0, p);
                            path = path.Substring(p + 1);
                        }
                        else
                        {
                            current += path;
                            path = "";
                        }

                        try
                        {
                            SftpFileAttributes attrs = _client.GetAttributes(current);
                            if (!attrs.IsDirectory)
                            {
                                throw new Exception("not directory");
                            }
                        }
                        catch (SftpPathNotFoundException)
                        {
                            _client.CreateDirectory(current);
                        }
                    }
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
