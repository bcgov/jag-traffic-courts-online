package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.Named;

import ca.bc.gov.open.jag.tco.oracledataapi.model.ContactType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceRoP;
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
	@Mapping(source = "courtAppearances", target = "jjDisputeCourtAppearanceRoPs")
	@Mapping(source = "detachmentLocationTxt", target = "policeDetachment")
	@Mapping(source = "disputantBirthDt", target = "disputantBirthdate")
	@Mapping(source = "disputantDrvLicNumberTxt", target = "driversLicenceNumber")
	@Mapping(source = "disputeCounts", target = "jjDisputedCounts")
	@Mapping(source = "disputeId", target = "id")
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
	@Mapping(source = "appearanceChargeCountId", target = "jjDisputedCountRoP.id")
	@Mapping(source = "findingResultCd", target = "jjDisputedCountRoP.finding", qualifiedByName="mapFindingResult")
	@Mapping(source = "lesserChargeDescTxt", target = "jjDisputedCountRoP.lesserDescription")
	@Mapping(source = "suspSntcProbationDurtnTxt", target = "jjDisputedCountRoP.ssProbationDuration")
	@Mapping(source = "suspSntcProbationCondsTxt", target = "jjDisputedCountRoP.ssProbationConditions")
	@Mapping(source = "jailDurationTxt", target = "jjDisputedCountRoP.jailDuration")
	@Mapping(source = "jailIntermittentYn", target = "jjDisputedCountRoP.jailIntermittent")
	@Mapping(source = "probationDurationTxt", target = "jjDisputedCountRoP.probationDuration")
	@Mapping(source = "probationConditionsTxt", target = "jjDisputedCountRoP.probationConditions")
	@Mapping(source = "drivingProhibDurationTxt", target = "jjDisputedCountRoP.drivingProhibition")
	@Mapping(source = "drivingProhibMvaSectionTxt", target = "jjDisputedCountRoP.drivingProhibitionMVASection")
	@Mapping(source = "dismissedYn", target = "jjDisputedCountRoP.dismissed")
	@Mapping(source = "dismissedForWantProsecYn", target = "jjDisputedCountRoP.forWantOfProsecution")
	@Mapping(source = "withdrawnYn", target = "jjDisputedCountRoP.withdrawn")
	@Mapping(source = "abatementYn", target = "jjDisputedCountRoP.abatement")
	@Mapping(source = "stayOfProceedingsByTxt", target = "jjDisputedCountRoP.stayOfProceedingsBy")
	@Mapping(source = "otherTxt", target = "jjDisputedCountRoP.other")
	@Mapping(source = "accEntDtm", target = "jjDisputedCountRoP.createdTs")
	@Mapping(source = "accEntUserId", target = "jjDisputedCountRoP.createdBy")
	@Mapping(source = "accUpdDtm", target = "jjDisputedCountRoP.modifiedTs")
	@Mapping(source = "accUpdUserId", target = "jjDisputedCountRoP.modifiedBy")
	public abstract JJDisputedCount convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeCount jjDisputeCount);

	/** Convert TCO ORDS -> Oracle Data API */
	@Mapping(source = "disputeRemarkId", target = "id")
	@Mapping(source = "disputeId", target = "jjDispute.id")
	@Mapping(source = "disputeRemarkTxt", target = "note")
	@Mapping(source = "fullUserNameTxt", target = "userFullName")
	@Mapping(source = "remarksMadeDtm", target = "remarksMadeTs")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "updDtm", target = "modifiedTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	public abstract JJDisputeRemark convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeRemark jjDisputeRemark);

	/** Convert Oracle Data API -> TCO*/
	@Mapping(source = "id", target = "disputeRemarkId")
	@Mapping(source = "jjDispute.id", target = "disputeId")
	@Mapping(source = "note", target = "disputeRemarkTxt")
	@Mapping(source = "userFullName", target = "fullUserNameTxt")
	@Mapping(source = "remarksMadeTs", target = "remarksMadeDtm")
	@Mapping(source = "createdBy", target = "entDtm")
	@Mapping(source = "createdTs", target = "entUserId")
	@Mapping(source = "modifiedBy", target = "updUserId")
	@Mapping(source = "modifiedTs", target = "updDtm")
	public abstract ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeRemark convert(JJDisputeRemark jjDisputeRemark);

	@Mapping(source = "appearanceDtm", target = "appearanceTs")
	@Mapping(source = "appearanceReasonTxt", target = "reason")
	@Mapping(source = "commentsTxt", target = "comments")
	@Mapping(source = "courtroomNumberTxt", target = "room")
	@Mapping(source = "courtAppearanceId", target = "id")
	@Mapping(source = "crownPresenceCd", target = "crown")
	@Mapping(source = "defenceCounselNameTxt", target = "defenceCounsel")
	@Mapping(source = "defenceCounselPresenceCd", target = "dattCd")
	@Mapping(source = "disputeId", target = "jjDispute.id")
	@Mapping(source = "disputantPresenceCd", target = "appCd")
	@Mapping(source = "disputantNotPresentDtm", target = "noAppTs")
	@Mapping(source = "judgeOrJjNameTxt", target = "adjudicator")
	@Mapping(source = "recordingClerkNameTxt", target = "clerkRecord")
	@Mapping(source = "seizedYn", target = "jjSeized")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "updDtm", target = "modifiedTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	public abstract JJDisputeCourtAppearanceRoP convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJCourtAppearance jjCourtAppearance);

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

}
