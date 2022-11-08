package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.security.Principal;
import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.transaction.Transactional;
import javax.validation.ConstraintViolationException;

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
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeRemark;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;
import ca.bc.gov.open.jag.tco.oracledataapi.security.PreAuthenticatedToken;

@Service
public class JJDisputeService {

	private Logger logger = LoggerFactory.getLogger(DisputeService.class);

	@Autowired
	JJDisputeRepository jjDisputeRepository;

	@PersistenceContext
    private EntityManager entityManager;

	/**
	 * Retrieves a {@link JJDispute} record by ID, delegating to CrudRepository
	 * @param ticketNumber the id (primary key) of the JJDispute to retrieve
	 * @return
	 */
	public JJDispute getJJDisputeById(String ticketNumber) {
		return jjDisputeRepository.findById(ticketNumber).orElseThrow();
	}

	/**
	 * Retrieves all {@link JJDispute} records, delegating to CrudRepository
	 * @param jjAssignedTo if specified, will filter the result set to those assigned to the specified jj staff.
	 * @param ticketNumber if specified, will filter results by JJDispute.ticketNumber
	 * @param violationTime if specified, will filter results by the time portion of JJDispute.violationDate
	 * @return
	 */
	public List<JJDispute> getJJDisputes(String jjAssignedTo, String ticketNumber, Date violationTime) {
		if (!StringUtils.isBlank(jjAssignedTo)) {
			return jjDisputeRepository.findByJjAssignedToIgnoreCase(jjAssignedTo);
		} else if (!StringUtils.isBlank(ticketNumber) || violationTime != null) {
			return jjDisputeRepository.findByTicketNumberAndTime(ticketNumber, violationTime);
		} else {
			return (List<JJDispute>) jjDisputeRepository.findAll();
		}
	}

	/**
	 * Assigns a specific {@link Dispute} to the IDIR username of the Staff with a timestamp
	 *
	 * @param ticketNumber
	 * @param principal the current user of the system
	 */
	public boolean assignJJDisputeToVtc(String id, Principal principal) {
		if (principal == null || principal.getName() == null || principal.getName().isEmpty()) {
			logger.error("Attempting to set JJDispute to null username - bad method call.");
			throw new NotAllowedException("Cannot set vtc assigned user to null");
		}

		// Find the jj-dispute to be assigned to the username
		JJDispute jjDispute = jjDisputeRepository.findById(id).orElseThrow();
		if (jjDispute == null) {
			logger.error("Cant find JJDispute for setting vtc assigned - bad method call.");
			throw new NotAllowedException("Cannot set vtc assigned for ticket not found.");
		}

		if (StringUtils.isBlank(jjDispute.getVtcAssignedTo()) || jjDispute.getVtcAssignedTo().equals(principal.getName())) {

			jjDispute.setVtcAssignedTo(principal.getName());
			jjDispute.setVtcAssignedTs(new Date());
			jjDisputeRepository.save(jjDispute);

			logger.debug("JJDispute with ticket Number {} has been assigned to {}", id, principal.getName());

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
			jjDisputeRepository.save(jjdispute);
			count++;
		}

		logger.debug("Unassigned {} record(s)", count);
	}

	/**
	 * Updates the properties of a specific {@link JJDispute}
	 *
	 * @param id
	 * @param {@link JJDispute}
	 * @return
	 */
	@Transactional
	public JJDispute updateJJDispute(String id, JJDispute jjDispute, Principal principal) {
		JJDispute jjDisputeToUpdate = jjDisputeRepository.findById(id).orElseThrow();

		// Update the status of the JJ Dispute if the status is not the same as current one
		if (jjDispute.getStatus() != null &&  jjDisputeToUpdate.getStatus() != jjDispute.getStatus()) {
			jjDisputeToUpdate = setStatus(id, jjDispute.getStatus(), principal, null);
		}

		BeanUtils.copyProperties(jjDispute, jjDisputeToUpdate, "createdBy", "createdTs", "ticketNumber", "jjDisputedCounts", "remarks", "status", "jjDisputeCourtAppearances");
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

		JJDispute updatedJJDispute = jjDisputeRepository.saveAndFlush(jjDisputeToUpdate);
		// We need to refresh the state of the instance from the database in order to return the fully updated object after persistence
		entityManager.refresh(updatedJJDispute);

		return updatedJJDispute;
	}

