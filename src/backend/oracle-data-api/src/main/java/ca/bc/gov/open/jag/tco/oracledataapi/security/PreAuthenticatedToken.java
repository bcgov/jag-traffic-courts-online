package ca.bc.gov.open.jag.tco.oracledataapi.security;

import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;

import ca.bc.gov.open.jag.tco.oracledataapi.model.CustomUserDetails;

public class PreAuthenticatedToken extends UsernamePasswordAuthenticationToken {

	private static final long serialVersionUID = 1L;

	public PreAuthenticatedToken(CustomUserDetails user) {
		super(user, null);
	}

	@Override
	public boolean isAuthenticated() {
		return true;
	}

}
