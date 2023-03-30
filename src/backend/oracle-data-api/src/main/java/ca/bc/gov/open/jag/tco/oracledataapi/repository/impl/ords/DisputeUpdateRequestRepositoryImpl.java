package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.function.Supplier;
import java.util.stream.Collectors;

import javax.ws.rs.InternalServerErrorException;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.mapper.DisputeUpdateRequestMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeUpdateRequestStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.DisputeUpdateRequestApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.handler.ApiException;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.UpdateRequestListResponse;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.UpdateRequestResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.DisputeUpdateRequestRepository;
import net.logstash.logback.argument.StructuredArguments;

@Qualifier("disputantUpdateRequestRepository")
@Repository
public class DisputeUpdateRequestRepositoryImpl implements DisputeUpdateRequestRepository{

	private static Logger logger = LoggerFactory.getLogger(DisputeUpdateRequestRepositoryImpl.class);

	// Delegate, OpenAPI generated client
	private final DisputeUpdateRequestApi disputeUpdateRequestApi;

	public DisputeUpdateRequestRepositoryImpl(DisputeUpdateRequestApi disputeUpdateRequestApi) {
		this.disputeUpdateRequestApi = disputeUpdateRequestApi;
	}

	@Override
	public List<DisputeUpdateRequest> findByOptionalDisputeIdAndOptionalStatus(Long disputeId, DisputeUpdateRequestStatus status) {
		String updateReqStatusShortName = status != null ? status.getShortName() : null;

		UpdateRequestListResponse response = disputeUpdateRequestApi.v1DisputeUpdateRequestListGet(updateReqStatusShortName, null, disputeId, null);

		List<DisputeUpdateRequest> disputeUpdateRequestsToReturn = new ArrayList<DisputeUpdateRequest>();
		if (response != null && !response.getDisputeUpdateRequests().isEmpty()) {
			logger.debug("Successfully returned dispute update requests from ORDS");

			disputeUpdateRequestsToReturn = response.getDisputeUpdateRequests().stream()
					.map(updateRequest -> DisputeUpdateRequestMapper.INSTANCE.convert(updateRequest))
					.collect(Collectors.toList());
		}

		return disputeUpdateRequestsToReturn;
	}

	@Override
	public DisputeUpdateRequest save(DisputeUpdateRequest entity) {
		if (entity == null) {
			throw new IllegalArgumentException("DisputeUpdateRequest body is null.");
		}

		ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeUpdateRequest updateRequest = DisputeUpdateRequestMapper.INSTANCE.convert(entity);
		try {
			UpdateRequestResponseResult result = assertNoExceptions(() -> disputeUpdateRequestApi.v1ProcessDisputeUpdateRequestPost(updateRequest));
			if (result.getDisputeUpdateRequestId() != null) {
				logger.debug("Successfully saved the dispute update request through ORDS for dispute id {}", StructuredArguments.value("disputeId", result.getDisputeId()));
				return findById(Long.valueOf(result.getDisputeUpdateRequestId()).longValue()).orElse(null);
			}
		} catch (ApiException e) {
			logger.error("ERROR inserting DisputeUpdateRequest to ORDS with update request data: {}", StructuredArguments.fields(updateRequest), e);
			throw new InternalServerErrorException(e);
		}

		return null;
	}

	@Override
	public Optional<DisputeUpdateRequest> findById(Long id) {
		if (id == null) {
			throw new IllegalArgumentException("DisputeUpdateRequest ID is null.");
		}
		try {
			ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeUpdateRequest updateRequest = disputeUpdateRequestApi.v1DisputeUpdateRequestGet(id);
			if (updateRequest == null || updateRequest.getDisputeUpdateRequestId() == null) {
				return Optional.empty();
			}
			else {
				logger.debug("Successfully returned the dispute update request from ORDS with id {}", StructuredArguments.value("disputeUpdateRequestId", id));
				DisputeUpdateRequest disputeUpdateRequest = DisputeUpdateRequestMapper.INSTANCE.convert(updateRequest);

				return Optional.ofNullable(disputeUpdateRequest);
			}
		} catch (ApiException e) {
			logger.error("ERROR retrieving dispute update request from ORDS with id {}", StructuredArguments.value("disputeId", id), e);
			throw new InternalServerErrorException(e);
		}
	}

	@Override
	public DisputeUpdateRequest update(DisputeUpdateRequest disputeUpdateRequest) {
		if (disputeUpdateRequest == null) {
			throw new IllegalArgumentException("DisputeUpdateRequest body is null.");
		}

		ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeUpdateRequest updateRequest = DisputeUpdateRequestMapper.INSTANCE.convert(disputeUpdateRequest);
		try {
			UpdateRequestResponseResult result = assertNoExceptions(() -> disputeUpdateRequestApi.v1UpdateDisputeUpdateRequestPut(updateRequest));
			if (result.getDisputeUpdateRequestId() != null) {
				logger.debug("Successfully updated the dispute update request through ORDS for dispute id {}", StructuredArguments.value("disputeId", result.getDisputeId()));
				return findById(Long.valueOf(result.getDisputeUpdateRequestId()).longValue()).orElse(null);
			}
		} catch (ApiException e) {
			logger.error("ERROR updating DisputeUpdateRequest through ORDS with update request data: {}", StructuredArguments.fields(updateRequest), e);
			throw new InternalServerErrorException(e);
		}

		return null;
	}

	@Override
	public void deleteById(Long id) {
		if (id == null) {
			throw new IllegalArgumentException("DisputeUpdateRequest ID is null.");
		}

		try {
			UpdateRequestResponseResult result = assertNoExceptions(() -> disputeUpdateRequestApi.v1DeleteDisputeUpdateRequestDelete(id, null));
			logger.debug("Successfully deleted the dispute update request through ORDS with id {}", StructuredArguments.value("disputeUpdateRequestId", id));

		} catch (ApiException e) {
			logger.error("ERROR deleting DisputeUpdateRequest through ORDS with id: {}", StructuredArguments.value("disputeUpdateRequestId", id), e);
			throw new InternalServerErrorException(e);
		}
	}

	/**
	 * A helper method that will throw an appropriate InternalServerErrorException based on the UpdateRequestResponseResult. Any RuntimeExceptions throw will propagate up to caller.
	 * @return
	 */
	private UpdateRequestResponseResult assertNoExceptions(Supplier<UpdateRequestResponseResult> m) {
		UpdateRequestResponseResult result = m.get();

		if (result == null) {
			// Missing response object.
			throw new InternalServerErrorException("Invalid UpdateRequestResponseResult object");
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
