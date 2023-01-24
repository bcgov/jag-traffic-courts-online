package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.apache.commons.lang3.StringUtils;
import org.mapstruct.AfterMapping;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.mapstruct.Named;

import ca.bc.gov.open.jag.tco.oracledataapi.model.ContactType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeHearingType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeRemark;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputedCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputedCountFinding;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Plea;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR) // This is required for tests to work
public abstract class JJDisputeMapper {

	@Mapping(source = "addressLine1Txt", target = "addressLine1")
	@Mapping(source = "addressLine2Txt", target = "addressLine2")
	@Mapping(source = "addressLine3Txt", target = "addressLine3")
	@Mapping(source = "addressCityTxt", target = "addressCity")
	@Mapping(source = "addressProvinceTxt", target = "addressProvince")
	@Mapping(source = "addressCountryTxt", target = "addressCountry")
	@Mapping(source = "addressPostalCodeTxt", target = "addressPostalCode")
	@Mapping(source = "courtHearingTypeCd", target = "hearingType", qualifiedByName="mapHearingType")
	@Mapping(source = "contactLawFirmNm", target = "contactLawFirmName")
	@Mapping(source = "contactGiven1Nm", target = "contactGivenName1")
	@Mapping(source = "contactGiven2Nm", target = "contactGivenName2")
	@Mapping(source = "contactGiven3Nm", target = "contactGivenName3")
	@Mapping(source = "contactSurnameNm", target = "contactSurname")
	@Mapping(source = "contactTypeCd", target = "contactType", qualifiedByName="mapContactType")
	@Mapping(source = "courtAgenId", target = "courtAgenId")
//	@Mapping(source = "courtAppearances", target = "courtAppearances")                 // TODO: map courtAppearances
	@Mapping(source = "detachmentLocationTxt", target = "policeDetachment")
	@Mapping(source = "disputantBirthDt", target = "disputantBirthdate")
	@Mapping(source = "disputantDrvLicNumberTxt", target = "disputantDrvLicNumber")
	@Mapping(source = "disputantGiven1Nm", target = "givenNames")
	@Mapping(source = "disputantSurnameTxt", target = "surname")
	@Mapping(source = "disputeCounts", target = "jjDisputedCounts")
//	 @Mapping(source = "disputeId", target = "jjDisputeId")                            // TODO: create new PK
	@Mapping(source = "disputeRemarks", target = "remarks")
	@Mapping(source = "disputeStatusTypeCd", target = "status", qualifiedByName="mapDisputeStatus")
	@Mapping(source = "drvLicIssuedProvSeqNo", target = "drvLicIssuedProvSeqNo")
	@Mapping(source = "drvLicIssuedCtryId", target = "drvLicIssuedCtryId")
	@Mapping(source = "emailAddressTxt", target = "emailAddress")
	@Mapping(source = "fineReductionReasonTxt", target = "fineReductionReason")
	@Mapping(source = "jjAssignedTo", target = "jjAssignedTo")
	@Mapping(source = "jjDecisionDt", target = "jjDecisionDate")
	@Mapping(source = "justinRccId", target = "justinRccId")
	@Mapping(source = "icbcReceivedDt", target = "icbcReceivedDate")
	@Mapping(source = "interpreterLanguageCd", target = "interpreterLanguageCd")
	@Mapping(source = "issuedTs", target = "issuedTs")
	@Mapping(source = "lawFirmNm", target = "lawFirmName")
	@Mapping(source = "lawyerGiven1Nm", target = "lawyerGivenName1")
	@Mapping(source = "lawyerGiven2Nm", target = "lawyerGivenName2")
	@Mapping(source = "lawyerGiven3Nm", target = "lawyerGivenName3")
	@Mapping(source = "lawyerSurnameNm", target = "lawyerSurname")
	@Mapping(source = "noticeOfDisputeGuid", target = "noticeOfDisputeGuid")
	@Mapping(source = "occamDisputantGiven1Nm", target = "occamDisputantGiven1Nm")
	@Mapping(source = "occamDisputantGiven2Nm", target = "occamDisputantGiven2Nm")
	@Mapping(source = "occamDisputantGiven3Nm", target = "occamDisputantGiven3Nm")
	@Mapping(source = "occamDisputantSurnameNm", target = "occamDisputantSurnameNm")
	@Mapping(source = "occamDisputeId", target = "occamDisputeId")
	@Mapping(source = "occamViolationTicketUpldId", target = "occamViolationTicketUpldId")
	@Mapping(source = "offenceLocationTxt", target = "offenceLocation")
	@Mapping(source = "submittedDt", target = "submittedTs")
	@Mapping(source = "ticketNumberTxt", target = "ticketNumber")
	@Mapping(source = "timeToPayReasonTxt", target = "timeToPayReason")
	@Mapping(source = "violationDt", target = "violationDate")
	@Mapping(source = "vtcAssignedTo", target = "vtcAssignedTo")
	@Mapping(source = "vtcAssignedDtm", target = "vtcAssignedTs")
	@Mapping(source = "witnessNo", target = "witnessNo")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "updDtm", target = "modifiedTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	public abstract JJDispute convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute jjDispute);

