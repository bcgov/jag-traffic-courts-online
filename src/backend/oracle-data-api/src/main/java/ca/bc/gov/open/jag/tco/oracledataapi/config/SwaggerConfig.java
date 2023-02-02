package ca.bc.gov.open.jag.tco.oracledataapi.config;

import org.springdoc.core.GroupedOpenApi;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import io.swagger.v3.oas.models.Components;
import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.info.Info;
import io.swagger.v3.oas.models.security.SecurityRequirement;
import io.swagger.v3.oas.models.security.SecurityScheme;

@Configuration
public class SwaggerConfig {

	/**
	 * Group all APIs with `v1.0` in the path
	 */
	@Bean
	public GroupedOpenApi v1_0Apis() {
		return GroupedOpenApi.builder()
				.group("v1.0").pathsToMatch("/**/api/v1.0/**")
				.build();
	}

	@Bean
	public OpenAPI customOpenAPI() {
		return new OpenAPI()
				.info(new Info().title("Oracle Data Api").version("1.0.0"))

				// Create an x-username header that can be set in Swagger via the "Authorize" button.
                .components(new Components()
                        .addSecuritySchemes("x-username", new SecurityScheme()
                                .type(SecurityScheme.Type.APIKEY)
                                .in(SecurityScheme.In.HEADER)
                                .name("x-username")))

                // AddSecurityItem section applies created scheme globally
                .addSecurityItem(new SecurityRequirement().addList("x-username"));
	}
}
