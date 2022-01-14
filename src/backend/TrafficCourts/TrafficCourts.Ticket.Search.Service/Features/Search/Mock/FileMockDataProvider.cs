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
            string mockDataPath = _configuration[ConfigurationKey];

            if (string.IsNullOrEmpty(mockDataPath))
            {
                _logger.LogInformation("{EnvironmentVariable} is not configured", ConfigurationKey);
                return null;
            }

            if (!File.Exists(mockDataPath))
            {
                _logger.LogInformation("Mock data not found, expected {Filename} to exist", mockDataPath);
                return null;
            }

            return File.OpenRead(mockDataPath);
        }
    }
}
