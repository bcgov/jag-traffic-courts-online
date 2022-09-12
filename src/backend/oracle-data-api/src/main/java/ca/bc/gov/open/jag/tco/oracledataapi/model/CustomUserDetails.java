package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Collection;

import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.userdetails.User;

/**
 * Custom user object which can be used as Principal to provide authenticated user details
 * 
 * @author 237563
 *
 */
public class CustomUserDetails extends User {

	private static final long serialVersionUID = 1L;
	
	private String fullName;
	
	public CustomUserDetails(String username, String password, String fullName, Collection<? extends GrantedAuthority> authorities) {
		super(username, password, authorities);
		this.fullName = fullName;
	}

	public String getFullName() {
		return fullName;
	}

	public void setFullName(String fullName) {
		this.fullName = fullName;
	}
	
}
