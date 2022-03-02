package ca.bc.gov.court.traffic.ticket.util;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;

import org.apache.commons.lang3.StringUtils;
import org.apache.commons.lang3.time.DateFormatUtils;

public class DateUtil {

	private static final String DATE_PATTERN = "yyyy-MM-dd";
	private static final String TIME_PATTERN = "HH:mm";

	/**
	 * Returns a String representation of the given date in the form yyyy-MM-dd, or
	 * blank if date is null.
	 */
	public static String toDateString(Date date) {
		if (date == null) {
			return "";
		}
		return DateFormatUtils.format(date, DATE_PATTERN);
	}

	/**
	 * Returns a new Date from the given string representation.
	 *
	 * @param dateStr String
	 * @return String
	 */
	public static Date fromDateString(String dateStr) {
		try {
			return new SimpleDateFormat(DATE_PATTERN).parse(dateStr);
		} catch (Exception ignore) {
			return null;
		}
	}

	/**
	 * Returns a String representation of the given date in the form HH:mm, or blank
	 * if date is null.
	 */
	public static String toTimeString(Date date) {
		if (date == null) {
			return "";
		}
		return DateFormatUtils.format(date, TIME_PATTERN);
	}

	/**
	 * Returns a new Date from the given string representation.
	 *
	 * @param dateStr String
	 * @return String
	 */
	public static Date fromTimeString(String dateStr) {
		try {
			return new SimpleDateFormat(TIME_PATTERN).parse(dateStr);
		} catch (Exception ignore) {
			return null;
		}
	}

	public static String getYear(Date date) {
		if (date != null) {
			Calendar calendar = Calendar.getInstance();
			calendar.setTime(date);
			String year = calendar.get(Calendar.YEAR) + "";
			return StringUtils.leftPad(year, 4, "0");
		}
		return null;
	}

	public static String getMonth(Date date) {
		if (date != null) {
			Calendar calendar = Calendar.getInstance();
			calendar.setTime(date);
			String month = (calendar.get(Calendar.MONTH) + 1) + "";
			return StringUtils.leftPad(month, 2, "0");

		}
		return null;
	}

	public static String getDay(Date date) {
		if (date != null) {
			Calendar calendar = Calendar.getInstance();
			calendar.setTime(date);
			String day = calendar.get(Calendar.DAY_OF_MONTH) + "";
			return StringUtils.leftPad(day, 2, "0");

		}
		return null;
	}

	public static String getHour(Date date) {
		if (date != null) {
			Calendar calendar = Calendar.getInstance();
			calendar.setTime(date);
			String hour = calendar.get(Calendar.HOUR_OF_DAY) + "";
			return StringUtils.leftPad(hour, 2, "0");

		}
		return null;
	}

	public static String getMinute(Date date) {
		Calendar calendar = Calendar.getInstance();
		calendar.setTime(date);
		String minute = calendar.get(Calendar.MINUTE) + "";
		return StringUtils.leftPad(minute, 2, "0");

	}

	public static Date startOfToday() {
		Calendar calendar = Calendar.getInstance();
		calendar.setTime(new Date());
		calendar.set(Calendar.HOUR_OF_DAY, 0);
		calendar.set(Calendar.MINUTE, 0);
		calendar.set(Calendar.SECOND, 0);
		calendar.set(Calendar.MILLISECOND, 0);
		return calendar.getTime();
	}

	public static Date endOfToday() {
		Calendar calendar = Calendar.getInstance();
		calendar.setTime(new Date());
		calendar.set(Calendar.HOUR_OF_DAY, 23);
		calendar.set(Calendar.MINUTE, 59);
		calendar.set(Calendar.SECOND, 59);
		calendar.set(Calendar.MILLISECOND, 999);
		return calendar.getTime();
	}

}
