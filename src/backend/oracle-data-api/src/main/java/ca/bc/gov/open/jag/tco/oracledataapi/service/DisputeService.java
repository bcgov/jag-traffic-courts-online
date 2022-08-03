package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.security.Principal;
import java.util.Date;
import java.util.List;
import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.error.NotAllowedException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputedCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicketCount;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;

@Service
public class DisputeService {

	private Logger logger = LoggerFactory.getLogger(DisputeService.class);

	@Autowired
	DisputeRepository disputeRepository;

	/**
	 * Retrieves all {@link Dispute} records, delegating to CrudRepository
	 * @param olderThan if specified, will filter the result set to those older than this date.
	 *
	 * @return
	 */
	public Iterable<Dispute> getAllDisputes(Date olderThan, DisputeStatus excludeStatus) {
		if (olderThan == null && excludeStatus == null) {
			return disputeRepository.findAll();
		} else if (olderThan == null) {
			return disputeRepository.findByStatusNot(excludeStatus);
		} else if (excludeStatus == null) {
			return disputeRepository.findByCreatedTsBefore(olderThan);
		} else {
			return disputeRepository.findByStatusNotAndCreatedTsBefore(excludeStatus, olderThan);
		}
	}

	/**
	 * Retrieves all {@link Dispute} records that are assigned to the provided JJ, delegating to CrudRepository
	 * @param jjAssigned, will filter the result set to those having this jjAssigned.
	 *
	 * @return
	 */
	public List<Dispute> getAllDisputesByJjAssigned(String jjAssigned) {
		return disputeRepository.findByJjAssignedIgnoreCase(jjAssigned);
	}

	/**
	 * Retrieves a specific {@link Dispute} by using the method findById() of CrudRepository
	 *
	 * @param id of the Dispute to be returned
	 * @return
	 */
	public Dispute getDisputeById(Long id) {
		return disputeRepository.findById(id).orElseThrow();
	}

	/**
	 * Create a new {@link Dispute} by using the method save() of CrudRepository
	 *
	 * @param dispute to be saved
	 */
	public void save(Dispute dispute) {
		// Ensure a new record is created, not updating an existing record. Updates are controlled by specific endpoints.
		dispute.setId(null);
		for (DisputedCount disputedCount : dispute.getDisputedCounts()) {
			disputedCount.setId(null);
		}
		if (dispute.getViolationTicket() != null) {
			dispute.getViolationTicket().setId(null);
			for (ViolationTicketCount violationTicketCount : dispute.getViolationTicket().getViolationTicketCounts()) {
				violationTicketCount.setId(null);
			}
		}
		disputeRepository.save(dispute);
	}

	/**
	 * Updates the properties of a specific {@link Dispute}
	 *
	 * @param id
	 * @param {@link Dispute}
	 * @return
	 */
	public Dispute update(Long id, Dispute dispute) {
		Dispute disputeToUpdate = disputeRepository.findById(id).orElseThrow();

		BeanUtils.copyProperties(dispute, disputeToUpdate, "createdBy", "createdTs", "id", "disputedCounts");
		// Remove all existing ticket counts that are associated to this dispute
		if (disputeToUpdate.getDisputedCounts() != null) {
			disputeToUpdate.getDisputedCounts().clear();
		}
		// Add updated ticket counts
		disputeToUpdate.addDisputedCounts(dispute.getDisputedCounts());

		return disputeRepository.save(disputeToUpdate);
	}

	/**
	 * Deletes a specific {@link Dispute} by using the method deleteById() of CrudRepository
	 *
	 * @param id of the dispute to be deleted
	 */
	public void delete(Long id) {
		disputeRepository.deleteById(id);
	}

	/**
	 * Updates the status of a specific {@link Dispute}
	 *
	 * @param id
	 * @param disputeStatus
	 * @return the saved Dispute
	 */
	public Dispute setStatus(Long id, DisputeStatus disputeStatus) {
		return setStatus(id, disputeStatus, null);
	}

