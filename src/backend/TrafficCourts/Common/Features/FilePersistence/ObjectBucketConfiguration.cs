using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Common.Features.FilePersistence;

[ExcludeFromCodeCoverage]
public class ObjectBucketConfiguration : IValidatable
{
    /// <summary>
    /// Same as <see cref="MinioClientConfiguration.Section"/>
    /// </summary>
    public const string Section = MinioClientConfiguration.Section;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string BucketName { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(BucketName)) throw new SettingsValidationException(Section, nameof(BucketName), "is required");
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

