package ca.bc.gov.open.jag.tco.oracledataapi.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.annotation.web.configuration.WebSecurityConfigurerAdapter;

@Configuration
@EnableWebSecurity
public class WebSecurityConfig extends WebSecurityConfigurerAdapter {

	@Override
	protected void configure(HttpSecurity httpSecurity) throws Exception {
		// Since this app is never exposed publicly, security is not needed. However, by enabling security, the Principal object (logged-in user) is
		// available to the AuditorAware and Controller classes. The user doesn't actually login, but rather the username is passed as a value of the
		// "x-username" header on all requests. This custom header is set by the calling staff-api project.
		httpSecurity.authorizeRequests().anyRequest().permitAll();
		httpSecurity.cors().and().csrf().disable();

		// Needed to allow access to /h2-console
		httpSecurity.headers().frameOptions().disable();
	}

}
