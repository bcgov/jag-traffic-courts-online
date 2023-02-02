package ca.bc.gov.open.jag.tco.oracledataapi.config;

import org.apache.commons.lang3.StringUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.autoconfigure.data.redis.RedisProperties;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.redis.connection.RedisSentinelConfiguration;
import org.springframework.data.redis.connection.RedisStandaloneConfiguration;
import org.springframework.data.redis.connection.lettuce.LettuceConnectionFactory;

@Configuration
public class RedisConfig {

	private Logger logger = LoggerFactory.getLogger(RedisConfig.class);

	@Bean
	public LettuceConnectionFactory redisConnectionFactory(RedisProperties redisProperties) {
		// Normally Spring automatically handles and builds a RedisConnectionFactory, but in seems Sentinel and Standalone
		// modes are mutually exclusive.  If spring.redis.sentinel.master configuration is set (or blank) then Spring auto-configures
		// redis to be in Sentinel mode.  But Sentinel mode does not work for local development.  For development, redis must be
		// in Standalone mode. Here we are manually creating one or the other based on if master is set or not.

		// Sentinel mode (for deployments)
		if (redisProperties.getSentinel() != null && !StringUtils.isBlank(redisProperties.getSentinel().getMaster())) {
			logger.debug("Configuring Redis to run in sentinal mode.");
			RedisSentinelConfiguration config = new RedisSentinelConfiguration();
	        config.master(redisProperties.getSentinel().getMaster());
	        for(String node : redisProperties.getSentinel().getNodes()) {
	            String[] props = node.split(":");
	            config.sentinel(props[0], Integer.parseInt(props[1]));
	        }
			config.setPassword(redisProperties.getPassword());
			return new LettuceConnectionFactory(config);
		}

		// Standalone mode (for local development)
		else {
			logger.debug("Configuring Redis to run in standalone mode.");
			RedisStandaloneConfiguration config = new RedisStandaloneConfiguration(redisProperties.getHost(), redisProperties.getPort());
			config.setPassword(redisProperties.getPassword());
			return new LettuceConnectionFactory(config);
		}
	}

}
