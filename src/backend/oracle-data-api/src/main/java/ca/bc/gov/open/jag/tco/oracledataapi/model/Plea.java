package ca.bc.gov.open.jag.tco.oracledataapi.model;

/**
 * An enumeration of Plea Type on a TicketCount record.
 */
public enum Plea implements ShortNamedEnum {

	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN("U"),
	G("G"),
	N("N");

	private String shortName;

	private Plea(String shortName) {
		this.shortName = shortName;
	}

	@Override
	public String getShortName() {
		return shortName;
	}
}