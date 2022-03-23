using Minio;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Features.FilePersistence;
/// <summary>
/// Contains the configuration for <see cref="MinioClient"/>
/// </summary>
[ExcludeFromCodeCoverage]
public class MinioClientConfiguration
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Required]
    public string Endpoint { get; set; }
    [Required]
    public string AccessKey { get; set; }
    [Required]
    public string SecretKey { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

