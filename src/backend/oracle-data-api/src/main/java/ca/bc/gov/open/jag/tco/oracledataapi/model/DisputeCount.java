package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.Id;
import javax.persistence.ManyToOne;
import javax.persistence.Table;
import javax.validation.constraints.Max;
import javax.validation.constraints.Min;

import com.fasterxml.jackson.annotation.JsonBackReference;
import com.fasterxml.jackson.annotation.JsonIgnore;

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
public class DisputeCount extends Auditable<String> {

	/**
	 * Primary key
	 */
	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	@GeneratedValue
	@JsonIgnore // FIXME: this field should not be excluded (temp excluded for now to get things working).
	private Long disputeCountId;

	/**
	 * The count number.
	 */
	@Column
	@Min(1) @Max(3)
	private int countNo;

	/**
	 * Represents the disputant plea on count.
	 */
	@Column(length = 3)
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
	private Plea pleaCode;

	/**
	 * The disputant is requesting time to pay the ticketed amount.
	 */
	@Column
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
    private YesNo requestTimeToPay;

	/**
	 * The disputant is requesting a reduction of the ticketed amount.
	 */
	@Column
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
    private YesNo requestReduction;

	/**
	 * Does the disputant want to appear in court?
	 */
	@Column
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
    private YesNo requestCourtAppearance;

	@JsonBackReference(value="dispute_count_reference")
	@ManyToOne(targetEntity=Dispute.class, fetch = FetchType.LAZY)
	@Schema(hidden = true)
	private Dispute dispute;
}