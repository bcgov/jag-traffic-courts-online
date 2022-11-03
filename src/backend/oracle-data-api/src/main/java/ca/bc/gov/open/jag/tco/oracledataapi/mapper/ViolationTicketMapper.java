package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import java.util.List;

import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.Named;
import org.mapstruct.factory.Mappers;

import ca.bc.gov.open.jag.tco.oracledataapi.api.model.ViolationTicket;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicketCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords.DisputeRepositoryImpl;

@Mapper
public interface ViolationTicketMapper {

	ViolationTicketMapper INSTANCE = Mappers.getMapper(ViolationTicketMapper.class);

	// Map from Oracle Data API dispute model to ORDS dispute data
	@Mapping(target = "dispute.entDtm", source = "createdTs", dateFormat = DisputeRepositoryImpl.DATE_TIME_FORMAT)
	@Mapping(target = "dispute.entUserId", source = "createdBy")
	@Mapping(target = "dispute.updDtm", source = "modifiedTs", dateFormat = DisputeRepositoryImpl.DATE_TIME_FORMAT)
	@Mapping(target = "dispute.updUserId", source = "modifiedBy")
	@Mapping(target = "dispute.disputeId", source = "disputeId")
	@Mapping(target = "dispute.disputeStatusTypeCd", source = "status", qualifiedByName="mapDisputeStatus")
	@Mapping(target = "dispute.courtLocationTxt", source = "courtLocation")
	@Mapping(target = "dispute.issuedDt", source = "issuedTs")
	@Mapping(target = "dispute.submittedDt", source = "submittedDate")
	@Mapping(target = "dispute.disputantSurnameNm", source = "disputantSurname")
	@Mapping(target = "dispute.disputantGiven1Nm", source = "disputantGivenName1")
	@Mapping(target = "dispute.disputantGiven2Nm", source = "disputantGivenName2")
	@Mapping(target = "dispute.disputantGiven3Nm", source = "disputantGivenName3")
	@Mapping(target = "dispute.disputantBirthDt", source = "disputantBirthdate")
	@Mapping(target = "dispute.disputantOrganizationNm", source = "disputantOrganization")
	@Mapping(target = "dispute.disputantDrvLicNumberTxt", source = "driversLicenceNumber")
	@Mapping(target = "dispute.drvLicIssuedIntlProvTxt", source = "driversLicenceProvince")
	@Mapping(target = "dispute.addressLine1Txt", source = "addressLine1")
	@Mapping(target = "dispute.addressLine2Txt", source = "addressLine2")
	@Mapping(target = "dispute.addressLine3Txt", source = "addressLine3")
	@Mapping(target = "dispute.addressIntlCityTxt", source = "addressCity")
	@Mapping(target = "dispute.addressIntlProvTxt", source = "addressProvince")
	@Mapping(target = "dispute.postalCodeTxt", source = "postalCode")
	@Mapping(target = "dispute.homePhoneNumberTxt", source = "homePhoneNumber")
	@Mapping(target = "dispute.workPhoneNumberTxt", source = "workPhoneNumber")
	@Mapping(target = "dispute.emailAddressTxt", source = "emailAddress")
	@Mapping(target = "dispute.noticeOfDisputeGuid", source = "noticeOfDisputeGuid")
	@Mapping(target = "dispute.emailVerifiedYn", source = "emailAddressVerified", qualifiedByName="mapBooleanToYn")
	@Mapping(target = "dispute.filingDt", source = "filingDate")
	@Mapping(target = "dispute.representedByLawyerYn", source = "representedByLawyer")
	@Mapping(target = "dispute.lawFirmNm", source = "lawFirmName")
	// TODO - need add an after mapping method to parse address source into multiple address fields
	@Mapping(target = "dispute.lawFirmAddrLine1Txt", ignore = true)
	@Mapping(target = "dispute.lawFirmAddrLine2Txt", ignore = true)
	@Mapping(target = "dispute.lawFirmAddrLine3Txt", ignore = true)
	@Mapping(target = "dispute.lawyerSurnameNm", source = "lawyerSurname")
	@Mapping(target = "dispute.lawyerGiven1Nm", source = "lawyerGivenName1")
	@Mapping(target = "dispute.lawyerGiven2Nm", source = "lawyerGivenName2")
	@Mapping(target = "dispute.lawyerGiven3Nm", source = "lawyerGivenName3")
	@Mapping(target = "dispute.lawyerPhoneNumberTxt", source = "lawyerPhoneNumber")
	@Mapping(target = "dispute.lawyerEmailAddressTxt", source = "lawyerEmail")
	@Mapping(target = "dispute.officerPinTxt", source = "officerPin")
	@Mapping(target = "dispute.detachmentLocationTxt", source = "detachmentLocation")
	@Mapping(target = "dispute.interpreterRequiredYn", source = "interpreterRequired")
	@Mapping(target = "dispute.witnessNo", source = "witnessNo")
	@Mapping(target = "dispute.fineReductionReasonTxt", source = "fineReductionReason")
	@Mapping(target = "dispute.timeToPayReasonTxt", source = "timeToPayReason")
	@Mapping(target = "dispute.disputantCommentTxt", source = "disputantComment")
	@Mapping(target = "dispute.rejectedReasonTxt", source = "rejectedReason")
	@Mapping(target = "dispute.userAssignedTo", source = "userAssignedTo")
	@Mapping(target = "dispute.userAssignedDtm", source = "userAssignedTs", dateFormat = DisputeRepositoryImpl.DATE_TIME_FORMAT)
	@Mapping(target = "dispute.disputantDetectOcrIssuesYn", source = "disputantDetectedOcrIssues")
	@Mapping(target = "dispute.disputantOcrIssuesTxt", source = "disputantOcrIssues")
	@Mapping(target = "dispute.systemDetectOcrIssuesYn", source = "systemDetectedOcrIssues")
	@Mapping(target = "dispute.ocrViolationTicketJsonTxt", source = "ocrViolationTicket")
	// TODO - need replace the default constant values below to set the IDs from the actual dispute model source from request
	@Mapping(target = "dispute.addressCtryId", constant = "1")
	@Mapping(target = "dispute.drvLicIssuedCtryId", constant = "1")
	@Mapping(target = "dispute.drvLicIssuedProvSeqNo", constant = "1")
	@Mapping(target = "dispute.addressProvCtryId", constant = "1")
	@Mapping(target = "dispute.addressProvSeqNo", constant = "1")
	// Map from Oracle Data API violation ticket model to ORDS violation ticket data
	@Mapping(target = "entUserId", source = "violationTicket.createdBy")
	@Mapping(target = "entDtm", source = "violationTicket.createdTs")
	@Mapping(target = "updUserId", source = "violationTicket.modifiedBy")
	@Mapping(target = "updDtm", source = "violationTicket.modifiedTs")
	@Mapping(target = "violationTicketId", source = "violationTicket.violationTicketId")
	@Mapping(target = "ticketNumberTxt", source = "violationTicket.ticketNumber")
	@Mapping(target = "disputantOrganizationNmTxt", source = "violationTicket.disputantOrganizationName")
	@Mapping(target = "disputantSurnameTxt", source = "violationTicket.disputantSurname")
	@Mapping(target = "disputantGivenNamesTxt", source = "violationTicket.disputantGivenNames")
	@Mapping(target = "isYoungPersonYn", source = "violationTicket.isYoungPerson")
	@Mapping(target = "disputantDrvLicNumberTxt", source = "violationTicket.disputantDriversLicenceNumber")
	@Mapping(target = "disputantClientNumberTxt", source = "violationTicket.disputantClientNumber")
	@Mapping(target = "drvLicIssuedProvinceTxt", source = "violationTicket.driversLicenceProvince")
	@Mapping(target = "drvLicIssuedYearNo", source = "violationTicket.driversLicenceIssuedYear")
	@Mapping(target = "drvLicExpiryYearNo", source = "violationTicket.driversLicenceExpiryYear")
	@Mapping(target = "disputantBirthDt", source = "violationTicket.disputantBirthdate")
	@Mapping(target = "addressTxt", source = "violationTicket.address")
	@Mapping(target = "addressCityTxt", source = "violationTicket.addressCity")
	@Mapping(target = "addressProvinceTxt", source = "violationTicket.addressProvince")
	@Mapping(target = "addressPostalCodeTxt", source = "violationTicket.addressPostalCode")
	@Mapping(target = "addressCountryTxt", source = "violationTicket.addressCountry")
	@Mapping(target = "officerPinTxt", source = "violationTicket.officerPin")
	@Mapping(target = "detachmentLocationTxt", source = "violationTicket.detachmentLocation")
	@Mapping(target = "issuedDt", source = "violationTicket.issuedTs")
	@Mapping(target = "issuedOnRoadOrHighwayTxt", source = "violationTicket.issuedOnRoadOrHighway")
	@Mapping(target = "issuedAtOrNearCityTxt", source = "violationTicket.issuedAtOrNearCity")
	@Mapping(target = "isChangeOfAddressYn", source = "violationTicket.isChangeOfAddress")
	@Mapping(target = "isDriverYn", source = "violationTicket.isDriver")
	@Mapping(target = "isOwnerYn", source = "violationTicket.isOwner")
	@Mapping(target = "courtLocationTxt", source = "violationTicket.courtLocation")
	// Map from Oracle Data API violation ticket count model to ORDS violation ticket counts data
	@Mapping(target = "violationTicketCounts", source = "violationTicket.violationTicketCounts")
	ViolationTicket convertDisputeToViolationTicketDto (Dispute dispute);

