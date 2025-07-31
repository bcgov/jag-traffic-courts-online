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

import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.AuditLogEntry;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJCourtAppearance;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeCount;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeRemark;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.PingResult;

/**
 * Integration tests for TCO date serialization to ensure
 * the complete ORDS service pipeline handles dates correctly with timezone preservation.
 * This is a lightweight test that doesn't require full Spring Boot context.
 */
public class TcoDateSerializationIntegrationTest {

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
    void testJJDisputeJsonSerialization() throws Exception {
        // Test JJDispute serialization with mixed date formats
        JJDispute testDispute = createTestJJDispute();
        
        String requestJson = objectMapper.writeValueAsString(testDispute);
        
        // Verify the JSON contains properly formatted dates
        assertTrue(requestJson.contains("\"disputantBirthDt\":\"2024-03-15\""),
                  "JJDispute disputantBirthDt should be serialized as date-only");
        assertTrue(requestJson.contains("\"jjDecisionDt\":\"2024-03-16T14:20:30\""),
                  "JJDispute jjDecisionDt should be serialized as datetime");
        assertTrue(requestJson.contains("\"entDtm\":\"2024-03-17T16:45:10\""),
                  "JJDispute entDtm should be serialized as datetime");
        
        // Test deserialization
        JJDispute deserializedDispute = objectMapper.readValue(requestJson, JJDispute.class);
        
        // For date-only field, compare date components
        Calendar originalCal = Calendar.getInstance();
        originalCal.setTime(testDispute.getDisputantBirthDt());
        Calendar deserializedCal = Calendar.getInstance();
        deserializedCal.setTime(deserializedDispute.getDisputantBirthDt());
        
        assertEquals(originalCal.get(Calendar.YEAR), deserializedCal.get(Calendar.YEAR));
        assertEquals(originalCal.get(Calendar.MONTH), deserializedCal.get(Calendar.MONTH));
        assertEquals(originalCal.get(Calendar.DAY_OF_MONTH), deserializedCal.get(Calendar.DAY_OF_MONTH));
        
        // For datetime fields, exact match
        assertEquals(testDispute.getJjDecisionDt(), deserializedDispute.getJjDecisionDt());
        assertEquals(testDispute.getEntDtm(), deserializedDispute.getEntDtm());
    }

    @Test
    void testJJDisputeCountMixedDateSerialization() throws Exception {
        // Test JJDisputeCount with both date-only and datetime fields
        JJDisputeCount testDisputeCount = createTestJJDisputeCount();
        
        String requestJson = objectMapper.writeValueAsString(testDisputeCount);
        
        // Verify mixed date format serialization
        assertTrue(requestJson.contains("\"fineDueDt\":\"2024-04-10\""),
                  "JJDisputeCount fineDueDt should be serialized as date-only");
        assertTrue(requestJson.contains("\"violationDt\":\"2024-04-12\""),
                  "JJDisputeCount violationDt should be serialized as date-only");
        assertTrue(requestJson.contains("\"latestPleaUpdateDtm\":\"2024-04-11T09:15:20\""),
                  "JJDisputeCount latestPleaUpdateDtm should be serialized as datetime");
        assertTrue(requestJson.contains("\"accEntDtm\":\"2024-04-13T16:45:10\""),
                  "JJDisputeCount accEntDtm should be serialized as datetime");
        
        // Test deserialization preserves both formats
        JJDisputeCount deserializedCount = objectMapper.readValue(requestJson, JJDisputeCount.class);
        
        // For date-only fields, compare date components
        Calendar originalCal = Calendar.getInstance();
        originalCal.setTime(testDisputeCount.getFineDueDt());
        Calendar deserializedCal = Calendar.getInstance();
        deserializedCal.setTime(deserializedCount.getFineDueDt());
        
        assertEquals(originalCal.get(Calendar.YEAR), deserializedCal.get(Calendar.YEAR));
        assertEquals(originalCal.get(Calendar.MONTH), deserializedCal.get(Calendar.MONTH));
        assertEquals(originalCal.get(Calendar.DAY_OF_MONTH), deserializedCal.get(Calendar.DAY_OF_MONTH));
        
        // For datetime fields, exact match
        assertEquals(testDisputeCount.getLatestPleaUpdateDtm(), deserializedCount.getLatestPleaUpdateDtm());
        assertEquals(testDisputeCount.getAccEntDtm(), deserializedCount.getAccEntDtm());
    }

