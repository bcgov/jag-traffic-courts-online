package ca.bc.gov.open.jag.tco.oracledataapi.repository;

import ca.bc.gov.open.jag.tco.oracledataapi.model.TicketImageDataJustinDocument;

public interface TicketImageDataRepository {

	/** Get a justin document record given ticket number and type */
	public TicketImageDataJustinDocument getTicketImageByRccId(String rccId, String reportType);

}
