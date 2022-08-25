using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Workflow.Service.Configuration
{
    public class OracleDataApiConfiguration : IValidatable
    {
        public const string Section = "OracleDataApi";

        public string? BaseUrl { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(BaseUrl)) throw new SettingsValidationException(Section, nameof(BaseUrl), "is required");
            if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out _)) throw new SettingsValidationException(Section, nameof(BaseUrl), "is not a valid uri");
        }
    }
}
