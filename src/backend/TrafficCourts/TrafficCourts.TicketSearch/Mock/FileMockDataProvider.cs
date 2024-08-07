using TrafficCourts.Configuration.Validation;

namespace TrafficCourts.TicketSearch.Mock
{
    public class FileMockDataSettings : IValidatable
    {
        public string MockDataPath { get; set; } = string.Empty;

        public void Validate()
        {
            string className = nameof(FileMockDataSettings);
            string propertyName = nameof(MockDataPath);

            if (string.IsNullOrEmpty(MockDataPath)) throw new SettingsValidationException(className, propertyName, " is required");
            if (!File.Exists(MockDataPath)) throw new SettingsValidationException(className, propertyName, $" refers to a file that does not exist. MockDataPath={MockDataPath}");
        }
    }

    public class FileMockDataProvider : IMockDataProvider
    {
        private readonly FileMockDataSettings _configuration;

        public FileMockDataProvider(FileMockDataSettings configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Stream? GetDataStream()
        {
            using var activity = Instrumentation.Diagnostics.Source.StartActivity("get mock file data");
            string mockDataPath = _configuration.MockDataPath;
            return File.OpenRead(mockDataPath);
        }
    }
}
