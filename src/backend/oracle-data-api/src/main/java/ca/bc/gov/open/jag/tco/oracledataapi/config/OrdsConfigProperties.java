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
public class OrdsConfigProperties {

	@Value("${ords.rest-api.occam.url}")
	private String ordsRestApiOccamUrl;

	@Value("${ords.rest-api.tco.url}")
	private String ordsRestApiTcoUrl;

	@Value("${ords.rest-api.basicauth.enabled}")
	private boolean ordsBasicAuthEnabled;

	@Value("${ords.rest-api.basicauth.username}")
	private String ordsRestApiUsername;

	@Value("${ords.rest-api.basicauth.password}")
	private String ordsRestApiPassword;

	@Value("${ords.rest-api.timeout}")
	private int ordsRestApiTimeout;

	@Value("${ords.rest-api.retry.count}")
	private long ordsRestApiRetryCount;

	@Value("${ords.rest-api.retry.delay}")
	private long ordsRestApiRetryDelay;

	@Value("${ords.rest-api.debug}")
	private boolean ordsRestApiDebug;

}
