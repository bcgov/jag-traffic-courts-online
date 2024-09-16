package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import java.util.Date;
import java.util.List;
import java.util.NoSuchElementException;
import java.util.Optional;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeListItem;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeResult;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;

public interface DisputeRepository {

	/** Fetch all records newer than the given date. */
	public List<DisputeListItem> findByCreatedTsAfter(Date newerThan);

	/** Fetch all records which do not have the specified status. */
	public List<DisputeListItem> findByStatusNot(DisputeStatus excludeStatus);

	/** Fetch all records which do not have the specified status and newer than the given date. */
	public List<DisputeListItem> findByStatusNotAndCreatedTsAfterAndNoticeOfDisputeGuid(DisputeStatus excludeStatus, Date newerThan, String noticeOfDisputeGuid);
	
	/** Fetch all Dispute List Items. */
	public List<DisputeListItem> getDisputeList();

	/** Fetch all records that matches the noticeOfDisputeGuid. */
	public List<Dispute> findByNoticeOfDisputeGuid(String noticeOfDisputeGuid);

	/** Fetch all records that match by Dispute.ticketNumber and the time portion of the Dispute.issuedTs. */
	public List<DisputeResult> findByTicketNumberAndTime(String ticketNumber, Date issuedTime);

	/** Fetch all records that match by Dispute.ticketNumber */
	public List<DisputeResult> findByTicketNumber(String ticketNumber);
	
	/** Fetch all records which do not have the specified status and have a matching Dispute.ticketNumber. */
	public List<DisputeResult> findByStatusNotAndTicketNumber(DisputeStatus excludeStatus, String ticketNumber);

	/** Deletes all entities managed by the repository. */
	public void deleteAll();

	/**
	 * Deletes the entity with the given id.
	 *
	 * @param id must not be {@literal null}.
	 * @throws NoSuchElementException if a Dispute with the given {@literal id} is not found.
	 * @throws IllegalArgumentException in case the given {@literal id} is {@literal null}
	 */
	public void deleteById(Long id);

	/**
	 * Returns all instances of the type.
	 *
	 * @return all entities
	 */
	public List<Dispute> findAll();

	/**
	 * Retrieves an entity by its id.
	 *
	 * @param id must not be {@literal null}.
	 * @return the entity with the given id or {@literal Optional#empty()} if none found.
	 * @throws IllegalArgumentException if {@literal id} is {@literal null}.
	 */
	public Optional<Dispute> findById(Long id);


	/**
	 * Saves a given entity. Use the returned instance for further operations as the save operation might have changed the
	 * entity instance completely.
	 *
	 * @param entity must not be {@literal null}.
	 * @return the saved entity; will never be {@literal null}.
	 * @throws IllegalArgumentException in case the given {@literal entity} is {@literal null}.
	 */
	public Dispute save(Dispute dispute);

	/**
	 * Saves an entity and flushes changes instantly.
	 *
	 * @param entity entity to be saved. Must not be {@literal null}.
	 * @return the saved entity
	 */
	public Dispute saveAndFlush(Dispute entity);

	/**
	 * Assigns the given Dispute to the userName provided.
	 * @param disputeId
	 * @param userName
	 */
	public void assignDisputeToUser(Long disputeId, String userName);

	/**
	 * Unassigns all Disputes whose assignedTs is older than 1 hour ago, resetting the assignedTo and assignedTs fields.
	 */
	public void unassignDisputes(Date olderThan);

	/**
	 * Sets the status and rejectedReason fields on the given dispute.
	 * @param disputeId
	 * @param disputeStatus
	 * @param rejectedReason
	 */
	public void setStatus(Long disputeId, DisputeStatus disputeStatus, String rejectedReason);

	/**
	 * Flushes all pending changes to the database and clears the session, if the underlying persistence layer needs it.
	 */
	void flushAndClear();

	/**
	 * Updates the given dispute entity.
	 *
	 * @param dispute entity to be updated. Must not be {@literal null}.
	 * @return the updated entity
	 */
	public Dispute update(Dispute dispute);
	
	
	/**
	 * Deletes the ViolationTicketCount entity with the given id.
	 * 
	 * @param violationTicketCountId
	 */
	public void deleteViolationTicketCountById(Long violationTicketCountId);
}
