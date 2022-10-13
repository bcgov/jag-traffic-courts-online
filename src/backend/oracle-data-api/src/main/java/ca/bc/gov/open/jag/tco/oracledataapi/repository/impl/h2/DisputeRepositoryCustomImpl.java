package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.h2;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.transaction.Transactional;

public class DisputeRepositoryCustomImpl implements DisputeRepositoryCustom {

	@PersistenceContext
	private EntityManager entityManager;

	@Override
	@Transactional
	public void flushAndClear() {
		entityManager.flush();
		entityManager.clear();
	}

}
