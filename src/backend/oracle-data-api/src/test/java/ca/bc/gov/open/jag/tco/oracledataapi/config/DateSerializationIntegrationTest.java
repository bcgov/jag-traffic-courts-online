package ca.bc.gov.open.jag.tco.oracledataapi.config;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.TimeZone;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;

import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.PingResult;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.ViolationTicket;

/**
 * Integration tests for date serialization to ensure
 * the complete ORDS service pipeline handles dates correctly with timezone preservation.
 * This is a lightweight test that doesn't require full Spring Boot context.
 */
public class DateSerializationIntegrationTest {

    private ObjectMapper objectMapper;

    @BeforeEach
    void setUp() {
        // Create ObjectMapper with our custom configuration
        JacksonConfig jacksonConfig = new JacksonConfig();
        objectMapper = jacksonConfig.objectMapper();
    }

    @Test
    void testJacksonConfigurationIsApplied() {
        // Verify that our custom Jackson configuration is properly loaded
        assertNotNull(objectMapper, "ObjectMapper should be available");
        
        // Check that WRITE_DATES_AS_TIMESTAMPS is disabled
        assertEquals(false, objectMapper.getSerializationConfig().isEnabled(
                com.fasterxml.jackson.databind.SerializationFeature.WRITE_DATES_AS_TIMESTAMPS),
                "WRITE_DATES_AS_TIMESTAMPS should be disabled");
    }

    @Test
    void testPingResultJsonSerialization() throws Exception {
        // Test the PingResult model to verify datetime serialization
        PingResult pingResult = new PingResult();
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.JANUARY, 15, 14, 30, 45);
        cal.set(Calendar.MILLISECOND, 0);
        Date testDate = cal.getTime();
        
        pingResult.setCurrentTimestamp(testDate);
        pingResult.setStatus("OK");
        
        String json = objectMapper.writeValueAsString(pingResult);
        
        // Parse the JSON response
        JsonNode jsonNode = objectMapper.readTree(json);
        
        // Verify that currentTimestamp is in the expected datetime format
        JsonNode timestampNode = jsonNode.get("currentTimestamp");
        assertNotNull(timestampNode, "currentTimestamp should be present");
        
        String timestamp = timestampNode.asText();
        
        // Verify format matches yyyy-MM-ddTHH:mm:ss
        assertTrue(timestamp.matches("\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}"),
                  "Ping currentTimestamp should be in format yyyy-MM-ddTHH:mm:ss, but was: " + timestamp);
        
