package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.ArrayList;
import java.util.List;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.FetchType;
import javax.persistence.OneToMany;
import javax.persistence.Table;

import com.fasterxml.jackson.annotation.JsonManagedReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

//mark class as an Entity
/**
 * @author 237563
 *
 *        Array entry for params to retrieve justin documents
 *
 */
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class TicketImageDataDocumentKey {

	/**
	 * Justin ticket identifier
	 */
	@Column
	@Schema(nullable = true)
	private String rccId;
	
	/**
	 * All the document types to retrieve for this justin ticket
	 */
	@JsonManagedReference
	@OneToMany(targetEntity = TicketImageDataDocumentType.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true)	
	public List<TicketImageDataDocumentType> documentTypes = new ArrayList<TicketImageDataDocumentType>();
}

