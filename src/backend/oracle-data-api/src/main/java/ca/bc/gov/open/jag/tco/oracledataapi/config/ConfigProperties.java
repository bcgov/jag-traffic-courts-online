package ca.bc.gov.open.jag.tco.oracledataapi.config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.stereotype.Component;

import lombok.Getter;
import lombok.Setter;

/**
 * Externalized configuration for easy access to properties
 * 
 * @author 237563
 *
 */
@Component
@ConfigurationProperties
@Getter
@Setter
public class ConfigProperties {
	
	// ORDS Service properties
	@Value("${ords.rest-api.url}")
    private String ordsRestApiUrl;
    
    @Value("${ords.rest-api.timeout}")
    private int ordsRestApiTimeout;
    
    @Value("${ords.rest-api.retry.count}")
    private long ordsRestApiRetryCount;
    
    @Value("${ords.rest-api.retry.delay}")
    private long ordsRestApiRetryDelay;
}
