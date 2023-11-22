package ca.bc.gov.open.ui;

import java.time.Duration;

import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.WebDriverManager;

public class SubmitToStaffWorkbench {

	private WebDriver driver;
	private String user;
	private static String bceidUSERNAME = System.getenv("USERNAME_BCEID");
	private static String bceidPASSWORD = System.getenv("PASSWORD_BCEID");

	@After
	public void tearDown() {
		driver.close();
		driver.quit();
	}

	@AfterClass
	public static void afterClass() {
		WebDriverManager.instance = null;
	}

	@Test
	public void test() throws Exception {
		driver = WebDriverManager.getDriver();
		WebDriverWait driverWait = WebDriverManager.getDriverWait();
		WebElement element = WebDriverManager.getElement();
		WebDriverManager.getElements();

		DisputeTicketOptionsPickerByMail dispute = new DisputeTicketOptionsPickerByMail();
		dispute.test();

		CommonUtils.loginStaffWorkbench();

		SubmitToStaffWorkbench loginStaff = new SubmitToStaffWorkbench();
		loginStaff.loginToStaffWorkbench(element, driverWait, driver);

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("ticketNumber")));
		element.sendKeys("EZ02005201");

		// Click first entry and verify the new created user is present

		setUser(DisputeTicketOptionsPickerByMail.getUser());

		System.out.println("Name to be checked in Staff Workbench: " + DisputeTicketOptionsPickerByMail.getUser());

		new WebDriverWait(driver, Duration.ofSeconds(50)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), '" + DisputeTicketOptionsPickerByMail.getUser() + "')]")));

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-staff-workbench-dashboard/app-page/div/div[3]/app-busy-overlay/div/div/div/app-ticket-inbox/div/table/tbody/tr[1]/td[3]/span/a")));
		element.click();

		new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), 'FAIL TO COMPLETE POST-TRIP INSPECTION REPORT AT FINAL STOPS')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '$138.00')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"//*[contains(text(), 'I do not want to attend a court hearing and want to supply written reasons.')]")));

	}

	public String getUser() {
		return user;
	}

	public void setUser(String user) {
		this.user = user;
	}

	public void loginToStaffWorkbench(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'IDIR')]"))).click();
		// Clcik Cancel on popup
		/*
		 * new WebDriverWait(driver, Duration.ofSeconds(10))
		 * .until(ExpectedConditions.presenceOfElementLocated(By.
		 * xpath("//*[contains(text(), 'Cancel')]"))).click();
		 */

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("user")));
		element.sendKeys(bceidUSERNAME);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("password")));
		element.sendKeys(bceidPASSWORD);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.name("btnSubmit")));
		element.click();

	}

}
