package ca.bc.gov.open.ui;

import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.Suite;

@RunWith(Suite.class)
@Suite.SuiteClasses({
	DisputeTicketUploadPNG.class, DisputeTicketUploadPNGIncorectRetriveData.class,
	SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject.class, SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndSubmitARCandCancel.class,
})


public class TestSuiteImageTickets {

	@Test
	public void test() throws Exception {

	}


}
