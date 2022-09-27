package ca.bc.gov.open.jag.tco.oracledataapi.dto;

import java.sql.Date;

import com.fasterxml.jackson.annotation.JsonFormat;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class ViolationTicketCountDTO {
	
	private String violationTicketCountId;
	private String countNo;
	private String descriptionTxt;
	private String actOrRegulationNameCd;
	private String isActYn;
	private String isRegulationYn;
	private String statSectionTxt;
	private String statSubSectionTxt;
	private String statParagraphTxt;
	private String statSubParagraphTxt;
	private String ticketedAmt;
	@JsonFormat(pattern="yyyy-MM-dd HH:mm:ss")
	private Date entDtm;
	private String entUserId;
	@JsonFormat(pattern="yyyy-MM-dd HH:mm:ss")
	private Date updDtm;
	private String updUserId;
	private DisputeCountDTO disputeCount;
}
