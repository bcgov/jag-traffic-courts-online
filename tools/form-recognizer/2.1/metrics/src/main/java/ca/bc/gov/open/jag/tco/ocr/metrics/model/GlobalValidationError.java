package ca.bc.gov.open.jag.tco.ocr.metrics.model;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;

@Entity
@Table
public class GlobalValidationError {

	@Id
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "global_validation_error_seq")
	@SequenceGenerator(name = "global_validation_error_seq", allocationSize = 1)
	private Integer globalValidationErrorId;

	@ManyToOne(cascade = CascadeType.ALL)
	@JoinColumn(name = "documentId")
	private Document document;

	@Column
	private String error;

	public GlobalValidationError() {
	}

	public GlobalValidationError(String validationError) {
		this.error = validationError;
	}

}
