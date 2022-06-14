package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.ManyToOne;
import javax.persistence.Table;
import javax.validation.constraints.Max;
import javax.validation.constraints.Min;

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
    private Long id;

	/**
	 * The count number.
	 */
	@Column
	@Min(1) @Max(3)
	private int count;

	/**
	 * The description of the offence.
	 */
	@Column
	@Schema(nullable = true)
    private String description;

	/**
	 * The act or regulation code the violation occurred against. For example, MVA, WLA, TCR, etc
	 */
	@Column
	@Schema(nullable = true)
    private String actRegulation;
	
	/**
	 * The full section of the act or regulation represented in a single string. For example, "127(1)(a)(ii)" which means "fail to yield to pedestrian"
	 */
	@Column
	@Schema(nullable = true)
    private String fullSection;

	/**
	 * The section part of the full section. For example, "127"
	 */
	@Column
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
    private String section;

	/**
	 * The subsection part of the full section. For example, "(1)"
	 */
	@Column
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
    private String subsection;

	/**
	 * The paragraph part of the full section. For example, "(a)"
	 */
	@Column
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
    private String paragraph;
	
	/**
	 * The subparagraph part of the full section. For example, "(ii)"
	 */
	@Column
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
    private String subparagraph;

	/**
	 * The ticketed amount.
	 */
	@Column
	@Schema(nullable = true)
    private Float ticketedAmount;
	
	/**
	 * The count is flagged as an offence to an act. Cannot be true, if is_regulation is true.
	 */
	@Column
	@Schema(nullable = true)
    private Boolean isAct;

	/**
	 * The count is flagged as an offence to a regulation. Cannot be true, if is_act is true.
	 */
	@Column
	@Schema(nullable = true)
    private Boolean isRegulation;

	@JsonBackReference
	@ManyToOne(targetEntity=ViolationTicket.class, fetch = FetchType.LAZY)
	@Schema(hidden = true)
	private ViolationTicket violationTicket;

}
