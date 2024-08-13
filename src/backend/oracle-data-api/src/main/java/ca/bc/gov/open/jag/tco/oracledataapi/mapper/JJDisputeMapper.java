package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.mapstruct.AfterMapping;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.mapstruct.Named;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeCourtAppearanceRoP;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeRemark;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputedCount;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR) // This is required for tests to work
public abstract class JJDisputeMapper extends BaseMapper {

	@Mapping(source = "accidentYn", target = "accidentYn")
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
	@Mapping(source = "requestCourtAppearanceYn", target = "appearInCourt")
	@Mapping(source = "courtAgenId", target = "courtAgenId")
	@Mapping(source = "courtAppearances", target = "jjDisputeCourtAppearanceRoPs")
	@Mapping(source = "detachmentLocationTxt", target = "policeDetachment")
	@Mapping(source = "disputantBirthDt", target = "disputantBirthdate")
	@Mapping(source = "disputantDrvLicNumberTxt", target = "driversLicenceNumber")
	@Mapping(source = "disputantPhoneNumberTxt", target = "occamDisputantPhoneNumber")
	@Mapping(source = "disputantSurnameTxt", target = "disputantSurname")
	@Mapping(source = "disputantGiven1Nm", target = "disputantGivenName1")
	@Mapping(source = "disputantGiven2Nm", target = "disputantGivenName2")
	@Mapping(source = "disputantGiven3Nm", target = "disputantGivenName3")
	@Mapping(source = "disputeCounts", target = "jjDisputedCounts")
	@Mapping(source = "disputeId", target = "id")
	@Mapping(source = "disputeRemarks", target = "remarks")
	@Mapping(source = "disputeStatusTypeCd", target = "status", qualifiedByName="mapJJDisputeStatus")
	@Mapping(source = "drvLicIssuedProvSeqNo", target = "drvLicIssuedProvSeqNo")
	@Mapping(source = "drvLicIssuedCtryId", target = "drvLicIssuedCtryId")
	@Mapping(source = "electronicTicketYn", target = "electronicTicketYn")
	@Mapping(source = "emailAddressTxt", target = "emailAddress")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "fineReductionReasonTxt", target = "fineReductionReason")
	@Mapping(source = "jjAssignedTo", target = "jjAssignedTo")
	@Mapping(source = "decisionMadeBy", target = "decisionMadeBy")
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
	@Mapping(source = "multipleOfficersYn", target = "multipleOfficersYn")
	@Mapping(source = "noticeOfDisputeGuid", target = "noticeOfDisputeGuid")
	@Mapping(source = "noticeOfHearingYn", target = "noticeOfHearingYn")
	@Mapping(source = "occamDisputantGiven1Nm", target = "occamDisputantGiven1Nm")
	@Mapping(source = "occamDisputantGiven2Nm", target = "occamDisputantGiven2Nm")
	@Mapping(source = "occamDisputantGiven3Nm", target = "occamDisputantGiven3Nm")
	@Mapping(source = "occamDisputantSurnameNm", target = "occamDisputantSurnameNm")
	@Mapping(source = "occamDisputeId", target = "occamDisputeId")
	@Mapping(source = "occamViolationTicketUpldId", target = "occamViolationTicketUpldId")
	@Mapping(source = "offenceLocationTxt", target = "offenceLocation")
	@Mapping(source = "signatoryNameTxt", target = "signatoryName")
	@Mapping(source = "signatoryTypeCd", target = "signatoryType")
	@Mapping(source = "submittedDt", target = "submittedTs")
	@Mapping(source = "ticketNumberTxt", target = "ticketNumber")
	@Mapping(source = "timeToPayReasonTxt", target = "timeToPayReason")
	@Mapping(source = "updDtm", target = "modifiedTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	@Mapping(source = "violationDt", target = "violationDate")
	@Mapping(source = "vtcAssignedTo", target = "vtcAssignedTo")
	@Mapping(source = "vtcAssignedDtm", target = "vtcAssignedTs")
	@Mapping(source = "witnessNo", target = "witnessNo")
	public abstract JJDispute convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute jjDispute);
	
	@Mapping(source = "accidentYn", target = "accidentYn")
	@Mapping(source = "addressLine1", target = "addressLine1Txt")
	@Mapping(source = "addressLine2", target = "addressLine2Txt")
	@Mapping(source = "addressLine3", target = "addressLine3Txt")
	@Mapping(source = "addressCity", target = "addressCityTxt")
	@Mapping(source = "addressCountry", target = "addressCountryTxt")
	@Mapping(source = "addressPostalCode", target = "addressPostalCodeTxt")
	@Mapping(source = "addressProvince", target = "addressProvinceTxt")
	@Mapping(source = "contactLawFirmName", target = "contactLawFirmNm")
	@Mapping(source = "contactGivenName1", target = "contactGiven1Nm")
	@Mapping(source = "contactGivenName2", target = "contactGiven2Nm")
	@Mapping(source = "contactGivenName3", target = "contactGiven3Nm")
	@Mapping(source = "contactSurname", target = "contactSurnameNm")
	@Mapping(source = "contactType", target = "contactTypeCd", qualifiedByName="mapShortNamedEnum")
	@Mapping(source = "courtAgenId", target = "courtAgenId")
	@Mapping(source = "createdBy", target = "entUserId")
	@Mapping(source = "createdTs", target = "entDtm")
	@Mapping(source = "disputantBirthdate", target = "disputantBirthDt")
	@Mapping(target = "disputantSurnameTxt", ignore = true) // ignore back reference mapping
	@Mapping(target = "disputantGiven1Nm", ignore = true) // ignore back reference mapping
	@Mapping(target = "disputantGiven2Nm", ignore = true) // ignore back reference mapping
	@Mapping(target = "disputantGiven3Nm", ignore = true) // ignore back reference mapping
	@Mapping(target = "disputantPhoneNumberTxt", ignore = true) // ignore back reference mapping
	@Mapping(source = "driversLicenceNumber", target = "disputantDrvLicNumberTxt")
	@Mapping(source = "drvLicIssuedCtryId", target = "drvLicIssuedCtryId")
	@Mapping(source = "drvLicIssuedProvSeqNo", target = "drvLicIssuedProvSeqNo")
	@Mapping(source = "electronicTicketYn", target = "electronicTicketYn")
	@Mapping(source = "emailAddress", target = "emailAddressTxt")
	@Mapping(source = "fineReductionReason", target = "fineReductionReasonTxt")
	@Mapping(source = "hearingType", target = "courtHearingTypeCd", qualifiedByName="mapShortNamedEnum")
	@Mapping(source = "icbcReceivedDate", target = "icbcReceivedDt")
	@Mapping(source = "id", target = "disputeId")
	@Mapping(source = "interpreterLanguageCd", target = "interpreterLanguageCd")
	@Mapping(source = "issuedTs", target = "issuedTs")
	@Mapping(source = "jjAssignedTo", target = "jjAssignedTo")
	@Mapping(source = "decisionMadeBy", target = "decisionMadeBy")
	@Mapping(source = "jjDecisionDate", target = "jjDecisionDt")
	@Mapping(source = "jjDisputeCourtAppearanceRoPs", target = "courtAppearances")
	@Mapping(source = "jjDisputedCounts", target = "disputeCounts")
	@Mapping(source = "justinRccId", target = "justinRccId")
	@Mapping(source = "lawFirmName", target = "lawFirmNm")
	@Mapping(source = "lawyerGivenName1", target = "lawyerGiven1Nm")
	@Mapping(source = "lawyerGivenName2", target = "lawyerGiven2Nm")
	@Mapping(source = "lawyerGivenName3", target = "lawyerGiven3Nm")
	@Mapping(source = "lawyerSurname", target = "lawyerSurnameNm")
	@Mapping(source = "modifiedBy", target = "updUserId")
	@Mapping(source = "modifiedTs", target = "updDtm")
	@Mapping(source = "multipleOfficersYn", target = "multipleOfficersYn")
	@Mapping(source = "noticeOfDisputeGuid", target = "noticeOfDisputeGuid")
	@Mapping(source = "noticeOfHearingYn", target = "noticeOfHearingYn")
	@Mapping(source = "occamDisputantGiven1Nm", target = "occamDisputantGiven1Nm")
	@Mapping(source = "occamDisputantGiven2Nm", target = "occamDisputantGiven2Nm")
	@Mapping(source = "occamDisputantGiven3Nm", target = "occamDisputantGiven3Nm")
	@Mapping(source = "occamDisputantSurnameNm", target = "occamDisputantSurnameNm")
	@Mapping(source = "occamDisputeId", target = "occamDisputeId")
	@Mapping(source = "occamViolationTicketUpldId", target = "occamViolationTicketUpldId")
	@Mapping(source = "offenceLocation", target = "offenceLocationTxt")
	@Mapping(source = "policeDetachment", target = "detachmentLocationTxt")
	@Mapping(source = "appearInCourt", target = "requestCourtAppearanceYn")
	@Mapping(source = "remarks", target = "disputeRemarks")
	@Mapping(source = "status", target = "disputeStatusTypeCd", qualifiedByName="mapShortNamedEnum")
	@Mapping(target = "signatoryNameTxt", ignore = true) // ignore back reference mapping
	@Mapping(target = "signatoryTypeCd", ignore = true) // ignore back reference mapping
	@Mapping(source = "submittedTs", target = "submittedDt")
	@Mapping(source = "ticketNumber", target = "ticketNumberTxt")
	@Mapping(source = "timeToPayReason", target = "timeToPayReasonTxt")
	@Mapping(source = "violationDate", target = "violationDt")
	@Mapping(source = "vtcAssignedTo", target = "vtcAssignedTo")
	@Mapping(source = "vtcAssignedTs", target = "vtcAssignedDtm")
	@Mapping(source = "witnessNo", target = "witnessNo")
	public abstract ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute convert(JJDispute jjDispute);

	@Mapping(source = "abatementYn", target = "jjDisputedCountRoP.abatement")
	@Mapping(source = "accEntDtm", target = "jjDisputedCountRoP.createdTs")
	@Mapping(source = "accEntUserId", target = "jjDisputedCountRoP.createdBy")
	@Mapping(source = "accUpdDtm", target = "jjDisputedCountRoP.modifiedTs")
	@Mapping(source = "accUpdUserId", target = "jjDisputedCountRoP.modifiedBy")
	@Mapping(source = "adjustedAmt", target = "lesserOrGreaterAmount")
	@Mapping(source = "appearanceChargeCountId", target = "jjDisputedCountRoP.id")
	@Mapping(source = "commentsTxt", target = "comments")
	@Mapping(source = "countNo", target = "count")
	@Mapping(source = "dismissedForWantProsecYn", target = "jjDisputedCountRoP.forWantOfProsecution")
	@Mapping(source = "dismissedYn", target = "jjDisputedCountRoP.dismissed")
	@Mapping(source = "disputeCountId", target = "id")
	@Mapping(source = "disputeId", target = "jjDispute.id")
	@Mapping(source = "drivingProhibDurationTxt", target = "jjDisputedCountRoP.drivingProhibition")
	@Mapping(source = "drivingProhibMvaSectionTxt", target = "jjDisputedCountRoP.drivingProhibitionMVASection")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "findingResultCd", target = "jjDisputedCountRoP.finding", qualifiedByName="mapFindingResult")
	@Mapping(source = "fineDueDt", target = "dueDate")
	@Mapping(source = "includesSurchargeYn", target = "includesSurcharge")
	@Mapping(source = "jailDurationTxt", target = "jjDisputedCountRoP.jailDuration")
	@Mapping(source = "jailIntermittentYn", target = "jjDisputedCountRoP.jailIntermittent")
	@Mapping(source = "latestPleaCd", target = "latestPlea", qualifiedByName="mapPlea")
	@Mapping(source = "latestPleaUpdateDtm", target = "latestPleaUpdateTs")
	@Mapping(source = "lesserChargeDescTxt", target = "jjDisputedCountRoP.lesserDescription")
	@Mapping(source = "otherTxt", target = "jjDisputedCountRoP.other")
	@Mapping(source = "pleaCd", target = "plea", qualifiedByName="mapPlea")
	@Mapping(source = "probationConditionsTxt", target = "jjDisputedCountRoP.probationConditions")
	@Mapping(source = "probationDurationTxt", target = "jjDisputedCountRoP.probationDuration")
	@Mapping(source = "requestCourtAppearanceYn", target = "appearInCourt")
	@Mapping(source = "requestReductionYn", target = "requestReduction")
	@Mapping(source = "requestTimeToPayYn", target = "requestTimeToPay")
	@Mapping(source = "revisedDueDt", target = "revisedDueDate")
	@Mapping(source = "stayOfProceedingsByTxt", target = "jjDisputedCountRoP.stayOfProceedingsBy")
	@Mapping(source = "suspSntcProbationCondsTxt", target = "jjDisputedCountRoP.ssProbationConditions")
	@Mapping(source = "suspSntcProbationDurtnTxt", target = "jjDisputedCountRoP.ssProbationDuration")
	@Mapping(source = "statuteId", target = "description")                              // TODO: adjust mapping to statuteId
	@Mapping(source = "ticketedAmt", target = "ticketedFineAmount")
	@Mapping(source = "totalFineAmt", target = "totalFineAmount")
	@Mapping(source = "updDtm", target = "modifiedTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	@Mapping(source = "violationDt", target = "violationDate")
	@Mapping(source = "withdrawnYn", target = "jjDisputedCountRoP.withdrawn")
	public abstract JJDisputedCount convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeCount jjDisputeCount);

	@Mapping(source = "appearInCourt", target = "requestCourtAppearanceYn")
	@Mapping(source = "comments", target = "commentsTxt")
	@Mapping(source = "count", target = "countNo")
	@Mapping(source = "createdBy", target = "entUserId")
	@Mapping(source = "createdTs", target = "entDtm")
	@Mapping(source = "description", target = "statuteId")                              // TODO: adjust mapping to statuteId
	@Mapping(source = "dueDate", target = "fineDueDt")
	@Mapping(source = "id", target = "disputeCountId")
	@Mapping(source = "includesSurcharge", target = "includesSurchargeYn")
	@Mapping(source = "jjDisputedCountRoP.abatement", target = "abatementYn")
	@Mapping(source = "jjDisputedCountRoP.createdBy", target = "accEntUserId")
	@Mapping(source = "jjDisputedCountRoP.createdTs", target = "accEntDtm")
	@Mapping(source = "jjDisputedCountRoP.dismissed", target = "dismissedYn")
	@Mapping(source = "jjDisputedCountRoP.drivingProhibition", target = "drivingProhibDurationTxt")
	@Mapping(source = "jjDisputedCountRoP.drivingProhibitionMVASection", target = "drivingProhibMvaSectionTxt")
	@Mapping(source = "jjDisputedCountRoP.finding", target = "findingResultCd", qualifiedByName="mapShortNamedEnum")
	@Mapping(source = "jjDisputedCountRoP.forWantOfProsecution", target = "dismissedForWantProsecYn")
	@Mapping(source = "jjDisputedCountRoP.id", target = "appearanceChargeCountId")
	@Mapping(source = "jjDisputedCountRoP.jailDuration", target = "jailDurationTxt")
	@Mapping(source = "jjDisputedCountRoP.jailIntermittent", target = "jailIntermittentYn")
	@Mapping(source = "jjDisputedCountRoP.lesserDescription", target = "lesserChargeDescTxt")
	@Mapping(source = "jjDisputedCountRoP.modifiedBy", target = "accUpdUserId")
	@Mapping(source = "jjDisputedCountRoP.modifiedTs", target = "accUpdDtm")
	@Mapping(source = "jjDisputedCountRoP.other", target = "otherTxt")
	@Mapping(source = "jjDisputedCountRoP.probationConditions", target = "probationConditionsTxt")
	@Mapping(source = "jjDisputedCountRoP.probationDuration", target = "probationDurationTxt")
	@Mapping(source = "jjDisputedCountRoP.ssProbationConditions", target = "suspSntcProbationCondsTxt")
	@Mapping(source = "jjDisputedCountRoP.ssProbationDuration", target = "suspSntcProbationDurtnTxt")
	@Mapping(source = "jjDisputedCountRoP.stayOfProceedingsBy", target = "stayOfProceedingsByTxt")
	@Mapping(source = "jjDisputedCountRoP.withdrawn", target = "withdrawnYn")
	@Mapping(source = "latestPlea", target = "latestPleaCd", qualifiedByName="mapShortNamedEnum")
	@Mapping(source = "latestPleaUpdateTs", target = "latestPleaUpdateDtm")
	@Mapping(source = "lesserOrGreaterAmount", target = "adjustedAmt")
	@Mapping(source = "modifiedBy", target = "updUserId")
	@Mapping(source = "modifiedTs", target = "updDtm")
	@Mapping(source = "plea", target = "pleaCd", qualifiedByName="mapShortNamedEnum")
	@Mapping(source = "requestReduction", target = "requestReductionYn")
	@Mapping(source = "requestTimeToPay", target = "requestTimeToPayYn")
	@Mapping(source = "revisedDueDate", target = "revisedDueDt")
	@Mapping(source = "ticketedFineAmount", target = "ticketedAmt")
	@Mapping(source = "totalFineAmount", target = "totalFineAmt")
	@Mapping(source = "violationDate", target = "violationDt")
	public abstract ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeCount convert(JJDisputedCount jjDisputeCount);

	/** Convert TCO ORDS -> Oracle Data API */
	@Mapping(source = "disputeRemarkId", target = "id")
	@Mapping(source = "disputeId", target = "jjDispute.id")
	@Mapping(source = "disputeRemarkTxt", target = "note")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "fullUserNameTxt", target = "userFullName")
	@Mapping(source = "remarksMadeDtm", target = "remarksMadeTs")
	@Mapping(source = "updDtm", target = "modifiedTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	public abstract JJDisputeRemark convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeRemark jjDisputeRemark);

	/** Convert Oracle Data API -> TCO*/
	@Mapping(source = "createdBy", target = "entUserId")
	@Mapping(source = "createdTs", target = "entDtm")
	@Mapping(source = "id", target = "disputeRemarkId")
	@Mapping(source = "jjDispute.id", target = "disputeId")
	@Mapping(source = "modifiedBy", target = "updUserId")
	@Mapping(source = "modifiedTs", target = "updDtm")
	@Mapping(source = "note", target = "disputeRemarkTxt")
	@Mapping(source = "remarksMadeTs", target = "remarksMadeDtm")
	@Mapping(source = "userFullName", target = "fullUserNameTxt")
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
	@Mapping(source = ".", target = "duration", qualifiedByName="getDurationInMinutes")
	@Mapping(source = "judgeOrJjNameTxt", target = "adjudicator")
	@Mapping(source = "justinAppearanceId", target = "justinAppearanceId")
	@Mapping(source = "recordingClerkNameTxt", target = "clerkRecord")
	@Mapping(source = "seizedYn", target = "jjSeized")
	public abstract JJDisputeCourtAppearanceRoP convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJCourtAppearance jjCourtAppearance);

	@Mapping(source = "appearanceTs", target = "appearanceDtm")
	@Mapping(source = "reason", target = "appearanceReasonTxt")
	@Mapping(source = "comments", target = "commentsTxt")
	@Mapping(source = "room", target = "courtroomNumberTxt")
	@Mapping(source = "id", target = "courtAppearanceId")
	@Mapping(source = "crown", target = "crownPresenceCd")
	@Mapping(source = "defenceCounsel", target = "defenceCounselNameTxt")
	@Mapping(source = "dattCd", target = "defenceCounselPresenceCd")
	@Mapping(source = "jjDispute.id", target = "disputeId")
	@Mapping(source = "appCd", target = "disputantPresenceCd")
	@Mapping(source = "noAppTs", target = "disputantNotPresentDtm")
	@Mapping(target = "durationHours", ignore = true) // ignore back reference mapping
	@Mapping(target = "durationMinutes", ignore = true) // ignore back reference mapping
	@Mapping(source = "adjudicator", target = "judgeOrJjNameTxt")
	@Mapping(source = "justinAppearanceId", target = "justinAppearanceId")
	@Mapping(source = "clerkRecord", target = "recordingClerkNameTxt")
	@Mapping(source = "jjSeized", target = "seizedYn")
	public abstract ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJCourtAppearance convert(JJDisputeCourtAppearanceRoP jjCourtAppearance);

	@Named("getDurationInMinutes")
	public short getDurationInMinutes(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJCourtAppearance jjCourtAppearance) {
		short duration = (short)0;
		if (jjCourtAppearance.getDurationHours() != null && jjCourtAppearance.getDurationHours() > 0) {
			short hours = jjCourtAppearance.getDurationHours().shortValue();
			duration = (short) (hours * 60);
		}
		if (jjCourtAppearance.getDurationMinutes() != null && jjCourtAppearance.getDurationMinutes() > 0) {
			short minutes = jjCourtAppearance.getDurationMinutes().shortValue();
			duration += minutes;
		}
		return duration;
	}

	@AfterMapping
	public void afterMapping(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeCount jjDisputeCount, @MappingTarget JJDisputedCount jjDisputedCount) {
		if (jjDisputedCount.getJjDisputedCountRoP() != null && jjDisputedCount.getJjDisputedCountRoP().getId() == null) {
			jjDisputedCount.setJjDisputedCountRoP(null);
		}
	}
}
