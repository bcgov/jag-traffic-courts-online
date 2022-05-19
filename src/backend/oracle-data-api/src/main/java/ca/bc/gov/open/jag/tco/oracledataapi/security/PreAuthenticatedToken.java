package ca.bc.gov.open.jag.tco.oracledataapi.security;

import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;

public class PreAuthenticatedToken extends UsernamePasswordAuthenticationToken {

	private static final long serialVersionUID = 1L;

	public PreAuthenticatedToken(String username) {
		super(username, username);
	}

	@Override
	public boolean isAuthenticated() {
		return true;
	}

}
