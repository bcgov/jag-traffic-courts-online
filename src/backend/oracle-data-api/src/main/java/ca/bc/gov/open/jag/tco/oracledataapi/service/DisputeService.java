package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.util.ArrayList;
import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketCount;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;

@Service
public class DisputeService {

	@Autowired
	DisputeRepository disputeRepository;

	/**
	 * Retrieves all {@link Dispute} records, delegating to CrudRepository
	 *
	 * @return
	 */
	public List<Dispute> getAllDisputes() {
		List<Dispute> disputes = new ArrayList<Dispute>();
		disputeRepository.findAll().forEach(dispute -> disputes.add(dispute));
		return disputes;
	}

	/**
	 * Retrieves a specific {@link Dispute} by using the method findById() of CrudRepository
	 *
	 * @param id of the Dispute to be returned
	 * @return
	 */
	public Dispute getDisputeById(Integer id) {
		return disputeRepository.findById(id).get();
	}

	/**
	 * Create a new {@link Dispute} by using the method save() of CrudRepository
	 *
	 * @param dispute to be saved
	 */
	public void save(Dispute dispute) {
		// Ensure a new record is created, not updating an existing record. Updates are controlled by specific endpoints.
		dispute.setId(null);
		for (TicketCount ticketCount : dispute.getTicketCounts()) {
			ticketCount.setId(null);
		}
		disputeRepository.save(dispute);
	}

	/**
	 * Deletes a specific {@link Dispute} by using the method deleteById() of CrudRepository
	 *
	 * @param id of the dispute to be deleted
	 */
	public void delete(Integer id) {
		disputeRepository.deleteById(id);
	}

	/**
	 * Updates the status of a specific {@link Dispute}
	 *
	 * @param id
	 * @param disputeStatus
	 */
	public void setStatus(Integer id, DisputeStatus disputeStatus) {
		setStatus(id, disputeStatus, null);
	}

	/**
	 * Updates the status of a specific {@link Dispute}
	 *
	 * @param id
	 * @param disputeStatus
	 * @param rejectedReason the rejected reason if the status is REJECTED.
	 */
	public void setStatus(Integer id, DisputeStatus disputeStatus, String rejectedReason) {
		Dispute dispute = disputeRepository.findById(id).orElseThrow();
		dispute.setStatus(disputeStatus);
		if (DisputeStatus.REJECTED.equals(disputeStatus)) {
			dispute.setRejectedReason(rejectedReason);
		}
		disputeRepository.save(dispute);
	}

}
