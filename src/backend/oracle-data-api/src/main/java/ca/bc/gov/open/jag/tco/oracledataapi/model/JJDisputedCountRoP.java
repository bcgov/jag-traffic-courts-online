package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.FetchType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.OneToOne;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.validation.constraints.Max;
import javax.validation.constraints.Min;
import javax.validation.constraints.Size;

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
	private String ssProbationDuration;
	
	/**
	 * Suspended sentence Probation Conditions
	 */
	private String ssProbationConditions;
	
	/**
	 * Jail Duration
	 */
	private String jailDuration;
	
	/**
	 * Jail Intermittent
	 */
	@Column
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
	private YesNo jailIntermittent;
	
	/**
	 * Probation Duration
	 */
	private String probationDuration;
	
	/**
	 * Probation Conditions
	 */
	private String probationConditions;
	
	/**
	 * Driving Prohibition
	 */
	private String drivingProhibition;
	
	/**
	 * Driving Prohibition MVA Section
	 */
	private String drivingProhibitionMVASection;
	
	/**
	 * Dismissed
	 */
	@Column
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
	private YesNo dismissed;
	
	/**
	 * For want of prosecution
	 */
	@Column
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
	private YesNo forWantOfProsecution;
	
	/**
	 * Withdrawn
	 */
	@Column
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
	private YesNo withdrawn;
	
	/**
	 * Abatement
	 */
	@Column
	@Schema(nullable = false)
	@Enumerated(EnumType.STRING)
	private YesNo abatement;
	
	/**
	 * Stay of Proceedings By
	 */
	@Column
	private String stayOfProceedingsBy;
	
	/**
	 * Other
	 */
	private String other;
	
	/**
	 * JJ Remarks
	 */
	private String jjRemarks;
	
	@JsonBackReference
	@OneToOne
	@JoinColumn(name = "jjdisputedcount_id", referencedColumnName = "id")
	@Schema(hidden = true)
	private JJDisputedCount jjDisputedCount;
}
