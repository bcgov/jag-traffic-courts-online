package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.util.List;

import javax.transaction.Transactional;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.EmailHistoryRepository;

@Service
public class EmailHistoryService {

	@Autowired
	EmailHistoryRepository emailHistoryRepository;

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
	 * @param {@link EmailHistory}
	 * @return
	 */
	@Transactional
	public Long insertEmailHistory(EmailHistory emailHistory) {
		return emailHistoryRepository.save(emailHistory);
	}
}
