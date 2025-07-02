package ca.bc.gov.open.jag.tco.oracledataapi.config;

import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.databind.DeserializationContext;
import com.fasterxml.jackson.databind.JsonDeserializer;

import java.io.IOException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

/**
 * Custom Jackson deserializer for Date fields that preserves the exact date/time values
 * without any timezone conversion. Treats incoming datetime strings as raw values.
 */
public class DateTimeDeserializer extends JsonDeserializer<Date> {
    
    private static final SimpleDateFormat DATE_TIME_FORMAT = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
    
    @Override
    public Date deserialize(JsonParser p, DeserializationContext ctxt) throws IOException {
        String dateString = p.getValueAsString();
        if (dateString == null || dateString.trim().isEmpty()) {
            return null;
        }
        
        try {
            // Parse without timezone interpretation - treats as local time
            return DATE_TIME_FORMAT.parse(dateString);
        } catch (ParseException e) {
            throw new IOException("Failed to parse date: " + dateString, e);
        }
    }
}

