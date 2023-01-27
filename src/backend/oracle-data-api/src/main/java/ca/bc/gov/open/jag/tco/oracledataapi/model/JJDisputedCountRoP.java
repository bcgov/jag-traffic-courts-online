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
	@Enumerated(EnumType.STRING)
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
	@Column(length = 500)
	private String ssProbationDuration;
	
	/**
	 * Suspended sentence Probation Conditions
	 */
	@Column(length = 500)
	private String ssProbationConditions;
	
	/**
	 * Jail Duration
	 */
	@Column(length = 500)
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
	@Column(length = 500)
	private String probationDuration;
	
	/**
	 * Probation Conditions
	 */
	@Column(length = 1000)
	private String probationConditions;
	
	/**
	 * Driving Prohibition
	 */
	@Column(length = 500)
	private String drivingProhibition;
	
	/**
	 * Driving Prohibition MVA Section
	 */
	@Column(length = 240)
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
	@Column(length = 500)
	private String stayOfProceedingsBy;
	
	/**
	 * Other
	 */
	@Column(length = 500)
	private String other;
	
	@JsonBackReference
	@OneToOne
	@JoinColumn(name = "jjdisputedcount_id", referencedColumnName = "id")
	@Schema(hidden = true)
	private JJDisputedCount jjDisputedCount;
}
