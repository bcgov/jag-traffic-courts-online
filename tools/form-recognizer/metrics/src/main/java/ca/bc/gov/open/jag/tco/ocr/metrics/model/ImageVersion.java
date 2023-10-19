package ca.bc.gov.open.jag.tco.ocr.metrics.model;

public enum ImageVersion {

	VT1("VT1 (2022-04)"),
	VT2("VT2 (2023-09)");

	private String displayName;

	private ImageVersion(String displayName) {
		this.displayName = displayName;
	}

	public String getDisplayName() {
		return displayName;
	}

	public String getValue() {
		return displayName;
	}

	public boolean isVT1() {
		return VT1.equals(this);
	}

}
