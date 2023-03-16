package ca.bc.gov.open.jag.tco.ocr.metrics.model;

import java.util.HashSet;
import java.util.Set;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.OneToMany;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;

@Entity
@Table
public class Document {

	@Id
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "document_seq")
	@SequenceGenerator(name = "document_seq", allocationSize = 1)
	private Integer documentId;

	@Column
	private String fileName;

	@Column
	@Enumerated(EnumType.STRING)
	private Source source;

	@Column
	private Float confidence;

	@OneToMany(cascade = CascadeType.ALL, orphanRemoval = true)
	@JoinColumn(name = "documentId")
	private Set<Field> fields = new HashSet<Field>();

	public Document() {
	}

	public Document(String fileName, Float confidence) {
		this.fileName = fileName;
		this.confidence = confidence;
		this.source = Source.OCR;
	}

	public Integer getDocumentId() {
		return documentId;
	}

	public void setDocumentId(Integer documentId) {
		this.documentId = documentId;
	}

	public String getFileName() {
		return fileName;
	}

	public void setFileName(String fileName) {
		this.fileName = fileName;
	}

	public Set<Field> getFields() {
		return fields;
	}

	public void setFields(Set<Field> fields) {
		this.fields = fields;
	}

}
