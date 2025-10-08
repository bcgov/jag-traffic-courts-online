package ca.bc.gov.open.jag.tco.oracledataapi.config;

import com.fasterxml.jackson.core.JsonGenerator;
import com.fasterxml.jackson.databind.JsonSerializer;
import com.fasterxml.jackson.databind.SerializerProvider;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimeZone;

/**
 * Custom Jackson serializer for Date fields that should only show date (yyyy-MM-dd)
 * without time component. Preserves the exact date value from the database.
 */
public class DateOnlySerializer extends JsonSerializer<Date> {
    
    private static final String DATE_PATTERN = "yyyy-MM-dd";
    private static final TimeZone SYSTEM_DEFAULT_TZ = TimeZone.getDefault();

    @Override
    public void serialize(Date date, JsonGenerator gen, SerializerProvider serializers) throws IOException {
        if (date != null) {
            SimpleDateFormat formatter = new SimpleDateFormat(DATE_PATTERN);
            formatter.setTimeZone(SYSTEM_DEFAULT_TZ);
            gen.writeString(formatter.format(date));
        }
    }
}