    @Test
    void testJJCourtAppearanceJsonSerialization() throws Exception {
        // Test JJCourtAppearance serialization
        JJCourtAppearance testCourtAppearance = createTestJJCourtAppearance();
        
        String requestJson = objectMapper.writeValueAsString(testCourtAppearance);
        
        // Verify the JSON contains properly formatted datetime
        assertTrue(requestJson.contains("\"appearanceDtm\":\"2024-05-20T10:30:45\""),
                  "JJCourtAppearance appearanceDtm should be serialized as datetime");
        assertTrue(requestJson.contains("\"disputantNotPresentDtm\":\"2024-05-21T14:20:30\""),
                  "JJCourtAppearance disputantNotPresentDtm should be serialized as datetime");
        
        // Test deserialization
        JJCourtAppearance deserializedAppearance = objectMapper.readValue(requestJson, JJCourtAppearance.class);
        assertEquals(testCourtAppearance.getAppearanceDtm(), deserializedAppearance.getAppearanceDtm(),
                    "Deserialized appearanceDtm should match original");
        assertEquals(testCourtAppearance.getDisputantNotPresentDtm(), deserializedAppearance.getDisputantNotPresentDtm(),
                    "Deserialized disputantNotPresentDtm should match original");
    }

    @Test
    void testJJDisputeRemarkJsonSerialization() throws Exception {
        // Test JJDisputeRemark serialization
        JJDisputeRemark testRemark = createTestJJDisputeRemark();
        
        String requestJson = objectMapper.writeValueAsString(testRemark);
        
        // Verify the JSON contains properly formatted datetime
        assertTrue(requestJson.contains("\"remarksMadeDtm\":\"2024-06-15T11:45:20\""),
                  "JJDisputeRemark remarksMadeDtm should be serialized as datetime");
        assertTrue(requestJson.contains("\"entDtm\":\"2024-06-16T09:30:15\""),
                  "JJDisputeRemark entDtm should be serialized as datetime");
        
        // Test deserialization
        JJDisputeRemark deserializedRemark = objectMapper.readValue(requestJson, JJDisputeRemark.class);
        assertEquals(testRemark.getRemarksMadeDtm(), deserializedRemark.getRemarksMadeDtm(),
                    "Deserialized remarksMadeDtm should match original");
        assertEquals(testRemark.getEntDtm(), deserializedRemark.getEntDtm(),
                    "Deserialized entDtm should match original");
    }

