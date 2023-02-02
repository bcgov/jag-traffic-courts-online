package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.util.List;

import org.hibernate.cfg.NotYetImplementedException;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.EmailHistoryRepository;

@ConditionalOnProperty(name = "repository.history", havingValue = "ords", matchIfMissing = false)
@Qualifier("disputeRepository")
@Repository
public class EmailHistoryRepositoryImpl implements EmailHistoryRepository {

	public EmailHistoryRepositoryImpl() {
		// TODO pass in OpenAPI generated client that delegates implementation in each of the below methods.
	}

	@Override
	public List<EmailHistory> findByTicketNumber(String ticketNumber) {
		throw new NotYetImplementedException();
	}

	@Override
	public void deleteById(Long id) {
		throw new NotYetImplementedException();
	}

	@Override
	public List<EmailHistory> findAll() {
		throw new NotYetImplementedException();
	}

	@Override
	public EmailHistory saveAndFlush(EmailHistory entity) {
		throw new NotYetImplementedException();
	}

}
