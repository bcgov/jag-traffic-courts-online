package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.util.Date;
import java.util.List;
import java.util.Optional;

import org.hibernate.cfg.NotYetImplementedException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.api.ViolationTicketApi;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeRepository;

@ConditionalOnProperty(name = "ords.enabled", havingValue = "true", matchIfMissing = false)
@Qualifier("disputeRepository")
@Repository
public class DisputeRepositoryImpl implements DisputeRepository {
	
	Logger logger = LoggerFactory.getLogger(DisputeRepositoryImpl.class);

    private final ViolationTicketApi violationTicketApi;

	public DisputeRepositoryImpl(ViolationTicketApi violationTicketApi) {
		// Pass in OpenAPI generated client that delegates implementation in each of the below methods.
		this.violationTicketApi = violationTicketApi;
	}

	@Override
	public Iterable<Dispute> findByCreatedTsBefore(Date olderThan) {
		throw new NotYetImplementedException();
	}

	@Override
	public Iterable<Dispute> findByStatusNot(DisputeStatus excludeStatus) {
		throw new NotYetImplementedException();
	}

	@Override
	public Iterable<Dispute> findByStatusNotAndCreatedTsBefore(DisputeStatus excludeStatus, Date olderThan) {
		throw new NotYetImplementedException();
	}

	@Override
	public Iterable<Dispute> findByUserAssignedTsBefore(Date olderThan) {
		throw new NotYetImplementedException();
	}

	@Override
	public List<Dispute> findByEmailVerificationToken(String emailVerificationToken) {
		throw new NotYetImplementedException();
	}

	@Override
	public void deleteAll() {
		throw new NotYetImplementedException();
	}

	@Override
	public void deleteById(Long id) {
		throw new NotYetImplementedException();
	}

	@Override
	public Iterable<Dispute> findAll() {
		throw new NotYetImplementedException();
	}

	@Override
	public Optional<Dispute> findById(Long id) {
		throw new NotYetImplementedException();
	}

	@Override
	public Dispute save(Dispute dispute) {
		throw new NotYetImplementedException();
	}

	@Override
	public Dispute saveAndFlush(Dispute entity) {
		throw new NotYetImplementedException();
	}

}
