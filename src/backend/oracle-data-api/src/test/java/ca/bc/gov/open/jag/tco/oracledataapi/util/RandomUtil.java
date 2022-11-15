package ca.bc.gov.open.jag.tco.oracledataapi.util;

import static org.junit.jupiter.api.Assertions.assertTrue;

import java.text.ParseException;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Random;
import java.util.UUID;

import org.apache.commons.lang3.RandomStringUtils;
import org.apache.commons.lang3.time.DateFormatUtils;
import org.apache.commons.lang3.time.DateUtils;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatusType;
import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.model.FileHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Plea;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicket;
import ca.bc.gov.open.jag.tco.oracledataapi.model.ViolationTicketCount;
import ca.bc.gov.open.jag.tco.oracledataapi.model.YesNo;

public class RandomUtil {

	public final static String[] COMMON_FIRST_NAMES = new String[] {
			"Abigail",
			"Aiden",
			"Amelia",
			"Aria",
			"Ava",
			"Benjamin",
			"Carter",
			"Charlotte",
			"Chloe",
			"Ella",
			"Emily",
			"Emma",
			"Ethan",
			"Grayson",
			"Isabella",
			"Jack",
			"Jackson",
			"Jacob",
			"James",
			"Leo",
			"Liam",
			"Lily",
			"Lincoln",
			"Logan",
			"Lucas",
			"Madison",
			"Mason",
			"Maya",
			"Mia",
			"Mila",
			"Nathan",
			"Noah",
			"Nora",
			"Oliver",
			"Olivia",
			"Owen",
			"Riley",
			"Sophia",
			"Steven",
			"William",
			"Zoey"
	};

	public final static String[] COMMON_LAST_NAMES = new String[] {
			"Anderson",
			"Baker",
			"Bell",
			"Brooks",
			"Brown",
			"Campbell",
			"Cook",
			"Dameron",
			"Davis",
			"Doe",
			"Edwards",
			"Evans",
			"Ford",
			"Foster",
			"Frik",
			"Graham",
			"Gray",
			"Green",
			"Hamilton",
			"Harris",
			"Henderson",
			"Johnson",
			"Jones",
			"Long",
			"Miller",
			"Organa",
			"Parker",
			"Price",
			"Sanders",
			"Simmons",
			"Skywalker",
			"Smith",
			"Solo",
			"Strange",
			"Thompson",
			"Tico",
			"Walker",
			"Ward",
			"West",
			"Williams",
			"Wilson",
			"Young"
	};

	public final static String[] COMMON_EMAIL_ADDRESSES = new String[] {
			"1@1.com",
			"2@2.ca",
			"3@3.com",
			"Lucas@4.com",
			"Stacy@5.com",
			"Arlington@6.com"
	};

	public final static String[] COMMON_CITY_NAMES = new String[] {
			"Arlington",
			"Ashland",
			"Auburn",
			"Bluesville",
			"Bristol",
			"Burlington",
			"Cedarland",
			"Centerville",
			"Clayton",
			"Cleveland",
			"Clinton",
			"Cottonwood",
			"Dayton",
			"Dover",
			"Evergreenland",
			"Fairview",
			"Firland",
			"Forestside",
			"Franklin",
			"Georgetown",
			"Goodview",
			"Greatview",
			"Greenville",
			"Hudson",
			"Jackson",
			"Kingston",
			"Lebanon",
			"Leftville",
			"Lexington",
			"Madison",
			"Manchester",
			"Mapleland",
			"Milford",
			"Milton",
			"Mount Vernon",
			"Mountainside",
			"Niteton",
			"Oakland",
			"Oceanside",
			"Oxford",
			"Pineland",
			"Pleasantville",
			"Portsville",
			"Queenston",
			"Redsville",
			"Rightville",
			"Riverside",
			"Salem",
			"Springfield",
			"Spruceland",
			"Summerfield",
			"Winchester",
			"Winterfield"
	};

	public static Dispute createDispute() {
		Dispute dispute = new Dispute();
		dispute.setStatus(DisputeStatus.NEW);
		dispute.setDisputantGivenName1(randomGivenName());
		dispute.setDisputantSurname(randomSurname());
		dispute.setViolationTicket(new ViolationTicket());
		return dispute;
	}

