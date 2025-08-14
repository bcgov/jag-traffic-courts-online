package ca.bc.gov.open.jag.tco.oracledataapi.config;

import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.databind.DeserializationContext;
import com.fasterxml.jackson.databind.JsonDeserializer;

import java.io.IOException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimeZone;

/**
 * Custom Jackson deserializer for Date fields that should only parse date (yyyy-MM-dd)
 * without time component. Creates Date objects from date-only strings.
 */
public class DateOnlyDeserializer extends JsonDeserializer<Date> {
    
    private static final String DATE_PATTERN = "yyyy-MM-dd";
    private static final TimeZone SYSTEM_DEFAULT_TZ = TimeZone.getDefault();

    @Override
    public Date deserialize(JsonParser parser, DeserializationContext context) throws IOException {
        String dateString = parser.getText();
        
        if (dateString == null || dateString.trim().isEmpty()) {
            return null;
        }
        
        try {
            String trimmed = dateString.trim();
            // Additional validation: ensure the string matches exact expected format
            if (!trimmed.matches("\\d{4}-\\d{2}-\\d{2}")) {
                throw new ParseException("Date string does not match expected format yyyy-MM-dd: " + trimmed, 0);
            }
            
            SimpleDateFormat formatter = new SimpleDateFormat(DATE_PATTERN);
            formatter.setTimeZone(SYSTEM_DEFAULT_TZ);
            formatter.setLenient(false);
            
            return formatter.parse(trimmed);
        } catch (ParseException e) {
            throw new IOException("Failed to parse date: " + dateString + ". Expected format: yyyy-MM-dd", e);
        }
    }
}