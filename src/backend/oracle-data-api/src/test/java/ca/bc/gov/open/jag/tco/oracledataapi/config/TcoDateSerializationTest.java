package ca.bc.gov.open.jag.tco.oracledataapi.config;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.junit.jupiter.api.Assertions.assertTrue;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.TimeZone;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;

import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.AuditLogEntry;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJCourtAppearance;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDispute;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeCount;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.JJDisputeRemark;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.tco.api.model.PingResult;

/**
 * Test class to verify that TCO date serialization/deserialization preserves exact database values
 * without timezone conversion, ensuring consistency across different system timezones.
 */
public class TcoDateSerializationTest {

    private ObjectMapper objectMapper;

    private SimpleDateFormat dateOnlyFormat;
    private SimpleDateFormat dateTimeFormat;

    @BeforeEach
    void setUp() {
        // Create ObjectMapper with our custom configuration
        JacksonConfig jacksonConfig = new JacksonConfig();
        objectMapper = jacksonConfig.objectMapper();
        
        dateOnlyFormat = new SimpleDateFormat("yyyy-MM-dd");
        dateTimeFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss");
        // Ensure formatters are timezone-neutral for testing
        dateOnlyFormat.setTimeZone(TimeZone.getDefault());
        dateTimeFormat.setTimeZone(TimeZone.getDefault());
    }

    @Test
    void testPingResultCurrentTimestampSerialization() throws JsonProcessingException, ParseException {
        // Test datetime serialization for PingResult.currentTimestamp
        PingResult pingResult = new PingResult();
        
        // Create a test datetime with specific values
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.JANUARY, 15, 14, 30, 45);
        cal.set(Calendar.MILLISECOND, 0);
        Date testDateTime = cal.getTime();
        
        pingResult.setCurrentTimestamp(testDateTime);
        pingResult.setStatus("OK");

        String json = objectMapper.writeValueAsString(pingResult);
        
        // Verify the datetime is serialized in the expected format
        assertTrue(json.contains("\"currentTimestamp\":\"2024-01-15T14:30:45\""), 
                   "PingResult currentTimestamp should be serialized as datetime format");
        
