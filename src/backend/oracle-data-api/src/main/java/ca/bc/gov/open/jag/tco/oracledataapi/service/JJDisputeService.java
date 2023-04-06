package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.security.Principal;
import java.util.Comparator;
import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;
import java.util.Optional;

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
import ca.bc.gov.open.jag.tco.oracledataapi.model.CustomUserDetails;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceAPP;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceDATT;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceRoP;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeHearingType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeRemark;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageDataDocumentType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageDataJustinDocument;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRemarkRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.TicketImageDataRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.security.PreAuthenticatedToken;
import net.logstash.logback.argument.StructuredArguments;

@Service
public class JJDisputeService {

	private Logger logger = LoggerFactory.getLogger(DisputeService.class);

	@Autowired
	private JJDisputeRepository jjDisputeRepository;

	@Autowired
	private TicketImageDataRepository ticketImageDataRepository;

	@Autowired
	private JJDisputeRemarkRepository jjDisputeRemarkRepository;

	/**
	 * Retrieves a {@link JJDispute} record by ticketNumber, delegating to CrudRepository
	 * @param ticketNumber the id (primary key) of the JJDispute to retrieve
	 * @return
	 */
	public JJDispute getJJDisputeByTicketNumber(String ticketNumber) {
		return findByTicketNumberUnique(ticketNumber).orElseThrow();
	}

	/**
	 * Retrieves all {@link JJDispute} records, delegating to CrudRepository
	 * @param jjAssignedTo if specified, will filter the result set to those assigned to the specified jj staff.
	 * @param ticketNumber if specified, will filter results by JJDispute.ticketNumber
	 * @return
	 */
	public List<JJDispute> getJJDisputes(String jjAssignedTo, String ticketNumber) {
		if (!StringUtils.isBlank(jjAssignedTo)) {
			return jjDisputeRepository.findByJjAssignedToIgnoreCase(jjAssignedTo);
		} else if (!StringUtils.isBlank(ticketNumber)) {
			return jjDisputeRepository.findByTicketNumber(ticketNumber);
		} else {
			return (List<JJDispute>) jjDisputeRepository.findAll();
		}
	}

	/**
	 * Assigns a specific {@link JJDispute} to the IDIR username of the Staff with a timestamp
	 *
	 * @param ticketNumber
	 * @param principal the current user of the system
	 */
	public boolean assignJJDisputeToVtc(String ticketNumber, Principal principal) {
		if (principal == null || principal.getName() == null || principal.getName().isEmpty()) {
			logger.error("Attempting to set JJDispute to null username - bad method call.");
			throw new NotAllowedException("Cannot set vtc assigned user to null");
		}

		JJDispute jjDispute = findByTicketNumberUnique(ticketNumber).orElseThrow();

		if (StringUtils.isBlank(jjDispute.getVtcAssignedTo()) || jjDispute.getVtcAssignedTo().equals(principal.getName())) {
			jjDisputeRepository.assignJJDisputeVtc(ticketNumber, principal.getName());

			logger.debug("JJDispute with ticket Number {} has been assigned to {}", StructuredArguments.value("ticketNumber", ticketNumber), StructuredArguments.value("userName", principal.getName()));

			return true;
		}

		return false;
	}

	/**
	 * Unassigns all JJDisputes whose assignedTs is older than 1 hour ago, resetting the assignedTo and assignedTs fields.
	 * @return number of records modified.
	 */
	public void unassignJJDisputes() {
		// Find all Disputes with an assignedTs older than 1 hour ago.
		Date hourAgo = DateUtils.addHours(new Date(), -1);
		logger.debug("Unassigning all jj-disputes older than {}", StructuredArguments.value("date", hourAgo.toInstant()));
		jjDisputeRepository.unassignJJDisputeVtc(null, hourAgo);
	}

