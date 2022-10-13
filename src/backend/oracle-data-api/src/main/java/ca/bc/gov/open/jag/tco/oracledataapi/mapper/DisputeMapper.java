package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import java.util.ArrayList;
import java.util.List;

import org.apache.commons.lang3.StringUtils;
import org.mapstruct.AfterMapping;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.mapstruct.Named;
import org.mapstruct.factory.Mappers;

import ca.bc.gov.open.jag.tco.oracledataapi.api.model.ViolationTicket;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Plea;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicketCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;
import ca.bc.gov.open.jag.tco.oracledataapi.repository.impl.ords.DisputeRepositoryImpl;

@Mapper
public interface DisputeMapper {

	DisputeMapper INSTANCE = Mappers.getMapper(DisputeMapper.class);

	// Map dispute data from ORDS to Oracle Data API dispute model
	@Mapping(source = "dispute.entDtm", target = "createdTs", dateFormat = DisputeRepositoryImpl.DATE_TIME_FORMAT)
	@Mapping(source = "dispute.entUserId", target = "createdBy")
	@Mapping(source = "dispute.updDtm", target = "modifiedTs", dateFormat = DisputeRepositoryImpl.DATE_TIME_FORMAT)
	@Mapping(source = "dispute.updUserId", target = "modifiedBy")
	@Mapping(source = "dispute.disputeId", target = "disputeId")
	@Mapping(source = "dispute.disputeStatusTypeCd", target = "status", qualifiedByName="mapDisputeStatus")
	//@Mapping(source = "dispute.noticeOfDisputeId", target = "noticeOfDisputeId")
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
	@Mapping(source = "dispute.emailVerifiedYn", target = "emailAddressVerified", qualifiedByName="mapYnToBoolean")
	@Mapping(source = "dispute.filingDt", target = "filingDate")
	@Mapping(source = "dispute.representedByLawyerYn", target = "representedByLawyer")
	@Mapping(source = "dispute.lawFirmNm", target = "lawFirmName")
	@Mapping(target = "lawyerAddress", ignore = true)
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
	@Mapping(source = "dispute.userAssignedDtm", target = "userAssignedTs", dateFormat = DisputeRepositoryImpl.DATE_TIME_FORMAT)
	@Mapping(source = "dispute.disputantDetectOcrIssuesYn", target = "disputantDetectedOcrIssues")
	@Mapping(source = "dispute.disputantOcrIssuesTxt", target = "disputantOcrIssues")
	@Mapping(source = "dispute.systemDetectOcrIssuesYn", target = "systemDetectedOcrIssues")
	@Mapping(source = "dispute.ocrViolationTicketJsonTxt", target = "ocrViolationTicket")
	// Map violation ticket data from ORDS to Oracle Data API violation ticket model
	@Mapping(source = "entUserId", target = "violationTicket.createdBy")
	@Mapping(source = "entDtm", target = "violationTicket.createdTs")
	@Mapping(source = "updUserId", target = "violationTicket.modifiedBy")
	@Mapping(source = "updDtm", target = "violationTicket.modifiedTs")
	@Mapping(source = "violationTicketId", target = "violationTicket.violationTicketId")
	@Mapping(source = "ticketNumberTxt", target = "violationTicket.ticketNumber")
	@Mapping(source = "disputantOrganizationNmTxt", target = "violationTicket.disputantOrganizationName")
	@Mapping(source = "disputantSurnameTxt", target = "violationTicket.disputantSurname")
	@Mapping(source = "disputantGivenNamesTxt", target = "violationTicket.disputantGivenNames")
	@Mapping(source = "isYoungPersonYn", target = "violationTicket.isYoungPerson")
	@Mapping(source = "disputantDrvLicNumberTxt", target = "violationTicket.disputantDriversLicenceNumber")
	@Mapping(source = "disputantClientNumberTxt", target = "violationTicket.disputantClientNumber")
	@Mapping(source = "drvLicIssuedProvinceTxt", target = "violationTicket.driversLicenceProvince")
	@Mapping(source = "drvLicIssuedYearNo", target = "violationTicket.driversLicenceIssuedYear")
	@Mapping(source = "drvLicExpiryYearNo", target = "violationTicket.driversLicenceExpiryYear")
	@Mapping(source = "disputantBirthDt", target = "violationTicket.disputantBirthdate")
	@Mapping(source = "addressTxt", target = "violationTicket.address")
	@Mapping(source = "addressCityTxt", target = "violationTicket.addressCity")
	@Mapping(source = "addressProvinceTxt", target = "violationTicket.addressProvince")
	@Mapping(source = "addressPostalCodeTxt", target = "violationTicket.addressPostalCode")
	@Mapping(source = "addressCountryTxt", target = "violationTicket.addressCountry")
	@Mapping(source = "officerPinTxt", target = "violationTicket.officerPin")
	@Mapping(source = "detachmentLocationTxt", target = "violationTicket.detachmentLocation")
	@Mapping(source = "issuedDt", target = "violationTicket.issuedDate")
	@Mapping(source = "issuedOnRoadOrHighwayTxt", target = "violationTicket.issuedOnRoadOrHighway")
	@Mapping(source = "issuedAtOrNearCityTxt", target = "violationTicket.issuedAtOrNearCity")
	@Mapping(source = "isChangeOfAddressYn", target = "violationTicket.isChangeOfAddress")
	@Mapping(source = "isDriverYn", target = "violationTicket.isDriver")
	@Mapping(source = "isOwnerYn", target = "violationTicket.isOwner")
	@Mapping(source = "courtLocationTxt", target = "violationTicket.courtLocation")
	// Map violation ticket counts data from ORDS to Oracle Data API violation ticket count model
	@Mapping(source = "violationTicketCounts", target = "violationTicket.violationTicketCounts")
	// Map dispute counts data from ORDS to Oracle Data API dispute count model
	@Mapping(source = "violationTicketCounts", target = "disputeCounts", qualifiedByName="mapCounts")
	Dispute convertViolationTicketDtoToDispute (ViolationTicket violationTicketDto);


	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	@Mapping(source = "updDtm", target = "modifiedTs")
	@Mapping(source = "descriptionTxt", target = "description")
	@Mapping(source = "actOrRegulationNameCd", target = "actOrRegulationNameCode")
	@Mapping(source = "isActYn", target = "isAct")
	@Mapping(source = "isRegulationYn", target = "isRegulation")
	@Mapping(source = "statSectionTxt", target = "section")
	@Mapping(source = "statSubSectionTxt", target = "subsection")
	@Mapping(source = "statParagraphTxt", target = "paragraph")
	@Mapping(source = "statSubParagraphTxt", target = "subparagraph")
	@Mapping(source = "ticketedAmt", target = "ticketedAmount")
	ViolationTicketCount convertViolationTicketCountDtoToViolationTicketCount (ca.bc.gov.open.jag.tco.oracledataapi.api.model.ViolationTicketCount violationTicketCountDto);