        // Test deserialization preserves the exact value
        PingResult deserializedResult = objectMapper.readValue(json, PingResult.class);
        assertEquals(testDateTime, deserializedResult.getCurrentTimestamp(),
                     "Deserialized datetime should match original exactly");
    }

    @Test
    void testJJDisputeDateOnlySerialization() throws JsonProcessingException, ParseException {
        // Test date-only serialization for JJDispute.disputantBirthDt
        JJDispute dispute = new JJDispute();
        
        // Create a date-only value (no time component)
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.MARCH, 10, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date testDate = cal.getTime();
        
        dispute.setDisputantBirthDt(testDate);
        dispute.setDisputeId("12345");

        String json = objectMapper.writeValueAsString(dispute);
        
        // Verify the date is serialized in date-only format
        assertTrue(json.contains("\"disputantBirthDt\":\"2024-03-10\""), 
                   "JJDispute disputantBirthDt should be serialized as date-only format");
        
        // Test deserialization preserves the date value
        JJDispute deserializedDispute = objectMapper.readValue(json, JJDispute.class);
        
        // Compare only the date components (ignore any time differences due to timezone handling)
        Calendar originalCal = Calendar.getInstance();
        originalCal.setTime(testDate);
        Calendar deserializedCal = Calendar.getInstance();
        deserializedCal.setTime(deserializedDispute.getDisputantBirthDt());
        
        assertEquals(originalCal.get(Calendar.YEAR), deserializedCal.get(Calendar.YEAR));
        assertEquals(originalCal.get(Calendar.MONTH), deserializedCal.get(Calendar.MONTH));
        assertEquals(originalCal.get(Calendar.DAY_OF_MONTH), deserializedCal.get(Calendar.DAY_OF_MONTH));
    }

    @Test
    void testJJDisputeDateTimeSerialization() throws JsonProcessingException {
        // Test datetime serialization for JJDispute audit fields
        JJDispute dispute = new JJDispute();
        
        // Create datetime values with specific time components
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.FEBRUARY, 20, 9, 15, 30);
        cal.set(Calendar.MILLISECOND, 0);
        Date entDtm = cal.getTime();
        
        cal.set(2024, Calendar.FEBRUARY, 21, 16, 45, 15);
        cal.set(Calendar.MILLISECOND, 0);
        Date updDtm = cal.getTime();

        dispute.setEntDtm(entDtm);
        dispute.setUpdDtm(updDtm);
        dispute.setEntUserId("testuser1");
        dispute.setUpdUserId("testuser2");
        dispute.setDisputeId("67890");

        String json = objectMapper.writeValueAsString(dispute);
        
        // Verify datetime fields are serialized with time components
        assertTrue(json.contains("\"entDtm\":\"2024-02-20T09:15:30\""), 
                   "JJDispute entDtm should be serialized as datetime format");
        assertTrue(json.contains("\"updDtm\":\"2024-02-21T16:45:15\""), 
                   "JJDispute updDtm should be serialized as datetime format");
        
        // Test deserialization
        JJDispute deserializedDispute = objectMapper.readValue(json, JJDispute.class);
        assertEquals(entDtm, deserializedDispute.getEntDtm(),
                     "Deserialized entDtm should match original exactly");
        assertEquals(updDtm, deserializedDispute.getUpdDtm(),
                     "Deserialized updDtm should match original exactly");
    }

    @Test
    void testJJDisputeMixedDateSerialization() throws JsonProcessingException {
        // Test JJDispute with both date-only and datetime fields
        JJDispute dispute = new JJDispute();
        
        Calendar cal = Calendar.getInstance();
        
        // Date-only field
        cal.set(2024, Calendar.APRIL, 10, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date disputantBirthDt = cal.getTime();
        
        // Datetime fields
        cal.set(2024, Calendar.APRIL, 11, 9, 15, 20);
        cal.set(Calendar.MILLISECOND, 0);
        Date submittedDt = cal.getTime();
        
        cal.set(2024, Calendar.APRIL, 12, 14, 30, 45);
        cal.set(Calendar.MILLISECOND, 0);
        Date jjDecisionDt = cal.getTime();
        
        dispute.setDisputantBirthDt(disputantBirthDt);
        dispute.setSubmittedDt(submittedDt);
        dispute.setJjDecisionDt(jjDecisionDt);
        dispute.setDisputeId("DISP001");

        String json = objectMapper.writeValueAsString(dispute);
        
        // Verify mixed date format serialization
        assertTrue(json.contains("\"disputantBirthDt\":\"2024-04-10\""),
                  "JJDispute disputantBirthDt should be serialized as date-only");
        assertTrue(json.contains("\"submittedDt\":\"2024-04-11T09:15:20\""),
                  "JJDispute submittedDt should be serialized as datetime");
        assertTrue(json.contains("\"jjDecisionDt\":\"2024-04-12T14:30:45\""),
                  "JJDispute jjDecisionDt should be serialized as datetime");
        
        // Test deserialization preserves both formats
        JJDispute deserializedDispute = objectMapper.readValue(json, JJDispute.class);
        
        // For date-only field, compare date components
        Calendar originalCal = Calendar.getInstance();
        originalCal.setTime(disputantBirthDt);
        Calendar deserializedCal = Calendar.getInstance();
        deserializedCal.setTime(deserializedDispute.getDisputantBirthDt());
        
        assertEquals(originalCal.get(Calendar.YEAR), deserializedCal.get(Calendar.YEAR));
        assertEquals(originalCal.get(Calendar.MONTH), deserializedCal.get(Calendar.MONTH));
        assertEquals(originalCal.get(Calendar.DAY_OF_MONTH), deserializedCal.get(Calendar.DAY_OF_MONTH));
        
        // For datetime fields, exact match
        assertEquals(submittedDt, deserializedDispute.getSubmittedDt());
        assertEquals(jjDecisionDt, deserializedDispute.getJjDecisionDt());
    }

    @Test
    void testJJDisputeCountDateSerialization() throws JsonProcessingException {
        // Test date and datetime serialization for JJDisputeCount
        JJDisputeCount disputeCount = new JJDisputeCount();
        
        Calendar cal = Calendar.getInstance();
        
        // Date-only fields
        cal.set(2024, Calendar.MAY, 15, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date fineDueDt = cal.getTime();
        
        cal.set(2024, Calendar.MAY, 20, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date violationDt = cal.getTime();
        
        // Datetime fields
        cal.set(2024, Calendar.MAY, 16, 10, 30, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date latestPleaUpdateDtm = cal.getTime();
        
        cal.set(2024, Calendar.MAY, 17, 14, 45, 30);
        cal.set(Calendar.MILLISECOND, 0);
        Date accEntDtm = cal.getTime();
        
        disputeCount.setFineDueDt(fineDueDt);
        disputeCount.setViolationDt(violationDt);
        disputeCount.setLatestPleaUpdateDtm(latestPleaUpdateDtm);
        disputeCount.setAccEntDtm(accEntDtm);
        disputeCount.setDisputeCountId("COUNT001");

        String json = objectMapper.writeValueAsString(disputeCount);
        
        // Verify date-only fields
        assertTrue(json.contains("\"fineDueDt\":\"2024-05-15\""),
                  "JJDisputeCount fineDueDt should be serialized as date-only");
        assertTrue(json.contains("\"violationDt\":\"2024-05-20\""),
                  "JJDisputeCount violationDt should be serialized as date-only");
        
        // Verify datetime fields
        assertTrue(json.contains("\"latestPleaUpdateDtm\":\"2024-05-16T10:30:00\""),
                  "JJDisputeCount latestPleaUpdateDtm should be serialized as datetime");
        assertTrue(json.contains("\"accEntDtm\":\"2024-05-17T14:45:30\""),
                  "JJDisputeCount accEntDtm should be serialized as datetime");
        
        // Test deserialization
        JJDisputeCount deserializedCount = objectMapper.readValue(json, JJDisputeCount.class);
        
        // Compare date-only fields (date components only)
        Calendar originalCal = Calendar.getInstance();
        Calendar deserializedCal = Calendar.getInstance();
        
        originalCal.setTime(fineDueDt);
        deserializedCal.setTime(deserializedCount.getFineDueDt());
        assertEquals(originalCal.get(Calendar.YEAR), deserializedCal.get(Calendar.YEAR));
        assertEquals(originalCal.get(Calendar.MONTH), deserializedCal.get(Calendar.MONTH));
        assertEquals(originalCal.get(Calendar.DAY_OF_MONTH), deserializedCal.get(Calendar.DAY_OF_MONTH));
        
        // Compare datetime fields (exact match)
        assertEquals(latestPleaUpdateDtm, deserializedCount.getLatestPleaUpdateDtm());
        assertEquals(accEntDtm, deserializedCount.getAccEntDtm());
    }

    @Test
    void testJJCourtAppearanceDateTimeSerialization() throws JsonProcessingException {
        // Test datetime serialization for JJCourtAppearance
        JJCourtAppearance courtAppearance = new JJCourtAppearance();
        
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.JUNE, 8, 11, 45, 20);
        cal.set(Calendar.MILLISECOND, 0);
        Date appearanceDtm = cal.getTime();
        
        cal.set(2024, Calendar.JUNE, 9, 16, 30, 15);
        cal.set(Calendar.MILLISECOND, 0);
        Date disputantNotPresentDtm = cal.getTime();
        
        courtAppearance.setAppearanceDtm(appearanceDtm);
        courtAppearance.setDisputantNotPresentDtm(disputantNotPresentDtm);
        courtAppearance.setCourtAppearanceId("COURT001");

        String json = objectMapper.writeValueAsString(courtAppearance);
        
        assertTrue(json.contains("\"appearanceDtm\":\"2024-06-08T11:45:20\""), 
                   "JJCourtAppearance appearanceDtm should be serialized as datetime format");
        assertTrue(json.contains("\"disputantNotPresentDtm\":\"2024-06-09T16:30:15\""), 
                   "JJCourtAppearance disputantNotPresentDtm should be serialized as datetime format");
        
        JJCourtAppearance deserializedAppearance = objectMapper.readValue(json, JJCourtAppearance.class);
        assertEquals(appearanceDtm, deserializedAppearance.getAppearanceDtm(),
                     "Deserialized appearanceDtm should match original exactly");
        assertEquals(disputantNotPresentDtm, deserializedAppearance.getDisputantNotPresentDtm(),
                     "Deserialized disputantNotPresentDtm should match original exactly");
    }

    @Test
    void testJJDisputeRemarkDateTimeSerialization() throws JsonProcessingException {
        // Test datetime serialization for JJDisputeRemark
        JJDisputeRemark disputeRemark = new JJDisputeRemark();
        
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.JULY, 15, 14, 20, 35);
        cal.set(Calendar.MILLISECOND, 0);
        Date remarksMadeDtm = cal.getTime();
        
        cal.set(2024, Calendar.JULY, 16, 9, 30, 45);
        cal.set(Calendar.MILLISECOND, 0);
        Date entDtm = cal.getTime();
        
        disputeRemark.setRemarksMadeDtm(remarksMadeDtm);
        disputeRemark.setEntDtm(entDtm);
        disputeRemark.setDisputeRemarkId("REMARK001");
        disputeRemark.setDisputeId("DISPUTE001");

        String json = objectMapper.writeValueAsString(disputeRemark);
        
        assertTrue(json.contains("\"remarksMadeDtm\":\"2024-07-15T14:20:35\""), 
                   "JJDisputeRemark remarksMadeDtm should be serialized as datetime format");
        assertTrue(json.contains("\"entDtm\":\"2024-07-16T09:30:45\""), 
                   "JJDisputeRemark entDtm should be serialized as datetime format");
        
        JJDisputeRemark deserializedRemark = objectMapper.readValue(json, JJDisputeRemark.class);
        assertEquals(remarksMadeDtm, deserializedRemark.getRemarksMadeDtm(),
                     "Deserialized remarksMadeDtm should match original exactly");
        assertEquals(entDtm, deserializedRemark.getEntDtm(),
                     "Deserialized entDtm should match original exactly");
    }

    @Test
    void testAuditLogEntryDateTimeSerialization() throws JsonProcessingException {
        // Test datetime serialization for AuditLogEntry audit fields
        AuditLogEntry auditEntry = new AuditLogEntry();
        
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.AUGUST, 22, 16, 55, 10);
        cal.set(Calendar.MILLISECOND, 0);
        Date entDtm = cal.getTime();
        
        cal.set(2024, Calendar.AUGUST, 23, 11, 20, 25);
        cal.set(Calendar.MILLISECOND, 0);
        Date updDtm = cal.getTime();
        
        auditEntry.setEntDtm(entDtm);
        auditEntry.setUpdDtm(updDtm);
        auditEntry.setEntUserId("audituser");
        auditEntry.setAuditLogEntryId("audit123");
        auditEntry.setDisputeId("dispute456");

        String json = objectMapper.writeValueAsString(auditEntry);
        
        assertTrue(json.contains("\"entDtm\":\"2024-08-22T16:55:10\""), 
                   "AuditLogEntry entDtm should be serialized as datetime format");
        assertTrue(json.contains("\"updDtm\":\"2024-08-23T11:20:25\""), 
                   "AuditLogEntry updDtm should be serialized as datetime format");
        
        AuditLogEntry deserializedEntry = objectMapper.readValue(json, AuditLogEntry.class);
        assertEquals(entDtm, deserializedEntry.getEntDtm(),
                     "Deserialized entDtm should match original exactly");
        assertEquals(updDtm, deserializedEntry.getUpdDtm(),
                     "Deserialized updDtm should match original exactly");
    }

    @Test
    void testTimezoneIndependence() throws JsonProcessingException {
        // Test that serialization is timezone-independent
        TimeZone originalTimezone = TimeZone.getDefault();
        
        try {
            // Create a test date in the current timezone
            Calendar cal = Calendar.getInstance();
            cal.set(2024, Calendar.SEPTEMBER, 10, 12, 30, 45);
            cal.set(Calendar.MILLISECOND, 0);
            Date testDate = cal.getTime();
            
            PingResult pingResult = new PingResult();
            pingResult.setCurrentTimestamp(testDate);
            pingResult.setStatus("OK");
            
            // Serialize in original timezone
            String jsonInOriginalTz = objectMapper.writeValueAsString(pingResult);
            
            // Change to a different timezone
            TimeZone.setDefault(TimeZone.getTimeZone("UTC"));
            
            // Serialize in different timezone
            String jsonInDifferentTz = objectMapper.writeValueAsString(pingResult);
            
            // The JSON should be identical regardless of system timezone
            assertEquals(jsonInOriginalTz, jsonInDifferentTz,
                        "Serialization should be timezone-independent");
            
            // Verify deserialization gives same result in both timezones
            PingResult deserializedInOriginal = objectMapper.readValue(jsonInOriginalTz, PingResult.class);
            PingResult deserializedInDifferent = objectMapper.readValue(jsonInDifferentTz, PingResult.class);
            
            assertEquals(deserializedInOriginal.getCurrentTimestamp(), 
                        deserializedInDifferent.getCurrentTimestamp(),
                        "Deserialization should give same result regardless of timezone");
            
        } finally {
            // Restore original timezone
            TimeZone.setDefault(originalTimezone);
        }
    }

    @Test
    void testNullDateHandling() throws JsonProcessingException {
        // Test that null dates are handled properly
        JJDispute dispute = new JJDispute();
        dispute.setDisputeId("nulltest");
        dispute.setDisputantBirthDt(null);
        dispute.setEntDtm(null);
        dispute.setUpdDtm(null);

        String json = objectMapper.writeValueAsString(dispute);
        
        // Verify null dates are serialized as null or omitted
        assertTrue(json.contains("\"disputantBirthDt\":null") || !json.contains("\"disputantBirthDt\""), 
                   "Null disputantBirthDt should be serialized as null or omitted");
        
        JJDispute deserializedDispute = objectMapper.readValue(json, JJDispute.class);
        assertEquals(null, deserializedDispute.getDisputantBirthDt(),
                     "Null disputantBirthDt should remain null after deserialization");
    }

    @Test
    void testObjectMapperConfiguration() {
        // Verify that the ObjectMapper is properly configured
        assertNotNull(objectMapper, "ObjectMapper should be available");
        
        // Verify that WRITE_DATES_AS_TIMESTAMPS is disabled
        assertEquals(false, objectMapper.getSerializationConfig().isEnabled(
                com.fasterxml.jackson.databind.SerializationFeature.WRITE_DATES_AS_TIMESTAMPS),
                     "WRITE_DATES_AS_TIMESTAMPS should be disabled");
    }
}