package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * 
 * An enumeration of documents that can be retrieved from JUSTIN
 *
 */
public enum TicketImageDataDocumentType implements ShortNamedEnum {
	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN("UNKNOWN"),
	NOTICE_OF_DISPUTE("NOTICE_OF_DISPUTE"),
	TICKET_IMAGE("TICKET_IMAGE");
	
	private String shortName;

	private TicketImageDataDocumentType(String shortName) {
		this.shortName = shortName;
	}

	@Override
	public String getShortName() {
		return shortName;
	}
}