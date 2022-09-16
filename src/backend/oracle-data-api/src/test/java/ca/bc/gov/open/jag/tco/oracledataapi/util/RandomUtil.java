package ca.bc.gov.open.jag.tco.oracledataapi.util;

import java.util.Date;
import java.util.UUID;

import org.apache.commons.lang3.time.DateUtils;

import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.DisputeStatus;
import ca.bc.gov.open.jag.tco.oracledataapi.model.EmailHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.model.FileHistory;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.model.JJDisputeStatus;
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
		emailHistory.setEmailSubject(UUID.randomUUID().toString());
		emailHistory.setPlainTextContent(UUID.randomUUID().toString());
		emailHistory.setSuccessfullySent(YesNo.N);
		emailHistory.setFromEmailAddress(randomEmailAddress());
		emailHistory.setRecipientEmailAddress(randomEmailAddress());
		return emailHistory;
	}
	
	public static FileHistory createFileHistory() {
		FileHistory fileHistory = new FileHistory();
		fileHistory.setTicketNumber(UUID.randomUUID().toString());
		fileHistory.setDescription(UUID.randomUUID().toString());
		return fileHistory;
	}
	
	public static String randomEmailAddress() {
    	int index = randomInt(0, COMMON_EMAIL_ADDRESSES.length-1);
    	return COMMON_EMAIL_ADDRESSES[index];		
	}

	public static int randomInt(int min, int max) {
		return (int) Math.round((Math.random() * (max-min)) + min);
	}

	public static long randomLong(long min, long max) {
		return Math.round((Math.random() * (max-min)) + min);
	}

	public static String randomSurname() {
    	int index = randomInt(0, COMMON_LAST_NAMES.length-1);
    	return COMMON_LAST_NAMES[index];
	}

	public static String randomGivenName() {
    	int index = randomInt(0, COMMON_FIRST_NAMES.length-1);
    	return COMMON_FIRST_NAMES[index];
	}

	private static String randomName() {
		return randomGivenName() + " " + randomSurname();
	}

	public static String randomCity() {
    	int index = randomInt(0, COMMON_CITY_NAMES.length-1);
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

}
