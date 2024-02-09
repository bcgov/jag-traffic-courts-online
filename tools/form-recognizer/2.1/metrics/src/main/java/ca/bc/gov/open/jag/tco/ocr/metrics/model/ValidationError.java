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
public class ValidationError {

	@Id
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "validation_error_seq")
	@SequenceGenerator(name = "validation_error_seq", allocationSize = 1)
	private Integer validationErrorId;

	@ManyToOne(cascade = CascadeType.ALL)
	@JoinColumn(name = "fieldId")
	private Field field;

	@Column
	private String error;

	public ValidationError() {
	}

	public ValidationError(String validationError) {
		this.error = validationError;
	}

}