        // Verify it can be parsed back to a Date
        SimpleDateFormat format = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
        Date parsedDate = format.parse(timestamp);
        assertNotNull(parsedDate, "Timestamp should be parseable");
    }

    @Test
    void testViolationTicketJsonSerialization() throws Exception {
        // Test ViolationTicket serialization
        ViolationTicket testTicket = createTestViolationTicket();
        
        String requestJson = objectMapper.writeValueAsString(testTicket);
        
        // Verify the JSON contains properly formatted dates
        assertTrue(requestJson.contains("\"issuedDt\":\"2024-03-15T10:30:45\""),
                  "ViolationTicket issuedDt should be serialized as datetime");
        assertTrue(requestJson.contains("\"entDtm\":\"2024-03-16T14:20:30\""),
                  "ViolationTicket entDtm should be serialized as datetime");
        
        // Test deserialization
        ViolationTicket deserializedTicket = objectMapper.readValue(requestJson, ViolationTicket.class);
        assertEquals(testTicket.getIssuedDt(), deserializedTicket.getIssuedDt(),
                    "Deserialized issuedDt should match original");
        assertEquals(testTicket.getEntDtm(), deserializedTicket.getEntDtm(),
                    "Deserialized entDtm should match original");
    }

    @Test
    void testDisputeMixedDateSerialization() throws Exception {
        // Test Dispute with both date-only and datetime fields
        Dispute testDispute = createTestDispute();
        
        String requestJson = objectMapper.writeValueAsString(testDispute);
        
        // Verify mixed date format serialization
        assertTrue(requestJson.contains("\"filingDt\":\"2024-04-10\""),
                  "Dispute filingDt should be serialized as date-only");
        assertTrue(requestJson.contains("\"submittedDt\":\"2024-04-11T09:15:20\""),
                  "Dispute submittedDt should be serialized as datetime");
        assertTrue(requestJson.contains("\"entDtm\":\"2024-04-12T16:45:10\""),
                  "Dispute entDtm should be serialized as datetime");
        
        // Test deserialization preserves both formats
        Dispute deserializedDispute = objectMapper.readValue(requestJson, Dispute.class);
        
        // For date-only field, compare date components
        Calendar originalCal = Calendar.getInstance();
        originalCal.setTime(testDispute.getFilingDt());
        Calendar deserializedCal = Calendar.getInstance();
        deserializedCal.setTime(deserializedDispute.getFilingDt());
        
        assertEquals(originalCal.get(Calendar.YEAR), deserializedCal.get(Calendar.YEAR));
        assertEquals(originalCal.get(Calendar.MONTH), deserializedCal.get(Calendar.MONTH));
        assertEquals(originalCal.get(Calendar.DAY_OF_MONTH), deserializedCal.get(Calendar.DAY_OF_MONTH));
        
        // For datetime fields, exact match
        assertEquals(testDispute.getSubmittedDt(), deserializedDispute.getSubmittedDt());
        assertEquals(testDispute.getEntDtm(), deserializedDispute.getEntDtm());
    }

    @Test
    void testTimezoneIndependentSerialization() throws Exception {
        // Test that serialization produces consistent results across different system timezones
        TimeZone originalTimezone = TimeZone.getDefault();
        
        try {
            // Create test data
            PingResult pingResult = new PingResult();
            Calendar cal = Calendar.getInstance();
            cal.set(2024, Calendar.MAY, 20, 13, 45, 30);
            cal.set(Calendar.MILLISECOND, 0);
            Date testDate = cal.getTime();
            pingResult.setCurrentTimestamp(testDate);
            pingResult.setStatus("OK");
            
            // Serialize in original timezone
            String jsonInOriginalTz = objectMapper.writeValueAsString(pingResult);
            
            // Change system timezone
            TimeZone.setDefault(TimeZone.getTimeZone("America/New_York"));
            
            // Serialize in different timezone
            String jsonInDifferentTz = objectMapper.writeValueAsString(pingResult);
            
            // Change to another timezone
            TimeZone.setDefault(TimeZone.getTimeZone("Europe/London"));
            
            // Serialize in third timezone
            String jsonInThirdTz = objectMapper.writeValueAsString(pingResult);
            
            // All JSON outputs should be identical
            assertEquals(jsonInOriginalTz, jsonInDifferentTz,
                        "JSON should be identical across timezone changes (original vs NY)");
            assertEquals(jsonInOriginalTz, jsonInThirdTz,
                        "JSON should be identical across timezone changes (original vs London)");
            
            // Verify the timestamp format is preserved
            assertTrue(jsonInOriginalTz.contains("\"currentTimestamp\":\"2024-05-20T13:45:30\""),
                      "Timestamp should maintain original format regardless of system timezone");
            
        } finally {
            // Restore original timezone
            TimeZone.setDefault(originalTimezone);
        }
    }

    @Test
    void testGlobalDateSerializerIntegration() throws Exception {
        // Test that the GlobalDateSerializer correctly determines date vs datetime formats
        
        // Create a date with no time component
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.JUNE, 15, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date dateOnly = cal.getTime();
        
        // Create a date with time component
        cal.set(2024, Calendar.JUNE, 16, 14, 30, 45);
        cal.set(Calendar.MILLISECOND, 0);
        Date dateTime = cal.getTime();
        
        Dispute dispute = new Dispute();
        dispute.setDisputeId("testGlobal");
        dispute.setFilingDt(dateOnly);  // Should be serialized as date-only
        dispute.setSubmittedDt(dateTime);  // Should be serialized as datetime
        
        String json = objectMapper.writeValueAsString(dispute);
        
        // Verify the global serializer correctly identifies and formats each type
        assertTrue(json.contains("\"filingDt\":\"2024-06-15\""),
                  "Date with no time component should be serialized as date-only");
        assertTrue(json.contains("\"submittedDt\":\"2024-06-16T14:30:45\""),
                  "Date with time component should be serialized as datetime");
    }

    @Test
    void testEdgeCaseDateHandling() throws Exception {
        // Test edge cases for date handling
        
        Calendar cal = Calendar.getInstance();
        
        // Test midnight (edge case between date-only and datetime)
        cal.set(2024, Calendar.JULY, 4, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 1); // 1 millisecond should trigger datetime format
        Date midnightWithMillis = cal.getTime();
        
        // Test exactly midnight with no sub-second components
        cal.set(Calendar.MILLISECOND, 0);
        Date exactMidnight = cal.getTime();
        
        Dispute dispute = new Dispute();
        dispute.setDisputeId("edgeCase");
        dispute.setSubmittedDt(midnightWithMillis);
        dispute.setUserAssignedDtm(exactMidnight);
        
        String json = objectMapper.writeValueAsString(dispute);
        
        // Even 1 millisecond should trigger datetime format
        assertTrue(json.contains("\"submittedDt\":\"2024-07-04T00:00:00\""),
                  "Midnight with milliseconds should be serialized as datetime");
        
        // Exact midnight with no time components should still be datetime if detected as having time
        assertTrue(json.contains("\"userAssignedDtm\":\"2024-07-04T00:00:00\"") || 
                  json.contains("\"userAssignedDtm\":\"2024-07-04\""),
                  "Exact midnight should be serialized consistently");
    }

    private ViolationTicket createTestViolationTicket() {
        ViolationTicket ticket = new ViolationTicket();
        
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.MARCH, 15, 10, 30, 45);
        cal.set(Calendar.MILLISECOND, 0);
        ticket.setIssuedDt(cal.getTime());
        
        cal.set(2024, Calendar.MARCH, 16, 14, 20, 30);
        cal.set(Calendar.MILLISECOND, 0);
        ticket.setEntDtm(cal.getTime());
        
        ticket.setTicketNumberTxt("TEST123456");
        ticket.setViolationTicketId("VT001");
        ticket.setEntUserId("testuser");
        
        return ticket;
    }

    private Dispute createTestDispute() {
        Dispute dispute = new Dispute();
        
        Calendar cal = Calendar.getInstance();
        
        // Date-only field
        cal.set(2024, Calendar.APRIL, 10, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        dispute.setFilingDt(cal.getTime());
        
        // Datetime fields
        cal.set(2024, Calendar.APRIL, 11, 9, 15, 20);
        cal.set(Calendar.MILLISECOND, 0);
        dispute.setSubmittedDt(cal.getTime());
        
        cal.set(2024, Calendar.APRIL, 12, 16, 45, 10);
        cal.set(Calendar.MILLISECOND, 0);
        dispute.setEntDtm(cal.getTime());
        
        dispute.setDisputeId("DISP001");
        dispute.setEntUserId("testuser");
        
        return dispute;
    }
}
