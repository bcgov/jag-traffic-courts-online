package ca.bc.gov.open.jag.tco.oracledataapi.config;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertThrows;

import java.io.IOException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.MockitoAnnotations;

import com.fasterxml.jackson.core.JsonGenerator;
import com.fasterxml.jackson.core.JsonParser;
import com.fasterxml.jackson.databind.DeserializationContext;
import com.fasterxml.jackson.databind.SerializerProvider;

/**
 * Unit tests for individual date serializers and deserializers to ensure 
 * they handle date formatting and timezone preservation correctly.
 */
public class DateSerializerDeserializerTest {

    @Mock
    private JsonGenerator jsonGenerator;
    
    @Mock
    private JsonParser jsonParser;
    
    @Mock
    private SerializerProvider serializerProvider;
    
    @Mock
    private DeserializationContext deserializationContext;

    private DateOnlySerializer dateOnlySerializer;
    private DateOnlyDeserializer dateOnlyDeserializer;
    private DateTimeSerializer dateTimeSerializer;
    private DateTimeDeserializer dateTimeDeserializer;
    private GlobalDateSerializer globalDateSerializer;

    @BeforeEach
    void setUp() {
        MockitoAnnotations.openMocks(this);
        dateOnlySerializer = new DateOnlySerializer();
        dateOnlyDeserializer = new DateOnlyDeserializer();
        dateTimeSerializer = new DateTimeSerializer();
        dateTimeDeserializer = new DateTimeDeserializer();
        globalDateSerializer = new GlobalDateSerializer();
    }

    @Test
    void testDateOnlySerializerWithValidDate() throws IOException {
        // Test date-only serialization
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.MARCH, 15, 10, 30, 45); // Time component should be ignored
        cal.set(Calendar.MILLISECOND, 0);
        Date testDate = cal.getTime();

        dateOnlySerializer.serialize(testDate, jsonGenerator, serializerProvider);

