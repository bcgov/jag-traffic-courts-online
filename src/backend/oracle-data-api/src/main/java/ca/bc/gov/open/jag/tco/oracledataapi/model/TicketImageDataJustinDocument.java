package ca.bc.gov.open.jag.tco.oracledataapi.model;

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
	 * File contents as per advice of John Revoy pass this through as JSON friendly base 64 encoded string
	 */
	@Schema(nullable = true)
	private String fileData;
}

