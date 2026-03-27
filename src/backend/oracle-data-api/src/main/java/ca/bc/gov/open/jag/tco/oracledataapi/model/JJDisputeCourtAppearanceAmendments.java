package ca.bc.gov.open.jag.tco.oracledataapi.model;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.FetchType;
import javax.persistence.Id;
import javax.persistence.ManyToOne;
import javax.persistence.OneToOne;
import javax.persistence.Table;
import javax.validation.constraints.Size;

import com.fasterxml.jackson.annotation.JsonBackReference;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;

import ca.bc.gov.open.jag.tco.oracledataapi.config.DateTimeDeserializer;
import ca.bc.gov.open.jag.tco.oracledataapi.config.DateTimeSerializer;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

/**
 *
 * Represents JJ Amendments for an Appearance
 *
 */
//mark class as an Entity
@Entity
//defining class name as Table name
@Table
@Getter
@Setter
@NoArgsConstructor
public class JJDisputeCourtAppearanceAmendments {

	@Schema(description = "Appearance Amendment ID", nullable = true, accessMode = Schema.AccessMode.READ_ONLY)
	@Id
	private Long appearanceAmendmentId;
	
	/**
	 * Disputant Surname
	 */
	@Column
	@Schema(nullable = true)
	private String disputantSurnameNm;
	
	/**
	 * Disputant Given Name(s)
	 */
	@Column
	@Schema(nullable = true)
	private String disputantGivenNamesNm;

	/**
	 * The date the violation ticket was issued.
	 */
	@Column
	@Schema(nullable = true)
	@JsonSerialize(using = DateTimeSerializer.class)
	@JsonDeserialize(using = DateTimeDeserializer.class)
	private Date violationDateDtm;
	
	/**
	 * Other Notes
	 */
	@Column
	@Schema(nullable = true)
	private String otherNotesTxt;
	
	/**
	 * Count 1 Act/Sect/Desc
	 */
	@Column
	@Schema(nullable = true)
	private String count1ActSectDescTxt;
	
	/**
	 * Count 1 Other
	 */
	@Column
	@Schema(nullable = true)
	private String count1OtherTxt;
	
	/**
	 * Count 2 Act/Sect/Desc
	 */
	@Column
	@Schema(nullable = true)
	private String count2ActSectDescTxt;
	
	/**
	 * Count 2 Other
	 */
	@Column
	@Schema(nullable = true)
	private String count2OtherTxt;
	
	/**
	 * Count 3 Act/Sect/Desc
	 */
	@Column
	@Schema(nullable = true)
	private String count3ActSectDescTxt;
	
	/**
	 * Count 3 Other
	 */
	@Column
	@Schema(nullable = true)
	private String count3OtherTxt;
	
	/**
	 * Created By
	 */
	@Column
	@Schema(nullable = true)
	private String createdBy;

	/**
	 * Created Timestamp
	 */
	@Column
	@Schema(nullable = true)
	@JsonSerialize(using = DateTimeSerializer.class)
	@JsonDeserialize(using = DateTimeDeserializer.class)
	private Date createdTs;
	
	/**
	 * Modified By
	 */
	@Column
	@Schema(nullable = true)
	private String modifiedBy;

	/**
	 * Modified Timestamp
	 */
	@Column
	@Schema(nullable = true)
	@JsonSerialize(using = DateTimeSerializer.class)
	@JsonDeserialize(using = DateTimeDeserializer.class)
	private Date modifiedTs;

	@JsonBackReference(value="jj_dispute_court_appearance_amendments_reference")
	@OneToOne(targetEntity=JJDisputeCourtAppearanceRoP.class, fetch = FetchType.LAZY)
	@Schema(hidden = true)
	private JJDisputeCourtAppearanceRoP jjDisputeCourtAppearanceRoP;
}