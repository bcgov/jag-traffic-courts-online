package ca.bc.gov.open.jag.tco.ocr.metrics.config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import __occam_package_.api.TicketsApi;
import __occam_package_.api.handler.ApiClient;

@Configuration
public class ApplicationConfig {

	@Bean
	public ApiClient apiClient() {
		ApiClient apiClient = new ApiClient();
		// Setting this to null will make it use the baseUrl instead
		apiClient.setServerIndex(null);
		apiClient.setBasePath("http://localhost:5000");
		return apiClient;
	}

	@Bean
	public TicketsApi ticketApi(ApiClient apiClient) {
		return new TicketsApi(apiClient);
	}

}
