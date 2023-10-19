package ca.bc.gov.open.jag.tco.ocr.metrics.controller;

import ca.bc.gov.open.jag.tco.ocr.metrics.model.ImageVersion;
import ca.bc.gov.open.jag.tco.ocr.metrics.model.Source;

public class ReportForm {

	private ImageVersion imageVersion;

	private Source source1;

	private Source source2;

	public ImageVersion getImageVersion() {
		return imageVersion;
	}

	public void setImageVersion(ImageVersion imageVersion) {
		this.imageVersion = imageVersion;
	}

	public Source getSource1() {
		return source1;
	}

	public void setSource1(Source source1) {
		this.source1 = source1;
	}

	public Source getSource2() {
		return source2;
	}

	public void setSource2(Source source2) {
		this.source2 = source2;
	}

}
