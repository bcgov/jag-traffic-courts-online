package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.List;
import java.util.Optional;

import org.apache.commons.lang3.StringUtils;
import org.hibernate.cfg.NotYetImplementedException;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.mapper.JJDisputeMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.JjDisputeApi;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

@ConditionalOnProperty(name = "repository.jjdispute", havingValue = "ords", matchIfMissing = false)
@Qualifier("jjDisputeRepository")
@Repository
public class JJDisputeRepositoryImpl implements JJDisputeRepository {

	// Delegate, OpenAPI generated client
	private final JjDisputeApi jjDisputeApi;

	@Autowired
	private JJDisputeMapper jjDisputeMapper;

	public JJDisputeRepositoryImpl(JjDisputeApi jjDisputeApi) {
		this.jjDisputeApi = jjDisputeApi;
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
		JJDispute jjDispute = map(jjDisputeApi.v1JjDisputeGet(ticketNumber));
		// For some reason ORDS returns a valid object but with null fields if no object is found.
		if (jjDispute != null && !StringUtils.isBlank(jjDispute.getTicketNumber())) {
			return Arrays.asList(jjDispute);
		}
		return new ArrayList<JJDispute>();
	}

	@Override
	public Optional<JJDispute> findById(Long id) {
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

	private JJDispute map(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute jjDispute) {
		return jjDisputeMapper.convert(jjDispute);
	}

}