	@Mapping(source = "disputeCountId", target = "id")
	@Mapping(source = "countNo", target = "count")
	@Mapping(source = "statuteId", target = "description")                              // TODO: adjust mapping to statuteId
	@Mapping(source = "pleaCd", target = "plea", qualifiedByName="mapPlea")
	@Mapping(source = "ticketedAmt", target = "ticketedFineAmount")
	@Mapping(source = "fineDueDt", target = "dueDate")
	@Mapping(source = "violationDt", target = "violationDate")
	@Mapping(source = "adjustedAmt", target = "lesserOrGreaterAmount")
	@Mapping(source = "includesSurchargeYn", target = "includesSurcharge")
	@Mapping(source = "revisedDueDt", target = "revisedDueDate")
	@Mapping(source = "totalFineAmt", target = "totalFineAmount")
	@Mapping(source = "commentsTxt", target = "comments")
	@Mapping(source = "requestTimeToPayYn", target = "requestTimeToPay")
	@Mapping(source = "requestReductionYn", target = "requestReduction")
	@Mapping(source = "requestCourtAppearanceYn", target = "appearInCourt")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "updDtm", target = "modifiedTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	@Mapping(source = "appearanceChargeCountId", target = "appearanceChargeCountId")
	@Mapping(source = "courtAppearanceId", target = "courtAppearanceId")
	@Mapping(source = "findingResultCd", target = "findingResultCd", qualifiedByName="mapFindingResult")
	@Mapping(source = "lesserChargeDescTxt", target = "lesserChargeDesc")
	@Mapping(source = "suspSntcProbationDurtnTxt", target = "suspSntcProbationDurtn")
	@Mapping(source = "suspSntcProbationCondsTxt", target = "suspSntcProbationConds")
	@Mapping(source = "jailDurationTxt", target = "jailDuration")
	@Mapping(source = "jailIntermittentYn", target = "jailIntermittent")
	@Mapping(source = "probationDurationTxt", target = "probationDuration")
	@Mapping(source = "probationConditionsTxt", target = "probationConditions")
	@Mapping(source = "drivingProhibDurationTxt", target = "drivingProhibDuration")
	@Mapping(source = "drivingProhibMvaSectionTxt", target = "drivingProhibMvaSection")
	@Mapping(source = "dismissedYn", target = "dismissed")
	@Mapping(source = "dismissedForWantProsecYn", target = "dismissedForWantProsec")
	@Mapping(source = "withdrawnYn", target = "withdrawn")
	@Mapping(source = "abatementYn", target = "abatement")
	@Mapping(source = "stayOfProceedingsByTxt", target = "stayOfProceedingsBy")
	@Mapping(source = "otherTxt", target = "otherTxt")
	@Mapping(source = "remarksTxt", target = "remarksTxt")
	@Mapping(source = "accEntDtm", target = "accEntDtm")
	@Mapping(source = "accEntUserId", target = "accEntUserId")
	@Mapping(source = "accUpdDtm", target = "accUpdDtm")
	@Mapping(source = "accUpdUserId", target = "accUpdUserId")
	public abstract JJDisputedCount convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeCount jjDisputeCount);

	@Mapping(source = "disputeRemarkId", target = "id")
//	@Mapping(source = "disputeId", target = "jjDispute.disputeId")                      // TODO: field missing in model but exists in database
	@Mapping(source = "disputeRemarkTxt", target = "note")
	@Mapping(source = "fullUserNameTxt", target = "userFullName")
//	@Mapping(source = "remarksMadeDtm", target = "remarksMadeDtm")                      // TODO: field missing in model but exists in database
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "updDtm", target = "modifiedTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	public abstract JJDisputeRemark convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeRemark jjDisputeRemark);

	@Named("mapContactType")
	public ContactType mapContactType(String statusShortCd) {
		ContactType[] values = ContactType.values();
		for (ContactType contactType : values) {
			if (contactType.getShortName().equals(statusShortCd)) {
				return contactType;
			}
		}
		return null;
	}

	@Named("mapDisputeStatus")
	public JJDisputeStatus mapDisputeStatus(String statusShortCd) {
		JJDisputeStatus[] values = JJDisputeStatus.values();
		for (JJDisputeStatus statusType : values) {
			if (statusType.getShortName().equals(statusShortCd)) {
				return statusType;
			}
		}
		return null;
	}

	@Named("mapFindingResult")
	public JJDisputedCountFinding mapFindingResult(String statusShortCd) {
		JJDisputedCountFinding[] values = JJDisputedCountFinding.values();
		for (JJDisputedCountFinding findingCd : values) {
			if (findingCd.getShortName().equals(statusShortCd)) {
				return findingCd;
			}
		}
		return null;
	}

	@Named("mapHearingType")
	public JJDisputeHearingType mapHearingType(String statusShortCd) {
		JJDisputeHearingType[] values = JJDisputeHearingType.values();
		for (JJDisputeHearingType hearingType : values) {
			if (hearingType.getShortName().equals(statusShortCd)) {
				return hearingType;
			}
		}
		return null;
	}

	@Named("mapPlea")
	public Plea mapPlea(String statusShortCd) {
		Plea[] values = Plea.values();
		for (Plea hearingType : values) {
			if (hearingType.getShortName().equals(statusShortCd)) {
				return hearingType;
			}
		}
		return null;
	}

	/**
	 * Given Names in TCO are stored as 3 separate fields, but the application requires this to be a single field.
	 * @param source data coming from TCO oracle schema
	 * @param target data mapped to the application's model
	 */
	@AfterMapping
	protected void mapGivenNames(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute source, @MappingTarget JJDispute target) {
		StringBuffer givenNames = new StringBuffer();

		if (!StringUtils.isBlank(source.getDisputantGiven1Nm())) {
			givenNames.append(source.getDisputantGiven1Nm().trim());
			givenNames.append(" ");
		}

		if (!StringUtils.isBlank(source.getDisputantGiven2Nm())) {
			givenNames.append(source.getDisputantGiven2Nm().trim());
			givenNames.append(" ");
		}

		if (!StringUtils.isBlank(source.getDisputantGiven3Nm())) {
			givenNames.append(source.getDisputantGiven3Nm().trim());
			givenNames.append(" ");
		}

		target.setGivenNames(givenNames.toString().trim());
	}
}