	/**
	 * Custom mapping for mapping YesNo fields to Boolean value
	 *
	 * @param value
	 * @return Boolean value of {@link YesNo} enum
	 */
	@Named("mapYnToBoolean")
	default Boolean mapYnToBoolean(YesNo value) {
		return YesNo.Y.equals(value);
	}

	@Named("mapDisputeStatus")
	default DisputeStatus mapYnToBoolean(String statusShortCd) {
		DisputeStatus[] values = DisputeStatus.values();
		for (DisputeStatus disputeStatus : values) {
			if (disputeStatus.toShortName().equals(statusShortCd)) {
				return disputeStatus;
			}
		}
		return null;
	}

	/**
	 * Custom mapping for dispute counts
	 *
	 * @param violationTicketCounts
	 * @return list of {@link DisputeCount}
	 */
	@Named("mapCounts")
	default List<DisputeCount> mapCounts(List<ca.bc.gov.open.jag.tco.oracledataapi.api.model.ViolationTicketCount> violationTicketCounts) {
		if ( violationTicketCounts == null || violationTicketCounts.isEmpty()) {
            return null;
        }

		List<DisputeCount> disputeCounts = new ArrayList<DisputeCount>();

		for (ca.bc.gov.open.jag.tco.oracledataapi.api.model.ViolationTicketCount violationTicketCount : violationTicketCounts) {
			ca.bc.gov.open.jag.tco.oracledataapi.api.model.DisputeCount disputeCount = violationTicketCount.getDisputeCount();
			if (disputeCount != null) {
				DisputeCount count = new DisputeCount();
				count.setCreatedBy(disputeCount.getEntUserId());
				count.setCreatedTs(disputeCount.getEntDtm());
				count.setModifiedBy(disputeCount.getUpdUserId());
				count.setModifiedTs(disputeCount.getUpdDtm());
				if (!StringUtils.isBlank(violationTicketCount.getCountNo())) {
					count.setCountNo(Integer.parseInt(violationTicketCount.getCountNo()));
				}
				if (!StringUtils.isBlank(violationTicketCount.getViolationTicketCountId())) {
					count.setDisputeCountId(Long.getLong(violationTicketCount.getViolationTicketCountId()));
				}
				if (disputeCount.getPleaCd() != null) {
					count.setPleaCode(Enum.valueOf(Plea.class, disputeCount.getPleaCd()));
				}
				if (disputeCount.getRequestCourtAppearanceYn() != null) {
					count.setRequestCourtAppearance(Enum.valueOf(YesNo.class, disputeCount.getRequestCourtAppearanceYn()));
				}
				if (disputeCount.getRequestReductionYn() != null) {
					count.setRequestReduction(Enum.valueOf(YesNo.class, disputeCount.getRequestReductionYn()));
				}
				if (disputeCount.getRequestTimeToPayYn() != null) {
					count.setRequestTimeToPay(Enum.valueOf(YesNo.class, disputeCount.getRequestTimeToPayYn()));
				}
				disputeCounts.add(count);
			}
		}
	    return disputeCounts;
	}

	@AfterMapping
    default void setLawyerAddress(@MappingTarget Dispute dispute, ViolationTicket violationTicket) {
		ca.bc.gov.open.jag.tco.oracledataapi.api.model.Dispute disputeFromApi = violationTicket.getDispute();
		if (dispute != null && violationTicket != null && disputeFromApi != null) {
			String addressLine1 = disputeFromApi.getLawFirmAddrLine1Txt();
			String addressLine2 = disputeFromApi.getLawFirmAddrLine2Txt();
			String addressLine3 = disputeFromApi.getLawFirmAddrLine3Txt();
			if (addressLine1 == null && addressLine2 == null && addressLine3 == null) {
				dispute.setLawyerAddress(null);
			} else {
				dispute.setLawyerAddress(
		        		addressLine1 == null ? "" : addressLine1 + " " +
		        		addressLine2 == null ? "" : addressLine2 + " " +
		        		addressLine3 == null ? "" : addressLine3);
			}
		}
    }
}
