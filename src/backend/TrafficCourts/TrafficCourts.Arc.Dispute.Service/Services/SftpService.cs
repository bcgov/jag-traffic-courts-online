using Microsoft.Extensions.Options;
using Renci.SshNet;
using System.Runtime.Serialization;
using TrafficCourts.Arc.Dispute.Service.Configuration;

namespace TrafficCourts.Arc.Dispute.Service.Services
{
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

        public void UploadFile(MemoryStream file, string remoteFilePath)
        {
            ArgumentNullException.ThrowIfNull(file);
            ArgumentNullException.ThrowIfNull(remoteFilePath);

            try
            {
                _client.Connect();

                if (_client.IsConnected && file != null)
                {
                    _client.BufferSize = 32 * 1024;
                    _client.UploadFile(file, remoteFilePath);
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

    [Serializable]
    internal class FileUploadFailedException : Exception
    {
        public FileUploadFailedException(){}
        public FileUploadFailedException(string? message) : base(message) { }
        public FileUploadFailedException(string? message, Exception? innerException) : base(message, innerException) { }
        protected FileUploadFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
