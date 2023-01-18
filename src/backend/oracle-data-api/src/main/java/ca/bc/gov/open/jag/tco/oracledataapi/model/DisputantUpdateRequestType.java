package ca.bc.gov.open.jag.tco.oracledataapi.model;


public enum DisputantUpdateRequestType implements ShortNamedEnum {

	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN("UKN"),
	DISPUTANT_ADDRESS("ADR"),
	DISPUTANT_PHONE("PHN"),
	DISPUTANT_NAME("NAM"),
	COUNT("CNT"),
	DISPUTANT_EMAIL("EML"),
	DISPUTANT_DOCUMENT("DOC"),
	COURT_OPTIONS("COP");

	private String shortName;

	private DisputantUpdateRequestType(String shortName) {
		this.shortName = shortName;
	}

	@Override
	public String getShortName() {
		return shortName;
	}

}
