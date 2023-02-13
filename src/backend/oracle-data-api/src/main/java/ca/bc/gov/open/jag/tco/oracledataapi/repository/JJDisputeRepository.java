package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.Date;
import java.util.List;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceAPP;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceDATT;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;

public interface JJDisputeRepository {

	/**
	 * Assigns a particular JJDispute (by ticketNumber) to the supplied username (sets jjAssignedTo).
	 * @param ticketNumber
	 * @param username
	 */
	public void assignJJDisputeJj(String ticketNumber, String username);

	/**
	 * Assigns a particular JJDispute (by ticketNumber) to the supplied username (sets vtcAssignedTo/vtcAssignedTs).
	 * @param ticketNumber
	 * @param username
	 */
	public void assignJJDisputeVtc(String ticketNumber, String username);

	/**
	 * Unassigned JJDisputes older than the supplied datestamp (or just the one JJDispute if ticketNumber is supplied), (clears vtcAssignedTo/vtcAssignedTs).
	 * @param ticketNumber
	 * @param assignedBeforeTs
	 */
	public void unassignJJDisputeVtc(String ticketNumber, Date assignedBeforeTs);

	/** Fetch all records which have the specified jjAssigned. */
	public List<JJDispute> findByJjAssignedToIgnoreCase(String jjAssigned);

	/**
	 * Deletes all entities managed by the repository.
	 */
	public void deleteAll();

	/**
	 * Returns all instances of the type.
	 *
	 * @return all entities
	 */
	public Iterable<JJDispute> findAll();

	/** Fetch all records that match by JJDispute.ticketNumber (should only ever be one). */
	public List<JJDispute> findByTicketNumber(String ticketNumber);

	/**
	 * Saves an entity and flushes changes instantly.
	 *
	 * @param entity entity to be saved. Must not be {@literal null}.
	 * @return the saved entity
	 */
	public JJDispute saveAndFlush(JJDispute entity);

	/**
	 * Sets the status field on the given JJDispute.
	 *
	 * @param disputeId
	 * @param disputeStatus
	 * @param userId
	 * @param courtAppearanceId
	 * @param seizedYn
	 * @param adjudicatorPartId
	 * @param aattCd
	 * @param dattCd
	 * @param staffPartId
	 */
	public void setStatus(Long disputeId, JJDisputeStatus disputeStatus, String userId, Long courtAppearanceId, YesNo seizedYn,
			String adjudicatorPartId, JJDisputeCourtAppearanceAPP aattCd, JJDisputeCourtAppearanceDATT dattCd, String staffPartId);

}
