package ca.bc.gov.open.jag.tco.keycloakuserinitializer.config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.stereotype.Component;

import lombok.Getter;
import lombok.Setter;

/**
 * Externalized IDIR API client configuration properties
 * 
 * @author 237563
 *
 */
@Component
@ConfigurationProperties
@Getter
@Setter
public class IdirApiClientConfigProperties {

	@Value("${idir.api.client.id}")
	private String idirApiClientId;
	
	@Value("${idir.api.client.secret}")
	private String idirApiClientSecret;
	
	@Value("${idir.api.debug}")
	private boolean idirApiDebug;
}
