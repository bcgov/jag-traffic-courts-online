package ca.bc.gov.open.jag.tco.oracledataapi.model;

public enum DisputantUpdateRequestStatus implements ShortNamedEnum {

	/** Unknown type (undefined). Must be index 0. */
	UNKNOWN("UKN"),
	ACCEPTED("ACC"),
	PENDING("PEN"),
	REJECTED("REJ");

	private String shortName;

	private DisputantUpdateRequestStatus(String shortName) {
		this.shortName = shortName;
	}

	@Override
	public String getShortName() {
		return shortName;
	}

}
