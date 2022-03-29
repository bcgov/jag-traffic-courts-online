using TrafficCourts.Ticket.Search.Service.Logging;

namespace TrafficCourts.Ticket.Search.Service.Features.Search.Mock
{
    public class FileMockDataProvider : IMockDataProvider
    {
        private const string ConfigurationKey = "MOCK_DATA_PATH";
        private readonly IConfiguration _configuration;
        private readonly ILogger<FileMockDataProvider> _logger;

        public FileMockDataProvider(IConfiguration configuration, ILogger<FileMockDataProvider> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public static bool HasValidConfiguration(IConfiguration configuration)
        {
            string mockDataPath = configuration[ConfigurationKey];
            return !string.IsNullOrEmpty(mockDataPath) && File.Exists(mockDataPath);
        }

        public Stream? GetDataStream()
        {
            using var activity = Diagnostics.Source.StartActivity("Get Mock File Data");

            string mockDataPath = _configuration[ConfigurationKey];

            if (string.IsNullOrEmpty(mockDataPath))
            {
                _logger.LogInformation("{EnvironmentVariable} is not configured", ConfigurationKey);
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, $"Environment variable {ConfigurationKey} empty");
                return null;
            }

            if (!File.Exists(mockDataPath))
            {
                _logger.LogInformation("Mock data not found, expected {Filename} to exist", mockDataPath);
                activity?.SetStatus(System.Diagnostics.ActivityStatusCode.Error, $"File {mockDataPath} not found");
                return null;
            }

            return File.OpenRead(mockDataPath);
        }
    }
}
