package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.Table;
import javax.validation.constraints.Max;
import javax.validation.constraints.Min;
import javax.validation.constraints.Size;

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
//mark class as an Entity
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class ViolationTicketCount extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
	private Long violationTicketCountId;

	/**
	 * The count number.
	 */
	@Column
	@Min(1)
	@Max(3)
	private int countNo;

	/**
	 * The description of the offence.
	 */
	@Column(length = 4000)
	@Schema(nullable = true)
	private String description;

	/**
	 * The act or regulation code the violation occurred against. For example, MVA, WLA, TCR, etc
	 */
	@Column(length = 5)
	@Schema(nullable = true)
	private String actOrRegulationNameCode;

	/**
	 * The count is flagged as an offence to an act. Cannot be true, if is_regulation is true.
	 */
	@Column
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private YesNo isAct;

	/**
	 * The count is flagged as an offence to a regulation. Cannot be true, if is_act is true.
	 */
	@Column
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private YesNo isRegulation;

	/**
	 * The section part of the full section. For example, "127"
	 */
	@Size(max = 10)
	@Column(length = 10)
	@Schema(nullable = true, maxLength = 10, accessMode = Schema.AccessMode.READ_ONLY)
	private String section;

	/**
	 * The subsection part of the full section. For example, "(1)"
	 */
	@Size(max = 4)
	@Column(length = 4)
	@Schema(nullable = true, maxLength = 4, accessMode = Schema.AccessMode.READ_ONLY)
	private String subsection;

	/**
	 * The paragraph part of the full section. For example, "(a)"
	 */
	@Size(max = 3)
	@Column(length = 3)
	@Schema(nullable = true, maxLength = 3, accessMode = Schema.AccessMode.READ_ONLY)
	private String paragraph;

	/**
	 * The subparagraph part of the full section. For example, "(ii)"
	 */
	@Size(max = 5)
	@Column(length = 5)
	@Schema(nullable = true, maxLength = 5, accessMode = Schema.AccessMode.READ_ONLY)
	private String subparagraph;

	/**
	 * The ticketed amount.
	 */
	@Column(precision = 8, scale = 2)
	@Schema(nullable = true)
	private Float ticketedAmount;

	@JsonBackReference
	@ManyToOne(targetEntity = ViolationTicket.class, fetch = FetchType.LAZY)
	@JoinColumn(name = "violation_ticket_id", referencedColumnName = "violationTicketId")
	@Schema(hidden = true)
	private ViolationTicket violationTicket;

}
