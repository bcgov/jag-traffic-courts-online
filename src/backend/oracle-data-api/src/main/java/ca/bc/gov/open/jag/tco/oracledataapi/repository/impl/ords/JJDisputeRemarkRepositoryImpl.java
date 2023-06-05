package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.util.function.Supplier;

import javax.ws.rs.InternalServerErrorException;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.mapper.JJDisputeMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeRemark;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.JjDisputeApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.RemarkResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRemarkRepository;

@ConditionalOnProperty(name = "repository.jjdispute", havingValue = "ords", matchIfMissing = true)
@Qualifier("jjDisputeRemarkRepository")
@Repository
public class JJDisputeRemarkRepositoryImpl implements JJDisputeRemarkRepository {

	// Delegate, OpenAPI generated client
	private final JjDisputeApi jjDisputeApi;

	@Autowired
	private JJDisputeMapper jjDisputeMapper;

	public JJDisputeRemarkRepositoryImpl(JjDisputeApi jjDisputeApi) {
		this.jjDisputeApi = jjDisputeApi;
	}

	@Override
	public JJDisputeRemark saveAndFlush(JJDisputeRemark jjDisputeRemark) {
		assertNoExceptions(() -> jjDisputeApi.processDisputeRemarkPost(jjDisputeMapper.convert(jjDisputeRemark)));
		return null; // There is no TCO ORDS endpoint that will fetch a JJDisputeRemark by id so we can't return the newly saved record.
	}

	/**
	 * A helper method that will throw an appropriate InternalServerErrorException based on the ResponseResult. Any RuntimeExceptions throw will propagate up to caller.
	 * @return
	 */
	private RemarkResponseResult assertNoExceptions(Supplier<RemarkResponseResult> m) {
		RemarkResponseResult result = m.get();

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
