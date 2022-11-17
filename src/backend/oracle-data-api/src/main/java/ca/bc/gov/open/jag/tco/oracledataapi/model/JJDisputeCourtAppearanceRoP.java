package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.FetchType;
import javax.persistence.Id;
import javax.persistence.ManyToOne;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
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
 * Represents JJ Written Reasons for each count
 *
 */
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class JJDisputeCourtAppearanceRoP extends Auditable<String> {

	@Schema(description = "ID", accessMode = Schema.AccessMode.READ_ONLY)
	@Id
    private Long id;

	/**
	 * The court appearance timestamp.
	 */
	@Column
	@Schema(nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date appearanceTs;

	/**
	 * Room
	 */
	@Column
	@Schema(nullable = true)
    private String room;
	
	/**
	 * Expected Duration in minutes
	 */
	@Column
	@Schema(nullable = true)
	private short duration;
	
	/**
     * Reason
     */
    @Column
    @Schema(nullable = true)
    private String reason;
    
    /**
	 * APP -- whether or not disputant appeared (agent = A, not present = N, present = P).
	 */
	@Column
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private JJDisputeCourtAppearanceAPP app;
	
	/**
     * No app -- timestamp when it was decided disputant did not appear
     */
    @Column
    @Schema(nullable = true)
    @Temporal(TemporalType.TIMESTAMP)
    private Date noAppTs;
    
    /**
	 * Clerk Rec
	 */
	@Column
	@Schema(nullable = true)
    private String clerkRecord;
	
	/**
	 * Defense Counsel
	 */
	@Column
	@Schema(nullable = true)
    private String defenseCounsel;
	
	/**
	 * Crown present (P) or not present (N)
	 */
	@Column
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private JJDisputeCourtAppearanceCrown crown;
	
	/**
	 * JJ Seized
	 */
	@Column
	@Schema(nullable = true)
    private String jjSeized;
	
	/**
	 * Adjudicator
	 */
	@Column
	@Schema(nullable = true)
	private String adjudicator;
	
	/**
	 * JJ's comments about court appearance 
	 */
	@Size(max = 500)
	@Column(length = 500)
	@Schema(nullable = true, maxLength = 500)
	private String comments;

	@JsonBackReference(value="jj_dispute_court_appearance_rop_reference")
	@ManyToOne(targetEntity=JJDispute.class, fetch = FetchType.LAZY)
	@Schema(hidden = true)
	private JJDispute jjDispute;
}
