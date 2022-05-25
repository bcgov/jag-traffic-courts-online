package ca.bc.gov.open.jag.tco.oracledataapi.filters;

import java.io.IOException;

import javax.servlet.Filter;
import javax.servlet.FilterChain;
import javax.servlet.ServletException;
import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;
import javax.servlet.http.HttpServletRequest;

import org.springframework.core.annotation.Order;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Component;

import ca.bc.gov.open.jag.tco.oracledataapi.security.PreAuthenticatedToken;

/**
 * This filter will extract the "x-username" header from a request, and if it exists, set the logged-in user to the value of the header element. The
 * logged in user defaults to "System" if the header is not available.
 */
@Component
@Order(1)
public class PreAuthenticatedTokenFilter implements Filter {

	@Override
	public void doFilter(ServletRequest request, ServletResponse response, FilterChain chain) throws IOException, ServletException {
		// Extract username from a custom header added via the staff-api (this user has already been authenticated by the staff-api using keycloak)
		HttpServletRequest httpRequest = (HttpServletRequest) request;
		String username = httpRequest.getHeader("x-username");

		// If the username is null, default to "System"
		Authentication authentication = new PreAuthenticatedToken(username == null ? "System" : username);
		SecurityContextHolder.getContext().setAuthentication(authentication);

		chain.doFilter(request, response);
	}

}
