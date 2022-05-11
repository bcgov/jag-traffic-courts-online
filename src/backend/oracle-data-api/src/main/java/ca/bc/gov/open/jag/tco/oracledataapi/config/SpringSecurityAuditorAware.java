package ca.bc.gov.open.jag.tco.oracledataapi.config;

import java.util.Optional;

import org.springframework.data.domain.AuditorAware;

/**
 * A class to extract the current user of the application. This data is used to auto-populate the createBy and modifiedBy audit fields on entity
 * records.
 */
public class SpringSecurityAuditorAware implements AuditorAware<String> {

	@Override
	public Optional<String> getCurrentAuditor() {
		// Just return a string representing the System default username for now
		// We could get the username from Keycloak, (i.e.
		// https://github.com/bcgov/jag-ai-reviewer/blob/main/src/backend/ai-reviewer-api/src/main/java/ca/bc/gov/open/jag/aireviewerapi/core/SecurityUtils.java)
		return Optional.ofNullable("System").filter(s -> !s.isEmpty());
	}

}
