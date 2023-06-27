package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.h2;

import java.util.List;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.transaction.Transactional;

import org.hibernate.Session;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicket;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicketCount;

public class DisputeRepositoryCustomImpl implements DisputeRepositoryCustom {

	@PersistenceContext
	private EntityManager entityManager;

	@Override
	@Transactional
	public void flushAndClear() {
		entityManager.flush();
		entityManager.clear();
	}

	@Override
	public Dispute update(Dispute dispute) {
		Session session = entityManager.unwrap(Session.class);
		ViolationTicket mergedTicket = null;

		if (dispute.getViolationTicket() != null && dispute.getViolationTicket().getViolationTicketCounts() != null) {
			List<ViolationTicketCount> mergedTicketCounts = dispute.getViolationTicket().getViolationTicketCounts();
			for (ViolationTicketCount violationTicketCount : mergedTicketCounts) {
				ViolationTicketCount mergedCount = (ViolationTicketCount) session.merge(violationTicketCount);
				mergedCount.setViolationTicket(dispute.getViolationTicket());
			}
			mergedTicket = (ViolationTicket) session.merge(dispute.getViolationTicket());
		} else if (dispute.getViolationTicket() != null) {
			mergedTicket = (ViolationTicket) session.merge(dispute.getViolationTicket());
		}

		dispute.setViolationTicket(mergedTicket);
		Dispute managedDispute = (Dispute) session.merge(dispute);
		session.saveOrUpdate(managedDispute);
		entityManager.flush();
		// We need to refresh the state of the instance from the database in order to return the fully updated object after persistence
		entityManager.refresh(dispute);
		return dispute;
	}

}
