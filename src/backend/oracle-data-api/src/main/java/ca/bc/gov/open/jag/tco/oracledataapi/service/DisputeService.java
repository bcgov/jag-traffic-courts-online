package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.security.Principal;
import java.util.Date;
import java.util.List;
import java.util.Optional;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.transaction.Transactional;

import org.apache.commons.collections4.CollectionUtils;
import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.error.NotAllowedException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicketCount;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;

@Service
public class DisputeService {

	private Logger logger = LoggerFactory.getLogger(DisputeService.class);

	@Autowired
	DisputeRepository disputeRepository;

	@PersistenceContext
    private EntityManager entityManager;

	/**
	 * Retrieves all {@link Dispute} records, delegating to CrudRepository
	 * @param olderThan if specified, will filter the result set to those older than this date.
	 *
	 * @return
	 */
	public List<Dispute> getAllDisputes(Date olderThan, DisputeStatus excludeStatus) {
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
	public Dispute save(Dispute dispute) {
		// Ensure a new record is created, not updating an existing record. Updates are controlled by specific endpoints.
		dispute.setDisputeId(null);
		for (DisputeCount disputeCount : dispute.getDisputeCounts()) {
			disputeCount.setDisputeCountId(null);
		}
		if (dispute.getViolationTicket() != null) {
			dispute.getViolationTicket().setViolationTicketId(null);
			for (ViolationTicketCount violationTicketCount : dispute.getViolationTicket().getViolationTicketCounts()) {
				violationTicketCount.setViolationTicketCountId(null);
			}
		}
		return disputeRepository.saveAndFlush(dispute);
	}

	/**
	 * Updates the properties of a specific {@link Dispute}
	 *
	 * @param id
	 * @param {@link Dispute}
	 * @return
	 */
	@Transactional
	public Dispute update(Long id, Dispute dispute) {
		Dispute disputeToUpdate = disputeRepository.findById(id).orElseThrow();

		BeanUtils.copyProperties(dispute, disputeToUpdate, "createdBy", "createdTs", "disputeId", "disputeCounts");
		// Remove all existing dispute counts that are associated to this dispute
		if (disputeToUpdate.getDisputeCounts() != null) {
			disputeToUpdate.getDisputeCounts().clear();
		}
		// Add updated ticket counts
		disputeToUpdate.addDisputeCounts(dispute.getDisputeCounts());

		Dispute updatedDispute = disputeRepository.saveAndFlush(disputeToUpdate);
		// We need to refresh the state of the instance from the database in order to return the fully updated object after persistance
		entityManager.refresh(updatedDispute);

		return updatedDispute;
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

		disputeRepository.setStatus(id, disputeStatus, DisputeStatus.REJECTED.equals(disputeStatus) ? rejectedReason : null);
		disputeRepository.flushAndClear();
		return disputeRepository.findById(id).orElseThrow();
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

		if (StringUtils.isBlank(dispute.getUserAssignedTo()) || dispute.getUserAssignedTo().equals(principal.getName())) {

			disputeRepository.assignDisputeToUser(id, principal.getName());
			disputeRepository.flushAndClear();

			logger.debug("Dispute with id {} has been assigned to {}", id, principal.getName());

			return true;
		}

		return false;
	}

	/**
	 * Unassigns all Disputes whose assignedTs is older than 1 hour ago, resetting the assignedTo and assignedTs fields.
	 */
	public void unassignDisputes() {
		disputeRepository.unassignDisputes(DateUtils.addHours(new Date(), -1));
	}

	/**
	 * Flips the Dispute.emailAddressVerified flag to true where on the target dispute if it exists
	 * @param token the Dispute record to update
	 */
	public boolean verifyEmail(Long id) {
		Optional<Dispute> findDispute = disputeRepository.findById(id);

		if (findDispute.isPresent()) {
			Dispute dispute = findDispute.get();
			dispute.setEmailAddressVerified(Boolean.TRUE);
			disputeRepository.save(dispute);
			return true;
		}

		return false;
	}

	/**
	 * Finds a Dispute by noticeOfDisputeId (UUID) or null if not found.
	 */
	public Dispute getDisputeByNoticeOfDisputeId(String noticeOfDisputeId) {
		List<Dispute> findByNoticeOfDisputeId = disputeRepository.findByNoticeOfDisputeId(noticeOfDisputeId);
		if (CollectionUtils.isEmpty(findByNoticeOfDisputeId)) {
			String msg = String.format("Dispute could not be found with noticeOfDisputeId: {1}", noticeOfDisputeId);
			logger.error(msg);
			return null;
		}
		return findByNoticeOfDisputeId.get(0);
	}

	/**
	 * Finds all records that match by Dispute.ticketNumber and the time portion of the Dispute.issuedDate.
	 */
	public List<DisputeResult> findDispute(String ticketNumber, Date issuedTime) {
		return disputeRepository.findByTicketNumberAndTime(ticketNumber, issuedTime);
	}

}
