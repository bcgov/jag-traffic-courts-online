package ca.bc.gov.open.jag.tco.ocr.metrics.model;

public class FieldComparison {

	private String fileName;

	private String fieldName;

	private String expected;

	private String actual;

	private Float confidence;

	public FieldComparison(String fileName, String fieldName, String expected, String actual, Float confidence) {
		super();
		this.fileName = fileName;
		this.fieldName = fieldName;
		this.expected = expected;
		this.actual = actual;
		this.confidence = confidence;
	}

	public String getFileName() {
		return fileName;
	}

	public void setFileName(String fileName) {
		this.fileName = fileName;
	}

	public String getFieldName() {
		return fieldName;
	}

	public void setFieldName(String fieldName) {
		this.fieldName = fieldName;
	}

	public String getExpected() {
		return expected;
	}

	public void setExpected(String expected) {
		this.expected = expected;
	}

	public String getActual() {
		return actual;
	}

	public void setActual(String actual) {
		this.actual = actual;
	}


	public Float getConfidence() {
		return confidence;
	}


	public void setConfidence(Float confidence) {
		this.confidence = confidence;
	}

}
