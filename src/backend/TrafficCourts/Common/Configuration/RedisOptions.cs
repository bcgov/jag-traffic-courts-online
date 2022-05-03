using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Common.Configuration;

[ExcludeFromCodeCoverage]
public class RedisOptions : IValidatable
{
    public const string Section = "Redis";

    public string ConnectionString { get; set; } = "localhost:6379";

    public void Validate()
    {
        if (string.IsNullOrEmpty(ConnectionString)) throw new SettingsValidationException(Section, nameof(ConnectionString), "is required");
    }
}
