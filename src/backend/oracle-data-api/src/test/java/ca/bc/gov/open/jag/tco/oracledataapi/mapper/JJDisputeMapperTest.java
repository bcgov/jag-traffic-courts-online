package ca.bc.gov.open.jag.tco.oracledataapi.mapper;

import static org.junit.jupiter.api.Assertions.assertEquals;

import java.util.Arrays;
import java.util.Date;

import org.junit.jupiter.api.Test;
import org.junit.jupiter.params.ParameterizedTest;
import org.junit.jupiter.params.provider.CsvSource;
import org.springframework.beans.factory.annotation.Autowired;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeHearingType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputedCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Plea;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;
import ca.bc.gov.open.jag.tco.oracledataapi.util.RandomUtil;

public class JJDisputeMapperTest extends BaseTestSuite {

	@Autowired
	private JJDisputeMapper jjDisputeMapper;

	@Test
	void testMapper_old() throws Exception {
		String disputeId = "42";
		String ticketNumber = "EA90100004";
		JJDisputeStatus disputeStatus = JJDisputeStatus.NEW;
		String disputantGiven1Nm = "Given1";
		String disputantGiven2Nm = "Given2";
		String disputantGiven3Nm = "Given3";
		String disputantSurname = "Surname";
		String offenceLocation = "Offence Location";
		String detachmentLocation = "Detachment Location";
		Date issuedTs = RandomUtil.randomDate();
		Date submittedTs =  RandomUtil.randomDate();
		Date icbcReceivedDt =  RandomUtil.randomDate();
		Date disputantBirthDt =  RandomUtil.randomDate();
		String disputantDrvLicNumber = "1234567";
		String drvLicIssuedProvSeqNo = "1";
		String drvLicIssuedCtryId = "2";
		String emailAddress = "someone@somewhere.com";
		String addressLine1 = "123 Main St";
		String addressLine2 = "234 Main St";
		String addressLine3 = "345 Main St";
		String lawyerSurnameNm = "LawyerSurname";
		String lawyerGiven1Nm = "LawyerGiven1";
		String lawyerGiven2Nm = "LawyerGiven2";
		String lawyerGiven3Nm = "LawyerGiven3";
		String lawFirmNm = "LawFirm";
		String interpreterLanguageCd = "1";
		JJDisputeHearingType hearingType = JJDisputeHearingType.WRITTEN_REASONS;
		Date createdTs =  RandomUtil.randomDate();
		String createdBy = "1";
		Date modifiedTs =  RandomUtil.randomDate();
		String modifiedBy = "2";

		String disputeCountId = "42";
		String countNo = "5";
		String statuteId = "77";
		Plea pleaCd = Plea.G;
		Date fineDueDt =  RandomUtil.randomDate();
		Date violationDt =  RandomUtil.randomDate();
		Double adjustedAmt = Double.valueOf("10.53");
		YesNo includesSurchargeYn = YesNo.Y;
		Date revisedDueDt =  RandomUtil.randomDate();
		Double totalFineAmt = Double.valueOf("123.45");
		String comments = "comments and more comments";
		YesNo requestTimeToPayYn = YesNo.Y;
		YesNo requestReductionYn = YesNo.Y;
		YesNo requestCourtAppearanceYn = YesNo.Y;
		Date countCreatedTs =  RandomUtil.randomDate();
		String countCreatedBy = "3";
		Date countModifedTs =  RandomUtil.randomDate();
		String countModifiedBy = "4";

		ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute source = new ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute();
		source.setDisputeId(disputeId);
		source.setTicketNumberTxt(ticketNumber);
		source.setDisputeStatusTypeCd(disputeStatus.toString());
		source.setDisputantGiven1Nm(disputantGiven1Nm);
		source.setDisputantGiven2Nm(disputantGiven2Nm);
		source.setDisputantGiven3Nm(disputantGiven3Nm);
		source.setDisputantSurnameTxt(disputantSurname);
		source.setOffenceLocationTxt(offenceLocation);
		source.setDetachmentLocationTxt(detachmentLocation);
		source.setIssuedTs(issuedTs);
		source.setSubmittedDt(submittedTs);
		source.setIcbcReceivedDt(icbcReceivedDt);
		source.setDisputantBirthDt(disputantBirthDt);
		source.setDisputantDrvLicNumberTxt(disputantDrvLicNumber);
		source.setDrvLicIssuedProvSeqNo(drvLicIssuedProvSeqNo);
		source.setDrvLicIssuedCtryId(drvLicIssuedCtryId);
		source.setEmailAddressTxt(emailAddress);
		source.setAddressLine1Txt(addressLine1);
		source.setAddressLine2Txt(addressLine2);
		source.setAddressLine3Txt(addressLine3);
		source.setLawyerSurnameNm(lawyerSurnameNm);
		source.setLawyerGiven1Nm(lawyerGiven1Nm);
		source.setLawyerGiven2Nm(lawyerGiven2Nm);
		source.setLawyerGiven3Nm(lawyerGiven3Nm);
		source.setLawFirmNm(lawFirmNm);
		source.setInterpreterLanguageCd(interpreterLanguageCd);
		source.setCourtHearingTypeCd(hearingType.getShortName());
		source.setEntDtm(createdTs);
		source.setEntUserId(createdBy);
		source.setUpdDtm(modifiedTs);
		source.setUpdUserId(modifiedBy);

		ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeCount disputeCount = new ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeCount();
		disputeCount.setDisputeCountId(disputeCountId);
		disputeCount.setDisputeId(disputeId);
		disputeCount.setCountNo(countNo);
		disputeCount.setStatuteId(statuteId);
		disputeCount.setPleaCd(pleaCd.getShortName());
		disputeCount.setFineDueDt(fineDueDt);
		disputeCount.setViolationDt(violationDt);
		disputeCount.setAdjustedAmt(adjustedAmt);
		disputeCount.setIncludesSurchargeYn(includesSurchargeYn.toString());
		disputeCount.setRevisedDueDt(revisedDueDt);
		disputeCount.setTotalFineAmt(totalFineAmt);
		disputeCount.setCommentsTxt(comments);
		disputeCount.setRequestTimeToPayYn(requestTimeToPayYn.toString());
		disputeCount.setRequestReductionYn(requestReductionYn.toString());
		disputeCount.setRequestCourtAppearanceYn(requestCourtAppearanceYn.toString());
		disputeCount.setEntDtm(countCreatedTs);
		disputeCount.setEntUserId(countCreatedBy);
		disputeCount.setUpdDtm(countModifedTs);
		disputeCount.setUpdUserId(countModifiedBy);
		source.setDisputeCounts(Arrays.asList(disputeCount));

		JJDispute target = jjDisputeMapper.convert(source);
//		assertEquals(disputeId, target.getDisputeId());                                                 // TODO: field missing in model but exists in database
		assertEquals(ticketNumber, target.getTicketNumber());
		assertEquals(JJDisputeStatus.NEW, target.getStatus());
		assertEquals(disputantGiven1Nm + " " + disputantGiven2Nm + " " + disputantGiven3Nm, target.getGivenNames());
		assertEquals(disputantSurname, target.getSurname());
		assertEquals(offenceLocation, target.getOffenceLocation());
		assertEquals(detachmentLocation, target.getPoliceDetachment());
		assertEquals(issuedTs, target.getViolationDate());
		assertEquals(submittedTs, target.getSubmittedTs());
		assertEquals(icbcReceivedDt, target.getIcbcReceivedDate());
		assertEquals(disputantBirthDt, target.getContactInformation().getBirthdate());
		assertEquals(disputantDrvLicNumber, target.getContactInformation().getDriversLicenceNumber());
//		assertEquals(drvLicIssuedProvSeqNo, target.getContactInformation().getDrvLicIssuedProvSeqNo()); // TODO: field missing in model but exists in database
//		assertEquals(drvLicIssuedCtryId, target.getContactInformation().getDrvLicIssuedCtryId());       // TODO: field missing in model but exists in database
		assertEquals(emailAddress, target.getContactInformation().getEmailAddress());
		assertEquals(addressLine1 + " " + addressLine2 + " " + addressLine3, target.getContactInformation().getAddress());
		assertEquals(lawyerSurnameNm, target.getLawyerSurname());
		assertEquals(lawyerGiven1Nm, target.getLawyerGivenName1());
		assertEquals(lawyerGiven2Nm, target.getLawyerGivenName2());
		assertEquals(lawyerGiven3Nm, target.getLawyerGivenName3());
		assertEquals(lawFirmNm, target.getLawFirmName());
		assertEquals(interpreterLanguageCd, target.getInterpreterLanguageCd());
		assertEquals(hearingType, target.getHearingType());
		assertEquals(createdTs, target.getCreatedTs());
		assertEquals(createdBy, target.getCreatedBy());
		assertEquals(modifiedTs, target.getModifiedTs());
		assertEquals(modifiedBy, target.getModifiedBy());

		JJDisputedCount jjDisputedCount = target.getJjDisputedCounts().get(0);
		assertEquals(Long.valueOf(disputeCountId), jjDisputedCount.getId());
//		assertEquals(disputeId, jjDisputedCount.getJjDispute().getDisputeId());                         // TODO: field missing in model but exists in database
		assertEquals(Integer.valueOf(countNo).intValue(), jjDisputedCount.getCount());
//		assertEquals(statuteId, jjDisputedCount.getStatuteId());                                        // TODO: field missing in model but exists in database
		assertEquals(pleaCd, jjDisputedCount.getPlea());
		assertEquals(fineDueDt, jjDisputedCount.getDueDate());
//		assertEquals(violationDt, jjDisputedCount.getViolationDt());                                    // TODO: field missing in model but exists in database
		assertEquals(Float.valueOf(adjustedAmt.toString()), jjDisputedCount.getLesserOrGreaterAmount());
		assertEquals(includesSurchargeYn, jjDisputedCount.getIncludesSurcharge());
		assertEquals(revisedDueDt, jjDisputedCount.getRevisedDueDate());
		assertEquals(Float.valueOf(totalFineAmt.toString()), jjDisputedCount.getTotalFineAmount());
		assertEquals(comments, jjDisputedCount.getComments());
		assertEquals(requestTimeToPayYn, jjDisputedCount.getRequestTimeToPay());
		assertEquals(requestReductionYn, jjDisputedCount.getRequestReduction());
		assertEquals(requestCourtAppearanceYn, jjDisputedCount.getAppearInCourt());
		assertEquals(countCreatedTs, jjDisputedCount.getCreatedTs());
		assertEquals(countCreatedBy, jjDisputedCount.getCreatedBy());
		assertEquals(countModifedTs, jjDisputedCount.getModifiedTs());
		assertEquals(countModifiedBy, jjDisputedCount.getModifiedBy());
	}

