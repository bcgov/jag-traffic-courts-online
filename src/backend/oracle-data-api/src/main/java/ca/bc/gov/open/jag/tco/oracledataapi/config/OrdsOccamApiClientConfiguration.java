package ca.bc.gov.open.jag.tco.oracledataapi.config;

import org.springframework.boot.autoconfigure.condition.ConditionalOnMissingBean;
import org.springframework.boot.context.properties.EnableConfigurationProperties;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.AuditLogEntryApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.DisputeUpdateRequestApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.HealthApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.LookupValuesApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.OutgoingEmailApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.ViolationTicketApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.ApiClient;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.auth.HttpBasicAuth;

@Configuration
@EnableConfigurationProperties(OrdsConfigProperties.class)
public class OrdsOccamApiClientConfiguration {

	@Bean({ "ordsOccamApiClient" })
	@ConditionalOnMissingBean(ApiClient.class)
	public ApiClient apiClient(OrdsConfigProperties ordsConfigProperties) {
		ApiClient apiClient = new ApiClient();
		// Setting this to null will make it use the baseUrl instead
		apiClient.setServerIndex(null);
		apiClient.setBasePath(ordsConfigProperties.getOrdsRestApiOccamUrl());

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
	public HealthApi ordsOccamHealthApi(ApiClient ordsOccamApiClient) {
		return new HealthApi(ordsOccamApiClient);
	}

	@Bean
	public ViolationTicketApi violationTicketApi(ApiClient ordsOccamApiClient) {
		return new ViolationTicketApi(ordsOccamApiClient);
	}

	@Bean
	public LookupValuesApi lookupValuesApi(ApiClient ordsOccamApiClient) {
		return new LookupValuesApi(ordsOccamApiClient);
	}

	@Bean
	public DisputeUpdateRequestApi disputeUpdateRequestApi(ApiClient ordsOccamApiClient) {
		return new DisputeUpdateRequestApi(ordsOccamApiClient);
	}

	@Bean
	public AuditLogEntryApi auditLogEntryApi(ApiClient ordsOccamApiClient) {
		return new AuditLogEntryApi(ordsOccamApiClient);
	}

	@Bean
	public OutgoingEmailApi outgoingEmailApi(ApiClient ordsOccamApiClient) {
		return new OutgoingEmailApi(ordsOccamApiClient);
	}

}
