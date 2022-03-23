namespace TrafficCourts.Common.Features.FilePersistence;

/// <summary>
/// Contains the configuration for where in S3 our data should be saved.
/// </summary>
[ExcludeFromCodeCoverage]
public class MinioConfiguration
{
    public string BucketName { get; set; }
    public string Location { get; set; }
}