	/**
	 * Updates the properties of a specific {@link JJDispute}
	 *
	 * @param ticketNumber
	 * @param {@link JJDispute}
	 * @return
	 */
	@Transactional
	public JJDispute updateJJDispute(String ticketNumber, JJDispute jjDispute, Principal principal) {
		JJDispute jjDisputeToUpdate = findByTicketNumberUnique(ticketNumber).orElseThrow();

		// Update the status of the JJ Dispute if the status is not the same as current one
		if (jjDispute.getStatus() != null &&  jjDisputeToUpdate.getStatus() != jjDispute.getStatus()) {
			jjDisputeToUpdate = setStatus(ticketNumber, jjDispute.getStatus(), principal, null, null);
		}

		BeanUtils.copyProperties(jjDispute, jjDisputeToUpdate, "id", "createdBy", "createdTs", "ticketNumber", "jjDisputedCounts", "remarks", "status", "jjDisputeCourtAppearanceRoPs");
		// Remove all existing jj disputed counts that are associated to this jj dispute
		if (jjDisputeToUpdate.getJjDisputedCounts() != null) {
			jjDisputeToUpdate.getJjDisputedCounts().clear();
		}
		// Add updated ticket counts
		jjDisputeToUpdate.addJJDisputedCounts(jjDispute.getJjDisputedCounts());

		// Remove all existing jj dispute court appearances that are associated to this jj dispute
		if (jjDisputeToUpdate.getJjDisputeCourtAppearanceRoPs() != null) {
			jjDisputeToUpdate.getJjDisputeCourtAppearanceRoPs().clear();
		}
		// Add updated court appearances
		jjDisputeToUpdate.addJJDisputeCourtAppearances(jjDispute.getJjDisputeCourtAppearanceRoPs());

		if (jjDispute.getRemarks() != null && jjDispute.getRemarks().size() > 0) {

			if (principal == null || principal.getName() == null || principal.getName().isBlank()) {
				logger.error("Attempting to save a remark with no user data - bad method call.");
				throw new NotAllowedException("Cannot set a remark from unknown user");
			}

			// Remove all existing remarks that are associated to this jj dispute
			jjDisputeToUpdate.getRemarks().clear();

			// Add the authenticated user's full name to the remark if the remark's full name is empty (new remark)
			for (JJDisputeRemark remark : jjDispute.getRemarks()) {
				if(StringUtils.isBlank(remark.getUserFullName()))
					remark.setUserFullName(principal.getName());
			}

			// Add updated remarks
			jjDisputeToUpdate.addRemarks(jjDispute.getRemarks());
		}

		return jjDisputeRepository.saveAndFlush(jjDisputeToUpdate);
	}

	/**
	 * Assigns a one or more {@link JJDispute} to the IDIR username of the JJ, or unassigns them if username not specified
	 *
	 * @param List of ticketNumber
	 * @param IDIR username of the JJ
	 */
	public void assignJJDisputesToJJ(List<String> ticketNumbers, String username) {
		if (ticketNumbers == null || ticketNumbers.isEmpty()) {
			logger.error("No JJDispute ticket numbers passed to assign to a username - bad method call.");
			throw new ConstraintViolationException("Cannot set empty list of ticket numbers to username - bad method call.", null);
		}

		for (String ticketNumber : ticketNumbers) {
			// Find the jj-dispute to be assigned to the username
			JJDispute jjDispute = findByTicketNumberUnique(ticketNumber).orElseThrow();

			if (jjDispute == null) {
				logger.error("Could not find JJDispute to be assigned to the JJ for the given ticket number: {} - element not found.", StructuredArguments.value("ticketNumber", ticketNumber));
				throw new NoSuchElementException("Could not find JJDispute to be assigned to the JJ for the given ticket number: " + ticketNumber);
			}

			jjDisputeRepository.assignJJDisputeJj(ticketNumber, username);
			if (!StringUtils.isBlank(username)) {
				logger.debug("JJDispute with ticket number {} has been assigned to JJ {}", StructuredArguments.value("ticketNumber", ticketNumber), StructuredArguments.value("userName", username));
			} else {
				logger.debug("Unassigned JJDispute with ticket number {} ", StructuredArguments.value("ticketNumber", ticketNumber));
			}
		}
	}

