package ca.bc.gov.open.jag.tco.oracledataapi.model;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.OneToOne;
import javax.persistence.Table;

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
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class JJDisputedCountRoP extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
    private Long id;

	/**
	 * Represents the JJ Finding on count.
	 */
	@Column
	@Schema(nullable = true)
	private JJDisputedCountFinding finding;

	/**
	 * The description of the lesser offence including the statute and act.
	 * Example: 92.1(1) MVA - Fail to stop resulting in pursuit
	 */
	@Column
	@Schema(nullable = true)
    private String lesserDescription;

	/**
	 * Suspended sentence Probation Duration
	 */
	@Column(nullable = true, length = 500)
	@Schema(nullable = true)
	private String ssProbationDuration;

	/**
	 * Suspended sentence Probation Conditions
	 */
	@Column(nullable = true, length = 500)
	@Schema(nullable = true)
	private String ssProbationConditions;

	/**
	 * Jail Duration
	 */
	@Column(nullable = true, length = 500)
	@Schema(nullable = true)
	private String jailDuration;

	/**
	 * Jail Intermittent
	 */
	@Column
	@Enumerated(EnumType.STRING)
	private YesNo jailIntermittent;

	/**
	 * Probation Duration
	 */
	@Column(nullable = true, length = 500)
	@Schema(nullable = true)
	private String probationDuration;

	/**
	 * Probation Conditions
	 */
	@Column(nullable = true, length = 1000)
	@Schema(nullable = true)
	private String probationConditions;

	/**
	 * Driving Prohibition
	 */
	@Column(nullable = true, length = 500)
	@Schema(nullable = true)
	private String drivingProhibition;

	/**
	 * Driving Prohibition MVA Section
	 */
	@Column(nullable = true, length = 240)
	@Schema(nullable = true)
	private String drivingProhibitionMVASection;

	/**
	 * Dismissed
	 */
	@Column
	@Enumerated(EnumType.STRING)
	private YesNo dismissed;

	/**
	 * For want of prosecution
	 */
	@Column
	@Enumerated(EnumType.STRING)
	private YesNo forWantOfProsecution;

	/**
	 * Withdrawn
	 */
	@Column
	@Enumerated(EnumType.STRING)
	private YesNo withdrawn;

	/**
	 * Abatement
	 */
	@Column
	@Enumerated(EnumType.STRING)
	private YesNo abatement;

	/**
	 * Stay of Proceedings By
	 */
	@Column(nullable = true, length = 500)
	@Schema(nullable = true)
	private String stayOfProceedingsBy;

	/**
	 * Other
	 */
	@Column(nullable = true, length = 500)
	@Schema(nullable = true)
	private String other;

	@JsonBackReference
	@OneToOne
	@JoinColumn(name = "jjdisputedcount_id", referencedColumnName = "id")
	@Schema(hidden = true)
	private JJDisputedCount jjDisputedCount;
}
