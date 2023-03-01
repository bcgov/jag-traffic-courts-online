package ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Repository;

import ca.bc.gov.open.jag.tco.oracledataapi.mapper.TicketImageDataMapper;
import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageDataJustinDocument;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.JjDisputeApi;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.TicketImageDataGetParms;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.TicketImageDataGetResponseResult;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.TicketImageDataRepository;

@ConditionalOnProperty(name = "repository.ticketimagedata", havingValue = "ords", matchIfMissing = true)
@Qualifier("ticketImageDataRepository")
@Repository
public class TicketImageDataRepositoryImpl implements TicketImageDataRepository {

	// Delegate, OpenAPI generated client
	private final JjDisputeApi jjDisputeApi;

	@Autowired
	private TicketImageDataMapper ticketImageDataMapper;

	public TicketImageDataRepositoryImpl(JjDisputeApi jjDisputeApi) {
		this.jjDisputeApi = jjDisputeApi;
	}

	@Override
	public TicketImageDataJustinDocument getTicketImageByRccId(String rccId, String reportType) {
		
		// set up parameters to pass in
		TicketImageDataGetParms parms = new TicketImageDataGetParms();
		ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.TicketImageDataDocumentKey docKey = new ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.TicketImageDataDocumentKey(); 
		docKey.setRccId(rccId);
		docKey.setReportTypes(reportType);
		parms.addDocumentKeysItem(docKey); // expects an array of document keys

		TicketImageDataGetResponseResult response = jjDisputeApi.v1TicketImageDataGetPost(parms);
		if (response == null || response.getDocuments().isEmpty())
			return new TicketImageDataJustinDocument();
		
		TicketImageDataJustinDocument mappedResponse = ticketImageDataMapper.convert(response.getDocuments().get(0));
		return mappedResponse;
	}
}
