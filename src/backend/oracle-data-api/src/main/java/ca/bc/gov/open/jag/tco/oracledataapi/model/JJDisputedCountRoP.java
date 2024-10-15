package ca.bc.gov.open.jag.tco.oracledataapi.model;


import com.fasterxml.jackson.annotation.JsonBackReference;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

//mark class as an Entity
/**
 * @author 237563
 *
 * Represents JJ Record of Proceedings working values for each count
 *
 */
@Getter
@Setter
@NoArgsConstructor
public class JJDisputedCountRoP extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	private Long id;

	/**
	 * Represents the JJ Finding on count.
	 */
	@Schema(nullable = true)
	private JJDisputedCountFinding finding;

	/**
	 * The description of the lesser offence including the statute and act.
	 * Example: 92.1(1) MVA - Fail to stop resulting in pursuit
	 */
	@Schema(nullable = true)
	private String lesserDescription;

	/**
	 * Suspended sentence Probation Duration
	 */
	@Schema(nullable = true)
	private String ssProbationDuration;

	/**
	 * Suspended sentence Probation Conditions
	 */
	@Schema(nullable = true)
	private String ssProbationConditions;

	/**
	 * Jail Duration
	 */
	@Schema(nullable = true)
	private String jailDuration;

	/**
	 * Jail Intermittent
	 */
	private YesNo jailIntermittent;

	/**
	 * Probation Duration
	 */
	@Schema(nullable = true)
	private String probationDuration;

	/**
	 * Probation Conditions
	 */
	@Schema(nullable = true)
	private String probationConditions;

	/**
	 * Driving Prohibition
	 */
	@Schema(nullable = true)
	private String drivingProhibition;

	/**
	 * Driving Prohibition MVA Section
	 */
	@Schema(nullable = true)
	private String drivingProhibitionMVASection;

	/**
	 * Dismissed
	 */
	private YesNo dismissed;

	/**
	 * For want of prosecution
	 */
	private YesNo forWantOfProsecution;

	/**
	 * Withdrawn
	 */
	private YesNo withdrawn;

	/**
	 * Abatement
	 */
	private YesNo abatement;

	/**
	 * Stay of Proceedings By
	 */
	@Schema(nullable = true)
	private String stayOfProceedingsBy;

	/**
	 * Other
	 */
	@Schema(nullable = true)
	private String other;

	@JsonBackReference
	@Schema(hidden = true)
	private JJDisputedCount jjDisputedCount;
}
