package ca.bc.gov.open.jag.tco.oracledataapi.config;

import java.time.Duration;

import org.apache.commons.lang3.StringUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.autoconfigure.data.redis.RedisProperties;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.data.redis.connection.RedisClusterConfiguration;
import org.springframework.data.redis.connection.RedisSentinelConfiguration;
import org.springframework.data.redis.connection.RedisStandaloneConfiguration;
import org.springframework.data.redis.connection.lettuce.LettuceClientConfiguration;
import org.springframework.data.redis.connection.lettuce.LettuceConnectionFactory;
import org.springframework.util.CollectionUtils;

import io.lettuce.core.cluster.ClusterClientOptions;
import io.lettuce.core.cluster.ClusterTopologyRefreshOptions;

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
		// Cluster mode (for test and production)
		else if (redisProperties.getCluster() != null && !CollectionUtils.isEmpty(redisProperties.getCluster().getNodes())) {
			logger.debug("Configuring Redis to run in cluster mode.");
			RedisClusterConfiguration config = new RedisClusterConfiguration(redisProperties.getCluster().getNodes());
			config.setPassword(redisProperties.getPassword());
			
			if(redisProperties.getCluster().getMaxRedirects() != null)
				config.setMaxRedirects(redisProperties.getCluster().getMaxRedirects());
			
			// Enable adaptive topology refreshing
			ClusterClientOptions clientOptions = ClusterClientOptions.builder()
				    .topologyRefreshOptions(ClusterTopologyRefreshOptions.builder()
			    		// The MOVED_REDIRECT trigger causes a topology refresh when the client receives a MOVED redirect from a Redis server.
			            // The PERSISTENT_RECONNECTS trigger causes a topology refresh when the client cannot reconnect to a Redis server for a certain period of time.
				        .enableAdaptiveRefreshTrigger(ClusterTopologyRefreshOptions.RefreshTrigger.MOVED_REDIRECT, ClusterTopologyRefreshOptions.RefreshTrigger.PERSISTENT_RECONNECTS)
				        // If a topology refresh is triggered, the client will wait for this period of time before triggering another topology refresh.
				        .adaptiveRefreshTriggersTimeout(Duration.ofSeconds(30)) // default is 30 seconds
				        .build())
				    .build();

	        LettuceClientConfiguration clientConfig = LettuceClientConfiguration.builder()
	                .clientOptions(clientOptions)
	                .build();

	        return new LettuceConnectionFactory(config, clientConfig);
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
