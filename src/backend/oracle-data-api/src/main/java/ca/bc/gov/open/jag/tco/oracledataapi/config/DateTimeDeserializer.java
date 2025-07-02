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
    
    static {
        // Make parsing strict - don't allow lenient parsing
        DATE_TIME_FORMAT.setLenient(false);
    }
    
    @Override
    public Date deserialize(JsonParser p, DeserializationContext ctxt) throws IOException {
        String dateString = p.getValueAsString();
        if (dateString == null || dateString.trim().isEmpty()) {
            return null;
        }
        
        try {
            String trimmed = dateString.trim();
            // Additional validation: ensure the string matches exact expected format
            if (!trimmed.matches("\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}")) {
                throw new ParseException("DateTime string does not match expected format yyyy-MM-ddTHH:mm:ss: " + trimmed, 0);
            }
            // Parse without timezone interpretation - treats as local time
            return DATE_TIME_FORMAT.parse(trimmed);
        } catch (ParseException e) {
            throw new IOException("Failed to parse date: " + dateString + ". Expected format: yyyy-MM-ddTHH:mm:ss", e);
        }
    }
}