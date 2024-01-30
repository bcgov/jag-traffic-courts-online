package ca.bc.gov.open.ui.roles;

import ca.bc.gov.open.cto.CustomWebDriverManager;
import ca.bc.gov.open.ui.imageTickets.DisputeTicketUploadPNG;
import ca.bc.gov.open.ui.eTickets.SubmitToStaffWorkbench;
import ca.bc.gov.open.ui.eTickets.SubmitToStaffWorkbenchReject;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.WebDriverWait;

import static ca.bc.gov.open.cto.ApiClient.generateImageTicket;

public class RejectImageTicketVtcStaffRole {

	private WebDriver driver;
	private String user;
	private static String appUSERNAME = System.getenv("USERNAME_APP");
	private static String appPASSWORD = System.getenv("PASSWORD_APP");
	private static String allRolesUser = System.getenv("USERNAME_APP_1");
	private static String vtcStaffUser = System.getenv("USERNAME_APP_2");
	private static String adminVtcStaffUser = System.getenv("USERNAME_APP_3");
	private static String judicalJusticeUser = System.getenv("USERNAME_APP_4");
	private static String AdminJudicalJusticeUser = System.getenv("USERNAME_APP_5");
	private static String supportStaffUser = System.getenv("USERNAME_APP_6");


	@After
	public void tearDown() {
		driver.close();
		driver.quit();
	}

	@AfterClass
	public static void afterClass() {
		CustomWebDriverManager.instance = null;
	}

	@Test
	public void test() throws Exception {
		driver = CustomWebDriverManager.getDriver();
		WebDriverWait driverWait = CustomWebDriverManager.getDriverWait();
		WebElement element = CustomWebDriverManager.getElement();
		CustomWebDriverManager.getElements();

		generateImageTicket();

		DisputeTicketUploadPNG upload = new DisputeTicketUploadPNG();
		upload.uploadPNG(element, driverWait, driver);

		upload.validateImageTicket(driver);

		upload.submitPNGticket(driverWait, driver);

		SubmitToStaffWorkbench staffWorkbench = new SubmitToStaffWorkbench();

		staffWorkbench.loginToStaffWorkbenchWithUser(driverWait, driver, vtcStaffUser);
		staffWorkbench.validateTicketOnStaffWOrkbench(driverWait, driver);

		SubmitToStaffWorkbenchReject reject = new SubmitToStaffWorkbenchReject();
		reject.rejectRequest(driverWait, driver);
	}

	public String getUser() {
		return user;
	}

	public void setUser(String user) {
		this.user = user;
	}
}
