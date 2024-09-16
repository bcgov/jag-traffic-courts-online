package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.security.Principal;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;

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
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeListItem;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicket;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicketCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeUpdateRequestRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;
import net.logstash.logback.argument.StructuredArguments;

@Service
public class DisputeService {

	private Logger logger = LoggerFactory.getLogger(DisputeService.class);

	@Autowired
	private DisputeRepository disputeRepository;

	@Autowired
	private JJDisputeRepository jjDisputeRepository;

	@Autowired
	private DisputeUpdateRequestRepository disputeUpdateRequestRepository;
	
	private static final String NA = "N/A";

	/**
	 * Retrieves all {@link DisputeListItem} records
	 * @param newerThan if specified, will filter the result set to those newer than this date.
	 * @param excludeStatus if specified, will retrieve records which do not have the specified status.
	 *
	 * @return
	 */
	public List<DisputeListItem> getAllDisputes(Date newerThan, DisputeStatus excludeStatus) {
		List<DisputeListItem> allDisputes = null;
		if (newerThan == null && excludeStatus == null) {
			allDisputes =  disputeRepository.getDisputeList();
		} else if (newerThan == null) {
			allDisputes =  disputeRepository.findByStatusNot(excludeStatus);
		} else if (excludeStatus == null) {
			allDisputes = disputeRepository.findByCreatedTsAfter(newerThan);
		} else {
			allDisputes = disputeRepository.findByStatusNotAndCreatedTsAfterAndNoticeOfDisputeGuid(excludeStatus, newerThan, null);
		}

//		// Convert Disputes to DisputeListItem objects
//		List<DisputeListItem> disputeListItems = allDisputes.stream()
//				.map(dispute -> DisputeMapper.INSTANCE.convertDisputeToDisputeListItem(dispute))
//				.collect(Collectors.toList());
		return allDisputes;
	}

