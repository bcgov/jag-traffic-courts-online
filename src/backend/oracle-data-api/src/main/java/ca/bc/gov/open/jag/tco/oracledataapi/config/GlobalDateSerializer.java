package ca.bc.gov.open.jag.tco.oracledataapi.config;

import com.fasterxml.jackson.core.JsonGenerator;
import com.fasterxml.jackson.databind.JsonSerializer;
import com.fasterxml.jackson.databind.SerializerProvider;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;

/**
 * Timezone-neutral Jackson serializer that preserves database date/time values exactly as stored,
 * without any timezone interpretation or conversion. Treats dates as raw timestamp values.
 */
public class GlobalDateSerializer extends JsonSerializer<Date> {
    
    // Create formatters without any timezone - they will use the date's raw millisecond value
    private static final SimpleDateFormat DATE_FORMAT = new SimpleDateFormat("yyyy-MM-dd");
    private static final SimpleDateFormat DATE_TIME_FORMAT = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");

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
                gen.writeString(DATE_TIME_FORMAT.format(date));
            } else {
                // Format as date-only using the date's raw value without timezone conversion
                gen.writeString(DATE_FORMAT.format(date));
            }
        }
    }
}