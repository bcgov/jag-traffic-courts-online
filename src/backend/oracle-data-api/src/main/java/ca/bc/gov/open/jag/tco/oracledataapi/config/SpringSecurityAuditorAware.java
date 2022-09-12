package ca.bc.gov.open.jag.tco.oracledataapi.config;

import java.util.Optional;

import org.springframework.data.domain.AuditorAware;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;

import ca.bc.gov.open.jag.tco.oracledataapi.model.CustomUserDetails;

/**
 * A class to extract the current user of the application. This data is used to auto-populate the createdBy and modifiedBy audit fields on entity
 * records.
 */
public class SpringSecurityAuditorAware implements AuditorAware<String> {

	@Override
	public Optional<String> getCurrentAuditor() {
		// The returned String is used to auto-populate the createdBy and modifiedBy fields on Entities.
		// The SecurityContextHolder holds a reference to the currently logged in user attached on the current thread - return that.
		CustomUserDetails principal = null;
		Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
		if (authentication != null) {
			principal = (CustomUserDetails) authentication.getPrincipal();
		}
		return Optional.of(principal == null ? "System" : principal.getUsername());
	}

}
