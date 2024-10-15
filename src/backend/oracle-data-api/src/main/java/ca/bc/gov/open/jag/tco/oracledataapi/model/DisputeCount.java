package ca.bc.gov.open.jag.tco.oracledataapi.model;

import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;

import com.fasterxml.jackson.annotation.JsonBackReference;
import com.fasterxml.jackson.annotation.JsonIgnore;

import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.Size;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class DisputeCount extends Auditable<String> {

	/**
	 * Primary key
	 */
	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@JsonIgnore // FIXME: this field should not be excluded (temp excluded for now to get things working).
	private Long disputeCountId;

	/**
	 * The count number.
	 */
	@Min(1) @Max(3)
	private int countNo;

	/**
	 * Represents the disputant plea on count.
	 */
	@Size(max = 3)
	@Schema(maxLength = 3, nullable = false)
	private Plea pleaCode;

	/**
	 * The disputant is requesting time to pay the ticketed amount.
	 */
	@Schema(nullable = false)
	private YesNo requestTimeToPay;

	/**
	 * The disputant is requesting a reduction of the ticketed amount.
	 */
	@Schema(nullable = false)
	private YesNo requestReduction;

	/**
	 * Does the disputant want to appear in court?
	 */
	@Schema(nullable = false)
	private YesNo requestCourtAppearance;

	@JsonBackReference(value="dispute_count_reference")
	@Schema(hidden = true)
	private Dispute dispute;
}
