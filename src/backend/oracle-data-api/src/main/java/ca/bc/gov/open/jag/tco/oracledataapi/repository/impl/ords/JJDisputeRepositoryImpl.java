package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.util.Date;
import java.util.List;
import java.util.Optional;

import org.hibernate.cfg.NotYetImplementedException;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

@ConditionalOnProperty(name = "repository.jjdispute", havingValue = "ords", matchIfMissing = false)
@Qualifier("jjDisputeRepository")
@Repository
public class JJDisputeRepositoryImpl implements JJDisputeRepository {

	public JJDisputeRepositoryImpl() {
		// TODO pass in OpenAPI generated client that delegates implementation in each of the below methods.
	}

	@Override
	public List<JJDispute> findByJjAssignedToIgnoreCase(String jjAssigned) {
		throw new NotYetImplementedException();
	}

	@Override
	public Iterable<JJDispute> findByVtcAssignedTsBefore(Date olderThan) {
		throw new NotYetImplementedException();
	}

	@Override
	public void deleteAll() {
		throw new NotYetImplementedException();
	}

	@Override
	public Iterable<JJDispute> findAll() {
		throw new NotYetImplementedException();
	}

	@Override
	public List<JJDispute> findByTicketNumber(String ticketNumber) {
		throw new NotYetImplementedException();
	}

	@Override
	public Optional<JJDispute> findById(String id) {
		throw new NotYetImplementedException();
	}

	@Override
	public JJDispute save(JJDispute entity) {
		throw new NotYetImplementedException();
	}

	@Override
	public JJDispute saveAndFlush(JJDispute entity) {
		throw new NotYetImplementedException();
	}

	@Override
	public void setStatus(String ticketNumber, JJDisputeStatus jjDisputeStatus, String userName) {
		throw new NotYetImplementedException();
	}

}