	@ParameterizedTest()
	@CsvSource({
		"Given1, Given2, Given3, Given1 Given2 Given3",
		"Given1, , , Given1",
		", Given2, , Given2",
		", , Given3, Given3",
		"Given1, Given2, , Given1 Given2",
		", Given2, Given3, Given2 Given3",
		"Given1, , Given3, Given1 Given3",
		})
	public void testGivenNames(String given1, String given2, String given3, String expectedGivenNames) {
		ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute source = new ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute();
		source.setDisputantGiven1Nm(given1);
		source.setDisputantGiven2Nm(given2);
		source.setDisputantGiven3Nm(given3);

		JJDispute target = jjDisputeMapper.convert(source);
		assertEquals(expectedGivenNames, target.getGivenNames());
	}

	@ParameterizedTest()
	@CsvSource({
		"123 Main St, Apt 2, Suite 302, 123 Main St Apt 2 Suite 302",
		"123 Main St, , , 123 Main St",
		", Apt 2, , Apt 2",
		", , Suite 302, Suite 302",
		", Apt 2, Suite 302, Apt 2 Suite 302",
		"123 Main St, , Suite 302, 123 Main St Suite 302",
		"123 Main St, Apt 2, , 123 Main St Apt 2",
		})
	public void testAddresses(String addr1, String addr2, String addr3, String expectedAddress) {
		ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute source = new ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute();
		source.setAddressLine1Txt(addr1);
		source.setAddressLine2Txt(addr2);
		source.setAddressLine3Txt(addr3);

		JJDispute target = jjDisputeMapper.convert(source);
		assertEquals(expectedAddress, target.getContactInformation().getAddress());
	}

}
