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

import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.AuditLogEntry;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.Dispute;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeListItem;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.DisputeUpdateRequest;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.OutgoingEmail;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.PingResult;
import ca.bc.gov.open.jag.tco.oracledataapi.ords.occam.api.model.ViolationTicket;

/**
 * Test class to verify that date serialization/deserialization preserves exact database values
 * without timezone conversion, ensuring consistency across different system timezones.
 */
public class DateSerializationTest {

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
    void testDisputeDateOnlySerialization() throws JsonProcessingException, ParseException {
        // Test date-only serialization for Dispute.filingDt
        Dispute dispute = new Dispute();
        
        // Create a date-only value (no time component)
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.MARCH, 10, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date testDate = cal.getTime();
        
        dispute.setFilingDt(testDate);
        dispute.setDisputeId("12345");

        String json = objectMapper.writeValueAsString(dispute);
        
        // Verify the date is serialized in date-only format
        assertTrue(json.contains("\"filingDt\":\"2024-03-10\""), 
                   "Dispute filingDt should be serialized as date-only format");
        
        // Test deserialization preserves the date value
        Dispute deserializedDispute = objectMapper.readValue(json, Dispute.class);
        
        // Compare only the date components (ignore any time differences due to timezone handling)
        Calendar originalCal = Calendar.getInstance();
        originalCal.setTime(testDate);
        Calendar deserializedCal = Calendar.getInstance();
        deserializedCal.setTime(deserializedDispute.getFilingDt());
        
        assertEquals(originalCal.get(Calendar.YEAR), deserializedCal.get(Calendar.YEAR));
        assertEquals(originalCal.get(Calendar.MONTH), deserializedCal.get(Calendar.MONTH));
        assertEquals(originalCal.get(Calendar.DAY_OF_MONTH), deserializedCal.get(Calendar.DAY_OF_MONTH));
    }

    @Test
    void testDisputeDateTimeSerialization() throws JsonProcessingException {
        // Test datetime serialization for Dispute audit fields
        Dispute dispute = new Dispute();
        
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
                   "Dispute entDtm should be serialized as datetime format");
        assertTrue(json.contains("\"updDtm\":\"2024-02-21T16:45:15\""), 
                   "Dispute updDtm should be serialized as datetime format");
        