	@Mapping(target = "entUserId", source = "createdBy")
	@Mapping(target = "entDtm", source = "createdTs")
	@Mapping(target = "updUserId", source = "modifiedBy")
	@Mapping(target = "updDtm", source = "modifiedTs")
	@Mapping(target = "descriptionTxt", source = "description")
	@Mapping(target = "actOrRegulationNameCd", source = "actOrRegulationNameCode")
	@Mapping(target = "isActYn", source = "isAct")
	@Mapping(target = "isRegulationYn", source = "isRegulation")
	@Mapping(target = "statSectionTxt", source = "section")
	// TODO - need add an after mapping method to split full section into the subsection, paragraph etc. fields below
	@Mapping(target = "statSubSectionTxt", source = "subsection")
	@Mapping(target = "statParagraphTxt", source = "paragraph")
	@Mapping(target = "statSubParagraphTxt", source = "subparagraph")
	@Mapping(target = "ticketedAmt", source = "ticketedAmount")
	// Map from Oracle Data API dispute count model to ORDS dispute counts data
	@Mapping(target = "disputeCount", source = "violationTicketCount", qualifiedByName="mapDisputeCount")
	ca.bc.gov.open.jag.tco.oracledataapi.api.model.ViolationTicketCount convertViolationTicketCountToViolationTicketCountDto (ViolationTicketCount violationTicketCount);

