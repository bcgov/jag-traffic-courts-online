package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.factory.Mappers;

import ca.bc.gov.open.jag.tco.oracledataapi.dto.ViolationTicketDTO;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;

@Mapper
public interface DisputeMapper {
	
	DisputeMapper INSTANCE = Mappers.getMapper(DisputeMapper.class);
	
	@Mapping(source = "dispute.disputeId", target = "disputeId")
	@Mapping(source = "dispute.disputeStatusTypeCd", target = "status")
	@Mapping(source = "dispute.courtAgenId", target = "courtLocation")
	@Mapping(source = "dispute.issuedDt", target = "issuedDate")
	@Mapping(source = "dispute.submittedDt", target = "submittedDate")
	@Mapping(source = "dispute.disputantSurnameNm", target = "disputantSurname")
	@Mapping(source = "dispute.disputantGiven1Nm", target = "disputantGivenName1")
	@Mapping(source = "dispute.disputantGiven2Nm", target = "disputantGivenName2")
	@Mapping(source = "dispute.disputantGiven3Nm", target = "disputantGivenName3")
	@Mapping(source = "dispute.disputantBirthDt", target = "disputantBirthdate")
	@Mapping(source = "dispute.disputantOrganizationNm", target = "disputantOrganization")
	@Mapping(source = "dispute.disputantDrvLicNumberTxt", target = "driversLicenceNumber")
	@Mapping(source = "dispute.drvLicIssuedIntlProvTxt", target = "driversLicenceProvince")
	@Mapping(source = "dispute.addressLine1Txt", target = "addressLine1")
	@Mapping(source = "dispute.addressLine2Txt", target = "addressLine2")
	@Mapping(source = "dispute.addressLine3Txt", target = "addressLine3")
	@Mapping(source = "dispute.addressIntlCityTxt", target = "addressCity")
	@Mapping(source = "dispute.addressIntlProvTxt", target = "addressProvince")
	@Mapping(source = "dispute.postalCodeTxt", target = "postalCode")
	@Mapping(source = "dispute.homePhoneNumberTxt", target = "homePhoneNumber")
	@Mapping(source = "dispute.workPhoneNumberTxt", target = "workPhoneNumber")
	@Mapping(source = "dispute.emailAddressTxt", target = "emailAddress")
	@Mapping(source = "dispute.emailVerificationGuid", target = "emailVerificationToken")
	@Mapping(source = "dispute.filingDt", target = "filingDate")
	@Mapping(source = "dispute.representedByLawyerYn", target = "representedByLawyer")
	@Mapping(source = "dispute.lawFirmNm", target = "lawFirmName")
	@Mapping(source = "dispute.lawFirmAddrLine1Txt", target = "lawyerAddress")
	@Mapping(source = "dispute.lawyerSurnameNm", target = "lawyerSurname")
	@Mapping(source = "dispute.lawyerGiven1Nm", target = "lawyerGivenName1")
	@Mapping(source = "dispute.lawyerGiven2Nm", target = "lawyerGivenName2")
	@Mapping(source = "dispute.lawyerGiven3Nm", target = "lawyerGivenName3")
	@Mapping(source = "dispute.lawyerPhoneNumberTxt", target = "lawyerPhoneNumber")
	@Mapping(source = "dispute.lawyerEmailAddressTxt", target = "lawyerEmail")
	@Mapping(source = "dispute.officerPinTxt", target = "officerPin")
	@Mapping(source = "dispute.detachmentLocationTxt", target = "detachmentLocation")
	@Mapping(source = "dispute.interpreterRequiredYn", target = "interpreterRequired")
	@Mapping(source = "dispute.witnessNo", target = "witnessNo")
	@Mapping(source = "dispute.fineReductionReasonTxt", target = "fineReductionReason")
	@Mapping(source = "dispute.timeToPayReasonTxt", target = "timeToPayReason")
	@Mapping(source = "dispute.disputantCommentTxt", target = "disputantComment")
	@Mapping(source = "dispute.rejectedReasonTxt", target = "rejectedReason")
	@Mapping(source = "dispute.userAssignedTo", target = "userAssignedTo")
	@Mapping(source = "dispute.userAssignedDtm", target = "userAssignedTs")
	@Mapping(source = "dispute.disputantDetectOcrIssuesYn", target = "disputantDetectedOcrIssues")
	@Mapping(source = "dispute.disputantOcrIssuesTxt", target = "disputantOcrIssues")
	@Mapping(source = "dispute.systemDetectOcrIssuesYn", target = "systemDetectedOcrIssues")
	@Mapping(source = "dispute.entDtm", target = "createdTs")
	@Mapping(source = "dispute.entUserId", target = "createdBy")
	@Mapping(source = "dispute.updDtm", target = "modifiedTs")
	@Mapping(source = "dispute.updUserId", target = "modifiedBy")
	Dispute convertViolationTicketDtoToDispute (ViolationTicketDTO violationTicketDto);
}
