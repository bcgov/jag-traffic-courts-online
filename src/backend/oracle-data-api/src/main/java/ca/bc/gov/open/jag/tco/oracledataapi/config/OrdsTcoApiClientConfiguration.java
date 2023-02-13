package ca.bc.gov.open.jag.tco.oracledataapi.config;

import org.springframework.boot.autoconfigure.condition.ConditionalOnMissingBean;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.HealthApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.JjDisputeApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.handler.ApiClient;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.handler.auth.HttpBasicAuth;

@Configuration
@EnableConfigurationProperties(OrdsConfigProperties.class)
public class OrdsTcoApiClientConfiguration {

	@Bean({ "ordsTcoApiClient" })
	@ConditionalOnMissingBean(ApiClient.class)
	public ApiClient apiClient(OrdsConfigProperties ordsConfigProperties) {
		ApiClient apiClient = new ApiClient();
		// Setting this to null will make it use the baseUrl instead
		apiClient.setServerIndex(null);
		apiClient.setBasePath(ordsConfigProperties.getOrdsRestApiTcoUrl());

		if(ordsConfigProperties.isOrdsBasicAuthEnabled()) {
			// Set basic auth header with credentials
			HttpBasicAuth authentication = (HttpBasicAuth) apiClient.getAuthentication("basicAuth");
			authentication.setUsername(ordsConfigProperties.getOrdsRestApiUsername());
			authentication.setPassword(ordsConfigProperties.getOrdsRestApiPassword());
		}

		// Set to true to see actual request/response messages sent/received from ORDs.
		apiClient.setDebugging(ordsConfigProperties.isOrdsRestApiDebug());

		return apiClient;
	}

	@Bean
	public HealthApi ordsTcoHealthApi(ApiClient ordsTcoApiClient) {
		return new HealthApi(ordsTcoApiClient);
	}

	@Bean
	public JjDisputeApi ordsTcoJjDisputeApi(ApiClient ordsTcoApiClient) {
		return new JjDisputeApi(ordsTcoApiClient);
	}

}
