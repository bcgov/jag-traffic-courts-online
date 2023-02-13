package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.security.Principal;
import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;
import java.util.Optional;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.transaction.Transactional;
import javax.validation.ConstraintViolationException;

import org.apache.commons.collections4.CollectionUtils;
import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.error.NotAllowedException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceAPP;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceDATT;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceRoP;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeHearingType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeRemark;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRemarkRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

@Service
public class JJDisputeService {

	private Logger logger = LoggerFactory.getLogger(DisputeService.class);

	@Autowired
	private JJDisputeRepository jjDisputeRepository;

	@Autowired
	private JJDisputeRemarkRepository jjDisputeRemarkRepository;

	@PersistenceContext
	private EntityManager entityManager;

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

			logger.debug("JJDispute with ticket Number {} has been assigned to {}", ticketNumber, principal.getName());

			return true;
		}

		return false;
	}

	/**
	 * Unassigns all JJDisputes whose assignedTs is older than 1 hour ago, resetting the assignedTo and assignedTs fields.
	 * @return number of records modified.
	 */
	public void unassignJJDisputes() {
		int count = 0;

		// Find all Disputes with an assignedTs older than 1 hour ago.
		Date hourAgo = DateUtils.addHours(new Date(), -1);
		logger.debug("Unassigning all jj-disputes older than {}", hourAgo.toInstant());
		for (JJDispute jjdispute : jjDisputeRepository.findByVtcAssignedTsBefore(hourAgo)) {
			jjdispute.setVtcAssignedTo(null);
			jjdispute.setVtcAssignedTs(null);
			jjDisputeRepository.saveAndFlush(jjdispute);
			count++;
		}

		logger.debug("Unassigned {} record(s)", count);
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
			jjDisputeToUpdate = setStatus(ticketNumber, jjDispute.getStatus(), principal, null, null, null);
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
				logger.error("Could not find JJDispute to be assigned to the JJ for the given ticket number: " + ticketNumber + " - element not found.");
				throw new NoSuchElementException("Could not find JJDispute to be assigned to the JJ for the given ticket number: " + ticketNumber);
			}

			if (!StringUtils.isBlank(username)) {

				// FIXME: setting JjAssignedTo doesn't work in ORDS, rather call the {{JUSTIN-TCO}}/v1/assignDisputeJj endpoint
				jjDispute.setJjAssignedTo(username);
				jjDisputeRepository.saveAndFlush(jjDispute);

				logger.debug("JJDispute with ticket number {} has been assigned to JJ {}", ticketNumber, username);
			} else {
				// FIXME: setting JjAssignedTo doesn't work in ORDS, rather call the {{JUSTIN-TCO}}/v1/unassignDisputeJj endpoint
				jjDispute.setJjAssignedTo(null);
				jjDisputeRepository.saveAndFlush(jjDispute);

				logger.debug("Unassigned JJDispute with ticket number {} ", ticketNumber);
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
			String adjudicatorPartId, Long courtAppearanceId) {
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
			logger.error("A JJ Dispute record has an unknown status '{}' - bad object state.", jjDisputeToUpdate.getStatus());
			throw new NotAllowedException("Unknown status of a JJ Dispute record: %s", jjDisputeToUpdate.getStatus());
		}

		// Calculate duplicate data for denormalization
		JJDisputeCourtAppearanceRoP courtAppearance = findCourtAppearanceById(jjDisputeToUpdate, courtAppearanceId, adjudicatorPartId);
		YesNo seizedYn = courtAppearance != null ? courtAppearance.getJjSeized() : null;
		JJDisputeCourtAppearanceAPP aattCd = courtAppearance != null ? courtAppearance.getAppCd() : null;
		JJDisputeCourtAppearanceDATT dattCd = courtAppearance != null ? courtAppearance.getDattCd() : null;
		String staffPartId = null; // TODO: Figure out mapping for staffPartId - is it the same partId??
		jjDisputeRepository.setStatus(jjDisputeToUpdate.getId(), jjDisputeStatus, principal.getName(), courtAppearanceId, seizedYn , adjudicatorPartId, aattCd, dattCd, staffPartId);

		// Set remarks with user's full name if a remark note is provided along with the status update
		if(!StringUtils.isBlank(remark)) {
			addRemark(jjDisputeToUpdate.getId(), remark, principal);
		}

		return findByTicketNumberUnique(ticketNumber).orElseThrow();
	}

	/**
	 * Helper method to find a JJDisputeCourtAppearanceRoP by id (but only if there is a partId)
	 * @param jjDispute
	 * @param courtAppearanceId
	 * @param partId
	 * @return
	 */
	private JJDisputeCourtAppearanceRoP findCourtAppearanceById(JJDispute jjDispute, Long courtAppearanceId, String partId) {
		if (!CollectionUtils.isEmpty(jjDispute.getJjDisputeCourtAppearanceRoPs()) && courtAppearanceId != null && partId != null) {
			return jjDispute.getJjDisputeCourtAppearanceRoPs().stream()
					.filter(courtAppearance -> courtAppearance.getId() == courtAppearanceId)
					.findAny()
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

		jjDisputeToUpdate = this.setStatus(id, JJDisputeStatus.REQUIRE_COURT_HEARING, principal, remark, null, null);
		jjDisputeToUpdate.setHearingType(JJDisputeHearingType.COURT_APPEARANCE);

		return jjDisputeRepository.saveAndFlush(jjDisputeToUpdate);
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
			logger.error("Cant find JJDispute by ticketNumber {}.", ticketNumber);
			return Optional.empty();
		}

		if (jjDisputes.size() > 1) {
			logger.error("Found more than one JJDispute for the given ticketNumber - should be unique. Using first one found.");
		}

		JJDispute jjDispute = jjDisputes.get(0);
		return Optional.of(jjDispute);
	}

}
