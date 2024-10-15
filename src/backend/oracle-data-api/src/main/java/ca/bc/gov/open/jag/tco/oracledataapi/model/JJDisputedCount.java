package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.Size;

import com.fasterxml.jackson.annotation.JsonBackReference;
import com.fasterxml.jackson.annotation.JsonManagedReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

//mark class as an Entity
/**
 * @author 237563
 *
 *         Represents JJ Written Reasons for each count
 *
 */
@Getter
@Setter
@NoArgsConstructor
public class JJDisputedCount extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long id;

	/**
	 * Represents the disputant's initial plea on count.
	 */
	@Schema(description = "Represents the disputant's initial plea on count.", nullable = false)
	private Plea plea;

	/**
	 * The count number.
	 */
	@Min(1)
	@Max(3)
	@Schema(nullable = false)
	private Integer count;

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
	 * Does the want to appear in court?
	 */
	@Schema(nullable = false)
	private YesNo appearInCourt;

	/**
	 * The description of the offence including the statute and act. Example: 92.1(1) MVA - Fail to stop resulting in pursuit
	 */
	@Schema(nullable = true)
	private String description;

	/**
	 * The due date for the offence to be paid.
	 */
	@Schema(nullable = true)
	private Date dueDate;

	/**
	 * The original fine amount from reconciled ticket data.
	 */
	@Schema(nullable = true)
	private Float ticketedFineAmount;

	/**
	 * The amount that JJ may enter to overwrite the ticketed fine amount.
	 */
	@Schema(nullable = true)
	private Float lesserOrGreaterAmount;

	/**
	 * JJ's decision whether to include surcharge in the calculated fine or not. Surcharge is always 15% of the original fine amount.
	 */
	@Schema(nullable = true)
	private YesNo includesSurcharge;

	/**
	 * Revised due date of the original due date for the offence to be paid.
	 */
	@Schema(nullable = true)
	private Date revisedDueDate;

	/**
	 * The final fine amount to be paid by the disputant.
	 */
	@Schema(nullable = true)
	private Float totalFineAmount;

	/**
	 * The date and time the violation ticket was issued.
	 */
	@Schema(nullable = true)
	private Date violationDate;

	/**
	 * JJ's comments that will be shared to disputant.
	 */
	@Size(max = 4000)
	@Schema(nullable = true, maxLength = 4000)
	private String comments;

	/**
	 * Represents the disputant's latest plea on count.
	 */
	@Schema(description = "Represents the disputant's latest plea on count.", nullable = true)
	private Plea latestPlea;

	/**
	 * The timestamp for when the last time disputant changed their plea.
	 */
	@Schema(description = "The timestamp for when the last time disputant changed their plea.", nullable = true)
	private Date latestPleaUpdateTs;

	@JsonBackReference(value = "jj_dispute_count_reference")
	@Schema(hidden = true)
	private JJDispute jjDispute;

	@JsonManagedReference
	@Schema(nullable = true)
	private JJDisputedCountRoP jjDisputedCountRoP;

}
