package ca.bc.gov.open.ui;

import ca.bc.gov.open.ui.roles.*;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.junit.runners.Suite;

@RunWith(Suite.class)
@Suite.SuiteClasses({
		CancelTicketAdminVtcStaffRole.class,
		CancelTicketVtcStaffRole.class,
		JJNegativeTestAdminVtcStaffRole.class,
		JJNegativeTestVtcStaffRole.class,
		RejectIETicketVtcStaffRole.class,
		RejectImageTicketVtcStaffRole.class,
		TCONegativeTestAdminJJRole.class,
		TCONegativeTestJJRole.class,
})

public class TestSuiteRolesTests {

	@Test
	public void test() throws Exception {

	}


}
