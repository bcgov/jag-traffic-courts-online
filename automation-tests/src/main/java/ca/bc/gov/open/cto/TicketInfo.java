package ca.bc.gov.open.cto;

import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.util.Random;

public class TicketInfo {
	public static String 			RANDOM_NUMBER 				= generateRandomNumber();
	public static String			TICKET_EMAIL 				= "max.chernohor@nttdata.com";
	public static String 			TICKET_DATE_M_D_Y 			= getFormattedYesterdayDateMDY();
	public static String 			TICKET_DATE_Y_M_D 			= getFormattedYesterdayDateYMD();
	public static String 			E_TICKET_NUMBER 			= "ET" + RANDOM_NUMBER;
	public static String 			E_TICKET_TIME_HOURS 		= "09";
	public static String 			E_TICKET_TIME_MINUTES 		= "54";
	public static String 			E_TICKET_INVOICE_1			= E_TICKET_NUMBER + "1";
	public static String 			E_TICKET_INVOICE_2			= E_TICKET_NUMBER + "2";
	public static String 			E_TICKET_INVOICE_3			= E_TICKET_NUMBER + "3";
	public static String 			E_TICKET_COUNT_1			= "Speed Against Area Sign";
	public static String 			E_TICKET_COUNT_2 			= "Permit Passenger Without Seatbelt";
	public static String 			E_TICKET_COUNT_3			= "Using Electronic Device While Driving";
	public static String 			E_TICKET_ACT_1				= "MVA";
	public static String 			E_TICKET_ACT_2 				= "MVA";
	public static String 			E_TICKET_ACT_3				= "MVA";
	public static String 			E_TICKET_SECTION_1			= "146(5)";
	public static String 			E_TICKET_SECTION_2 			= "220(6)";
	public static String 			E_TICKET_SECTION_3			= "214.2(1)";
	public static String 			E_TICKET_AMOUNT_1			= "139";
	public static String 			E_TICKET_AMOUNT_2			= "109";
	public static String 			E_TICKET_AMOUNT_3			= "368";
	public static String 			TICKET_NUMBER 				= "AT" + RANDOM_NUMBER;
	public static String 			IMAGE_TICKET_DL_NUMBER		= "1234567";
	public static String 			IMAGE_TICKET_PROVINCE 		= "BC";
	public static String 			IMAGE_TICKET_STATE_OF_DL 	= "BC";
	public static String 			IMAGE_TICKET_TIME_HOURS 	= "15";
	public static String 			IMAGE_TICKET_TIME_MINUTES 	= "23";
	public static String 			IMAGE_TICKET_NAME 			= "TestName";
	public static String 			IMAGE_TICKET_SURNAME 		= RANDOM_NUMBER + " Surname";
	public static String 			IMAGE_TICKET_COUNT_1		= "Excessive Speeding";
	public static String 			IMAGE_TICKET_COUNT_2 		= "Drive Without Licence";
	public static String 			IMAGE_TICKET_COUNT_3		= "Slow Driving";
	public static String 			IMAGE_TICKET_ACT_1			= "MVA";
	public static String 			IMAGE_TICKET_ACT_2 			= "MVA";
	public static String 			IMAGE_TICKET_ACT_3			= "MVA";
	public static String 			IMAGE_TICKET_SECTION_1		= "148(1)";
	public static String 			IMAGE_TICKET_SECTION_2 		= "24(1)";
	public static String 			IMAGE_TICKET_SECTION_3		= "145(1)";
	public static String 			IMAGE_TICKET_PAY_AMOUNT_1	= "368";
	public static String 			IMAGE_TICKET_PAY_AMOUNT_2 	= "276";
	public static String 			IMAGE_TICKET_PAY_AMOUNT_3	= "121";

	public static String getFormattedYesterdayDateMDY() {
		// Get yesterday's date
		LocalDate yesterday = LocalDate.now().minusDays(1);

		// Format the date as "MMM d, yyyy"
		DateTimeFormatter formatter = DateTimeFormatter.ofPattern("MMM dd, yyyy");
		return yesterday.format(formatter);
	}

	public static String getFormattedYesterdayDateYMD() {
		// Get yesterday's date
		LocalDate yesterday = LocalDate.now().minusDays(1);

		// Format the date as "yyyy-MM-dd"
		DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd");
		return yesterday.format(formatter);
	}

	public static String generateRandomNumber() {
		// Get the current time in milliseconds
		long currentTimeMillis = System.currentTimeMillis();

		// Use Random class to generate an 8-digit random number
		Random random = new Random(currentTimeMillis);
		return String.valueOf(10000000 + random.nextInt(90000000));
	}

	public static void assignRandomVariables() {

		RANDOM_NUMBER = generateRandomNumber();
		E_TICKET_NUMBER = "ET" + RANDOM_NUMBER;
		E_TICKET_INVOICE_1 = E_TICKET_NUMBER + "1";
		E_TICKET_INVOICE_2 = E_TICKET_NUMBER + "2";
		E_TICKET_INVOICE_3 = E_TICKET_NUMBER + "3";
		TICKET_NUMBER = "AT" + RANDOM_NUMBER;
		IMAGE_TICKET_SURNAME = RANDOM_NUMBER + " Surname";
	}

}
