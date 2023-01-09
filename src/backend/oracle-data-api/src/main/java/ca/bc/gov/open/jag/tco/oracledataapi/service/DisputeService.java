package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.security.Principal;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.transaction.Transactional;
import javax.validation.ConstraintViolationException;

import org.apache.commons.collections4.CollectionUtils;
import org.apache.commons.lang3.ObjectUtils;
import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.error.NotAllowedException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputantUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicket;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicketCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputantUpdateRequestRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.util.DateUtil;

@Service
public class DisputeService {

	private Logger logger = LoggerFactory.getLogger(DisputeService.class);

	@Autowired
	private DisputeRepository disputeRepository;

	@Autowired
	private JJDisputeRepository jjDisputeRepository;

	@Autowired
	private DisputantUpdateRequestRepository disputantUpdateRequestRepository;

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
			return disputeRepository.findByStatusNotAndCreatedTsBeforeAndNoticeOfDisputeGuid(excludeStatus, olderThan, null);
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

		// FIXME: remove me - this field will get removed from the model entirely as it's not used.
		dispute.setSystemDetectedOcrIssues(YesNo.N);

		for (DisputeCount disputeCount : dispute.getDisputeCounts()) {
			disputeCount.setDisputeCountId(null);
		}
		if (dispute.getViolationTicket() != null) {
			dispute.getViolationTicket().setViolationTicketId(null);
			for (ViolationTicketCount violationTicketCount : dispute.getViolationTicket().getViolationTicketCounts()) {
				violationTicketCount.setViolationTicketCountId(null);
			}

			// It is an error if the duplicate field TicketNumber has different values.
			if (ObjectUtils.compare(dispute.getTicketNumber(), dispute.getViolationTicket().getTicketNumber()) != 0) {
				String msg = String.format("TicketNumber of the Dispute (%s) and ViolationTicket (%s) are different!", dispute.getTicketNumber(), dispute.getViolationTicket().getTicketNumber());
				logger.error(msg);
				throw new ConstraintViolationException(msg, null);
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
		List<DisputeCount> disputeCountsToUpdate = null;
		if (disputeToUpdate.getDisputeCounts() != null) {
			disputeCountsToUpdate = disputeToUpdate.getDisputeCounts();
		}
		ViolationTicket violationTicketToUpdate = null;
		List<ViolationTicketCount> violationTicketCountsToUpdate = null;
		if (disputeToUpdate.getViolationTicket() != null) {
			violationTicketToUpdate = disputeToUpdate.getViolationTicket();
			if (disputeToUpdate.getViolationTicket().getViolationTicketCounts() != null) {
				violationTicketCountsToUpdate = disputeToUpdate.getViolationTicket().getViolationTicketCounts();
			}
		}

		BeanUtils.copyProperties(dispute, disputeToUpdate, "createdBy", "createdTs", "disputeId", "disputeCounts", "violationTicket");
		// Copy all new dispute counts data to be saved from the request to disputeCountsToUpdate ignoring the disputeCountId, creation audit fields
		if (dispute.getDisputeCounts() != null && disputeCountsToUpdate != null) {
			if (dispute.getDisputeCounts().size() == disputeCountsToUpdate.size()) {
				for (int i = 0; i < dispute.getDisputeCounts().size(); i++) {
					BeanUtils.copyProperties(dispute.getDisputeCounts().get(i), disputeCountsToUpdate.get(i), "createdBy", "createdTs", "disputeCountId");
				}
				logger.warn("Unexpected number of disputeCounts: " + dispute.getDisputeCounts().size() +
						" received from the request whereas updatable number of disputeCounts from database is: " + disputeCountsToUpdate.size() +
						". This should not happen with current dispute update use case unless something has been changed");
				// TODO - determine what to do if the disputeCount list sizes don't match
			}
		}

		if (dispute.getViolationTicket() != null) {
			BeanUtils.copyProperties(dispute.getViolationTicket(), violationTicketToUpdate, "createdBy", "createdTs", "violationTicketId", "violationTicketCounts");

			if (dispute.getViolationTicket().getViolationTicketCounts() != null && violationTicketCountsToUpdate != null) {
				int violationTicketCountSize = dispute.getViolationTicket().getViolationTicketCounts().size();
				if (violationTicketCountSize == violationTicketCountsToUpdate.size()) {
					for (int i = 0; i < violationTicketCountSize; i++) {
						BeanUtils.copyProperties(dispute.getViolationTicket().getViolationTicketCounts().get(i), violationTicketCountsToUpdate.get(i), "createdBy", "createdTs", "violationTicketCountId");
					}
				}
				logger.warn("Unexpected number of violationTicketCounts: " + violationTicketCountSize +
						" received from the request whereas updatable number of violationTicketCounts from database is: " + violationTicketCountsToUpdate.size() +
						". This should not happen with current dispute update use case unless something has been changed");
				// TODO - determine what to do if the violationTicketCount list sizes don't match
			}
		}

		// Add updated ticket counts
		disputeToUpdate.addDisputeCounts(disputeCountsToUpdate);

		if (violationTicketToUpdate != null && violationTicketCountsToUpdate != null) {
			// Add updated violation ticket counts to parent violation ticket
			violationTicketToUpdate.setViolationTicketCounts(violationTicketCountsToUpdate);
		}
		// Add updated violation ticket
		disputeToUpdate.setViolationTicket(violationTicketToUpdate);

		Dispute updatedDispute = disputeRepository.update(disputeToUpdate);

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
	public void verifyEmail(Long id) {
		Dispute dispute = disputeRepository.findById(id).orElseThrow();
		dispute.setEmailAddressVerified(Boolean.TRUE);
		disputeRepository.update(dispute);
	}

	/**
	 * Finds a Dispute by noticeOfDisputeGuid (UUID) or null if not found.
	 */
	public Dispute getDisputeByNoticeOfDisputeGuid(String noticeOfDisputeGuid) {
		List<Dispute> findByNoticeOfDisputeGuid = disputeRepository.findByNoticeOfDisputeGuid(noticeOfDisputeGuid);
		if (CollectionUtils.isEmpty(findByNoticeOfDisputeGuid)) {
			String msg = String.format("Dispute could not be found with noticeOfDisputeGuid: %s", noticeOfDisputeGuid);
			logger.error(msg);
			return null;
		}
		if (findByNoticeOfDisputeGuid.size() > 1) {
			logger.warn("Unexpected number of disputes returned. More than 1 dispute have been returned based on the provided noticeOfDisputeGuid: " + noticeOfDisputeGuid);
		}
		return findByNoticeOfDisputeGuid.get(0);
	}

	/**
	 * Finds all records that match by Dispute.ticketNumber and the time portion of the Dispute.issuedTs, or by noticeOfDisputeGuid if specified.
	 * @param noticeOfDisputeGuid
	 */
	public List<DisputeResult> findDispute(String ticketNumber, Date issuedTime, String noticeOfDisputeGuid) {
		List<DisputeResult> disputeResults = new ArrayList<DisputeResult>();

		// if noticeOfDisputeGuid is specified, use that
		if (StringUtils.isNotBlank(noticeOfDisputeGuid)) {
			for (Dispute dispute : disputeRepository.findByNoticeOfDisputeGuid(noticeOfDisputeGuid)) {
				disputeResults.add(new DisputeResult(dispute.getDisputeId(), dispute.getNoticeOfDisputeGuid(), dispute.getStatus()));
				ticketNumber = dispute.getTicketNumber();
				issuedTime = dispute.getIssuedTs();
			}
		}
		// otherwise, find by ticketNumber and time
		else {
			disputeResults.addAll(disputeRepository.findByTicketNumberAndTime(ticketNumber, issuedTime));
		}

		if (CollectionUtils.isNotEmpty(disputeResults)) {
			// If we have at least on Dispute, find the associated JJDispute to add the jjDisputeStatus and JJDisputeHearingType
			List<JJDispute> jjDisputeResults = jjDisputeRepository.findByTicketNumberAndTime(ticketNumber, issuedTime);
			if (CollectionUtils.isNotEmpty(jjDisputeResults)) {
				if (jjDisputeResults.size() > 1) {
					logger.error("More than one JJDispute found for TicketNumber '{}' and IssuedTime '{}'", ticketNumber, DateUtil.formatAsHourMinuteUTC(issuedTime));
				}
				JJDispute jjDispute = jjDisputeResults.get(0);
				for (DisputeResult disputeResult : disputeResults) {
					disputeResult.setJjDisputeStatus(jjDispute.getStatus());
					disputeResult.setJjDisputeHearingType(jjDispute.getHearingType());
				}
			}
		}

		return disputeResults;
	}

	/**
	 * Persists an update request for a Disputant's contact information
	 * @param noticeOfDisputeGuid guid of the Dispute to associate with.
	 * @param updateRequest the updateRequest to persist
	 * @return the newly saved DisputantUpdateRequest record, never null.
	 * @throws NoSuchElementException if the Dispute referenced by noticeOfDisputeGuid was not found.
	 */
	public DisputantUpdateRequest saveDisputantUpdateRequest(String noticeOfDisputeGuid, DisputantUpdateRequest updateRequest) {
		Dispute dispute = getDisputeByNoticeOfDisputeGuid(noticeOfDisputeGuid);
		if (dispute == null) {
			throw new NoSuchElementException();
		}

		// Dispute found. Use the ID as the FK in the disputantUpdateRequest object.
		updateRequest.setDisputeId(dispute.getDisputeId());

		return disputantUpdateRequestRepository.save(updateRequest);
	}

	/**
	 * Retrieves all DisputantUpdateRequests by disputeId
	 * @param disputeId must not be null
	 */
	public List<DisputantUpdateRequest> findDisputantUpdateRequestByDisputeIdAndStatus(Long disputeId, DisputantUpdateRequestStatus status) {
		return disputantUpdateRequestRepository.findByDisputeIdAndOptionalStatus(disputeId, status);
	}

	/**
	 * Updates the status of the given DisputantUpdateStatus record
	 * @param updateRequestId
	 * @param status
	 * @return the newly saved DisputantUpdateRequest record, never null.
	 */
	public DisputantUpdateRequest updateDisputantUpdateRequest(Long updateRequestId, DisputantUpdateRequestStatus status) {
		DisputantUpdateRequest disputantUpdateRequest = disputantUpdateRequestRepository.findById(updateRequestId).orElseThrow();
		disputantUpdateRequest.setStatus(status);
		return disputantUpdateRequestRepository.save(disputantUpdateRequest);
	}

}
