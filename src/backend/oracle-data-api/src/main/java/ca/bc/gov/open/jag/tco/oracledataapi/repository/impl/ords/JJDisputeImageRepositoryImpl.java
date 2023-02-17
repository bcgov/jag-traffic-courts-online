package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import java.util.function.Supplier;

import javax.ws.rs.InternalServerErrorException;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.mapper.JJDisputeImageDataMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.mapper.JJDisputeMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeRemark;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.JjDisputeApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.RemarkResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.JJDisputeRemarkRepository;

@ConditionalOnProperty(name = "repository.jjdispute", havingValue = "ords", matchIfMissing = false)
@Qualifier("jjDisputeRemarkRepository")
@Repository
public class JJDisputeImageRepositoryImpl implements JJDisputeImageRepository {

	// Delegate, OpenAPI generated client
	private final JjDisputeApi jjDisputeApi;

	@Autowired
	private JJDisputeImageDataMapper jjDisputeImageDataMapper;

	public JJDisputeImageRepositoryImpl(JjDisputeApi jjDisputeApi) {
		this.jjDisputeApi = jjDisputeApi;
	}

	@Override
	public JJDisputeImageData get(JJDisputeImageDataParams jjDisputeImageDataParams) {
		assertNoExceptions(() -> jjDisputeApi.v1TicketImageDataGetPost(jjDisputeImageMapper.convert(jjDisputeImageDataParams)));
		return null; 
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
