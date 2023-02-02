package ca.bc.gov.open.jag.tco.oracledataapi.util;

import static org.junit.jupiter.api.Assertions.assertEquals;

import java.text.ParseException;
import java.util.Date;

import org.apache.commons.lang3.time.DateFormatUtils;
import org.apache.commons.lang3.time.DateUtils;
import org.junit.jupiter.api.Test;

class DateUtilTest {

	@Test
	void test() throws ParseException {
		Date date = DateUtils.parseDate("2022-03-29T13:45:30-07:00", DateFormatUtils.ISO_8601_EXTENDED_DATETIME_TIME_ZONE_FORMAT.getPattern());
		assertEquals("20:45", DateUtil.formatAsHourMinuteUTC(date));
	}

}
