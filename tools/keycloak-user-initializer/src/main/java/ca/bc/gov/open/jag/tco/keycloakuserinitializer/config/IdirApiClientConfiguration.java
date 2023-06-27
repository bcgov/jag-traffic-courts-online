package ca.bc.gov.open.jag.tco.keycloakuserinitializer.config;

import org.springframework.boot.autoconfigure.condition.ConditionalOnMissingBean;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import ca.bc.gov.open.jag.tco.keycloakuserinitializer.idir.api.UsersApi;
import ca.bc.gov.open.jag.tco.keycloakuserinitializer.idir.api.handler.ApiClient;
import ca.bc.gov.open.jag.tco.keycloakuserinitializer.idir.api.handler.auth.OAuth;

@Configuration
@EnableConfigurationProperties(IdirApiClientConfigProperties.class)
public class IdirApiClientConfiguration {
	
	@Bean({ "idirApiClient" })
	@ConditionalOnMissingBean(ApiClient.class)
	public ApiClient apiClient(IdirApiClientConfigProperties idirApiClientConfigProperties) {
		ApiClient apiClient = new ApiClient();

		// Set OAuth authentication with client credentials
		OAuth authentication = (OAuth) apiClient.getAuthentication("oAuth2ClientCredentials");
		authentication.setCredentials(idirApiClientConfigProperties.getIdirApiClientId(), 
				idirApiClientConfigProperties.getIdirApiClientSecret(), 
				true);

		// Set to true to see actual request/response messages sent/received from the API.
		apiClient.setDebugging(idirApiClientConfigProperties.isIdirApiDebug());

		return apiClient;
	}
	
	@Bean
	public UsersApi IdirUsersApi(ApiClient idirApiClient) {
		return new UsersApi(idirApiClient);
	}
}
