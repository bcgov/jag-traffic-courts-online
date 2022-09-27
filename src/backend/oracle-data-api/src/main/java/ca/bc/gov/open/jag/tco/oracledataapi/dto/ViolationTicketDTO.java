package ca.bc.gov.open.jag.tco.oracledataapi.dto;

import java.sql.Date;
import java.util.List;

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
public class ViolationTicketDTO {
	
	private String violationTicketId;
	private String ticketNumberTxt;
	private String disputantSurnameTxt;
	private String disputantGivenNamesTxt;
	private String disputantOrganizationNmTxt;
	private String isYoungPersonYn;
	private String disputantDrvLicNumberTxt;
	private String disputantClientNumberTxt;
	private String drvLicIssuedProvinceTxt;
	private String drvLicIssuedCountryTxt;
	private String drvLicIssuedYearNo;
	private String drvLicExpiryYearNo;
	@JsonFormat(pattern="yyyy-MM-dd")
	private Date disputantBirthDt;
	private String addressTxt;
	private String addressCityTxt;
	private String addressProvinceTxt;
	private String addressCountryTxt;
	private String addressPostalCodeTxt;
	private String officerPinTxt;
	private String detachmentLocationTxt;
	@JsonFormat(pattern="yyyy-MM-dd")
	private Date issuedDt;
	private String issuedOnRoadOrHighwayTxt;
	private String issuedAtOrNearCityTxt;
	private String isChangeOfAddressYn;
	private String isDriverYn;
	private String isOwnerYn;
	private String courtLocationTxt;
	@JsonFormat(pattern="yyyy-MM-dd HH:mm:ss")
	private Date entDtm;
	private String entUserId;
	@JsonFormat(pattern="yyyy-MM-dd HH:mm:ss")
	private Date updDtm;
	private String updUserId;
	private DisputeDTO dispute;
	private List<ViolationTicketCountDTO> violationTicketCounts;
}
