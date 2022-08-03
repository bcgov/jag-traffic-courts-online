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

//mark class as an Entity
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class DisputedCount extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
    private Long id;

	/**
	 * Represents the disputant plea on count.
	 */
	@Column
	private Plea plea;
	
	/**
	 * Represents the disputant plea on count.
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String pleaCd;

	/**
	 * The count number.
	 */
	@Column
	@Min(1) @Max(3)
	private int count;

	/**
	 * The disputant is requesting time to pay the ticketed amount.
	 */
	@Column
	private boolean requestTimeToPay;
	
	/**
	 * The disputant is requesting time to pay the ticketed amount.
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String requestTimeToPayYN;

	/**
	 * The disputant is requesting a reduction of the ticketed amount.
	 */
	@Column
	private boolean requestReduction;
	
	/**
	 * The disputant is requesting a reduction of the ticketed amount.
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String requestReductionYN;

	/**
	 * Does the want to appear in court?
	 */
	@Column
	private boolean appearInCourt;
	
	/**
	 * Does the want to appear in court?
	 */
	@Schema(nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private String requestCourtAppearanceYN;

	@JsonBackReference
	@ManyToOne(targetEntity=Dispute.class, fetch = FetchType.LAZY)
	@Schema(hidden = true)
	private Dispute dispute;

}
