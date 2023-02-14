package ca.bc.gov.open.jag.tco.oracledataapi.util;

import java.util.Date;

import org.apache.commons.lang3.time.DateFormatUtils;

public class DateUtil {

	public static final String DATE_TIME_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";
	public static final String DATE_FORMAT = "yyyy-MM-dd";
	public static final String TIME_FORMAT = "HH:mm";

	/**
	 * Returns the time portion (HH:mm) of the given date in 24-hour clock (GMT).
	 * @param date
	 * @return
	 */
	public static String formatAsHourMinuteUTC(Date date) {
		return DateFormatUtils.formatUTC(date, TIME_FORMAT);
	}

	/**
	 * Returns the date in the format "yyyy-MM-dd'T'HH:mm:ss'Z'"
	 * @param date
	 * @return
	 */
	public static String formatAsDateTimeUTC(Date date) {
		return DateFormatUtils.formatUTC(date, DATE_TIME_FORMAT);
	}

}
