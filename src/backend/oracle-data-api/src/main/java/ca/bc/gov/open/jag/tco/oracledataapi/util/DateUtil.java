package ca.bc.gov.open.jag.tco.oracledataapi.util;

import java.util.Date;

import org.apache.commons.lang3.time.DateFormatUtils;

public class DateUtil {

	public static final String DATE_TIME_FORMAT = "yyyy-MM-dd'T'HH:mm:ss";
	public static final String DATE_FORMAT = "yyyy-MM-dd";
	public static final String TIME_FORMAT = "HH:mm";

	/**
	 * Returns the time portion (HH:mm) of the given date without any timezone conversion.
	 * @param date
	 * @return
	 */
	public static String formatAsHourMinute(Date date) {
		return DateFormatUtils.format(date, TIME_FORMAT);
	}

	/**
	 * Returns the date in the format "yyyy-MM-dd'T'HH:mm:ss" without any timezone conversion
	 * @param date
	 * @return
	 */
	public static String formatAsDateTime(Date date) {
		return DateFormatUtils.format(date, DATE_TIME_FORMAT);
	}

	/**
	 * Returns the date in the format "yyyy-MM-dd" without any timezone conversion
	 * @param date
	 * @return
	 */
	public static String formatAsDate(Date date) {
		return DateFormatUtils.format(date, DATE_FORMAT);
	}

	// Legacy methods for backward compatibility - now timezone neutral
	
	/**
	 * @deprecated Use formatAsHourMinute instead. This method is now timezone neutral.
	 */
	@Deprecated
	public static String formatAsHourMinuteUTC(Date date) {
		return formatAsHourMinute(date);
	}

	/**
	 * @deprecated Use formatAsDateTime instead. This method is now timezone neutral.
	 */
	@Deprecated
	public static String formatAsDateTimeUTC(Date date) {
		return formatAsDateTime(date);
	}

}