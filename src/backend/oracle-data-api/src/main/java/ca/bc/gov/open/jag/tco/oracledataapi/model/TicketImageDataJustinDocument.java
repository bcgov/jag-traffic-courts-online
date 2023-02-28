package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.sql.Blob;

import javax.persistence.EnumType;
import javax.persistence.Enumerated;
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
//defining class name as Table name
@Getter
@Setter
@NoArgsConstructor
public class TicketImageDataJustinDocument {

	/**
	 * The report type from JUSTIN
	 */
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private TicketImageDataDocumentType reportType;

	/**
	 * report format
	 */
	@Schema(nullable = true)
	private String reportFormat;
	
	/**
	 * JUSTIN Participant Id
	 */
	@Schema(nullable = true)
	private String partId;
	
	/**
	 * Participant Name
	 */
	@Schema(nullable = true)
	private String participantName;
	
	/**
	 * Index
	 */
	@Schema(nullable = true)
	private String index;
	
	/**
	 * File blob
	 */
	@Schema(nullable = true)
	private String fileData;
}