	/**
	 * Updates the status of a specific {@link Dispute}
	 *
	 * @param id
	 * @param disputeStatus
	 * @param rejectedReason the rejected reason if the status is REJECTED.
	 * @return the saved Dispute
	 */
	public Dispute setStatus(Long id, DisputeStatus disputeStatus, String rejectedReason) {
		if (disputeStatus == null) {
			logger.error("Attempting to set Dispute status to null - bad method call.");
			throw new NotAllowedException("Cannot set Dispute status to null");
		}

		Dispute dispute = disputeRepository.findById(id).orElseThrow();

		// TCVP-1058 - business rules
		// - current status must be NEW,REJECTED to change to PROCESSING
		// - current status must be NEW to change to REJECTED
		// - current status must be NEW,PROCESSING,REJECTED to change to CANCELLED
		switch (disputeStatus) {
		case PROCESSING:
			if (!List.of(DisputeStatus.NEW, DisputeStatus.REJECTED, DisputeStatus.VALIDATED).contains(dispute.getStatus())) {
				throw new NotAllowedException("Changing the status of a Dispute record from %s to %s is not permitted.", dispute.getStatus(), DisputeStatus.PROCESSING);
			}
			break;
		case CANCELLED:
			if (!List.of(DisputeStatus.NEW, DisputeStatus.PROCESSING, DisputeStatus.REJECTED, DisputeStatus.VALIDATED).contains(dispute.getStatus())) {
				throw new NotAllowedException("Changing the status of a Dispute record from %s to %s is not permitted.", dispute.getStatus(), DisputeStatus.CANCELLED);
			}
			break;
		case REJECTED:
			if (!List.of(DisputeStatus.NEW, DisputeStatus.VALIDATED).contains(dispute.getStatus())) {
				throw new NotAllowedException("Changing the status of a Dispute record from %s to %s is not permitted.", dispute.getStatus(), DisputeStatus.REJECTED);
			}
			break;
		case VALIDATED:
			if (!List.of(DisputeStatus.NEW).contains(dispute.getStatus())) {
				throw new NotAllowedException("Changing the status of a Dispute record from %s to %s is not permitted.", dispute.getStatus(), DisputeStatus.VALIDATED);
			}
			break;
		case NEW:
			// This should never happen since setting the status to NEW should only happen during initial creation of the Dispute record.
			// If we got here, then this means the Dispute record is in an invalid state.
			logger.error("Attempting to set the status of a Dispute record to NEW after it was created - bad object state.");
			throw new NotAllowedException("Changing the status of a Dispute record to %s is not permitted.", DisputeStatus.NEW);
		default:
			// This should never happen, but if so, then it means a new DisputeStatus was added and these business rules were not updated accordingly.
			logger.error("A Dispute record has an unknown status '{}' - bad object state.", dispute.getStatus());
			throw new NotAllowedException("Unknown status of a Dispute record: %s", dispute.getStatus());
		}

		dispute.setStatus(disputeStatus);
		dispute.setRejectedReason(DisputeStatus.REJECTED.equals(disputeStatus) ? rejectedReason : null);
		return disputeRepository.save(dispute);
	}

	/**
	 * Assigns a specific {@link Dispute} to the IDIR username of the Staff with a timestamp
	 *
	 * @param id
	 * @param principal the current user of the system
	 */
	public boolean assignDisputeToUser(Long id, Principal principal) {
		if (principal == null || principal.getName() == null || principal.getName().isEmpty()) {
			logger.error("Attempting to set Dispute to null username - bad method call.");
			throw new NotAllowedException("Cannot set assigned user to null");
		}

		// Find the dispute to be assigned to the username
		Dispute dispute = disputeRepository.findById(id).orElseThrow();

		if (StringUtils.isBlank(dispute.getAssignedTo()) || dispute.getAssignedTo().equals(principal.getName())) {

			dispute.setAssignedTo(principal.getName());
			dispute.setAssignedTs(new Date());
			disputeRepository.save(dispute);

			logger.debug("Dispute with id {} has been assigned to {}", id, principal.getName());

			return true;
		}

		return false;
	}

	/**
	 * Unassigns all Disputes whose assignedTs is older than 1 hour ago, resetting the assignedTo and assignedTs fields.
	 * @return number of records modified.
	 */
	public void unassignDisputes() {
		int count = 0;

		// Find all Disputes with an assignedTs older than 1 hour ago.
		Date hourAgo = DateUtils.addHours(new Date(), -1);
		logger.debug("Unassigning all disputes older than {}", hourAgo.toInstant());
		for (Dispute dispute : disputeRepository.findByAssignedTsBefore(hourAgo)) {
			dispute.setAssignedTo(null);
			dispute.setAssignedTs(null);
			disputeRepository.save(dispute);
			count++;
		}

		logger.debug("Unassigned {} record(s)", count);
	}

}
