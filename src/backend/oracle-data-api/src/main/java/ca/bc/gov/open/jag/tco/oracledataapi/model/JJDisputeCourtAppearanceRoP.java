package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import javax.persistence.CascadeType;
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
import javax.validation.constraints.Size;

import com.fasterxml.jackson.annotation.JsonBackReference;
import com.fasterxml.jackson.annotation.JsonManagedReference;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;

import ca.bc.gov.open.jag.tco.oracledataapi.config.DateTimeDeserializer;
import ca.bc.gov.open.jag.tco.oracledataapi.config.DateTimeSerializer;
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
public class JJDisputeCourtAppearanceRoP {

	@Schema(description = "Justin Appearance ID", nullable = false, accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	private String justinAppearanceId;
	
	@Schema(description = "TCO Court Appearance ID", nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	@Column
	private Long id;

	/**
	 * The court appearance timestamp.
	 */
	@Column
	@Schema(nullable = true)
	@JsonSerialize(using = DateTimeSerializer.class)
	@JsonDeserialize(using = DateTimeDeserializer.class)
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
	private JJDisputeCourtAppearanceAPP appCd;

	/**
	 * No app -- timestamp when it was decided disputant did not appear
	 */
	@Column
	@Schema(nullable = true)
	@JsonSerialize(using = DateTimeSerializer.class)
	@JsonDeserialize(using = DateTimeDeserializer.class)
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
	private String defenceCounsel;

	/**
	 * Defense Counsel Attendance
	 */
	@Column
	@Schema(nullable = true)
	@Enumerated(EnumType.STRING)
	private JJDisputeCourtAppearanceDATT dattCd;

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
	@Enumerated(EnumType.STRING)
	@Schema(nullable = true)
	private YesNo jjSeized;

	/**
	 * Adjudicator
	 */
	@Column
	@Schema(nullable = true)
	private String adjudicator;

	/**
	 * JJ's comments about court appearance
	 */
	@Size(max = 4000)
	@Column(length = 4000)
	@Schema(nullable = true, maxLength = 4000)
	private String comments;

	/**
	 * Language Choice - the language choice indicated by disputant (English, French, or Bilingual)
	 */
	@Column
	@Schema(nullable = true)
	private String languageChoice;

	@JsonManagedReference(value = "jj_dispute_court_appearance_amendments_reference")
	@OneToOne(targetEntity = JJDisputeCourtAppearanceAmendments.class, cascade = CascadeType.ALL, fetch = FetchType.LAZY, orphanRemoval = true, optional = true)
	@JoinColumn(name = "jjcourt_appearance_id")
	@Schema(nullable = true)
	public JJDisputeCourtAppearanceAmendments amendments;

	@JsonBackReference(value="jj_dispute_court_appearance_rop_reference")
	@ManyToOne(targetEntity=JJDispute.class, fetch = FetchType.LAZY)
	@Schema(hidden = true)
	private JJDispute jjDispute;
}