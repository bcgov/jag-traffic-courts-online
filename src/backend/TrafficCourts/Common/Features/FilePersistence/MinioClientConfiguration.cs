using Minio;
using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Features.FilePersistence;

/// <summary>
/// Contains the configuration for <see cref="MinioClient"/>
/// </summary>
[ExcludeFromCodeCoverage]
public class MinioClientConfiguration
{
    public string Endpoint { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
}