	public static JJDispute createJJDispute() {
		JJDispute dispute = new JJDispute();
		dispute.setTicketNumber(UUID.randomUUID().toString());
		dispute.setStatus(JJDisputeStatus.NEW);
		dispute.setCourthouseLocation(randomCity());
		dispute.setGivenNames(randomGivenName());
		dispute.setSurname(randomSurname());
		dispute.setEnforcementOfficer(randomName());
		dispute.setJjAssignedTo(randomName());
		dispute.setViolationDate(randomDate(DateUtils.addDays(new Date(), -30), new Date())); // random date in the last 30 days
		return dispute;
	}

	public static EmailHistory createEmailHistory() {
		EmailHistory emailHistory = new EmailHistory();
		emailHistory.setTicketNumber(UUID.randomUUID().toString());
		emailHistory.setSubject(UUID.randomUUID().toString());
		emailHistory.setPlainTextContent(UUID.randomUUID().toString());
		emailHistory.setSuccessfullySent(YesNo.N);
		emailHistory.setFromEmailAddress(randomEmailAddress());
		emailHistory.setToEmailAddress(randomEmailAddress());
		return emailHistory;
	}

	public static FileHistory createFileHistory() {
		FileHistory fileHistory = new FileHistory();
		fileHistory.setTicketNumber(UUID.randomUUID().toString());
		fileHistory.setDescription(UUID.randomUUID().toString());
		return fileHistory;
	}

	public static String randomEmailAddress() {
		int index = randomInt(0, COMMON_EMAIL_ADDRESSES.length - 1);
		return COMMON_EMAIL_ADDRESSES[index];
	}

	public static int randomInt(int min, int max) {
		return (int) Math.round((Math.random() * (max - min)) + min);
	}

	public static Float randomCurrency(double min, double max) {
		double amount = ((Math.random() * (max - min)) + min);
		return Float.valueOf((float) (Math.round(amount * 100.0) / 100.0));
	}

	public static long randomLong(long min, long max) {
		return Math.round((Math.random() * (max - min)) + min);
	}

	public static String randomSurname() {
		int index = randomInt(0, COMMON_LAST_NAMES.length - 1);
		return COMMON_LAST_NAMES[index];
	}

	public static String randomGivenName() {
		int index = randomInt(0, COMMON_FIRST_NAMES.length - 1);
		return COMMON_FIRST_NAMES[index];
	}

	private static String randomName() {
		return randomGivenName() + " " + randomSurname();
	}

	public static String randomCity() {
		int index = randomInt(0, COMMON_CITY_NAMES.length - 1);
		return COMMON_CITY_NAMES[index];
	}

	public static Date randomDate(Date minDate, Date maxDate) {
		Date date = new Date();
		long minMilliSecs = minDate.getTime();
		long maxMilliSecs = maxDate.getTime();
		long l = randomLong(minMilliSecs, maxMilliSecs);
		date.setTime(l);
		return date;
	}

	/**
	 * Creates a random TicketNumber of the form AA00000000
	 */
	public static String randomTicketNumber() {
		return randomAlphabetic(2).toUpperCase() + randomNumeric(8);
	}

	/**
	 * Creates a random string whose length is the number of characters specified. Characters will be chosen from the set of Latin alphabetic
	 * characters (a-z, A-Z).
	 */
	public static String randomAlphabetic(int count) {
		return RandomStringUtils.randomAlphabetic(count);
	}

	/**
	 * Creates a random string whose length is the number of characters specified. Characters will be chosen from the set of numeric characters.
	 */
	public static String randomNumeric(int count) {
		return RandomStringUtils.randomNumeric(count);
	}

	/**
	 * Creates a random string whose length is the number of characters specified.
	 * Characters will be chosen from the set of Latin alphabetic characters (a-z, A-Z) and the digits 0-9.
	 */
	public static String randomAlphanumeric(int count) {
		return RandomStringUtils.randomAlphanumeric(count);
	}

