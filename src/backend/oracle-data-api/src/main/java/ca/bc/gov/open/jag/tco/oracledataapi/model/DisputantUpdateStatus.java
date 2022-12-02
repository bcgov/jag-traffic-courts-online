package ca.bc.gov.open.jag.tco.oracledataapi.model;

public enum DisputantUpdateStatus implements ShortNamedEnum {

	PENDING("PEN"),
	ACCEPTED("ACC"),
	REJECTED("REJ");

	private String shortName;

	private DisputantUpdateStatus(String shortName) {
		this.shortName = shortName;
	}

	@Override
	public String getShortName() {
		return shortName;
	}

}
