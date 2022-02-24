using Renci.SshNet;
using TrafficCourts.Arc.Dispute.Service.Configuration;

namespace TrafficCourts.Arc.Dispute.Service.Services
{
    public class SftpService : ISftpService
    {
        private readonly ILogger<SftpService> _logger;
        private readonly SftpConfig _config;

        public SftpService(ILogger<SftpService> logger, SftpConfig sftpConfig)
        {
            _logger = logger;
            _config = sftpConfig;
        }

        public void UploadFile(Stream file, string remoteFilePath)
        {
            using var client = new SftpClient(_config.Host, _config.Port == 0 ? 22 : _config.Port, _config.Username, _config.Password);
            try
            {
                client.Connect();

                if (client.IsConnected && file != null)
                {
                    client.BufferSize = 4 * 1024;
                    client.UploadFile(file, remoteFilePath);
                    _logger.LogInformation($"Finished uploading file to [{remoteFilePath}]");
                }
                
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed in uploading file to [{remoteFilePath}]");
            }
            finally
            {
                client.Disconnect();
                client.Dispose();
            }
        }
    }
}