	/**
	 * Returns a random date within the last year.
	 * @throws ParseException
	 */
	public static Date randomDate() throws ParseException {
		// ceiling dateTime to top of day
		return DateUtils.parseDate(DateFormatUtils.ISO_8601_EXTENDED_DATE_FORMAT.format(randomDateTime()), DateFormatUtils.ISO_8601_EXTENDED_DATE_FORMAT.getPattern());
	}

	/**
	 * Returns a random date within the last year.
	 */
	public static Date randomDateTime() {
		return randomDate(DateUtils.addYears(new Date(), -1), new Date());
	}

	public static YesNo randomYN() {
		return (new Random().nextBoolean()) ? YesNo.Y : YesNo.N;
	}


	/**
	 * Creates a Dispute where every field is populated with random data to the maximum character limit appropriate for each field.
	 * @throws ParseException
	 */
	public static Dispute createFullyPopulatedDispute() throws ParseException {
		Dispute dispute = new Dispute();

		dispute.setAddressCity(randomAlphanumeric(30));
		dispute.setAddressLine1(randomAlphanumeric(100)); // FIXME: Column length disparity - ORDs is 100, H2 is 500
		dispute.setAddressLine2(randomAlphanumeric(100)); // FIXME: Column length disparity - ORDs is 100, H2 is 500
		dispute.setAddressLine3(randomAlphanumeric(100)); // FIXME: Column length disparity - ORDs is 100, H2 is 500
		dispute.setAddressProvince(randomAlphabetic(30));
		dispute.setDetachmentLocation(randomAlphabetic(150));
		dispute.setDisputantBirthdate(randomDate());
		dispute.setDisputantClientId(randomNumeric(5));
		dispute.setDisputantComment(randomAlphabetic(4000));
		dispute.setDisputantDetectedOcrIssues(randomYN());
		dispute.setDisputantGivenName1(randomAlphabetic(30));
		dispute.setDisputantGivenName2(randomAlphabetic(30));
		dispute.setDisputantGivenName3(randomAlphabetic(30));
		dispute.setDisputantOcrIssues(randomAlphabetic(500));
		dispute.setDisputantOrganization(randomAlphabetic(150));
		dispute.setDisputantSurname(randomAlphabetic(30));
		dispute.setDisputeCounts(createDisputeCounts(dispute));
		dispute.setDisputeStatusType(createDisputeStatusType());
		dispute.setDriversLicenceNumber(randomAlphanumeric(30));
		dispute.setDriversLicenceProvince(randomAlphabetic(30));
		dispute.setEmailAddress(randomAlphanumeric(100));
		dispute.setFilingDate(randomDate());
		dispute.setFineReductionReason(randomAlphabetic(500));
		dispute.setHomePhoneNumber(randomNumeric(20));
		dispute.setInterpreterLanguageCd(randomAlphabetic(3));
		dispute.setInterpreterRequired(randomYN());
		dispute.setIssuedTs(randomDateTime());
		dispute.setLawyerAddress(randomAlphanumeric(300));
		dispute.setLawyerEmail(randomAlphanumeric(100));
		dispute.setLawyerGivenName1(randomAlphabetic(30));
		dispute.setLawyerGivenName2(randomAlphabetic(30));
		dispute.setLawyerGivenName3(randomAlphabetic(30));
		dispute.setLawyerPhoneNumber(randomNumeric(20));
		dispute.setLawyerSurname(randomAlphabetic(30));
		dispute.setNoticeOfDisputeGuid(UUID.randomUUID().toString());
		dispute.setOcrTicketFilename(randomAlphabetic(100));
		dispute.setOfficerPin(randomAlphabetic(10));
		dispute.setPostalCode(randomAlphanumeric(10));
		dispute.setRejectedReason(randomAlphabetic(500));
		dispute.setRepresentedByLawyer(randomYN());
		dispute.setStatus(DisputeStatus.NEW);
		dispute.setSubmittedTs(randomDateTime());
		dispute.setSystemDetectedOcrIssues(randomYN());
		dispute.setTicketNumber(randomTicketNumber());
		dispute.setTimeToPayReason(randomAlphabetic(500));
		dispute.setUserAssignedTo(randomAlphabetic(30));
		dispute.setUserAssignedTs(randomDateTime());
		dispute.setViolationTicket(createViolationTicket(dispute));
		dispute.setWitnessNo(Integer.valueOf(randomNumeric(3)));
		dispute.setWorkPhoneNumber(randomNumeric(20));

		// 36 character field
		assertTrue(dispute.getNoticeOfDisputeGuid().length() == 36);

		return dispute;
	}

