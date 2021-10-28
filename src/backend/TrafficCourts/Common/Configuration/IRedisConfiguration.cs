using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCourts.Common.Configuration
{
    public interface IRedisConfiguration
    {
        RedisConfigurationProperties? Redis { get; set; }
    }

    public class RedisConfigurationProperties
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
    }
}
