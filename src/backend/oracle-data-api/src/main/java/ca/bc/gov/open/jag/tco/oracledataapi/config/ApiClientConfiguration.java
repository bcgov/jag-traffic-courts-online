package ca.bc.gov.open.jag.tco.oracledataapi.config;

import org.springframework.boot.autoconfigure.condition.ConditionalOnMissingBean;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import ca.bc.gov.open.jag.tco.oracledataapi.api.LookupValuesApi;
import ca.bc.gov.open.jag.tco.oracledataapi.api.ViolationTicketApi;
import ca.bc.gov.open.jag.tco.oracledataapi.api.handler.ApiClient;

@Configuration
@EnableConfigurationProperties(ConfigProperties.class)
public class ApiClientConfiguration {

	@Bean({"ordsApiClient"})
    @ConditionalOnMissingBean(ApiClient.class)
    public ApiClient apiClient(ConfigProperties configProperties)  {
        ApiClient apiClient = new ApiClient();
        //Setting this to null will make it use the baseUrl instead
        apiClient.setServerIndex(null);
        apiClient.setBasePath(configProperties.getOrdsRestApiUrl());

        // Set to true to see actual request/response messages sent/received from ORDs.
        apiClient.setDebugging(false);

        return apiClient;
    }

	@Bean
	public ViolationTicketApi violationTicketApi(ApiClient ordsApiClient) {
		return new ViolationTicketApi(ordsApiClient);
	}

	@Bean
	public LookupValuesApi lookupValuesApi(ApiClient ordsApiClient) {
		return new LookupValuesApi(ordsApiClient);
	}

}
