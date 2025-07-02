package ca.bc.gov.open.jag.tco.oracledataapi.config;

import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.databind.DeserializationContext;
import com.fasterxml.jackson.databind.JsonDeserializer;

import java.io.IOException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

/**
 * Custom Jackson deserializer for Date fields that should only parse date (yyyy-MM-dd)
 * without time component. Creates Date objects from date-only strings.
 */
public class DateOnlyDeserializer extends JsonDeserializer<Date> {
    
    private static final SimpleDateFormat DATE_FORMAT = new SimpleDateFormat("yyyy-MM-dd");

    @Override
    public Date deserialize(JsonParser parser, DeserializationContext context) throws IOException {
        String dateString = parser.getText();
        
        if (dateString == null || dateString.trim().isEmpty()) {
            return null;
        }
        
        try {
            return DATE_FORMAT.parse(dateString.trim());
        } catch (ParseException e) {
            throw new IOException("Failed to parse date: " + dateString + ". Expected format: yyyy-MM-dd", e);
        }
    }
}
