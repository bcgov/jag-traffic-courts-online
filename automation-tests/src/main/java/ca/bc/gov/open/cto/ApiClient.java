package ca.bc.gov.open.cto;

import com.fasterxml.jackson.databind.ObjectMapper;
import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.entity.StringEntity;
import org.apache.http.impl.client.HttpClients;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;

import static ca.bc.gov.open.cto.TicketInfo.*;

public class ApiClient {
    public static final String IMAGE_ENDPOINT_URL = "https://ticket-gen-0198bb-tools.apps.silver.devops.gov.bc.ca/generate/v2?writingStyle=1";
    public static final String E_TICKET_ENDPOINT_URL = "https://citizen-api-0198bb-dev.apps.silver.devops.gov.bc.ca/api/invoice";

    public static void generateImageTicket() {
        try {
            assignRandomVariables();

            // Create an HTTP client
            HttpClient httpClient = HttpClients.createDefault();

            HttpPost httpPost = new HttpPost(IMAGE_ENDPOINT_URL);
            String jsonRequest = buildImageJsonRequest();
            StringEntity requestEntity = new StringEntity(jsonRequest);
            httpPost.setEntity(requestEntity);
            httpPost.setHeader("Content-Type", "application/json");

            // Execute the request and get the response
            HttpResponse response = httpClient.execute(httpPost);
            HttpEntity responseEntity = response.getEntity();

            System.out.println(response);
            // Check if the response entity is not null
            if (responseEntity != null) {
                // Save the response content as a PNG file
                saveResponseAsImage(responseEntity.getContent(), "TestUpload.png");
            }

        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public static void saveResponseAsImage(InputStream inputStream, String filename) throws IOException {
        try (FileOutputStream outputStream = new FileOutputStream(filename)) {
            byte[] buffer = new byte[1024];
            int bytesRead;
            while ((bytesRead = inputStream.read(buffer)) != -1) {
                outputStream.write(buffer, 0, bytesRead);
            }
        }
    }

    public static String buildImageJsonRequest() {
        try {
            ObjectMapper objectMapper = new ObjectMapper();

            String jsonTemplate = "{\n" +
                    "  \"violationTicketNumber\": \"" + TICKET_NUMBER + "\",\n" +
                    "  \"surname\": \"" + IMAGE_TICKET_SURNAME + "\",\n" +
                    "  \"givenName\": \"" + IMAGE_TICKET_NAME+ "\",\n" +
                    "  \"driversLicenceProvince\": \"" + IMAGE_TICKET_PROVINCE + "\",\n" +
                    "  \"driversLicenceNumber\": \"" + IMAGE_TICKET_DL_NUMBER + "\",\n" +
                    "  \"driversLicenceCreated\": \"2020\",\n" +
                    "  \"driversLicenceExpiry\": \"2025\",\n" +
                    "  \"birthdate\": \"2006-01-15\",\n" +
                    "  \"address\": \"123 Small Lane\",\n" +
                    "  \"city\": \"Smallville\",\n" +
                    "  \"province\": \"BC\",\n" +
                    "  \"postalCode\": \"V9A1L8\",\n" +
                    "  \"namedIsDriver\": \"X\",\n" +
                    "  \"namedIsCyclist\": \"\",\n" +
                    "  \"namedIsOwner\": \"\",\n" +
                    "  \"namedIsPedestrain\": \"\",\n" +
                    "  \"namedIsPassenger\": \"\",\n" +
                    "  \"namedIsOther\": \"\",\n" +
                    "  \"namedIsOtherDescription\": \"Created Automatically\",\n" +
                    "  \"violationDate\": \"" + TICKET_DATE_Y_M_D + "\",\n" +
                    "  \"violationTime\": \"" + IMAGE_TICKET_TIME_HOURS + ":" + IMAGE_TICKET_TIME_MINUTES + "\",\n" +
                    "  \"violationOnHighway\": \"Smallville Bypass\",\n" +
                    "  \"violationNearPlace\": \"Kent Farm\",\n" +
                    "  \"offenseIsMVA\": \"X\",\n" +
                    "  \"offenseIsCTA\": \"\",\n" +
                    "  \"offenseIsWLA\": \"\",\n" +
                    "  \"offenseIsOther\": \"\",\n" +
                    "  \"offenseIsOtherDescription\": \"\",\n" +
                    "  \"count1Description\": \"" + IMAGE_TICKET_COUNT_1 + "\",\n" +
                    "  \"count1IsACT\": \"X\",\n" +
                    "  \"count1IsREGS\": \"\",\n" +
                    "  \"count1Section\": \"67(b)\",\n" +
                    "  \"count1TicketAmount\": \"350\",\n" +
                    "  \"count2Description\": \"" + IMAGE_TICKET_COUNT_2 + "\",\n" +
                    "  \"count2ActReg\": \"MVA\",\n" +
                    "  \"count2IsACT\": \"X\",\n" +
                    "  \"count2IsREGS\": \"\",\n" +
                    "  \"count2Section\": \"45(a)\",\n" +
                    "  \"count2TicketAmount\": \"145\",\n" +
                    "  \"count3Description\": \"" + IMAGE_TICKET_COUNT_3 + "\",\n" +
                    "  \"count3ActReg\": \"MVA\",\n" +
                    "  \"count3IsACT\": \"X\",\n" +
                    "  \"count3IsREGS\": \"\",\n" +
                    "  \"count3Section\": \"124(c)(i)\",\n" +
                    "  \"count3TicketAmount\": \"75\",\n" +
                    "  \"vehicleLicensePlateProvince\": \"BC\",\n" +
                    "  \"vehicleLicensePlateNumber\": \"123ABC\",\n" +
                    "  \"vehicleNscPuj\": \"AABB\",\n" +
                    "  \"vehicleNscNumber\": \"12345\",\n" +
                    "  \"vehicleRegisteredOwnerName\": \"Jonathan Kent\",\n" +
                    "  \"vehicleMake\": \"John Deer\",\n" +
                    "  \"vehicleType\": \"Tracker\",\n" +
                    "  \"vehicleColour\": \"Green\",\n" +
                    "  \"vehicleYear\": \"2011\",\n" +
                    "  \"noticeOfDisputeAddress\": \"123 Main St.\",\n" +
                    "  \"hearingLocation\": \"Metropolis Court\",\n" +
                    "  \"dateOfService\": \"" + TICKET_DATE_Y_M_D + "\",\n" +
                    "  \"enforcementOfficerNumber\": \"12345\",\n" +
                    "  \"detachmentLocation\": \"Metropolitan Police\",\n" +
                    "  \"isYoungPerson\": \"Y\",\n" +
                    "  \"isChangeOfAddress\": \"N\",\n" +
                    "  \"offenseIsMVAR\": \"\",\n" +
                    "  \"offenseIsLCLA\": \"\",\n" +
                    "  \"offenseIsFVPA\": \"\",\n" +
                    "  \"offenseIsCCLA\": \"\",\n" +
                    "  \"offenseIsTCSR\": \"\",\n" +
                    "  \"isAccident\": \"N\",\n" +
                    "  \"witnessingEnforcementOfficerName\": \"Sergeant Smith\",\n" +
                    "  \"witnessingEnforcementOfficerNumber\": \"12345\"\n" +
                    "}";

            System.out.println("Ticket Image Request:\n" + jsonTemplate);
            return objectMapper.writeValueAsString(objectMapper.readTree(jsonTemplate));

        } catch (IOException e) {
            e.printStackTrace();
            return null;
        }
    }

    public static String generateMockETicket() {
        try {

            assignRandomVariables();

            // Create an HTTP client
            HttpClient httpClient = HttpClients.createDefault();

            HttpPost httpPost = new HttpPost(E_TICKET_ENDPOINT_URL);
            String jsonRequest = buildETicketJsonRequest(E_TICKET_INVOICE_1, E_TICKET_AMOUNT_1, E_TICKET_COUNT_1);
            StringEntity requestEntity = new StringEntity(jsonRequest);
            httpPost.setEntity(requestEntity);
            httpPost.setHeader("Content-Type", "application/json");
            HttpResponse response = httpClient.execute(httpPost);
            System.out.println("JSON Response:\n" + response);

            HttpPost httpPost2 = new HttpPost(E_TICKET_ENDPOINT_URL);
            String jsonRequest2 = buildETicketJsonRequest(E_TICKET_INVOICE_2, E_TICKET_AMOUNT_2, E_TICKET_COUNT_2);
            StringEntity requestEntity2 = new StringEntity(jsonRequest2);
            httpPost2.setEntity(requestEntity2);
            httpPost2.setHeader("Content-Type", "application/json");
            HttpResponse response2 = httpClient.execute(httpPost2);
            System.out.println("JSON Response:\n" + response2);

            HttpPost httpPost3 = new HttpPost(E_TICKET_ENDPOINT_URL);
            String jsonRequest3 = buildETicketJsonRequest(E_TICKET_INVOICE_3, E_TICKET_AMOUNT_3, E_TICKET_COUNT_3);
            StringEntity requestEntity3 = new StringEntity(jsonRequest3);
            httpPost3.setEntity(requestEntity3);
            httpPost3.setHeader("Content-Type", "application/json");
            HttpResponse response3 = httpClient.execute(httpPost3);
            System.out.println("JSON Response:\n" + response3);

        } catch (IOException e) {
            e.printStackTrace();
        }
        return null;
    }


    public static String buildETicketJsonRequest(String invoiceConstant, String amountConstant, String countConstant) {
        try {
            ObjectMapper objectMapper = new ObjectMapper();

            String jsonTemplate = "{\n" +
                    "    \"invoice_number\": \"" + invoiceConstant + "\",\n" +
                    "    \"pbc_ref_number\": \"\",\n" +
                    "    \"party_number\": \"\",\n" +
                    "    \"party_name\": \"\",\n" +
                    "    \"account_number\": \"\",\n" +
                    "    \"site_number\": \"\",\n" +
                    "    \"cust_trx_type\": \"Traffic Violation Ticket\",\n" +
                    "    \"term_due_date\": \"" + getFormattedYesterdayDateYMD() + "T" + E_TICKET_TIME_HOURS + ":" + E_TICKET_TIME_MINUTES + "\",\n" +
                    "    \"total\": " + amountConstant + ",\n" +
                    "    \"amount_due\": " + amountConstant + ",\n" +
                    "    \"attribute1\": \"" + countConstant + "\",\n" +
                    "    \"attribute2\": \"\",\n" +
                    "    \"attribute3\": \"" + getFormattedYesterdayDateYMD() + "\",\n" +
                    "    \"attribute4\": \"\",\n" +
                    "    \"evt_form_number\": \"\"\n" +
                    "}";

            System.out.println("E-Ticket Request:\n" + jsonTemplate);
            return objectMapper.writeValueAsString(objectMapper.readTree(jsonTemplate));

        } catch (IOException e) {
            e.printStackTrace();
            return null;
        }
    }

    // Print the generated JSON request for Postman
    public static void main(String[] args) {
        String jsonRequest = generateMockETicket();
        System.out.println("JSON Request:\n" + jsonRequest);
    }
}
