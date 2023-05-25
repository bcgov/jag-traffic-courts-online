package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * An enumeration of available Statuses on a Dispute record.
 */
public enum DisputeStatus {

	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN("UKN"),
	NEW("NEW"),
	VALIDATED("VALD"),
	PROCESSING("PROC"),
	REJECTED("REJ"),
	CANCELLED("CANC"),
	CONCLUDED("CNLD");

	private String shortName;

	private DisputeStatus(String shortName) {
		this.shortName = shortName;
	}

	public String toShortName() {
		return shortName;
	}

}
