package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.assertEquals;

import org.junit.jupiter.api.Test;

import ca.bc.gov.open.jag.tco.oracledataapi.BaseTestSuite;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;

public class DisputeServiceTest extends BaseTestSuite {

    @Test
    public void testReplaceProvinceValuesWithNA() {
        DisputeService disputeService = new DisputeService();
        Dispute dispute = new Dispute();

        dispute.setAddressProvince(null);
        dispute.setDriversLicenceProvince(null);

        disputeService.replaceProvinceValuesWithNA(dispute);

        assertEquals("N/A", dispute.getAddressProvince());
        assertEquals("N/A", dispute.getDriversLicenceProvince());
    }

    @Test
    public void testReplaceNAValuesWithEmpty() {
        DisputeService disputeService = new DisputeService();
        Dispute dispute = new Dispute();

        dispute.setAddressProvince("N/A");
        dispute.setDriversLicenceProvince("N/A");

        disputeService.replaceNAValuesWithEmpty(dispute);

        assertEquals("", dispute.getAddressProvince());
        assertEquals("", dispute.getDriversLicenceProvince());
    }
}