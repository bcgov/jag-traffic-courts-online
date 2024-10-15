package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import jakarta.validation.constraints.Size;

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
@Getter
@Setter
@NoArgsConstructor
public class JJDisputeCourtAppearanceRoP {

	@Schema(description = "Justin Appearance ID", nullable = false, accessMode = Schema.AccessMode.READ_ONLY)
	private String justinAppearanceId;

	@Schema(description = "TCO Court Appearance ID", nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	private Long id;

	/**
	 * The court appearance timestamp.
	 */
	@Schema(nullable = true)
	private Date appearanceTs;

	/**
	 * Room
	 */
	@Schema(nullable = true)
	private String room;

	/**
	 * Expected Duration in minutes
	 */
	@Schema(nullable = true)
	private short duration;

	/**
	 * Reason
	 */
	@Schema(nullable = true)
	private String reason;

	/**
	 * APP -- whether or not disputant appeared (agent = A, not present = N, present = P).
	 */
	@Schema(nullable = true)
	private JJDisputeCourtAppearanceAPP appCd;

	/**
	 * No app -- timestamp when it was decided disputant did not appear
	 */
	@Schema(nullable = true)
	private Date noAppTs;

	/**
	 * Clerk Rec
	 */
	@Schema(nullable = true)
	private String clerkRecord;

	/**
	 * Defense Counsel
	 */
	@Schema(nullable = true)
	private String defenceCounsel;

	/**
	 * Defense Counsel Attendance
	 */
	@Schema(nullable = true)
	private JJDisputeCourtAppearanceDATT dattCd;

	/**
	 * Crown present (P) or not present (N)
	 */
	@Schema(nullable = true)
	private JJDisputeCourtAppearanceCrown crown;

	/**
	 * JJ Seized
	 */
	@Schema(nullable = true)
	private YesNo jjSeized;

	/**
	 * Adjudicator
	 */
	@Schema(nullable = true)
	private String adjudicator;

	/**
	 * JJ's comments about court appearance
	 */
	@Size(max = 4000)
	@Schema(nullable = true, maxLength = 4000)
	private String comments;

	@JsonBackReference(value="jj_dispute_court_appearance_rop_reference")
	@Schema(hidden = true)
	private JJDispute jjDispute;
}
