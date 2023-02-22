package ca.bc.gov.open.jag.tco.oracledataapi.service;

import java.util.List;

import javax.persistence.EntityManager;
import javax.persistence.PersistenceContext;
import javax.transaction.Transactional;
import java.security.Principal;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import ca.bc.gov.open.jag.tco.oracledataapi.model.FileHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.FileHistoryRepository;

@Service
public class FileHistoryService {

	@Autowired
	FileHistoryRepository fileHistoryRepository;

	@PersistenceContext
	private EntityManager entityManager;

	/**
	 * Retrieves {@link FileHistory} records by Ticket Number, delegating to CrudRepository
	 * @param ticketNumber the id for which to retrieve file history records
	 * @return
	 */
	public List<FileHistory> getFileHistoryByTicketNumber(String ticketNumber) {
		return fileHistoryRepository.findByTicketNumber(ticketNumber);
	}

	/**
	 * Inserts a file history record
	 *
	 * @param {@link FileHistory}
	 * @return
	 */
	@Transactional
	public Long insertFileHistory(FileHistory fileHistory, boolean disputantAction, Principal principal) {
		if (disputantAction == true) fileHistory.setActionByApplicationUser("Disputant");
		else fileHistory.setActionByApplicationUser(principal.getName());
		return fileHistoryRepository.save(fileHistory);
	}
}
