package ca.bc.gov.open.ui;

import ca.bc.gov.open.ui.eTickets.*;
import ca.bc.gov.open.ui.imageTickets.MoreThan500CharsOnAdditionalInfoNegTest;
import ca.bc.gov.open.ui.imageTickets.UploadInfoNotMatchValidationChars;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.Suite;

@RunWith(Suite.class)
@Suite.SuiteClasses({
        AdditionalInfoValidation.class,
        ContactINfoLawyerValidationChars.class,
        ContactInfoValidationChars.class,
        DisputeTicketAttendCourtHearing.class,
        DisputeTicketByLawyerContactDetails.class,
        DisputeTicketByOtherContactDetails.class,
        DisputeTicketOptionsPicker.class,
        DisputeTicketOptionsPickerByMail.class,
        DisputeTicketOptionsPickerDiffCountry.class,
        DisputeTicketOptionsPickerDiffCountryFormat.class,
        ManageOrUpdateTrafficDispute.class,
        MoreThan500CharsOnAdditionalInfoNegTest.class,
        SubmitToStaffWorkbench.class,
        SubmitToStaffWorkbenchCancel.class,
        SubmitToStaffWorkbenchReject.class,
        UploadInfoNotMatchValidationChars.class,
})


public class TestSuiteETickets {

    @Test
    public void test() throws Exception {

    }


}
