using Minio;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Features.FilePersistence;

[ExcludeFromCodeCoverage]
public class ObjectBucketConfiguration
{
    [Required]
    public string BucketName { get; set; }
}
/// <summary>
/// Contains the configuration for <see cref="MinioClient"/>
/// </summary>
[ExcludeFromCodeCoverage]
public class MinioClientConfiguration
{
    [Required]
    public string Endpoint { get; set; }
    [Required]
    public string AccessKey { get; set; }
    [Required]
    public string SecretKey { get; set; }
}
