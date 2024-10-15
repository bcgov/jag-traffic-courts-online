package ca.bc.gov.open.jag.tco.oracledataapi.model;


import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * @author Relation between JUSTIN Part Id && IDIR.
 *
 */

@Getter
@Setter
@NoArgsConstructor
public class ParticipantIDIR extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long id;

	/**
	 * IDIR (not including @IDIR)
	 */
	@Schema(nullable = false)
	private String IDIR;

	/**
	 * JUSTIN participant id
	 */
	@Schema(nullable = true)
	private String participantId;
}