        Mockito.verify(jsonGenerator).writeString("2024-03-15");
    }

    @Test
    void testDateOnlySerializerWithNullDate() throws IOException {
        // Test null handling
        dateOnlySerializer.serialize(null, jsonGenerator, serializerProvider);

        Mockito.verify(jsonGenerator, Mockito.never()).writeString(Mockito.anyString());
    }

    @Test
    void testDateOnlyDeserializerWithValidDateString() throws IOException, ParseException {
        // Test date-only deserialization
        Mockito.when(jsonParser.getText()).thenReturn("2024-03-15");

        Date result = dateOnlyDeserializer.deserialize(jsonParser, deserializationContext);

        SimpleDateFormat expectedFormat = new SimpleDateFormat("yyyy-MM-dd");
        Date expectedDate = expectedFormat.parse("2024-03-15");
        assertEquals(expectedDate, result);
    }

    @Test
    void testDateOnlyDeserializerWithNullString() throws IOException {
        // Test null string handling
        Mockito.when(jsonParser.getText()).thenReturn(null);

        Date result = dateOnlyDeserializer.deserialize(jsonParser, deserializationContext);

        assertNull(result);
    }

    @Test
    void testDateOnlyDeserializerWithEmptyString() throws IOException {
        // Test empty string handling
        Mockito.when(jsonParser.getText()).thenReturn("   ");

        Date result = dateOnlyDeserializer.deserialize(jsonParser, deserializationContext);

        assertNull(result);
    }

    @Test
    void testDateOnlyDeserializerWithInvalidFormat() throws IOException {
        // Test invalid date format
        Mockito.when(jsonParser.getText()).thenReturn("15-03-2024");

        assertThrows(IOException.class, () -> {
            dateOnlyDeserializer.deserialize(jsonParser, deserializationContext);
        });
    }

    @Test
    void testDateTimeSerializerWithValidDateTime() throws IOException {
        // Test datetime serialization
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.APRIL, 20, 14, 45, 30);
        cal.set(Calendar.MILLISECOND, 0);
        Date testDateTime = cal.getTime();

        dateTimeSerializer.serialize(testDateTime, jsonGenerator, serializerProvider);

        Mockito.verify(jsonGenerator).writeString("2024-04-20T14:45:30");
    }

    @Test
    void testDateTimeSerializerWithNullDate() throws IOException {
        // Test null handling
        dateTimeSerializer.serialize(null, jsonGenerator, serializerProvider);

        Mockito.verify(jsonGenerator, Mockito.never()).writeString(Mockito.anyString());
    }

    @Test
    void testDateTimeDeserializerWithValidDateTime() throws IOException, ParseException {
        // Test datetime deserialization
        Mockito.when(jsonParser.getValueAsString()).thenReturn("2024-04-20T14:45:30");

        Date result = dateTimeDeserializer.deserialize(jsonParser, deserializationContext);

        SimpleDateFormat expectedFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
        Date expectedDate = expectedFormat.parse("2024-04-20T14:45:30");
        assertEquals(expectedDate, result);
    }

    @Test
    void testDateTimeDeserializerWithNullString() throws IOException {
        // Test null string handling
        Mockito.when(jsonParser.getValueAsString()).thenReturn(null);

        Date result = dateTimeDeserializer.deserialize(jsonParser, deserializationContext);

        assertNull(result);
    }

    @Test
    void testDateTimeDeserializerWithEmptyString() throws IOException {
        // Test empty string handling
        Mockito.when(jsonParser.getValueAsString()).thenReturn("  ");

        Date result = dateTimeDeserializer.deserialize(jsonParser, deserializationContext);

        assertNull(result);
    }

    @Test
    void testDateTimeDeserializerWithInvalidFormat() throws IOException {
        // Test invalid datetime format
        Mockito.when(jsonParser.getValueAsString()).thenReturn("2024-04-20 14:45:30");

        assertThrows(IOException.class, () -> {
            dateTimeDeserializer.deserialize(jsonParser, deserializationContext);
        });
    }

    @Test
    void testGlobalDateSerializerWithDateOnly() throws IOException {
        // Test global serializer with date-only (no time component)
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.MAY, 10, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date dateOnly = cal.getTime();

        globalDateSerializer.serialize(dateOnly, jsonGenerator, serializerProvider);

        Mockito.verify(jsonGenerator).writeString("2024-05-10");
    }

    @Test
    void testGlobalDateSerializerWithDateTime() throws IOException {
        // Test global serializer with datetime (has time component)
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.MAY, 10, 15, 30, 45);
        cal.set(Calendar.MILLISECOND, 500);
        Date dateTime = cal.getTime();

        globalDateSerializer.serialize(dateTime, jsonGenerator, serializerProvider);

        Mockito.verify(jsonGenerator).writeString("2024-05-10T15:30:45");
    }

    @Test
    void testGlobalDateSerializerWithTimeComponentButZeroValues() throws IOException {
        // Test edge case: hour/minute/second are 0 but milliseconds are not
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.MAY, 10, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 100); // Non-zero milliseconds should trigger datetime format
        Date dateTime = cal.getTime();

        globalDateSerializer.serialize(dateTime, jsonGenerator, serializerProvider);

        Mockito.verify(jsonGenerator).writeString("2024-05-10T00:00:00");
    }

    @Test
    void testGlobalDateSerializerWithNullDate() throws IOException {
        // Test null handling
        globalDateSerializer.serialize(null, jsonGenerator, serializerProvider);

        Mockito.verify(jsonGenerator, Mockito.never()).writeString(Mockito.anyString());
    }

    @Test
    void testGlobalDateSerializerTimeDetection() throws IOException {
        // Test various time combinations to ensure proper detection
        Calendar cal = Calendar.getInstance();
        
        // Test with only hour set
        cal.set(2024, Calendar.JUNE, 1, 1, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date hourOnly = cal.getTime();
        
        globalDateSerializer.serialize(hourOnly, jsonGenerator, serializerProvider);
        Mockito.verify(jsonGenerator).writeString("2024-06-01T01:00:00");
        
        Mockito.reset(jsonGenerator);
        
        // Test with only minute set
        cal.set(2024, Calendar.JUNE, 2, 0, 1, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date minuteOnly = cal.getTime();
        
        globalDateSerializer.serialize(minuteOnly, jsonGenerator, serializerProvider);
        Mockito.verify(jsonGenerator).writeString("2024-06-02T00:01:00");
        
        Mockito.reset(jsonGenerator);
        
        // Test with only second set
        cal.set(2024, Calendar.JUNE, 3, 0, 0, 1);
        cal.set(Calendar.MILLISECOND, 0);
        Date secondOnly = cal.getTime();
        
        globalDateSerializer.serialize(secondOnly, jsonGenerator, serializerProvider);
        Mockito.verify(jsonGenerator).writeString("2024-06-03T00:00:01");
    }

    @Test
    void testSerializerConsistency() throws IOException, ParseException {
        // Test that serialization and deserialization are consistent
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.JULY, 25, 16, 20, 10);
        cal.set(Calendar.MILLISECOND, 0);
        Date originalDate = cal.getTime();
        
        // Serialize with DateTime serializer
        dateTimeSerializer.serialize(originalDate, jsonGenerator, serializerProvider);
        
        // Capture the serialized string
        Mockito.verify(jsonGenerator).writeString("2024-07-25T16:20:10");
        
        // Mock parser to return the serialized string
        Mockito.when(jsonParser.getValueAsString()).thenReturn("2024-07-25T16:20:10");
        
        // Deserialize
        Date deserializedDate = dateTimeDeserializer.deserialize(jsonParser, deserializationContext);
        
        // Verify consistency
        assertEquals(originalDate, deserializedDate);
    }

    @Test
    void testDateOnlyConsistency() throws IOException, ParseException {
        // Test date-only serialization/deserialization consistency
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.AUGUST, 15, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date originalDate = cal.getTime();
        
        // Serialize with DateOnly serializer
        dateOnlySerializer.serialize(originalDate, jsonGenerator, serializerProvider);
        
        // Capture the serialized string
        Mockito.verify(jsonGenerator).writeString("2024-08-15");
        
        // Mock parser to return the serialized string
        Mockito.when(jsonParser.getText()).thenReturn("2024-08-15");
        
        // Deserialize
        Date deserializedDate = dateOnlyDeserializer.deserialize(jsonParser, deserializationContext);
        
        // For date-only, we only compare date components since time might differ
        SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd");
        assertEquals(dateFormat.format(originalDate), dateFormat.format(deserializedDate));
    }
}