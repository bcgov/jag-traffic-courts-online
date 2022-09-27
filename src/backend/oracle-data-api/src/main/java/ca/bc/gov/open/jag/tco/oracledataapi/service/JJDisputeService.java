package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.security.Principal;
import java.util.Date;
import java.util.List;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.transaction.Transactional;

import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.error.NotAllowedException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeRemark;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

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
	 * @return
	 */
	public List<JJDispute> getAllJJDisputes(String jjAssignedTo) {
		if (jjAssignedTo == null) {
			return (List<JJDispute>) jjDisputeRepository.findAll();
		} else return jjDisputeRepository.findByJjAssignedToIgnoreCase(jjAssignedTo);
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

		JJDisputeStatus jjDisputeStatus = jjDispute.getStatus();

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

		BeanUtils.copyProperties(jjDispute, jjDisputeToUpdate, "createdBy", "createdTs", "ticketNumber", "jjDisputedCounts", "remarks");
		// Remove all existing jj disputed counts that are associated to this jj dispute
		if (jjDisputeToUpdate.getJjDisputedCounts() != null) {
			jjDisputeToUpdate.getJjDisputedCounts().clear();
		}
		// Add updated ticket counts
		jjDisputeToUpdate.addJJDisputedCounts(jjDispute.getJjDisputedCounts());

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
}
