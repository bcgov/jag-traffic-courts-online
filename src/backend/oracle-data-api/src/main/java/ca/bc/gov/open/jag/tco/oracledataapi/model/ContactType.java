package ca.bc.gov.open.jag.tco.oracledataapi.model;


public enum ContactType implements ShortNamedEnum {

	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN("UKN"),
	INDIVIDUAL("I"),
	LAWYER("L"),
	OTHER("O");

	private String shortName;

	private ContactType(String shortName) {
		this.shortName = shortName;
	}

	@Override
	public String getShortName() {
		return shortName;
	}

}