	/**
	 * Custom mapping for mapping Boolean value to YesNo fields
	 *
	 * @param value
	 * @return {@link YesNo} enum value of Boolean
	 */
	@Named("mapBooleanToYn")
	default YesNo mapBooleanToYn(Boolean value) {
		if (value) {
			return YesNo.Y;
		}
		return YesNo.N;
	}

	@Named("mapDisputeStatus")
	default String mapDisputeStatus(DisputeStatus status) {
		return status.toShortName();
	}

	/**
	 * Custom mapping for dispute count
	 *
	 * @param violationTicketCount
	 * @return {@link ca.bc.gov.open.jag.tco.oracledataapi.api.model.DisputeCount}
	 */
	@Named("mapDisputeCount")
	default ca.bc.gov.open.jag.tco.oracledataapi.api.model.DisputeCount mapDisputeCount(ViolationTicketCount violationTicketCount) {
		int countNo = violationTicketCount.getCountNo();

		if ( violationTicketCount == null
				|| violationTicketCount.getViolationTicket() == null
				|| violationTicketCount.getViolationTicket().getDispute() == null
				|| violationTicketCount.getViolationTicket().getDispute().getDisputeCounts() == null
				|| violationTicketCount.getViolationTicket().getDispute().getDisputeCounts().isEmpty()) {
            return null;
        }

		List<DisputeCount> disputeCounts = violationTicketCount.getViolationTicket().getDispute().getDisputeCounts();

		for (DisputeCount disputeCount : disputeCounts) {
			if (disputeCount.getCountNo() == countNo) {
				ca.bc.gov.open.jag.tco.oracledataapi.api.model.DisputeCount count = new ca.bc.gov.open.jag.tco.oracledataapi.api.model.DisputeCount();
				count.setEntDtm(disputeCount.getCreatedTs());
				count.setEntUserId(disputeCount.getCreatedBy());
				count.setUpdDtm(disputeCount.getModifiedTs());
				count.setUpdUserId(disputeCount.getModifiedBy());

				if (disputeCount.getPleaCode() != null) {
					count.setPleaCd(disputeCount.getPleaCode().toString());
				}
				if (disputeCount.getRequestCourtAppearance() != null) {
					count.setRequestCourtAppearanceYn(disputeCount.getRequestCourtAppearance().toString());
				}
				if (disputeCount.getRequestReduction() != null) {
					count.setRequestReductionYn(disputeCount.getRequestReduction().toString());
				}
				if (disputeCount.getRequestTimeToPay() != null) {
					count.setRequestTimeToPayYn(disputeCount.getRequestTimeToPay().toString());
				}
				return count;
			}
		}
	    return null;
	}
}
