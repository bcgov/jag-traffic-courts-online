namespace TrafficCourts.Citizen.Service.Services
{
    /// <summary>
    /// Contains the configuration for where in S3 our data should be saved.
    /// </summary>
    public class MinioConfiguration
    {
        public string BucketName { get; set; }
        public string Location { get; set; }
    }



}
