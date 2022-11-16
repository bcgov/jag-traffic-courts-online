package ca.bc.gov.open.jag.tco.oracledataapi.filters;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import javax.servlet.Filter;
import javax.servlet.FilterChain;
import javax.servlet.ServletException;
import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;
import javax.servlet.http.HttpServletRequest;

import org.springframework.core.annotation.Order;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Component;

import ca.bc.gov.open.jag.tco.oracledataapi.model.CustomUserDetails;
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
		// Extract username and full name from the custom headers added via the staff-api (this user has already been authenticated by the staff-api using keycloak)
		HttpServletRequest httpRequest = (HttpServletRequest) request;
		String username = httpRequest.getHeader("x-username");
		String fullName = httpRequest.getHeader("x-fullName");
		String partId = httpRequest.getHeader("x-partId");
		
		// Add the required authority role
		List<GrantedAuthority> authority = new ArrayList<>();
        authority.add(new SimpleGrantedAuthority("User"));

		// If the username is null, default to "System"
		// If the fullName is null, default to "System"
		CustomUserDetails user = new CustomUserDetails(username == null ? "System" : username, username == null ? "Password" : username, fullName == null ? "System" : fullName, partId == null ? "System" : partId, authority);
		Authentication authentication = new PreAuthenticatedToken(user);
		SecurityContextHolder.getContext().setAuthentication(authentication);

		chain.doFilter(request, response);
	}

}
