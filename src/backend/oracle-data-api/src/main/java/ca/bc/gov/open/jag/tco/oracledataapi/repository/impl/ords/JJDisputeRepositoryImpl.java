package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Date;
import java.util.List;
import java.util.Objects;
import java.util.function.Supplier;
import java.util.stream.Collectors;

import javax.ws.rs.InternalServerErrorException;

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
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceAPP;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceDATT;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.JjDisputeApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.DisputeResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeListResponse;
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
	public void assignJJDisputeVtc(String ticketNumber, String username) {
		assertNoExceptionsGeneric(() -> jjDisputeApi.v1AssignDisputeVtcPost(username, ticketNumber));
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
	public List<JJDispute> findAll() {
		JJDisputeListResponse response = jjDisputeApi.v1JjDisputeListGet(null, null, null);
		if (response == null)
			return new ArrayList<JJDispute>();

		// convert a list of TCO ORDS JJDisputes to Oracle Data JJDisputes
		return response.getJjDisputes().stream()
				.map(jjDispute -> map(jjDispute))
				.collect(Collectors.toList());
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
	public JJDispute saveAndFlush(JJDispute jjDispute) {
		try {
			DisputeResponseResult responseResult = assertNoExceptions(() -> {
				ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute convert = jjDisputeMapper.convert(jjDispute);
				return jjDisputeApi.v1UpdateDisputePut(convert);
			});
			if (responseResult.getDisputeId() != null) {
				logger.debug("Successfully updated the jjDispute through ORDS with dispute id {}", responseResult.getDisputeId());

				// There is no endpoint to retrieve a JJDispute by id, so we'll use the ticketNumber that was in the update request (hopefully it wasn't changed)
				//return findById(Long.valueOf(result.getDisputeId()).longValue()).orElse(null);

				List<JJDispute> jjDisputes = findByTicketNumber(jjDispute.getTicketNumber());
				if (jjDisputes.isEmpty()) {
					throw new InternalServerErrorException("Update was successful, but retrieving the same record failed");
				}
				else if (jjDisputes.size() > 1) {
					logger.error("More than on JJDispute found with the [supposedly-unique] ticketNumber: {}", jjDispute.getTicketNumber());
				}
				return jjDisputes.get(0);
			}
		} catch (ApiException e) {
			logger.error("ERROR updating JJDispute to ORDS with data: {}", jjDispute.toString(), e);
			throw new InternalServerErrorException(e);
		}

		return null;
	}

	@Override
	public void setStatus(Long disputeId, JJDisputeStatus disputeStatus, String userId, Long courtAppearanceId, YesNo seizedYn, String adjudicatorPartId, JJDisputeCourtAppearanceAPP aattCd, JJDisputeCourtAppearanceDATT dattCd, String staffPartId) {
		assertNoExceptions(() -> jjDisputeApi.v1DisputeStatusPost(
				disputeId,
				disputeStatus.getShortName(),
				userId,
				courtAppearanceId,
				Objects.toString(seizedYn, null),
				adjudicatorPartId,
				Objects.toString(aattCd, null),
				Objects.toString(dattCd, null),
				staffPartId));
	}

	private JJDispute map(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute jjDispute) {
		return jjDisputeMapper.convert(jjDispute);
	}

	/**
	 * A helper method that will throw an appropriate InternalServerErrorException based on the ResponseResult. Any RuntimeExceptions throw will propagate up to caller.
	 * @return
	 */
	private ResponseResult assertNoExceptionsGeneric(Supplier<ResponseResult> m) {
		ResponseResult result = m.get();

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

	/**
	 * A helper method that will throw an appropriate InternalServerErrorException based on the ResponseResult. Any RuntimeExceptions throw will propagate up to caller.
	 * @return
	 */
	private DisputeResponseResult assertNoExceptions(Supplier<DisputeResponseResult> m) {
		DisputeResponseResult result = m.get();

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

}
