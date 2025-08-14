package ca.bc.gov.open.jag.tco.oracledataapi.config;

import com.fasterxml.jackson.core.JsonGenerator;
import com.fasterxml.jackson.databind.JsonSerializer;
import com.fasterxml.jackson.databind.SerializerProvider;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.TimeZone;

/**
 * Thread-safe, timezone-neutral Jackson serializer that preserves database date/time values exactly as stored,
 * without any timezone interpretation or conversion. Treats dates as raw timestamp values.
 */
public class GlobalDateSerializer extends JsonSerializer<Date> {

    private static final String DATE_PATTERN = "yyyy-MM-dd";
    private static final String DATE_TIME_PATTERN = "yyyy-MM-dd'T'HH:mm:ss";
    private static final TimeZone SYSTEM_DEFAULT_TZ = TimeZone.getDefault();

    @Override
    public void serialize(Date date, JsonGenerator gen, SerializerProvider serializers) throws IOException {
        if (date != null) {
        	// Use default calendar (no timezone override) to check for time components
            Calendar cal = Calendar.getInstance();
            cal.setTime(date);
            
            boolean hasTimeComponent = cal.get(Calendar.HOUR_OF_DAY) != 0 || 
                                     cal.get(Calendar.MINUTE) != 0 || 
                                     cal.get(Calendar.SECOND) != 0 ||
                                     cal.get(Calendar.MILLISECOND) != 0;
            
            if (hasTimeComponent) {
            	// Format as date-time using the date's raw value without timezone conversion
                SimpleDateFormat dateTimeFormat = new SimpleDateFormat(DATE_TIME_PATTERN);
                dateTimeFormat.setTimeZone(SYSTEM_DEFAULT_TZ);
                gen.writeString(dateTimeFormat.format(date));
            } else {
            	// Format as date-only using the date's raw value without timezone conversion
                SimpleDateFormat dateFormat = new SimpleDateFormat(DATE_PATTERN);
                dateFormat.setTimeZone(SYSTEM_DEFAULT_TZ);
                gen.writeString(dateFormat.format(date));
            }
        }
    }
}