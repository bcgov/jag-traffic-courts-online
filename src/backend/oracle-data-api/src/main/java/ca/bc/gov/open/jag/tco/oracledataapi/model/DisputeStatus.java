package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * An enumeration of available Statuses on a Dispute record.
 */
public enum DisputeStatus {

	NEW("NEW"),
	VALIDATED("VALD"),
	PROCESSING("PROC"),
	REJECTED("REJ"),
	CANCELLED("CANC");

	private String shortName;

	private DisputeStatus(String shortName) {
		this.shortName = shortName;
	}

	public String toShortName() {
		return shortName;
	}

}
