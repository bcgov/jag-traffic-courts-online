package ca.bc.gov.open.jag.tco.oracledataapi.service;

import static org.junit.jupiter.api.Assertions.assertEquals;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.TestInstance;
import org.mockito.InjectMocks;
import org.mockito.MockitoAnnotations;
import ca.bc.gov.open.jag.tco.oracledataapi.model.Dispute;

@TestInstance(TestInstance.Lifecycle.PER_CLASS)
public class DisputeServiceTest  {

    @InjectMocks
	private DisputeService disputeService;

    @BeforeEach
    public void setUp() {
        MockitoAnnotations.openMocks(this);
    }


    @Test
    public void testReplaceProvinceValuesWithNA() {
        Dispute dispute = new Dispute();

        dispute.setAddressProvince(null);
        dispute.setDriversLicenceIssuedCountryId(73);
        dispute.setDriversLicenceProvince(null);

        disputeService.replaceProvinceValuesWithNA(dispute);

        assertEquals("N/A", dispute.getAddressProvince());
        assertEquals("N/A", dispute.getDriversLicenceProvince());
    }

    @Test
    public void testReplaceNAValuesWithEmpty() {
        Dispute dispute = new Dispute();

        dispute.setAddressProvince("N/A");
        dispute.setDriversLicenceProvince("N/A");

        disputeService.replaceNAValuesWithEmpty(dispute);

        assertEquals("", dispute.getAddressProvince());
        assertEquals("", dispute.getDriversLicenceProvince());
    }
}