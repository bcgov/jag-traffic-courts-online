package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeRemark;

public interface JJDisputeRemarkRepository {

	/**
	 * Saves a given entity. Use the returned instance for further operations as the save operation might have changed the entity instance completely.
	 *
	 * @param entity must not be {@literal null}.
	 * @return the saved entity; will never be {@literal null}.
	 * @throws IllegalArgumentException in case the given {@literal entity} is {@literal null}.
	 */
	public JJDisputeRemark saveAndFlush(JJDisputeRemark entity);

}
