package ca.bc.gov.open.jag.tco.oracledataapi.config;

import org.springdoc.core.GroupedOpenApi;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class ApplicationConfig {

	/**
	 * Group all APIs with `v1.0` in the path
	 */
	@Bean
	GroupedOpenApi v1_0Apis() {
		return GroupedOpenApi.builder().group("v1.0").pathsToMatch("/**/api/v1.0/**").build();
	}

}