	/**
	 * Retrieves a specific {@link Dispute} by using the method findById() of CrudRepository
	 *
	 * @param id of the Dispute to be returned
	 * @return
	 */
	public Dispute getDisputeById(Long id) {
		Dispute dispute = disputeRepository.findById(id).orElseThrow();
		
		// TCVP-2748 - Replace NA values with empty strings
		replaceNAValuesWithEmpty(dispute);
		
		return dispute;
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
				logger.error("TicketNumber of the Dispute {} and ViolationTicket {} are different!",
						StructuredArguments.value("disputeTicketNumber", dispute.getTicketNumber()),
						StructuredArguments.value("violationTicketNumber", dispute.getViolationTicket().getTicketNumber()));
				throw new ConstraintViolationException(msg, null);
			}
		}
		
		// TCVP-2748 Replace null province values with NA if specific province IDs are null since db check constraints expect values if IDs are null for provinces
		replaceProvinceValuesWithNA(dispute);
		
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
		
		// TCVP-2748 Replace null province values with NA if specific province IDs are null since db check constraints expect values if IDs are null for provinces
		replaceProvinceValuesWithNA(disputeToUpdate);
		
		// Copy all new dispute counts data to be saved from the request to disputeCountsToUpdate ignoring the disputeCountId, creation audit fields
		if (dispute.getDisputeCounts() != null && disputeCountsToUpdate != null) {
			if (dispute.getDisputeCounts().size() == disputeCountsToUpdate.size()) {
				for (int i = 0; i < dispute.getDisputeCounts().size(); i++) {
					BeanUtils.copyProperties(dispute.getDisputeCounts().get(i), disputeCountsToUpdate.get(i), "createdBy", "createdTs", "disputeCountId");
				}
			} else {
				logger.warn("Unexpected number of disputeCounts: {}" +
						" received from the request whereas updatable number of disputeCounts from database is: {}" +
						". This should not happen with current dispute update use case unless something has been changed",
						StructuredArguments.value("disputeCounts", dispute.getDisputeCounts().size()),
						StructuredArguments.value("disputeCountsFromDatabase", dispute.getDisputeCounts().size()));
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
				} else {
					logger.warn("Unexpected number of violationTicketCounts: " +
							" received from the request whereas updatable number of violationTicketCounts from database is: " +
							". This should not happen with current dispute update use case unless something has been changed",
							StructuredArguments.value("violationTicketCounts", violationTicketCountSize),
							StructuredArguments.value("violationTicketCountsFromDatabase", violationTicketCountsToUpdate.size()));
					// TODO - determine what to do if the violationTicketCount list sizes don't match
				}
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
	
	protected void replaceProvinceValuesWithNA(Dispute disputeToUpdate) {
	    if (disputeToUpdate.getAddressProvinceCountryId() == null && disputeToUpdate.getAddressProvinceSeqNo() == null && StringUtils.isBlank(disputeToUpdate.getAddressProvince())) {
	        disputeToUpdate.setAddressProvince(NA);
	    }
	    
	    if (disputeToUpdate.getDriversLicenceIssuedCountryId() != null && disputeToUpdate.getDriversLicenceIssuedProvinceSeqNo() == null && StringUtils.isBlank(disputeToUpdate.getDriversLicenceProvince())) {
	        disputeToUpdate.setDriversLicenceProvince(NA);
	    } else if (disputeToUpdate.getDriversLicenceIssuedCountryId() == null && disputeToUpdate.getDriversLicenceIssuedProvinceSeqNo() == null && !StringUtils.isBlank(disputeToUpdate.getDriversLicenceProvince())) {
	    	disputeToUpdate.setDriversLicenceProvince(null);
	    }
	}
	
	protected void replaceNAValuesWithEmpty(Dispute disputeToUpdate) {
	    if (NA.equals(disputeToUpdate.getAddressProvince())) {
	        disputeToUpdate.setAddressProvince("");
	    }
	    
	    if (NA.equals(disputeToUpdate.getDriversLicenceProvince())) {
	        disputeToUpdate.setDriversLicenceProvince("");
	    }
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
			logger.error("Attempting to set Dispute status to null for disputeId: {} - bad method call.", StructuredArguments.value("disputeId", id));
			throw new NotAllowedException("Cannot set Dispute status to null");
		}

		Dispute dispute = disputeRepository.findById(id).orElseThrow();

		// TCVP-1058, TCVP-2997 - business rules
		// - current status must be NEW, VALIDATED to change to PROCESSING
		// - current status must be NEW, VALIDATED, PROCESSING to change to REJECTED
		// - current status must be PROCESSING, REJECTED to change to CANCELLED
		switch (disputeStatus) {
		case PROCESSING:
			if (!List.of(DisputeStatus.NEW, DisputeStatus.VALIDATED).contains(dispute.getStatus())) {
				throw new NotAllowedException("Changing the status of a Dispute record from %s to %s is not permitted.", dispute.getStatus(), DisputeStatus.PROCESSING);
			}
			break;
		case CANCELLED:
			if (!List.of(DisputeStatus.PROCESSING, DisputeStatus.REJECTED).contains(dispute.getStatus())) {
				throw new NotAllowedException("Changing the status of a Dispute record from %s to %s is not permitted.", dispute.getStatus(), DisputeStatus.CANCELLED);
			}
			break;
		case REJECTED:
			if (!List.of(DisputeStatus.NEW, DisputeStatus.VALIDATED, DisputeStatus.PROCESSING).contains(dispute.getStatus())) {
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
		case CONCLUDED:
			// This should never happen since setting the status to CONCLUDED should only happen from the Justin side.
			// If we got here, then this means the Dispute record is in an invalid state.
			logger.error("Attempting to set the status of a Dispute record to CONCLUDED - bad object state.");
			throw new NotAllowedException("Changing the status of a Dispute record to %s is not permitted.", DisputeStatus.CONCLUDED);
		default:
			// This should never happen, but if so, then it means a new DisputeStatus was added and these business rules were not updated accordingly.
			logger.error("A Dispute record has an unknown status {} - bad object state.", StructuredArguments.value("disputeStatus", dispute.getStatus().toString()));
			throw new NotAllowedException("Unknown status of a Dispute record: %s", dispute.getStatus());
		}

		disputeRepository.setStatus(id, disputeStatus, DisputeStatus.REJECTED.equals(disputeStatus) || DisputeStatus.CANCELLED.equals(disputeStatus) ? rejectedReason : null);
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

			logger.debug("Dispute with id {} has been assigned to {}", StructuredArguments.value("disputeId", id), StructuredArguments.value("userName", principal.getName()));

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
	 * Sets the email address of the specified Dispute
	 *
	 * @param id the Dispute record to update
	 * @param emailAddress Dispute.emailAddress will be updated with this value
	 * @throws NoSuchElementException if the Dispute could not be found.
	 */
	public Dispute resetEmail(Long id, String emailAddress) {
		Dispute dispute = disputeRepository.findById(id).orElseThrow();

		// permit setting the emailAddress to null
		if (StringUtils.isBlank(emailAddress)) {
			dispute.setEmailAddress(null);
			dispute.setEmailAddressVerified(Boolean.TRUE);
		}
		else {
			dispute.setEmailAddress(emailAddress);
			dispute.setEmailAddressVerified(Boolean.FALSE);
		}

		return disputeRepository.update(dispute);
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
			logger.error("Dispute could not be found with noticeOfDisputeGuid: {}", StructuredArguments.value("noticeOfDisputeGuid", noticeOfDisputeGuid));
			return null;
		}
		if (findByNoticeOfDisputeGuid.size() > 1) {
			logger.warn("Unexpected number of disputes returned. More than 1 dispute have been returned based on the provided noticeOfDisputeGuid: {}", StructuredArguments.value("noticeOfDisputeGuid", noticeOfDisputeGuid));
		}
		
		Dispute dispute = findByNoticeOfDisputeGuid.get(0);
		// TCVP-2748 - Replace NA values with empty strings
		replaceNAValuesWithEmpty(dispute);
		
		return dispute;
	}

	/**
	 * This method is used to find all DisputeResult records based on the given parameters. 
	 * It primarily matches records by Dispute.ticketNumber and the time portion of the Dispute.issuedTs. 
	 * However, if a noticeOfDisputeGuid is specified, it prioritizes this for the matching.
	 * If none of these are provided, it will find by ticketNumber only.
	 * If an excludeStatus is provided, it will find all records excluding the given status.
	 * 
	 * @param ticketNumber the ticket number to match
	 * @param issuedTime the issued time to match
	 * @param noticeOfDisputeGuid the Guid of the notice of dispute, if this is specified, it will be used for matching
	 * @param excludeStatus the status to exclude while finding
	 * @return a list of DisputeResult records that match the given parameters
	 */
	public List<DisputeResult> findDisputeStatuses(String ticketNumber, Date issuedTime, String noticeOfDisputeGuid, DisputeStatus excludeStatus) {
		List<DisputeResult> disputeResults = new ArrayList<DisputeResult>();

		// if noticeOfDisputeGuid is specified, use that
		if (StringUtils.isNotBlank(noticeOfDisputeGuid)) {
			for (Dispute dispute : disputeRepository.findByNoticeOfDisputeGuid(noticeOfDisputeGuid)) {
				disputeResults.add(new DisputeResult(
						dispute.getDisputeId(),
						dispute.getNoticeOfDisputeGuid(),
						dispute.getStatus(),
						StringUtils.isBlank(dispute.getEmailAddress()) ? null : dispute.getEmailAddressVerified(),
						dispute.getRequestCourtAppearanceYn()));
				ticketNumber = dispute.getTicketNumber();
				issuedTime = dispute.getIssuedTs();
			}
		}
		else if (issuedTime != null) {
			disputeResults.addAll(disputeRepository.findByTicketNumberAndTime(ticketNumber, issuedTime));
		}
		else if (excludeStatus != null && !DisputeStatus.UNKNOWN.equals(excludeStatus)) {
			disputeResults.addAll(disputeRepository.findByStatusNotAndTicketNumber(excludeStatus, ticketNumber));
		}
		else {
			disputeResults.addAll(disputeRepository.findByTicketNumber(ticketNumber));
		}

		if (CollectionUtils.isNotEmpty(disputeResults)) {
			// If we have at least one Dispute, find the associated JJDispute to add the jjDisputeStatus and JJDisputeHearingType
			List<JJDispute> jjDisputeResults = jjDisputeRepository.findByTicketNumber(ticketNumber);
			if (CollectionUtils.isNotEmpty(jjDisputeResults)) {
				if (jjDisputeResults.size() > 1) {
					logger.error("More than one JJDispute found for TicketNumber {}", StructuredArguments.value("ticketNumber", ticketNumber));
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
	 * @return the newly saved DisputeUpdateRequest record, never null.
	 * @throws NoSuchElementException if the Dispute referenced by noticeOfDisputeGuid was not found.
	 */
	public DisputeUpdateRequest saveDisputeUpdateRequest(String noticeOfDisputeGuid, DisputeUpdateRequest updateRequest) {
		Dispute dispute = getDisputeByNoticeOfDisputeGuid(noticeOfDisputeGuid);
		if (dispute == null) {
			throw new NoSuchElementException();
		}

		// Dispute found. Use the ID as the FK in the disputeUpdateRequest object.
		updateRequest.setDisputeId(dispute.getDisputeId());

		return disputeUpdateRequestRepository.save(updateRequest);
	}

	/**
	 * Retrieves all DisputeUpdateRequests by disputeId
	 * @param disputeId must not be null
	 */
	public List<DisputeUpdateRequest> findDisputeUpdateRequestByDisputeIdAndStatus(Long disputeId, DisputeUpdateRequestStatus status) {
		return disputeUpdateRequestRepository.findByOptionalDisputeIdAndOptionalStatus(disputeId, status);
	}

	/**
	 * Updates the status of the given DisputeUpdateStatus record
	 * @param updateRequestId
	 * @param status
	 * @return the newly saved DisputeUpdateRequest record, never null.
	 */
	public DisputeUpdateRequest updateDisputeUpdateRequest(Long updateRequestId, DisputeUpdateRequestStatus status) {
		DisputeUpdateRequest disputeUpdateRequest = disputeUpdateRequestRepository.findById(updateRequestId).orElseThrow();
		disputeUpdateRequest.setStatus(status);
		disputeUpdateRequest.setStatusUpdateTs(new Date());
		return disputeUpdateRequestRepository.update(disputeUpdateRequest);
	}

	/**
	 * Deletes a specific {@link DisputeUpdateRequest}
	 *
	 * @param id of the DisputeUpdateRequest to be deleted
	 */
	public void deleteDisputeUpdateRequest(Long id) {
		disputeUpdateRequestRepository.deleteById(id);
	}
	
	/**
	 * This method is used to delete a ViolationTicketCount entity from the
	 * database.
	 *
	 * @param id The ID of the ViolationTicketCount entity to be deleted.
	 */
	public void deleteViolationTicketCount(Long id) {
		disputeRepository.deleteViolationTicketCountById(id);
	}

}
