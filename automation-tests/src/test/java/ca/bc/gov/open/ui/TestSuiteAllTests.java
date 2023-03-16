package ca.bc.gov.open.ui;

import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.Suite;

@RunWith(Suite.class)
@Suite.SuiteClasses({

	DisputeTicketAttendCourtHearing.class, DisputeTicketByLawyerContactDetails.class, DisputeTicketByOtherContactDetails.class, 
	DisputeTicketOptionsPicker.class, DisputeTicketOptionsPickerDiffCountry.class, DisputeTicketOptionsPickerDiffCountryFormat.class,
	DisputeTicketUploadPNG.class, DisputeTicketUploadPNGIncorectRetriveData.class, MoreThan500CharsOnAdditionalInfoNegTest.class,
	ContactInfoValidationChars.class,

})


public class TestSuiteAllTests {
	
	@Test
	public void test() throws Exception {

	}


}
