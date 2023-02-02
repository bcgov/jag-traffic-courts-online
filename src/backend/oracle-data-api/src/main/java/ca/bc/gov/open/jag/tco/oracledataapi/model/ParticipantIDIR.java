package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.Table;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 * @author Relation between JUSTIN Part Id && IDIR.
 *
 */
//mark class as an Entity
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class ParticipantIDIR extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
	private Long id;

	/**
	 * IDIR (not including @IDIR)
	 */
	@Column(length = 50)
	@Schema(nullable = false)
	private String IDIR;

	/**
	 * JUSTIN participant id
	 */
	@Column(length = 50)
	@Schema(nullable = true)
	private String participantId;
}
