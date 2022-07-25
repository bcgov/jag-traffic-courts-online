package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.util.List;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.error.NotAllowedException;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

@Service
public class JJDisputeService {

	private Logger logger = LoggerFactory.getLogger(DisputeService.class);

	@Autowired
	JJDisputeRepository jjDisputeRepository;

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
	 * @param jjGroupAssignedTo if specified, will filter the result set to those assigned to the specified jj group.
	 * @param jjAssignedTo if specified, will filter the result set to those assigned to the specified jj staff.
	 * @return
	 */
	public List<JJDispute> getAllJJDisputes(String jjGroupAssignedTo, String jjAssignedTo) {
		if (jjGroupAssignedTo == null && jjAssignedTo == null) {
			return (List<JJDispute>) jjDisputeRepository.findAll();
		} else if (jjGroupAssignedTo == null) {
			return jjDisputeRepository.findByJjAssignedToIgnoreCase(jjAssignedTo);
		} else if (jjAssignedTo == null) {
			return jjDisputeRepository.findByJjGroupAssignedToIgnoreCase(jjGroupAssignedTo);
		} else {
			return jjDisputeRepository.findByJjGroupAssignedToIgnoreCaseAndJjAssignedToIgnoreCase(jjGroupAssignedTo, jjAssignedTo);
		}
	}
	
	/**
	 * Updates the properties of a specific {@link JJDispute}
	 *
	 * @param id
	 * @param {@link JJDispute}
	 * @return
	 */
	public JJDispute updateJJDispute(String id, JJDispute jjDispute) {
		JJDispute jjDisputeToUpdate = jjDisputeRepository.findById(id).orElseThrow();
		
		JJDisputeStatus jjDisputeStatus = jjDispute.getStatus();
		
		// TCVP-1435 - business rules
		// - current status must be NEW, REVIEW, IN_PROGRESS to change to IN_PROGRESS
		// - current status must be NEW, IN_PROGRESS, REVIEW to change to REVIEW
		// - current status must be REVIEW to change to COMPLETED
		switch (jjDisputeStatus) {
		case IN_PROGRESS:
			if (!List.of(JJDisputeStatus.NEW, JJDisputeStatus.REVIEW, JJDisputeStatus.IN_PROGRESS).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), JJDisputeStatus.IN_PROGRESS);
			}
			break;
		case REVIEW:
			if (!List.of(JJDisputeStatus.NEW, JJDisputeStatus.REVIEW, JJDisputeStatus.IN_PROGRESS).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), JJDisputeStatus.REVIEW);
			}
			break;
		case COMPLETED:
			if (!List.of(JJDisputeStatus.REVIEW).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), JJDisputeStatus.COMPLETED);
			}
			break;
		case NEW:
			if (!List.of(JJDisputeStatus.NEW).contains(jjDisputeToUpdate.getStatus())) {
				throw new NotAllowedException("Changing the status of a JJ Dispute record from %s to %s is not permitted.", jjDisputeToUpdate.getStatus(), JJDisputeStatus.NEW);
			}
			break;
		default:
			// This should never happen, but if so, then it means a new JJDisputeStatus was added and these business rules were not updated accordingly.
			logger.error("A JJ Dispute record has an unknown status '{}' - bad object state.", jjDisputeToUpdate.getStatus());
			throw new NotAllowedException("Unknown status of a JJ Dispute record: %s", jjDisputeToUpdate.getStatus());
		}

		BeanUtils.copyProperties(jjDispute, jjDisputeToUpdate, "createdBy", "createdTs", "ticketNumber", "jjDisputedCounts");
		// Remove all existing jj disputed counts that are associated to this jj dispute
		if (jjDisputeToUpdate.getJjDisputedCounts() != null) {
			jjDisputeToUpdate.getJjDisputedCounts().clear();
		}
		// Add updated ticket counts
		jjDisputeToUpdate.addJJDisputedCounts(jjDispute.getJjDisputedCounts());

		return jjDisputeRepository.save(jjDisputeToUpdate);
	}
}
