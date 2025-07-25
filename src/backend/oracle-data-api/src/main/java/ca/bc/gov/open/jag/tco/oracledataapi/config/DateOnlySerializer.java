package ca.bc.gov.open.jag.tco.oracledataapi.config;

import com.fasterxml.jackson.core.JsonGenerator;
import com.fasterxml.jackson.databind.JsonSerializer;
import com.fasterxml.jackson.databind.SerializerProvider;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;

/**
 * Custom Jackson serializer for Date fields that should only show date (yyyy-MM-dd)
 * without time component. Preserves the exact date value from the database.
 */
public class DateOnlySerializer extends JsonSerializer<Date> {
    
    private static final SimpleDateFormat DATE_FORMAT = new SimpleDateFormat("yyyy-MM-dd");

    @Override
    public void serialize(Date date, JsonGenerator gen, SerializerProvider serializers) throws IOException {
        if (date != null) {
            gen.writeString(DATE_FORMAT.format(date));
        }
    }
}