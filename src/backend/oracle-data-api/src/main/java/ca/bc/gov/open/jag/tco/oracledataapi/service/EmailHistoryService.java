package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.util.List;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.transaction.Transactional;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.EmailHistoryRepository;

@Service
public class EmailHistoryService {

	@Autowired
	EmailHistoryRepository emailHistoryRepository;
	
	@PersistenceContext
    private EntityManager entityManager;

	/**
	 * Retrieves {@link EmailHistory} records by Ticket Number, delegating to CrudRepository
	 * @param ticketNumber the id for which to retrieve email history records
	 * @return
	 */
	public List<EmailHistory> getEmailHistoryByTicketNumber(String ticketNumber) {
		return emailHistoryRepository.findByTicketNumber(ticketNumber);
	}
	
	/**
	 * Inserts an email history record
	 *
	 * @param ticketNumber
	 * @param {@link EmailHistory}
	 * @return
	 */
	@Transactional
	public Long insertEmailHistory(String ticketNumber, EmailHistory emailHistory) {
		
		emailHistory.setEmailHistoryId(null);
		emailHistory.setTicketNumber(ticketNumber);
		emailHistoryRepository.saveAndFlush(emailHistory);
		return emailHistory.getEmailHistoryId();
	}
}
