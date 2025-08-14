package ca.bc.gov.open.jag.tco.oracledataapi.config;

import com.fasterxml.jackson.core.JsonGenerator;
import com.fasterxml.jackson.databind.JsonSerializer;
import com.fasterxml.jackson.databind.SerializerProvider;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimeZone;

/**
 * Custom Jackson serializer for Date fields that preserves the exact date/time values
 * from the database without any timezone conversion. Uses the raw Date value as-is.
 */
public class DateTimeSerializer extends JsonSerializer<Date> {
    
    // Pattern constant for consistency
    private static final String DATE_TIME_PATTERN = "yyyy-MM-dd'T'HH:mm:ss";
    private static final TimeZone SYSTEM_DEFAULT_TZ = TimeZone.getDefault();

    @Override
    public void serialize(Date date, JsonGenerator gen, SerializerProvider serializers) throws IOException {
        if (date != null) {
            SimpleDateFormat formatter = new SimpleDateFormat(DATE_TIME_PATTERN);
            formatter.setTimeZone(SYSTEM_DEFAULT_TZ);
            gen.writeString(formatter.format(date));
        }
    }
}