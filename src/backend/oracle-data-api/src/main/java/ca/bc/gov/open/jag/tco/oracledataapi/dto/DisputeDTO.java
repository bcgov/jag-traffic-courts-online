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
public class DisputeDTO {
	
	private String disputeId;
	private String disputeStatusTypeCd;
	private String courtAgenId;
	@JsonFormat(pattern="yyyy-MM-dd")
	private Date issuedDt;
	@JsonFormat(pattern="yyyy-MM-dd")
	private Date submittedDt;
	private String disputantSurnameNm;
	private String disputantGiven1Nm;
	private String disputantGiven2Nm;
	private String disputantGiven3Nm;
	@JsonFormat(pattern="yyyy-MM-dd")
	private Date disputantBirthDt;
	private String disputantOrganizationNm;
	private String disputantDrvLicNumberTxt;
	private String drvLicIssuedCtryId;
	private String drvLicIssuedProvSeqNo;
	private String drvLicIssuedIntlProvTxt;
	private String disputantClientId;
	private String addressLine1Txt;
	private String addressLine2Txt;
	private String addressLine3Txt;
	private String addressCityCtryId;
	private String addressCitySeqNo;
	private String addressProvCtryId;
	private String addressProvSeqNo;
	private String addressIntlCityTxt;
	private String addressIntlProvTxt;
	private String addressCtryId;
	private String postalCodeTxt;
	private String homePhoneNumberTxt;
	private String workPhoneNumberTxt;
	private String emailAddressTxt;
	private String emailVerifiedYn;
	private String emailVerificationGuid;
	@JsonFormat(pattern="yyyy-MM-dd HH:mm:ss")
	private Date filingDt;
	private String representedByLawyerYn;
	private String lawFirmNm;
	private String lawFirmAddrLine1Txt;
	private String lawFirmAddrLine2Txt;
	private String lawFirmAddrLine3Txt;
	private String lawFirmAddrCityCtryId;
	private String lawFirmAddrCitySeqNo;
	private String lawFirmAddrProvCtryId;
	private String lawFirmAddrProvSeqNo;
	private String lawFirmAddrIntlCityTxt;
	private String lawFirmAddrIntlProvTxt;
	private String lawFirmAddrCtryId;
	private String lawFirmAddrPostalCodeTxt;
	private String lawyerSurnameNm;
	private String lawyerGiven1Nm;
	private String lawyerGiven2Nm;
	private String lawyerGiven3Nm;
	private String lawyerPhoneNumberTxt;
	private String lawyerEmailAddressTxt;
	private String officerPinTxt;
	private String detachmentLocationTxt;
	private String languageCd;
	private String interpreterRequiredYn;
	private String witnessNo;
	private String fineReductionReasonTxt;
	private String timeToPayReasonTxt;
	private String disputantCommentTxt;
	private String rejectedReasonTxt;
	private String jjAssignedTo;
	private String userAssignedTo;
	@JsonFormat(pattern="yyyy-MM-dd HH:mm:ss")
	private Date userAssignedDtm;
	private String disputantDetectOcrIssuesYn;
	private String disputantOcrIssuesTxt;
	private String systemDetectOcrIssuesYn;
	@JsonFormat(pattern="yyyy-MM-dd HH:mm:ss")
	private Date entDtm;
	private String entUserId;
	@JsonFormat(pattern="yyyy-MM-dd HH:mm:ss")
	private Date updDtm;
	private String updUserId;
}
