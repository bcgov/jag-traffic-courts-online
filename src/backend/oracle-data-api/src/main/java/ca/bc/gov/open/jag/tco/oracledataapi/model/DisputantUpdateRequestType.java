package ca.bc.gov.open.jag.tco.oracledataapi.model;


public enum DisputantUpdateRequestType implements ShortNamedEnum {

	DISPUTANT_ADDRESS("ADR"),
	DISPUTANT_PHONE("PHN"),
	DISPUTANT_NAME("NAM"),
	COUNT("CNT"),
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
