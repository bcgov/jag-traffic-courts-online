package ca.bc.gov.open.jag.tco.keycloakuserinitializer.config;

import org.keycloak.OAuth2Constants;
import org.keycloak.admin.client.Keycloak;
import org.keycloak.admin.client.KeycloakBuilder;
import org.springframework.boot.autoconfigure.condition.ConditionalOnMissingBean;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
@EnableConfigurationProperties(KeycloakClientConfigProperties.class)
public class KeycloakConfig {
	
	public static String realm;

    @Bean({ "KeycloakClient" })
	@ConditionalOnMissingBean(Keycloak.class)
    protected Keycloak keycloak(KeycloakClientConfigProperties keycloakProperties) {
    	
    	KeycloakConfig.realm = keycloakProperties.getKeycloakRealm();
    	
        Keycloak keycloak = KeycloakBuilder.builder()
        		     .serverUrl(keycloakProperties.getKeycloakServerUrl())
        		     .realm(keycloakProperties.getKeycloakRealm())
        		     .grantType(OAuth2Constants.CLIENT_CREDENTIALS)
        		     .clientId(keycloakProperties.getKeycloakClientId())
        		     .clientSecret(keycloakProperties.getKeycloakClientSecret())
        		     .build();

        return keycloak;
    }
}
