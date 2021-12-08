namespace TrafficCourts.Common.Configuration
{
    public interface IRedisConfiguration
    {
        RedisConfigurationProperties? Redis { get; set; }
    }
}
