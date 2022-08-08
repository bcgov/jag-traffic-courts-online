package ca.bc.gov.court.traffic.ticket.util;

import java.awt.Point;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Random;

import org.apache.commons.lang3.RandomStringUtils;
import org.apache.commons.lang3.StringUtils;

import com.opencsv.bean.CsvToBeanBuilder;

import ca.bc.gov.court.traffic.ticket.model.Statute;
import ca.bc.gov.court.traffic.ticket.model.Style;
import ca.bc.gov.court.traffic.ticket.model.ViolationTicket;

public class RandomUtil {

	public final static List<Statute> statutes = new ArrayList<Statute>();
	static {
		try (InputStream stream = RandomUtil.class.getClassLoader().getResourceAsStream("data/statutes.csv");
			BufferedReader reader = new BufferedReader(new InputStreamReader(stream))) {
			statutes.addAll(new CsvToBeanBuilder<Statute>(reader)
					.withType(Statute.class)
					.withSkipLines(1)
					.build()
					.parse());
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public final static String[] COMMON_FIRST_NAMES = new String[] {
		"Noah",
		"Liam",
		"Jackson",
		"Lucas",
		"Logan",
		"Benjamin",
		"Jacob",
		"William",
		"Oliver",
		"James",
		"Lincoln",
		"Jack",
		"Ethan",
		"Carter",
		"Aiden",
		"Grayson",
		"Mason",
		"Owen",
		"Leo",
		"Nathan",
		"Olivia",
		"Emma",
		"Charlotte",
		"Sophia",
		"Aria",
		"Ava",
		"Chloe",
		"Zoey",
		"Abigail",
		"Amelia",
		"Emily",
		"Isabella",
		"Mila",
		"Maya",
		"Lily",
		"Riley",
		"Madison",
		"Mia",
		"Nora",
		"Ella",
	};

	public final static String[] COMMON_LAST_NAMES = new String[] {
		"Doe",
		"Smith",
		"Johnson",
		"Skywalker",
		"Organa",
		"Solo",
		"Dameron",
		"Tico",
		"Frik",
		"Williams",
		"Jones",
		"Miller",
		"Davis",
		"Wilson",
		"Anderson",
		"Harris",
		"Thompson",
		"Walker",
		"Young",
		"Green",
		"Brown",
		"Baker",
		"Campbell",
		"Evans",
		"Edwards",
		"Bell",
		"Cook",
		"Ward",
		"Gray",
		"Brooks",
		"Price",
		"Sanders",
		"Henderson",
		"Long",
		"Simmons",
		"Foster",
		"Ford",
		"Hamilton",
		"Graham",
		"West",
		"Parker",
	};

	public final static String[] COMMON_CITY_NAMES = new String[] {
			"Springfield",
			"Summerfield",
			"Winterfield",
			"Franklin",
			"Lebanon",
			"Clinton",
			"Greenville",
			"Bluesville",
			"Redsville",
			"Portsville",
			"Bristol",
			"Fairview",
			"Greatview",
			"Goodview",
			"Salem",
			"Madison",
			"Georgetown",
			"Arlington",
			"Ashland",
			"Dover",
			"Oxford",
			"Jackson",
			"Burlington",
			"Manchester",
			"Milton",
			"Auburn",
			"Centerville",
			"Leftville",
			"Rightville",
			"Clayton",
			"Dayton",
			"Niteton",
			"Lexington",
			"Milford",
			"Mount Vernon",
			"Oakland",
			"Firland",
			"Spruceland",
			"Mapleland",
			"Pineland",
			"Cedarland",
			"Evergreenland",
			"Winchester",
			"Cleveland",
			"Hudson",
			"Kingston",
			"Queenston",
			"Riverside",
			"Mountainside",
			"Forestside",
			"Oceanside",
			"Pleasantville",
			"Cottonwood",
	};

	public final static String[] COMMON_STREET_NAMES = new String[] {
			"Park",
			"Oak",
			"Main",
			"Pine",
			"Maple",
			"Cedar",
			"Elm",
			"View",
			"Lake",
			"Hill",
			"Hillside",
			"First",
			"Second",
			"Third",
			"Fourth",
			"Fifth",
			"Sixth",
			"Seventh",
			"Eighth",
			"Dogwood",
			"Park",
			"Birch",
			"Willow",
			"Sunset",
			"Quail",
			"Cypress",
			"Redwood",
			"Lake",
			"Lakeside",
			"Meadow",
			"Hillside",
			"Ridge",
			"Bay",
			"Lakeview",
			"Hickory",
			"Sunset",
			"Hemlock",
			"Pleasant",
			"Walnut",
			"Canyon",
			"River",
			"Shore",
			"Broadway",
			"Cherry",
			"Highland",
			"Hampton",
	};

	public final static String[] COMMON_STREET_TYPES = new String[] {
			"Street",
			"Avenue",
			"Boulevard",
			"Drive",
			"Lane",
			"Place",
			"Road",
			"Way",
			"Crescent",
			"Lane",
			"Terrace",
	};

	public final static String[] COMMON_VIOLATION_DESCRIPTIONS = new String[] {
			"Speeding",
			"Excessive speeding",
			"Reckless driving",
			"Running a red light / stop sign",
			"Failure to yield",
			"Driving without a valid driver's license",
			"Distracted driving",
			"Leaving the scene of an accident",
			"Driving a car with burned-out headlights",
			"Parking in a handicap spot without the required sticker",
			"Overdue parking meter",
			"Illegal U-Turn",
			"Driving under the influence",
			"Exceed axle weight",
			"Exceed licensed gross vehicle weight",
			"Fail to carry licence"
	};

	public final static String[] COMMON_VEHICLE_MAKES = new String[] {
			"Acura",
			"Audi",
			"BMW",
			"Buick",
			"Chrysler",
			"Dodge",
			"Ford",
			"GMC",
			"Honda",
			"Jaguar",
			"Kia",
			"Lexus",
			"Mazda",
			"Tesla",
			"Toyota",
	};

	public final static String[] COMMON_VEHICLE_TYPES = new String[] {
			"Camry",
			"Civic",
			"Ram",
			"Sierra",
			"Silverado",
			"Tacoma",
	};

	public final static String[] COMMON_VEHICLE_COLOURS = new String[] {
			"Black",
			"Blue",
			"Red",
			"Silver",
			"White",
	};

	public final static String[] BC_ACTS = new String[] {
			"MVA",
			"CTA"
	};

	public final static String[] PROVINCES = new String[] { "BC", "AB", "SK", "MB" };

	public static int randomInt(int min, int max) {
		return (int) Math.round((Math.random() * (max-min)) + min);
	}

	public static long randomLong(long min, long max) {
		return Math.round((Math.random() * (max-min)) + min);
	}

	public static double randomDouble(int min, int max) {
		return (Math.random() * (max-min)) + min;
	}

	public static String randomAlphabetic(int count) {
		return RandomStringUtils.random(count, 0, 0, true, false, null, new Random());
	}

	public static String randomNumeric(int count) {
		return RandomStringUtils.random(count, 0, 0, false, true, null, new Random());
	}

	public static boolean randomBool() {
		return (randomInt(0, 1) == 1);
	}

	public static String randomSurname() {
    	int index = randomInt(0, COMMON_LAST_NAMES.length-1);
    	return COMMON_LAST_NAMES[index];
	}

	public static Statute randomStatute() {
		Statute statute = null;
		do {
			statute = statutes.get(randomInt(0, statutes.size()-1));
		} while (statute == null || StringUtils.isBlank(statute.getDescription()));
		return statute;
	}

	public static String randomGivenName() {
    	int index = randomInt(0, COMMON_FIRST_NAMES.length-1);
    	return COMMON_FIRST_NAMES[index];
	}

	public static String randomProvince() {
    	int index = randomInt(0, PROVINCES.length-1);
    	return PROVINCES[index];
	}

	public static String randomCity() {
    	int index = randomInt(0, COMMON_CITY_NAMES.length-1);
    	return COMMON_CITY_NAMES[index];
	}

	private static String randomViolationDescription() {
    	int index = randomInt(0, COMMON_VIOLATION_DESCRIPTIONS.length-1);
    	return COMMON_VIOLATION_DESCRIPTIONS[index];
	}

	public static String randomVehicleMake() {
    	int index = randomInt(0, COMMON_VEHICLE_MAKES.length-1);
    	return COMMON_VEHICLE_MAKES[index];
	}

	public static String randomVehicleType() {
    	int index = randomInt(0, COMMON_VEHICLE_TYPES.length-1);
    	return COMMON_VEHICLE_TYPES[index];
	}

	public static String randomVehicleColour() {
    	int index = randomInt(0, COMMON_VEHICLE_COLOURS.length-1);
    	return COMMON_VEHICLE_COLOURS[index];
	}

	private static String randomBCAct() {
    	int index = randomInt(0, BC_ACTS.length-1);
    	return BC_ACTS[index];
	}

	private static String randomBCActSection() {
    	StringBuffer sb = new StringBuffer();
    	sb.append(randomInt(1, 200));
    	if (randomBool()) {
    		sb.append("(");
    		sb.append(RandomStringUtils.random(1, 97, 105, true, false));
    		sb.append(")");
    	}
    	if (randomBool()) {
    		sb.append("(");
    		sb.append(randomNumeric(1));
    		sb.append(")");
    	}
    	return sb.toString();
	}

    public static String randomPostalCode() {
    	StringBuilder postalCode = new StringBuilder();
    	postalCode.append(randomAlphabetic(1).toUpperCase());
    	postalCode.append(randomNumeric(1));
    	postalCode.append(randomAlphabetic(1).toUpperCase());
    	postalCode.append(randomNumeric(1));
    	postalCode.append(randomAlphabetic(1).toUpperCase());
    	postalCode.append(randomNumeric(1));

    	return postalCode.toString();
    }

    public static String randomAddress() {
    	StringBuilder address = new StringBuilder();
    	address.append("" + randomInt(1, 1000));

    	int randomStreetNameIndex = randomInt(0, COMMON_STREET_NAMES.length-1);
    	address.append(" " + COMMON_STREET_NAMES[randomStreetNameIndex]);

    	int randomStreetTypeIndex = randomInt(0, COMMON_STREET_TYPES.length-1);
    	address.append(" " + COMMON_STREET_TYPES[randomStreetTypeIndex]);

    	return address.toString();
    }

	public static Point getRandomLocation(Style style, int textWidth, int textHeight, int x1, int x2, int x3, int y1, int y2, int y3) {
		int th = (int) ((textHeight / style.getHeightScale()) + style.getHeightOffset());
		int minX = x1;
		int maxX = x3 - textWidth;
		int x = (int) (Math.random() * (maxX - minX)) + minX;
		int minY = (x < x2 ? y2 : y1) + th;
		int maxY = y3;
		int y = (int) (Math.random() * (maxY - minY)) + minY;

		return new Point(x, y);
	}

	public static String randomYear(int len, int min, int max) {
		return StringUtils.right(randomInt(min, max) + "", len);
	}

    public static Date randomDate(Date minDate, Date maxDate) {
        Date date = new Date();
        long minMilliSecs = minDate.getTime();
        long maxMilliSecs = maxDate.getTime();
        long l = randomLong(minMilliSecs, maxMilliSecs);
        date.setTime(l);
        return date;
    }

	public static String randomMonth(boolean pad) {
		String mm = randomInt(1, 12) + "";
		return pad ? StringUtils.leftPad(mm, 2, "0") : mm;  // sometime lead month with a zero
	}

	public static String randomDay(boolean pad) {
		String mm = randomInt(1, 29) + "";
		return pad ? StringUtils.leftPad(mm, 2, "0") : mm;  // sometime lead month with a zero
	}

	public static String randomHour(boolean pad) {
		String mm = randomInt(0, 23) + "";
		return pad ? StringUtils.leftPad(mm, 2, "0") : mm;  // sometime lead month with a zero
	}

	public static String randomMinute(boolean pad) {
		String mm = randomInt(1, 59) + "";
		return pad ? StringUtils.leftPad(mm, 2, "0") : mm;  // sometime lead month with a zero
	}

	public static String randomLicensePlate() {
		return RandomStringUtils.randomAlphanumeric(3).toUpperCase() + " " +
		       RandomStringUtils.randomAlphanumeric(3).toUpperCase();
	}

	public static String randomCheck(int min, int max) {
		switch (randomInt(min, max)) {
		case 0:
			return "x";
		case 1:
		default:
			return "";
		}
	}

	public static ViolationTicket randomTicket(Integer numCounts) {
		if (numCounts == null)
			numCounts = Integer.valueOf(3);

		ViolationTicket violationTicket = new ViolationTicket();
		violationTicket.setViolationTicketNumber("A" + RandomStringUtils.random(1, 65, 90, true, false) + randomNumeric(8));
		violationTicket.setSurname(randomSurname());
		violationTicket.setGivenName(randomGivenName());
		violationTicket.setIsYoungPerson(randomCheck(0, 4));
		violationTicket.setDriversLicenceProvince(randomProvince());
		violationTicket.setDriversLicenceNumber(randomNumeric(7));
		violationTicket.setDriversLicenceCreated(randomYear(randomBool() ? 2 : 4, 2016, 2021));
		violationTicket.setDriversLicenceExpiry(randomYear(randomBool() ? 2 : 4, 2022, 2025));
		violationTicket.setBirthdate(DateUtil.toDateString(randomDate(DateUtil.fromDateString("1950-01-01"), DateUtil.fromDateString("2005-12-31"))));
		violationTicket.setAddress(randomAddress());
		violationTicket.setIsChangeOfAddress(randomCheck(0, 4));
		violationTicket.setCity(randomCity());
		violationTicket.setProvince(randomProvince());
		violationTicket.setPostalCode(randomPostalCode());
		switch (randomInt(0, 5)) {
		case 0:
			violationTicket.setNamedIsDriver(randomCheck(0, 1));
			break;
		case 1:
			violationTicket.setNamedIsCyclist(randomCheck(0, 1));
			break;
		case 2:
			violationTicket.setNamedIsOwner(randomCheck(0, 1));
			break;
		case 3:
			violationTicket.setNamedIsPedestrain(randomCheck(0, 1));
			break;
		case 4:
			violationTicket.setNamedIsPassenger(randomCheck(0, 1));
			break;
		case 5:
			violationTicket.setNamedIsOther(randomCheck(0, 1));
			violationTicket.setNamedIsOtherDescription("Other");
			break;
		}
		violationTicket.setViolationDate(DateUtil.toDateString(new Date()));
		violationTicket.setViolationTime(DateUtil.toTimeString(randomDate(DateUtil.startOfToday(), DateUtil.endOfToday())));
		violationTicket.setViolationOnHighway(randomAddress());
		violationTicket.setViolationNearPlace(randomCity());
//		switch (randomInt(0, 7)) {
//		case 0:
//			violationTicket.setOffenseIsMVA(randomCheck(0, 0));
			violationTicket.setOffenseIsMVA("x");
//			break;
//		case 1:
//			violationTicket.setOffenseIsWLA(randomCheck(0, 1));
//			break;
//		case 2:
//			violationTicket.setOffenseIsLCA(randomCheck(0, 1));
//			break;
//		case 3:
//			violationTicket.setOffenseIsMCA(randomCheck(0, 1));
//			break;
//		case 4:
//			violationTicket.setOffenseIsFAA(randomCheck(0, 1));
//			break;
//		case 5:
//			violationTicket.setOffenseIsTCR(randomCheck(0, 1));
//			break;
//		case 6:
//			violationTicket.setOffenseIsCTA(randomCheck(0, 1));
//			break;
//		case 7:
//			violationTicket.setOffenseIsOther(randomCheck(0, 1));
//			violationTicket.setOffenseIsOtherDescription("Other");
//			break;
//		}

		Statute count1 = randomStatute();
		violationTicket.setCount1Description(count1.getDescription());
		violationTicket.setCount1ActReg(count1.getAct());
		violationTicket.setCount1IsACT("x");
		violationTicket.setCount1Section(count1.getSection());
		violationTicket.setCount1TicketAmount((randomInt(1, 50) * 10) + "");

		if (numCounts.intValue() > 1) {
			Statute count2 = randomStatute();
			violationTicket.setCount2Description(count2.getDescription());
			violationTicket.setCount2ActReg(count2.getAct());
			violationTicket.setCount2IsACT("x");
			violationTicket.setCount2Section(count2.getSection());
			violationTicket.setCount2TicketAmount((randomInt(1, 50) * 10) + "");
		}

		if (numCounts.intValue() > 2) {
			Statute count3 = randomStatute();
			violationTicket.setCount3Description(count3.getDescription());
			violationTicket.setCount3ActReg(count3.getAct());
			violationTicket.setCount3IsACT("x");
			violationTicket.setCount3Section(count3.getSection());
			violationTicket.setCount3TicketAmount((randomInt(1, 50) * 10) + "");
		}

		violationTicket.setVehicleLicensePlateProvince(randomProvince());
		violationTicket.setVehicleLicensePlateNumber(randomLicensePlate());
		violationTicket.setVehicleNscPuj(randomNumeric(2));
		violationTicket.setVehicleNscNumber(randomNumeric(7));
		violationTicket.setVehicleRegisteredOwnerName(randomGivenName() + " " + randomSurname());
		violationTicket.setVehicleMake(randomVehicleMake());
		violationTicket.setVehicleType(randomVehicleType());
		violationTicket.setVehicleColour(randomVehicleColour());
		violationTicket.setVehicleYear(randomInt(1980, 2021) + "");
		violationTicket.setNoticeOfDisputeAddress(randomAddress());
		violationTicket.setHearingLocation(randomCity());
		violationTicket.setDateOfService(DateUtil.toDateString(new Date()));
		violationTicket.setEnforcementOfficerNumber(randomNumeric(7));
		violationTicket.setDetachmentLocation(violationTicket.getHearingLocation() + " Police");
		return violationTicket;
	}
}