        // Test deserialization
        Dispute deserializedDispute = objectMapper.readValue(json, Dispute.class);
        assertEquals(entDtm, deserializedDispute.getEntDtm(),
                     "Deserialized entDtm should match original exactly");
        assertEquals(updDtm, deserializedDispute.getUpdDtm(),
                     "Deserialized updDtm should match original exactly");
    }

    @Test
    void testViolationTicketDateTimeSerialization() throws JsonProcessingException {
        // Test datetime serialization for ViolationTicket.issuedDt
        ViolationTicket ticket = new ViolationTicket();
        
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.APRIL, 5, 13, 20, 45);
        cal.set(Calendar.MILLISECOND, 0);
        Date issuedDt = cal.getTime();
        
        ticket.setIssuedDt(issuedDt);
        ticket.setTicketNumberTxt("AA12345678");
        ticket.setViolationTicketId("98765");

        String json = objectMapper.writeValueAsString(ticket);
        
        assertTrue(json.contains("\"issuedDt\":\"2024-04-05T13:20:45\""), 
                   "ViolationTicket issuedDt should be serialized as datetime format");
        
        ViolationTicket deserializedTicket = objectMapper.readValue(json, ViolationTicket.class);
        assertEquals(issuedDt, deserializedTicket.getIssuedDt(),
                     "Deserialized issuedDt should match original exactly");
    }

    @Test
    void testDisputeListItemMixedDateFormats() throws JsonProcessingException {
        // Test both date-only and datetime serialization in DisputeListItem
        DisputeListItem listItem = new DisputeListItem();
        
        // Date-only field
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.MAY, 12, 0, 0, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date filingDt = cal.getTime();
        
        // Datetime field
        cal.set(2024, Calendar.MAY, 13, 10, 30, 0);
        cal.set(Calendar.MILLISECOND, 0);
        Date submittedDt = cal.getTime();
        
        listItem.setFilingDt(filingDt);
        listItem.setSubmittedDt(submittedDt);
        listItem.setDisputeId(12345);
        listItem.setTicketNumberTxt("BB98765432");

        String json = objectMapper.writeValueAsString(listItem);
        
        // The field with @JsonSerialize(using = DateOnlySerializer.class) should be date-only
        assertTrue(json.contains("\"filing_dt\":\"2024-05-12\""), 
                   "DisputeListItem filing_dt should be serialized as date-only format, but was: " + json);
        
        // Verify datetime field works correctly
        assertTrue(json.contains("\"submitted_dt\":\"2024-05-13T10:30:00\""), 
                   "DisputeListItem submitted_dt should be serialized as datetime, but was: " + json);
        
        // Test deserialization
        DisputeListItem deserializedItem = objectMapper.readValue(json, DisputeListItem.class);
        
        // Verify date-only field preserves date (compare only date components)
        Calendar originalFilingCal = Calendar.getInstance();
        originalFilingCal.setTime(filingDt);
        Calendar deserializedFilingCal = Calendar.getInstance();
        deserializedFilingCal.setTime(deserializedItem.getFilingDt());
        
        assertEquals(originalFilingCal.get(Calendar.YEAR), deserializedFilingCal.get(Calendar.YEAR));
        assertEquals(originalFilingCal.get(Calendar.MONTH), deserializedFilingCal.get(Calendar.MONTH));
        assertEquals(originalFilingCal.get(Calendar.DAY_OF_MONTH), deserializedFilingCal.get(Calendar.DAY_OF_MONTH));
        
        // Verify datetime field preserves exact time
        assertEquals(submittedDt, deserializedItem.getSubmittedDt());
    }

    @Test
    void testAuditLogEntryDateTimeSerialization() throws JsonProcessingException {
        // Test datetime serialization for AuditLogEntry audit fields
        AuditLogEntry auditEntry = new AuditLogEntry();
        
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.JUNE, 8, 11, 45, 20);
        cal.set(Calendar.MILLISECOND, 0);
        Date entDtm = cal.getTime();
        
        auditEntry.setEntDtm(entDtm);
        auditEntry.setEntUserId("audituser");
        auditEntry.setAuditLogEntryId("audit123");
        auditEntry.setDisputeId("dispute456");

        String json = objectMapper.writeValueAsString(auditEntry);
        
        assertTrue(json.contains("\"entDtm\":\"2024-06-08T11:45:20\""), 
                   "AuditLogEntry entDtm should be serialized as datetime format");
        
        AuditLogEntry deserializedEntry = objectMapper.readValue(json, AuditLogEntry.class);
        assertEquals(entDtm, deserializedEntry.getEntDtm(),
                     "Deserialized entDtm should match original exactly");
    }

    @Test
    void testOutgoingEmailDateTimeSerialization() throws JsonProcessingException {
        // Test datetime serialization for OutgoingEmail.emailSentDtm
        OutgoingEmail email = new OutgoingEmail();
        
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.JULY, 15, 14, 20, 35);
        cal.set(Calendar.MILLISECOND, 0);
        Date emailSentDtm = cal.getTime();
        
        email.setEmailSentDtm(emailSentDtm);
        email.setOutgoingEmailId("email789");
        email.setDisputeId("dispute321");

        String json = objectMapper.writeValueAsString(email);
        
        assertTrue(json.contains("\"emailSentDtm\":\"2024-07-15T14:20:35\""), 
                   "OutgoingEmail emailSentDtm should be serialized as datetime format");
        
        OutgoingEmail deserializedEmail = objectMapper.readValue(json, OutgoingEmail.class);
        assertEquals(emailSentDtm, deserializedEmail.getEmailSentDtm(),
                     "Deserialized emailSentDtm should match original exactly");
    }

    @Test
    void testDisputeUpdateRequestDateTimeSerialization() throws JsonProcessingException {
        // Test datetime serialization for DisputeUpdateRequest.statusUpdateDtm
        DisputeUpdateRequest updateRequest = new DisputeUpdateRequest();
        
        Calendar cal = Calendar.getInstance();
        cal.set(2024, Calendar.AUGUST, 22, 16, 55, 10);
        cal.set(Calendar.MILLISECOND, 0);
        Date statusUpdateDtm = cal.getTime();
        
        updateRequest.setStatusUpdateDtm(statusUpdateDtm);
        updateRequest.setDisputeUpdateRequestId("update123");
        updateRequest.setDisputeId("dispute789");

        String json = objectMapper.writeValueAsString(updateRequest);
        
        assertTrue(json.contains("\"statusUpdateDtm\":\"2024-08-22T16:55:10\""), 
                   "DisputeUpdateRequest statusUpdateDtm should be serialized as datetime format");
        
        DisputeUpdateRequest deserializedRequest = objectMapper.readValue(json, DisputeUpdateRequest.class);
        assertEquals(statusUpdateDtm, deserializedRequest.getStatusUpdateDtm(),
                     "Deserialized statusUpdateDtm should match original exactly");
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
        Dispute dispute = new Dispute();
        dispute.setDisputeId("nulltest");
        dispute.setFilingDt(null);
        dispute.setEntDtm(null);
        dispute.setUpdDtm(null);

        String json = objectMapper.writeValueAsString(dispute);
        
        // Verify null dates are serialized as null
        assertTrue(json.contains("\"filingDt\":null") || !json.contains("\"filingDt\""), 
                   "Null filingDt should be serialized as null or omitted");
        
        Dispute deserializedDispute = objectMapper.readValue(json, Dispute.class);
        assertEquals(null, deserializedDispute.getFilingDt(),
                     "Null filingDt should remain null after deserialization");
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
