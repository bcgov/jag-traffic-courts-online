package ca.bc.gov.open.jag.tco.keycloakuserinitializer.config;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.context.properties.ConfigurationProperties;
import org.springframework.context.annotation.Primary;
import org.springframework.stereotype.Component;

import lombok.Getter;
import lombok.Setter;

/**
 * Externalized Keycloak client configuration properties
 * 
 * @author 237563
 *
 */
@Component
@ConfigurationProperties
@Primary
@Getter
@Setter
public class KeycloakClientConfigProperties {
	
	@Value("${keycloak.server.url}")
	private String keycloakServerUrl;
	
	@Value("${keycloak.realm}")
	private String keycloakRealm;
	
	@Value("${keycloak.client.id}")
	private String keycloakClientId;
	
	@Value("${keycloak.client.secret}")
	private String keycloakClientSecret;

}
