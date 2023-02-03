package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.List;
import java.util.Optional;
import java.util.function.Supplier;

import javax.ws.rs.InternalServerErrorException;

import org.apache.commons.collections.CollectionUtils;
import org.apache.commons.lang3.StringUtils;
import org.hibernate.cfg.NotYetImplementedException;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.mapper.JJDisputeMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceRoP;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.JjDisputeApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.ResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRepository;

@ConditionalOnProperty(name = "repository.jjdispute", havingValue = "ords", matchIfMissing = false)
@Qualifier("jjDisputeRepository")
@Repository
public class JJDisputeRepositoryImpl implements JJDisputeRepository {

	private static Logger logger = LoggerFactory.getLogger(JJDisputeRepositoryImpl.class);

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
	public void setStatus(String ticketNumber, JJDisputeStatus jjDisputeStatus, String userName, String partId, Long courtAppearanceId) {
		try {
			JJDispute jjDispute = map(jjDisputeApi.v1JjDisputeGet(ticketNumber));
			// For some reason ORDS returns a valid object but with null fields if no object is found.
			if (jjDispute != null && !StringUtils.isBlank(jjDispute.getTicketNumber())) {

				String seizedYn = null;
				String adjudicatorPartId = null;
				String aattCd = null;
				String dattCd = null;
				String staffPartId = null;

				if (!CollectionUtils.isEmpty(jjDispute.getJjDisputeCourtAppearanceRoPs()) && courtAppearanceId != null && partId != null) {
					// Get the associated jj dispute's courtAppearance by id
					JJDisputeCourtAppearanceRoP courtAppearance = jjDispute.getJjDisputeCourtAppearanceRoPs().stream()
							.filter(app -> app.getId() == courtAppearanceId)
							.findAny()
							.orElse(null);

					// Populate fields required to update court appearance
					seizedYn = courtAppearance.getJjSeized() != null ? courtAppearance.getJjSeized().toString() : null;
					adjudicatorPartId = partId;
					aattCd = courtAppearance.getAppCd() != null ? courtAppearance.getAppCd().toString() : null;
					dattCd = courtAppearance.getDattCd() != null ? courtAppearance.getDattCd().toString() : null;
					// TODO: Figure out mapping for staffPartId - is it the same partId??
				}

				ResponseResult result = jjDisputeApi.v1DisputeStatusPost(jjDispute.getId(), jjDisputeStatus.getShortName(), userName, courtAppearanceId, seizedYn, adjudicatorPartId, aattCd, dattCd, staffPartId);

				assertNoExceptions(() -> result);
			}
			logger.error("Could not find JJDispute by ticketNumber {}.", ticketNumber);
			throw new InternalServerErrorException("JJDispute is null for setting the status");

		} catch (ApiException e) {
			logger.error("ERROR updating status of JJDispute through ORDS with ticketNumber: {}", ticketNumber, e);
			throw new InternalServerErrorException(e);
		}
	}

	/**
	 * A helper method that will throw an appropriate InternalServerErrorException based on the ResponseResult. Any RuntimeExceptions throw will propagate up to caller.
	 * @return
	 */
	private ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.ResponseResult assertNoExceptions(Supplier<ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.ResponseResult> m) {
		ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.ResponseResult result = m.get();

		if (result == null) {
			// Missing response object.
			throw new InternalServerErrorException("Invalid ResponseResult object");
		} else if (result.getException() != null) {
			// Exception in response exists
			throw new InternalServerErrorException(result.getException());
		} else if (!"1".equals(result.getStatus())) {
			// Status is not 1 (success)
			throw new InternalServerErrorException("Status is not 1 (success)");
		} else {
			return result;
		}
	}

	private JJDispute map(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute jjDispute) {
		return jjDisputeMapper.convert(jjDispute);
	}

}
