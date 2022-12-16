package ca.bc.gov.open.jag.tco.oracledataapi.model;

public enum DisputantUpdateRequestStatus implements ShortNamedEnum {

	PENDING("PEN"),
	ACCEPTED("ACC"),
	HOLD("HOL"),
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
