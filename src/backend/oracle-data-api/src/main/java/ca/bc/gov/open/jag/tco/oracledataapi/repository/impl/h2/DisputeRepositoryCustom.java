package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.h2;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;

public interface DisputeRepositoryCustom {

	/**
	 * Flushes all pending changes to the database.
	 */
	void flushAndClear();

	/**
	 * Updates the given dispute entity in the database.
	 *
	 * @param dispute entity to be updated. Must not be {@literal null}.
	 * @return the updated entity
	 */
	Dispute update(Dispute dispute);
}
