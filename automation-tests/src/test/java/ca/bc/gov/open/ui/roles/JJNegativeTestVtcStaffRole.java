package ca.bc.gov.open.ui.roles;

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.CustomWebDriverManager;
import ca.bc.gov.open.ui.DisputeTicketOptionsPickerByMail;
import ca.bc.gov.open.ui.SubmitToStaffWorkbench;
import ca.bc.gov.open.ui.SubmitToStaffWorkbenchReject;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

public class JJNegativeTestVtcStaffRole {

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
		CustomWebDriverManager.getElements();

		CommonUtils.loginJJWorkbench();

		SubmitToStaffWorkbench staffWorkbench = new SubmitToStaffWorkbench();
		staffWorkbench.loginToStaffWorkbenchWithUser(driverWait, driver, vtcStaffUser);

		driverWait.until(ExpectedConditions.not(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'WR Assignments')]"))));
		driverWait.until(ExpectedConditions.not(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Bulk Assignments')]"))));
		driverWait.until(ExpectedConditions.not(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'TEAM')]"))));
		driverWait.until(ExpectedConditions.not(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'WR Inbox')]"))));
		driverWait.until(ExpectedConditions.not(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Hearing Inbox')]"))));
		driverWait.until(ExpectedConditions.not(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'DCF')]"))));
	}

	public String getUser() {
		return user;
	}

	public void setUser(String user) {
		this.user = user;
	}
}
