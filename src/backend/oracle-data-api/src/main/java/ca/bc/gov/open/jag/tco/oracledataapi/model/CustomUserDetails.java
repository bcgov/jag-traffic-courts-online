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
	private String partId; // JUSTIN participant id
	
	public CustomUserDetails(String username, String password, String fullName, String partId, Collection<? extends GrantedAuthority> authorities) {
		super(username, password, authorities);
		this.fullName = fullName;
		this.partId = partId;
	}

	public String getFullName() {
		return fullName;
	}

	public void setFullName(String fullName) {
		this.fullName = fullName;
	}
	
	public String getPartId() {
		return partId;
	}
	
	public void setPartId(String partId) {
		this.partId = partId;
	}
	
}
