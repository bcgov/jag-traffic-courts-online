using Minio;

namespace TrafficCourts.Citizen.Service.Services
{
    /// <summary>
    /// Contains the configuration for <see cref="MinioClient"/>
    /// </summary>
    public class MinioClientConfiguration
    {
        public string Endpoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }



}
