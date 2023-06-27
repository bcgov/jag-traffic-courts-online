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

	@Value("${ords.api.occam.url}")
	private String ordsApiOccamUrl;

	@Value("${ords.api.tco.url}")
	private String ordsApiTcoUrl;

	@Value("${ords.api.auth.enabled}")
	private boolean ordsApiAuthEnabled;

	@Value("${ords.api.occam.username}")
	private String ordsApiOccamUsername;

	@Value("${ords.api.occam.password}")
	private String ordsApiOccamPassword;

	@Value("${ords.api.tco.username}")
	private String ordsApiTcoUsername;

	@Value("${ords.api.tco.password}")
	private String ordsApiTcoPassword;

	@Value("${ords.api.timeout}")
	private int ordsApiTimeout;

	@Value("${ords.api.retry.count}")
	private long ordsApiRetryCount;

	@Value("${ords.api.retry.delay}")
	private long ordsApiRetryDelay;

	@Value("${ords.api.debug}")
	private boolean ordsApiDebug;

}
