using Minio;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Common.Features.FilePersistence;
/// <summary>
/// Contains the configuration for <see cref="MinioClient"/>
/// </summary>
[ExcludeFromCodeCoverage]
public class MinioClientConfiguration : IValidatable
{
    public const string Section = "ObjectStorage";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Required]
    public string Endpoint { get; set; }
    [Required]
    public string AccessKey { get; set; }
    [Required]
    public string SecretKey { get; set; }

    /// <summary>
    /// Connects with HTTPS, defaults to true.
    /// </summary>
    public bool Ssl { get; set; } = true;

    public void Validate()
    {
        if (string.IsNullOrEmpty(Endpoint)) throw new SettingsValidationException(Section, nameof(Endpoint), "is required");
        if (string.IsNullOrEmpty(AccessKey)) throw new SettingsValidationException(Section, nameof(AccessKey), "is required");
        if (string.IsNullOrEmpty(SecretKey)) throw new SettingsValidationException(Section, nameof(SecretKey), "is required");
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

