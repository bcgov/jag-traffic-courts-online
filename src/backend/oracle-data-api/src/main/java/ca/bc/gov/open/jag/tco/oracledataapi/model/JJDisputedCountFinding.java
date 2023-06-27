package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * An enumeration of Finding Type on a TicketCount record with a court appearance.
 */
public enum JJDisputedCountFinding implements ShortNamedEnum {

	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN("UKN"),
	GUILTY("GU"),
	NOT_GUILTY("NG"),
	CANCELLED("CA"),
	PAID_PRIOR_TO_APPEARANCE("PP"),
	GUILTY_LESSER("GL");

	private String shortName;

	private JJDisputedCountFinding(String shortName) {
		this.shortName = shortName;
	}

	@Override
	public String getShortName() {
		return shortName;
	}

}