    @Test
    void testAuditLogEntryJsonSerialization() throws Exception {
        // Test AuditLogEntry serialization
        AuditLogEntry testAuditEntry = createTestAuditLogEntry();
        
        String requestJson = objectMapper.writeValueAsString(testAuditEntry);
        
        // Verify the JSON contains properly formatted datetime
        assertTrue(requestJson.contains("\"entDtm\":\"2024-07-20T13:25:35\""),
                  "AuditLogEntry entDtm should be serialized as datetime");
        assertTrue(requestJson.contains("\"updDtm\":\"2024-07-21T15:40:50\""),
                  "AuditLogEntry updDtm should be serialized as datetime");
        
        // Test deserialization
        AuditLogEntry deserializedEntry = objectMapper.readValue(requestJson, AuditLogEntry.class);
        assertEquals(testAuditEntry.getEntDtm(), deserializedEntry.getEntDtm(),
                    "Deserialized entDtm should match original");
        assertEquals(testAuditEntry.getUpdDtm(), deserializedEntry.getUpdDtm(),
                    "Deserialized updDtm should match original");
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
        
        JJDispute dispute = new JJDispute();
        dispute.setDisputeId("testGlobal");
        dispute.setDisputantBirthDt(dateOnly);  // Should be serialized as date-only
        dispute.setJjDecisionDt(dateTime);  // Should be serialized as datetime
        
        String json = objectMapper.writeValueAsString(dispute);
        
        // Verify the global serializer correctly identifies and formats each type
        assertTrue(json.contains("\"disputantBirthDt\":\"2024-06-15\""),
                  "Date with no time component should be serialized as date-only");
        assertTrue(json.contains("\"jjDecisionDt\":\"2024-06-16T14:30:45\""),
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
        
        JJDispute dispute = new JJDispute();
        dispute.setDisputeId("edgeCase");
        dispute.setSubmittedDt(midnightWithMillis);
        dispute.setJjDecisionDt(exactMidnight);
        
        String json = objectMapper.writeValueAsString(dispute);
        
        // Even 1 millisecond should trigger datetime format
        assertTrue(json.contains("\"submittedDt\":\"2024-07-04T00:00:00\""),
                  "Midnight with milliseconds should be serialized as datetime");
        
        // Exact midnight with no time components should still be datetime if detected as having time
        assertTrue(json.contains("\"jjDecisionDt\":\"2024-07-04T00:00:00\"") || 
                  json.contains("\"jjDecisionDt\":\"2024-07-04\""),
                  "Exact midnight should be serialized consistently");
    }

    @Test
    void testComplexObjectSerialization() throws Exception {
        // Test serialization of complex TCO objects with nested date fields
        JJDispute dispute = createComplexJJDispute();
        
        String json = objectMapper.writeValueAsString(dispute);
        
        // Verify all date formats are correct in the complex object
        assertTrue(json.contains("\"disputantBirthDt\":\"2024-08-10\""),
                  "Complex object date-only field should be serialized correctly");
        assertTrue(json.contains("\"submittedDt\":\"2024-08-11T10:15:30\""),
                  "Complex object datetime field should be serialized correctly");
        
        // Test deserialization maintains consistency
        JJDispute deserializedDispute = objectMapper.readValue(json, JJDispute.class);
        
        // Verify all nested date fields are preserved
        assertEquals(dispute.getSubmittedDt(), deserializedDispute.getSubmittedDt(),
                    "Complex object datetime should be preserved");
        
        // Check date components for date-only field
        Calendar originalCal = Calendar.getInstance();
        originalCal.setTime(dispute.getDisputantBirthDt());
        Calendar deserializedCal = Calendar.getInstance();
        deserializedCal.setTime(deserializedDispute.getDisputantBirthDt());
        
        assertEquals(originalCal.get(Calendar.YEAR), deserializedCal.get(Calendar.YEAR));
        assertEquals(originalCal.get(Calendar.MONTH), deserializedCal.get(Calendar.MONTH));
        assertEquals(originalCal.get(Calendar.DAY_OF_MONTH), deserializedCal.get(Calendar.DAY_OF_MONTH));
    }

    private JJDispute createTestJJDispute() {
        JJDispute dispute = new JJDispute();
        
        Calendar cal = Calendar.getInstance();
        
        // Date-only field
        cal.set(2024, Calendar.MARCH, 15, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        dispute.setDisputantBirthDt(cal.getTime());
        
        // Datetime fields
        cal.set(2024, Calendar.MARCH, 16, 14, 20, 30);
        cal.set(Calendar.MILLISECOND, 0);
        dispute.setJjDecisionDt(cal.getTime());
        
        cal.set(2024, Calendar.MARCH, 17, 16, 45, 10);
        cal.set(Calendar.MILLISECOND, 0);
        dispute.setEntDtm(cal.getTime());
        
        dispute.setDisputeId("DISP001");
        dispute.setEntUserId("testuser");
        
        return dispute;
    }

    private JJDisputeCount createTestJJDisputeCount() {
        JJDisputeCount disputeCount = new JJDisputeCount();
        
        Calendar cal = Calendar.getInstance();
        
        // Date-only fields
        cal.set(2024, Calendar.APRIL, 10, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        disputeCount.setFineDueDt(cal.getTime());
        
        cal.set(2024, Calendar.APRIL, 12, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        disputeCount.setViolationDt(cal.getTime());
        
        // Datetime fields
        cal.set(2024, Calendar.APRIL, 11, 9, 15, 20);
        cal.set(Calendar.MILLISECOND, 0);
        disputeCount.setLatestPleaUpdateDtm(cal.getTime());
        
        cal.set(2024, Calendar.APRIL, 13, 16, 45, 10);
        cal.set(Calendar.MILLISECOND, 0);
        disputeCount.setAccEntDtm(cal.getTime());
        
        disputeCount.setDisputeCountId("COUNT001");
        disputeCount.setAccEntUserId("testuser");
        
        return disputeCount;
    }

    private JJCourtAppearance createTestJJCourtAppearance() {
        JJCourtAppearance courtAppearance = new JJCourtAppearance();
        
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.MAY, 20, 10, 30, 45);
        cal.set(Calendar.MILLISECOND, 0);
        courtAppearance.setAppearanceDtm(cal.getTime());
        
        cal.set(2024, Calendar.MAY, 21, 14, 20, 30);
        cal.set(Calendar.MILLISECOND, 0);
        courtAppearance.setDisputantNotPresentDtm(cal.getTime());
        
        courtAppearance.setCourtAppearanceId("COURT001");
        
        return courtAppearance;
    }

    private JJDisputeRemark createTestJJDisputeRemark() {
        JJDisputeRemark remark = new JJDisputeRemark();
        
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.JUNE, 15, 11, 45, 20);
        cal.set(Calendar.MILLISECOND, 0);
        remark.setRemarksMadeDtm(cal.getTime());
        
        cal.set(2024, Calendar.JUNE, 16, 9, 30, 15);
        cal.set(Calendar.MILLISECOND, 0);
        remark.setEntDtm(cal.getTime());
        
        remark.setDisputeRemarkId("REMARK001");
        remark.setDisputeId("DISPUTE001");
        remark.setEntUserId("testuser");
        
        return remark;
    }

    private AuditLogEntry createTestAuditLogEntry() {
        AuditLogEntry auditEntry = new AuditLogEntry();
        
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.JULY, 20, 13, 25, 35);
        cal.set(Calendar.MILLISECOND, 0);
        auditEntry.setEntDtm(cal.getTime());
        
        cal.set(2024, Calendar.JULY, 21, 15, 40, 50);
        cal.set(Calendar.MILLISECOND, 0);
        auditEntry.setUpdDtm(cal.getTime());
        
        auditEntry.setAuditLogEntryId("AUDIT001");
        auditEntry.setDisputeId("DISPUTE001");
        auditEntry.setEntUserId("testuser");
        
        return auditEntry;
    }

    private JJDispute createComplexJJDispute() {
        JJDispute dispute = new JJDispute();
        
        Calendar cal = Calendar.getInstance();
        
        // Date-only field
        cal.set(2024, Calendar.AUGUST, 10, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        dispute.setDisputantBirthDt(cal.getTime());
        
        // Datetime fields
        cal.set(2024, Calendar.AUGUST, 11, 10, 15, 30);
        cal.set(Calendar.MILLISECOND, 0);
        dispute.setSubmittedDt(cal.getTime());
        
        cal.set(2024, Calendar.AUGUST, 12, 14, 30, 45);
        cal.set(Calendar.MILLISECOND, 0);
        dispute.setEntDtm(cal.getTime());
        
        dispute.setDisputeId("COMPLEX001");
        dispute.setEntUserId("testuser");
        dispute.setTicketNumberTxt("AA12345678");
        
        return dispute;
    }
}