package ca.bc.gov.open.jag.tco.ocr.metrics.model;

import java.util.HashSet;
import java.util.Set;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.OneToMany;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;

@Entity
@Table
public class Field {

	@Id
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "field_seq")
	@SequenceGenerator(name = "field_seq", allocationSize = 1)
	private Integer fieldId;

	@ManyToOne(cascade = CascadeType.ALL)
	@JoinColumn(name = "documentId")
	private Document document;

	@Column
	private String fieldName;

	@Column
	private String rawContent;

	@Column
	private String content;

	@Column
	private Float confidence;

	@OneToMany(cascade = CascadeType.ALL, orphanRemoval = true)
	@JoinColumn(name = "fieldId")
	private Set<ValidationError> validationErrors = new HashSet<ValidationError>();

	public Field() {
	}

	public Field(String fieldName, __occam_package_.api.model.Field field) {
		this.fieldName = fieldName;
		this.confidence = field.getFieldConfidence();
		this.rawContent = field.getValue();
		this.content = parse(this.fieldName, this.rawContent);
		for (String validationError : field.getValidationErrors()) {
			this.validationErrors.add(new ValidationError(validationError));
		}
	}

	public Integer getFieldId() {
		return fieldId;
	}

	public void setFieldId(Integer fieldId) {
		this.fieldId = fieldId;
	}

	public String getFieldName() {
		return fieldName;
	}

	public void setFieldName(String fieldName) {
		this.fieldName = fieldName;
	}

	public String getRawContent() {
		return rawContent;
	}

	public void setRawContent(String rawContent) {
		this.rawContent = rawContent;
	}

	public String getContent() {
		return content;
	}

	public void setContent(String content) {
		this.content = content;
	}

	public Float getConfidence() {
		return confidence;
	}

	public void setConfidence(Float confidence) {
		this.confidence = confidence;
	}

	public Set<ValidationError> getValidationErrors() {
		return this.validationErrors;
	}

	public void setValidationErrors(Set<ValidationError> validationErrors) {
		this.validationErrors = validationErrors;
	}

	private String parse(String fieldName, String rawContent) {
		if (fieldName == null || rawContent == null) {
			return rawContent;
		}

		if (fieldName.endsWith("_date")) {
			String date = rawContent.replaceAll("\\D", "");
			if (date.length() == 8) {
				return date.subSequence(0, 4) + "-" + date.subSequence(4, 6) + "-" + date.subSequence(6, 8);
			}
		}

		if (fieldName.endsWith("_time")) {
			String date = rawContent.replaceAll("\\D", "");
			if (date.length() == 4) {
				return date.subSequence(0, 2) + ":" + date.subSequence(2, 4);
			}
		}

		if (fieldName.contains("is_")) {
			return rawContent.replaceAll(":", "");
		}

		if (fieldName.endsWith("section")) {
			return rawContent.replaceAll("\\s", "");
		}

		if (fieldName.endsWith("description")) {
			return rawContent.replaceAll("\\n", " ");
		}

		return rawContent.trim();

	}

}
