package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.UUID;

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
public class ViolationTicketCount {
	
	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
    private UUID id;
	
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
    private String actOrRegulation;
	
	/**
	 * The section of the act or regulation. For example, "147(1)" which means "Speed in school zone"
	 */
	@Column
    private String section;
	
	/**
	 * The ticketed amount.
	 */
	@Column
    private double ticketedAmount;
	
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
