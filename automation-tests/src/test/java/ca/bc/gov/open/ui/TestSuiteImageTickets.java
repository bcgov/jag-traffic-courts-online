package ca.bc.gov.open.ui;

import ca.bc.gov.open.ui.imageTickets.*;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.Suite;

@RunWith(Suite.class)
@Suite.SuiteClasses({
        DisputeTicketUploadPNG.class,
        DisputeTicketUploadPNGIncorectRetriveData.class,
        SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject.class,
        MoreThan500CharsOnAdditionalInfoNegTest.class,
        SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndSubmitARCandCancel.class,
        UploadInfoNotMatchValidationChars.class,
})


public class TestSuiteImageTickets {

    @Test
    public void test() throws Exception {
    }

}