	public static ViolationTicket createViolationTicket(Dispute dispute) throws ParseException {
		ViolationTicket violationTicket = new ViolationTicket();
		violationTicket.setAddress(randomAlphanumeric(100));
		violationTicket.setAddressCity(randomAlphanumeric(100));
		violationTicket.setAddressCountry(randomAlphanumeric(100));
		violationTicket.setAddressPostalCode(randomAlphanumeric(10));
		violationTicket.setAddressProvince(randomAlphanumeric(100));
		violationTicket.setCourtLocation(randomAlphanumeric(150));
		violationTicket.setDetachmentLocation(randomAlphanumeric(150));
		violationTicket.setDisputantOrganizationName(randomAlphabetic(150));
		violationTicket.setDisputantSurname(randomAlphabetic(100));
		violationTicket.setDisputantGivenNames(randomAlphabetic(200));
		violationTicket.setDisputantDriversLicenceNumber(randomAlphanumeric(30));
		violationTicket.setDisputantClientNumber(randomAlphanumeric(30));
		violationTicket.setDriversLicenceProvince(randomAlphabetic(100));
		violationTicket.setDriversLicenceCountry(randomAlphabetic(100));
		violationTicket.setDriversLicenceIssuedYear(Integer.valueOf(randomNumeric(4)));
		violationTicket.setDriversLicenceExpiryYear(Integer.valueOf(randomNumeric(4)));
		violationTicket.setDisputantBirthdate(randomDate());
		violationTicket.setIssuedAtOrNearCity(randomAlphanumeric(100));
		violationTicket.setIssuedOnRoadOrHighway(randomAlphanumeric(100));
		violationTicket.setIssuedTs(randomDateTime());
		violationTicket.setIsChangeOfAddress(randomYN());
		violationTicket.setIsDriver(randomYN());
		violationTicket.setIsOwner(randomYN());
		violationTicket.setIsYoungPerson(randomYN());
		violationTicket.setOfficerPin(randomAlphanumeric(10));
		violationTicket.setTicketNumber(dispute.getTicketNumber());
		violationTicket.setViolationTicketCounts(createViolationTicketCounts(violationTicket));
		violationTicket.setDispute(dispute);

		return violationTicket;
	}

	public static List<ViolationTicketCount> createViolationTicketCounts(ViolationTicket violationTicket) {
		ArrayList<ViolationTicketCount> violationTickets = new ArrayList<ViolationTicketCount>();

		ViolationTicketCount count = new ViolationTicketCount();
		count.setCountNo(1);
		count.setDescription(randomAlphanumeric(4000));
		count.setActOrRegulationNameCode(randomAlphabetic(5));
		count.setIsAct(randomYN());
		count.setIsRegulation(randomYN());
		count.setSection(randomAlphanumeric(15));
		count.setSubsection(randomAlphanumeric(4));
		count.setParagraph(randomAlphanumeric(3));
		count.setSubparagraph(randomAlphanumeric(5));
		count.setTicketedAmount(randomCurrency(0.01, 999999.99));
		count.setViolationTicket(violationTicket);

		violationTickets.add(count);
		return violationTickets;
	}

	public static DisputeStatusType createDisputeStatusType() {
		return null;
	}

	public static List<DisputeCount> createDisputeCounts(Dispute dispute) {
		ArrayList<DisputeCount> disputeCounts = new ArrayList<DisputeCount>();

		DisputeCount count = new DisputeCount();
		count.setCountNo(1);
		count.setPleaCode(Plea.G);
		count.setRequestTimeToPay(randomYN());
		count.setRequestReduction(randomYN());
		count.setRequestCourtAppearance(randomYN());
		count.setDispute(dispute);
		disputeCounts.add(count);

		return disputeCounts;
	}

}