	/**
	 * Assigns a one or more {@link JJDispute} to the IDIR username of the JJ, or unassigns them if username not specified
	 *
	 * @param List of ticketNumber
	 * @param IDIR username of the JJ
	 */
	public void assignJJDisputesToJJ(List<String> ids, String username) {
		if (ids == null || ids.isEmpty()) {
			logger.error("No JJDispute ids (ticket numbers) passed to assign to a username - bad method call.");
			throw new ConstraintViolationException("Cannot set empty list of ticket numbers to username - bad method call.", null);
		}

		for (String id : ids) {
			// Find the jj-dispute to be assigned to the username
			JJDispute jjDispute = jjDisputeRepository.findById(id).orElseThrow();
			if (jjDispute == null) {
				logger.error("Could not find JJDispute to be assigned to the JJ for the given ticket number: " + id + " - element not found.");
				throw new NoSuchElementException("Could not find JJDispute to be assigned to the JJ for the given ticket number: " + id);
			}

			if (!StringUtils.isBlank(username)) {

				jjDispute.setJjAssignedTo(username);
				jjDisputeRepository.save(jjDispute);

				logger.debug("JJDispute with ticket number {} has been assigned to JJ {}", id, username);
			} else {
				jjDispute.setJjAssignedTo(null);
				jjDisputeRepository.save(jjDispute);

				logger.debug("Unassigned JJDispute with ticket number {} ", id);
			}
		}
	}

	/**
	 * Updates the status of a specific {@link JJDispute}
	 *
	 * @param id
	 * @param JJDisputeStatus
	 * @param remark note by the staff if the status is REVIEW.
	 * @return the saved JJDispute
	 */
	public JJDispute setStatus(String id, JJDisputeStatus jjDisputeStatus, Principal principal, String remark) {
		if (jjDisputeStatus == null) {
			logger.error("Attempting to set JJDispute status to null - bad method call.");
			throw new NotAllowedException("Cannot set JJDispute status to null");
		}

		JJDispute jjDisputeToUpdate = jjDisputeRepository.findById(id).orElseThrow();

		// TCVP-1435 - business rules
		// - current status must be NEW, IN_PROGRESS to change to IN_PROGRESS
		// - current status must be CONFIRMED, REVIEW to change to REVIEW
		// - current status must be NEW, IN_PROGRESS, REVIEW, CONFIRMED to change to CONFIRMED
		// - current status must be NEW to change to NEW
		// - current status must be NEW, REVIEW, IN_PROGRESS or same to change to DATA_UPDATE, REQUIRE_COURT_HEARING, REQUIRE_MORE_INFO
		// - current status must be CONFIRMED to change to ACCEPTED
		switch (jjDisputeStatus) {
		case IN_PROGRESS:
			if (!List.of(JJDisputeStatus.NEW, JJDisputeStatus.IN_PROGRESS).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case REVIEW:
			if (!List.of(JJDisputeStatus.CONFIRMED, JJDisputeStatus.REVIEW).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case CONFIRMED:
			if (!List.of(JJDisputeStatus.REVIEW, JJDisputeStatus.NEW, JJDisputeStatus.IN_PROGRESS, JJDisputeStatus.CONFIRMED).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case NEW:
			if (!List.of(JJDisputeStatus.NEW).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case DATA_UPDATE:
			if (!List.of(JJDisputeStatus.NEW, JJDisputeStatus.IN_PROGRESS, JJDisputeStatus.REVIEW, JJDisputeStatus.DATA_UPDATE).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case REQUIRE_COURT_HEARING:
			if (!List.of(JJDisputeStatus.NEW, JJDisputeStatus.IN_PROGRESS, JJDisputeStatus.REVIEW, JJDisputeStatus.REQUIRE_COURT_HEARING).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), jjDisputeStatus);
			}
			break;
		case REQUIRE_MORE_INFO:
			if (!List.of(JJDisputeStatus.NEW, JJDisputeStatus.IN_PROGRESS, JJDisputeStatus.REVIEW, JJDisputeStatus.REQUIRE_MORE_INFO).contains(jjDisputeToUpdate.getStatus())) {
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

		jjDisputeToUpdate.setStatus(jjDisputeStatus);

		// Set remarks with user's full name if a remark note is provided along with the status update
		if(!StringUtils.isBlank(remark)) {
			if (principal == null || principal.getName() == null || principal.getName().isBlank()) {
				logger.error("Attempting to save a remark with no user data - bad method call.");
				throw new NotAllowedException("Cannot set a remark from unknown user");
			}

			JJDisputeRemark jjDisputeRemark = new JJDisputeRemark();
			jjDisputeRemark.setNote(remark);

			PreAuthenticatedToken pat = (PreAuthenticatedToken) principal;
			CustomUserDetails user = (CustomUserDetails) pat.getPrincipal();
			jjDisputeRemark.setUserFullName(user.getFullName());

			jjDisputeRemark.setJjDispute(jjDisputeToUpdate);

			List<JJDisputeRemark> remarks = jjDisputeToUpdate.getRemarks();
			remarks.add(jjDisputeRemark);
			jjDisputeToUpdate.setRemarks(remarks);
		}

		return jjDisputeRepository.saveAndFlush(jjDisputeToUpdate);
	}
}
