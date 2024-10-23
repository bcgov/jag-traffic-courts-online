package ca.bc.gov.open.jag.tco.oracledataapi.model;

import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.Size;

import com.fasterxml.jackson.annotation.JsonBackReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;


/**
 * @author 237563
 * Represents a violation ticket count.
 *
 */

@Getter
@Setter
@NoArgsConstructor
public class ViolationTicketCount extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long violationTicketCountId;

	/**
	 * The count number.
	 */
	@Min(1)
	@Max(3)
	private int countNo;

	/**
	 * The description of the offence.
	 */
	@Schema(nullable = true)
	private String description;

	/**
	 * The act or regulation code the violation occurred against. For example, MVA, WLA, TCR, etc
	 */
	@Schema(nullable = true)
	private String actOrRegulationNameCode;

	/**
	 * The count is flagged as an offence to an act. Cannot be true, if is_regulation is true.
	 */
	@Schema(nullable = true)
	private YesNo isAct;

	/**
	 * The count is flagged as an offence to a regulation. Cannot be true, if is_act is true.
	 */
	@Schema(nullable = true)
	private YesNo isRegulation;

	/**
	 * The section part of the full section. For example, "127"
	 */
	@Size(max = 10)
	@Schema(nullable = true, maxLength = 10, accessMode = Schema.AccessMode.READ_ONLY)
	private String section;

	/**
	 * The subsection part of the full section. For example, "(1)"
	 */
	@Size(max = 4)
	@Schema(nullable = true, maxLength = 4, accessMode = Schema.AccessMode.READ_ONLY)
	private String subsection;

	/**
	 * The paragraph part of the full section. For example, "(a)"
	 */
	@Size(max = 3)
	@Schema(nullable = true, maxLength = 3, accessMode = Schema.AccessMode.READ_ONLY)
	private String paragraph;

	/**
	 * The subparagraph part of the full section. For example, "(ii)"
	 */
	@Size(max = 5)
	@Schema(nullable = true, maxLength = 5, accessMode = Schema.AccessMode.READ_ONLY)
	private String subparagraph;

	/**
	 * The ticketed amount.
	 */
	@Schema(nullable = true)
	private Float ticketedAmount;

	@JsonBackReference
	@Schema(hidden = true)
	private ViolationTicket violationTicket;

}
