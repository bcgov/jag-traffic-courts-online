package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * An enumeration of available Statuses on a JJ Dispute record.
 */
public enum JJDisputeHearingType implements ShortNamedEnum {

	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN("U"),
	COURT_APPEARANCE("C"),
	WRITTEN_REASONS("W");

	private String shortName;

	private JJDisputeHearingType(String shortName) {
		this.shortName = shortName;
	}

	@Override
	public String getShortName() {
		return shortName;
	}
}