	/**
	 * Updates the status of a specific {@link JJDispute}
	 *
	 * @param ticketNumber
	 * @param JJDisputeStatus
	 * @param principal
	 * @param remark note by the staff if the status is REVIEW.
	 * @return the saved JJDispute
	 */
	public JJDispute setStatus(String ticketNumber, JJDisputeStatus jjDisputeStatus, Principal principal, String remark,
			String adjudicatorPartId) {
		if (jjDisputeStatus == null) {
			logger.error("Attempting to set JJDispute status to null - bad method call.");
			throw new NotAllowedException("Cannot set JJDispute status to null");
		}

		if (principal == null || principal.getName() == null || principal.getName().isBlank()) {
			logger.error("Attempting to set the status with no user data - bad method call.");
			throw new NotAllowedException("Cannot set the status from unknown user");
		}

		JJDispute jjDisputeToUpdate = findByTicketNumberUnique(ticketNumber).orElseThrow();

		// TCVP-1435 - business rules
		// - current status can be unchanged
		// - current status must be REQUIRE_COURT_HEARING to change to HEARING_SCHEDULED
		// - current status must be NEW, IN_PROGRESS, HEARING_SCHEDULED to change to IN_PROGRESS
		// - current status must be CONFIRMED, REVIEW to change to REVIEW
		// - current status must be NEW, IN_PROGRESS, REVIEW, CONFIRMED, HEARING_SCHEDULED to change to CONFIRMED
		// - current status must be NEW, REVIEW, IN_PROGRESS, or same to change to REQUIRE_COURT_HEARING, DATA_UPDATE, REQUIRE_MORE_INFO
		// - current status must be CONFIRMED to change to ACCEPTED
		switch (jjDisputeStatus) {
		case HEARING_SCHEDULED:
			if (!List.of(JJDisputeStatus.REQUIRE_COURT_HEARING, JJDisputeStatus.HEARING_SCHEDULED).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case IN_PROGRESS:
			if (!List.of(JJDisputeStatus.NEW, JJDisputeStatus.HEARING_SCHEDULED, JJDisputeStatus.IN_PROGRESS).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case REVIEW:
			if (!List.of(JJDisputeStatus.CONFIRMED, JJDisputeStatus.REVIEW).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case CONFIRMED:
			if (!List.of(JJDisputeStatus.REVIEW, JJDisputeStatus.NEW, JJDisputeStatus.HEARING_SCHEDULED, JJDisputeStatus.IN_PROGRESS, JJDisputeStatus.CONFIRMED).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case NEW:
			if (!List.of(JJDisputeStatus.NEW).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case DATA_UPDATE:
			if (!List.of(JJDisputeStatus.NEW, JJDisputeStatus.IN_PROGRESS, JJDisputeStatus.REVIEW, JJDisputeStatus.DATA_UPDATE, JJDisputeStatus.HEARING_SCHEDULED).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case REQUIRE_COURT_HEARING:
			if (!List.of(JJDisputeStatus.NEW, JJDisputeStatus.IN_PROGRESS, JJDisputeStatus.HEARING_SCHEDULED, JJDisputeStatus.REVIEW, JJDisputeStatus.CONFIRMED, JJDisputeStatus.REQUIRE_COURT_HEARING).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case REQUIRE_MORE_INFO:
			if (!List.of(JJDisputeStatus.NEW, JJDisputeStatus.IN_PROGRESS, JJDisputeStatus.REVIEW, JJDisputeStatus.REQUIRE_MORE_INFO, JJDisputeStatus.HEARING_SCHEDULED).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case ACCEPTED:
			if (!List.of(JJDisputeStatus.CONFIRMED, JJDisputeStatus.ACCEPTED).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		default:
			// This should never happen, but if so, then it means a new JJDisputeStatus was added and these business rules were not updated accordingly.
			logger.error("A JJ Dispute record has an unknown status {} - bad object state.", StructuredArguments.value("disputeStatus", jjDisputeToUpdate.getStatus().toString()));
			throw new NotAllowedException("Unknown status of a JJ Dispute record: %s", jjDisputeToUpdate.getStatus());
		}

		// Calculate duplicate data for denormalization
		JJDisputeCourtAppearanceRoP courtAppearance = findCourtAppearanceByJJDispute(jjDisputeToUpdate, adjudicatorPartId);
		Long courtAppearanceId = courtAppearance != null && courtAppearance.getId() != null ? courtAppearance.getId() : null;
		YesNo seizedYn = courtAppearance != null ? courtAppearance.getJjSeized() : null;
		JJDisputeCourtAppearanceAPP aattCd = courtAppearance != null ? courtAppearance.getAppCd() : null;
		JJDisputeCourtAppearanceDATT dattCd = courtAppearance != null ? courtAppearance.getDattCd() : null;

		CustomUserDetails user = (CustomUserDetails) ((PreAuthenticatedToken) principal).getPrincipal();
		String staffPartId = user.getPartId(); // staffPartId comes from the person currently logged in, the Principal (aka. CustomUserDetails).

		jjDisputeRepository.setStatus(jjDisputeToUpdate.getId(), jjDisputeStatus, principal.getName(), courtAppearanceId, seizedYn , adjudicatorPartId, aattCd, dattCd, staffPartId);

		// Set remarks with user's full name if a remark note is provided along with the status update
		if(!StringUtils.isBlank(remark)) {
			addRemark(jjDisputeToUpdate.getId(), remark, principal);
		}

		return findByTicketNumberUnique(ticketNumber).orElseThrow();
	}

	/**
	 * Helper method to find a JJDisputeCourtAppearanceRoP for the given JJDispute (but only if there is a partId)
	 * @param jjDispute
	 * @param courtAppearanceId
	 * @param partId
	 * @return
	 */
	private JJDisputeCourtAppearanceRoP findCourtAppearanceByJJDispute(JJDispute jjDispute, String partId) {
		if (!CollectionUtils.isEmpty(jjDispute.getJjDisputeCourtAppearanceRoPs()) &&
				partId != null && JJDisputeStatus.ACCEPTED.equals(jjDispute.getStatus())) {

			// Return the latest record iff the status is ACCEPTED
			return jjDispute.getJjDisputeCourtAppearanceRoPs().stream()
					.sorted(new Comparator<JJDisputeCourtAppearanceRoP>() {
						@Override
						public int compare(JJDisputeCourtAppearanceRoP o1, JJDisputeCourtAppearanceRoP o2) {
							return ObjectUtils.compare(o1.getAppearanceTs(), o2.getAppearanceTs());
						}
					})
					.findFirst()
					.orElse(null);
		}
		return null;
	}

	/**
	 * Updates the status of a specific {@link JJDispute} to REQUIRE_COURT_HEARING, hearing type to COURT_APPEARANCE
	 *
	 * @param id
	 * @param remark note by the staff
	 * @return the saved JJDispute
	 */
	public JJDispute requireCourtHearing(String id, Principal principal, String remark) {

		JJDispute jjDisputeToUpdate = findByTicketNumberUnique(id).orElseThrow();

		jjDisputeToUpdate = this.setStatus(id, JJDisputeStatus.REQUIRE_COURT_HEARING, principal, remark, null);
		jjDisputeToUpdate.setHearingType(JJDisputeHearingType.COURT_APPEARANCE);

		return jjDisputeRepository.saveAndFlush(jjDisputeToUpdate);
	}

	/**
	 * Gets a Ticket Image by ticketNumber. Callers can optionally throw {@link NoSuchElementException} if not found.
	 * @param ticketNumber
	 * @param documentType
	 * @return
	 */
	public TicketImageDataJustinDocument getTicketImageByTicketNumber(String ticketNumber, TicketImageDataDocumentType documentType) {

		List<JJDispute> jjDisputes = jjDisputeRepository.findByTicketNumber(ticketNumber);
		if (jjDisputes.isEmpty()) {
			logger.error("Cant find JJDispute by ticketNumber {}.", StructuredArguments.value("ticketNumber", ticketNumber));
			return null;
		}

		// Get justin document by rcc id and document type. There should be one and only one.
		TicketImageDataJustinDocument ticketImage = ticketImageDataRepository.getTicketImageByRccId(jjDisputes.get(0).getJustinRccId(), documentType.getShortName());
		if (ticketImage == null) {
			logger.error("Cant find Ticket Image by ticketNumber {} and type {}.", StructuredArguments.value("ticketNumber", ticketNumber), StructuredArguments.value("documentType", documentType));
			return null;
		}

		return ticketImage;
	}

	/**
	 * Creates a new remark with the user name and surname who added the remark and adds it to the given {@link JJDispute}
	 *
	 * @param ticketNumber
	 * @param remark
	 * @param principal
	 * @return the saved JJDispute
	 */
	private void addRemark(Long jjDisputeId, String remark, Principal principal) {
		JJDisputeRemark jjDisputeRemark = new JJDisputeRemark();
		jjDisputeRemark.setJjDispute(new JJDispute(jjDisputeId));
		jjDisputeRemark.setNote(remark);
		jjDisputeRemark.setRemarksMadeTs(new Date());
		jjDisputeRemark.setUserFullName(principal.getName());
		jjDisputeRemarkRepository.saveAndFlush(jjDisputeRemark);
	}

	/**
	 * Finds a JJDispute by ticketNumber. Callers can optionally throw {@link NoSuchElementException} if not found.
	 * @param ticketNumber
	 * @return
	 */
	private Optional<JJDispute> findByTicketNumberUnique(String ticketNumber) {
		// Find a JJDispute by ticketNumber. There should be one and only one record - this field "should" be unique.
		List<JJDispute> jjDisputes = jjDisputeRepository.findByTicketNumber(ticketNumber);
		if (jjDisputes.isEmpty()) {
			logger.error("Cant find JJDispute by ticketNumber {}.", StructuredArguments.value("ticketNumber", ticketNumber));
			return Optional.empty();
		}

		if (jjDisputes.size() > 1) {
			logger.error("Found more than one JJDispute for the given ticketNumber {} - should be unique. Using first one found.", StructuredArguments.value("ticketNumber", ticketNumber));
		}

		JJDispute jjDispute = jjDisputes.get(0);
		return Optional.of(jjDispute);
	}

	/**
	 * Deletes a specific {@link Dispute} by using the jjDisputeId or TicketNumber
	 *
	 * @param id of the JJDispute to be deleted
	 * @param ticketNumber of the JJDispute to be deleted
	 */
	public void delete(Long id, String ticketNumber) {
		jjDisputeRepository.deleteByIdOrTicketNumber(id, ticketNumber);
	}
}