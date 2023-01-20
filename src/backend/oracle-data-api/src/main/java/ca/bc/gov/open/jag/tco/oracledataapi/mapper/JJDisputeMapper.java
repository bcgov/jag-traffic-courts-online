package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import org.apache.commons.lang3.StringUtils;
import org.mapstruct.AfterMapping;
import org.mapstruct.InjectionStrategy;
import org.mapstruct.Mapper;
import org.mapstruct.Mapping;
import org.mapstruct.MappingTarget;
import org.mapstruct.Named;

import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeHearingType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeRemark;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputedCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Plea;

@Mapper(componentModel = "spring", injectionStrategy = InjectionStrategy.CONSTRUCTOR) // This is required for tests to work
public abstract class JJDisputeMapper {

	@Mapping(source = "ticketNumberTxt", target = "ticketNumber")
	@Mapping(source = "disputeStatusTypeCd", target = "status")
	@Mapping(source = "disputantSurnameTxt", target = "surname")
	@Mapping(source = "disputantGiven1Nm", target = "givenNames")
	@Mapping(source = "offenceLocationTxt", target = "offenceLocation")
	@Mapping(source = "detachmentLocationTxt", target = "policeDetachment")
	@Mapping(source = "issuedTs", target = "violationDate")
	@Mapping(source = "submittedDt", target = "submittedTs")
	@Mapping(source = "icbcReceivedDt", target = "icbcReceivedDate")
	@Mapping(source = "disputantBirthDt", target = "contactInformation.birthdate")
	@Mapping(source = "disputantDrvLicNumberTxt", target = "contactInformation.driversLicenceNumber")
//	@Mapping(source = "drvLicIssuedProvSeqNo", target = "contactInformation.province") // TODO: field missing in model but exists in database, see TCVP-2063,2070,2071
//	@Mapping(source = "drvLicIssuedCtryId", target = "drvLicIssuedCtryId")             // TODO: field missing in model but exists in database, see TCVP-2063,2070,2071
	@Mapping(source = "emailAddressTxt", target = "contactInformation.emailAddress")
	@Mapping(source = "lawyerSurnameNm", target = "lawyerSurname")
	@Mapping(source = "lawyerGiven1Nm", target = "lawyerGivenName1")
	@Mapping(source = "lawyerGiven2Nm", target = "lawyerGivenName2")
	@Mapping(source = "lawyerGiven3Nm", target = "lawyerGivenName3")
	@Mapping(source = "lawFirmNm", target = "lawFirmName")
	@Mapping(source = "interpreterLanguageCd", target = "interpreterLanguageCd")
	@Mapping(source = "courtHearingTypeCd", target = "hearingType", qualifiedByName="mapHearingType")
	@Mapping(source = "disputeCounts", target = "jjDisputedCounts")
	@Mapping(source = "disputeRemarks", target = "remarks")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "updDtm", target = "modifiedTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
	public abstract JJDispute convert(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute jjDispute);

	@Mapping(source = "disputeCountId", target = "id")
//	@Mapping(source = "disputeId", target = "jjDispute.disputeId")                      // TODO: field missing in model but exists in database
	@Mapping(source = "countNo", target = "count")
	@Mapping(source = "statuteId", target = "description")                              // TODO: adjust mapping to statuteId
	@Mapping(source = "pleaCd", target = "plea", qualifiedByName="mapPlea")
	@Mapping(source = "fineDueDt", target = "dueDate")
//	@Mapping(source = "violationDt", target = "violationDt")                            // TODO: field missing in model but exists in database
	@Mapping(source = "adjustedAmt", target = "lesserOrGreaterAmount")
	@Mapping(source = "includesSurchargeYn", target = "includesSurcharge")
	@Mapping(source = "revisedDueDt", target = "revisedDueDate")
	@Mapping(source = "totalFineAmt", target = "totalFineAmount")
	@Mapping(source = "totalFineAmt", target = "ticketedFineAmount")                    // TODO: remove duplicate field
	@Mapping(source = "commentsTxt", target = "comments")
	@Mapping(source = "requestTimeToPayYn", target = "requestTimeToPay")
	@Mapping(source = "requestReductionYn", target = "requestReduction")
	@Mapping(source = "requestCourtAppearanceYn", target = "appearInCourt")
	@Mapping(source = "entDtm", target = "createdTs")
	@Mapping(source = "entUserId", target = "createdBy")
	@Mapping(source = "updDtm", target = "modifiedTs")
	@Mapping(source = "updUserId", target = "modifiedBy")
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

	/**
	 * Addresses in TCO are stored as 3 separate fields, but the application requires this to be a single field.
	 * @param source data coming from TCO oracle schema
	 * @param target data mapped to the application's model
	 */
	@AfterMapping
	protected void mapAddresses(ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute source, @MappingTarget JJDispute target) {
		StringBuffer address = new StringBuffer();

		if (!StringUtils.isBlank(source.getAddressLine1Txt())) {
			address.append(source.getAddressLine1Txt().trim());
			address.append(" ");
		}

		if (!StringUtils.isBlank(source.getAddressLine2Txt())) {
			address.append(source.getAddressLine2Txt().trim());
			address.append(" ");
		}

		if (!StringUtils.isBlank(source.getAddressLine3Txt())) {
			address.append(source.getAddressLine3Txt().trim());
			address.append(" ");
		}

		target.getContactInformation().setAddress(address.toString().trim());
	}
}
