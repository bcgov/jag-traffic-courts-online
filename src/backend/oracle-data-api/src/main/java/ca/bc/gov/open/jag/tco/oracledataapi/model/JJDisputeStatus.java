package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * An enumeration of available Statuses on a JJ Dispute record.
 */
public enum JJDisputeStatus implements ShortNamedEnum {

	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN("UNKN"),
	NEW("NEW"),
	IN_PROGRESS("PROG"),
	DATA_UPDATE("UPD"),
	CONFIRMED("CONF"),
	REQUIRE_COURT_HEARING("REQH"),
	REQUIRE_MORE_INFO("REQM"),
	ACCEPTED("ACCP"),
	REVIEW("REV"),
	HEARING_SCHEDULED("HEAR");

	private String shortName;

	private JJDisputeStatus(String shortName) {
		this.shortName = shortName;
	}

	@Override
	public String getShortName() {
		return shortName;
	}

}
