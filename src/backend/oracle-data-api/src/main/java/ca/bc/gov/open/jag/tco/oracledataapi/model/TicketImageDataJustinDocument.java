package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.Table;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

//mark class as an Entity
/**
 * @author 237563
 *
 *         Represents JJ Document retrieved from justin
 *
 */
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class TicketImageDataJustinDocument {

	/**
	 * The report type from JUSTIN
	 */
	@Column(nullable = true)
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private TicketImageDataDocumentType reportType;

	/**
	 * report format
	 */
	@Column
	@Schema(nullable = true)
	private String reportFormat;
	
	/**
	 * JUSTIN Participant Id
	 */
	@Column
	@Schema(nullable = true)
	private String partId;
	
	/**
	 * Participant Name
	 */
	@Column
	@Schema(nullable = true)
	private String participantName;
	
	/**
	 * Index
	 */
	@Column
	@Schema(nullable = true)
	private String index;
	
	/**
	 * File blob
	 */
	@Column
	@Schema(nullable = true)
	private String data;
}

