package ca.bc.gov.open.ui.eTickets;

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.CustomWebDriverManager;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import java.time.Duration;

import static ca.bc.gov.open.cto.TicketInfo.*;

public class SubmitToStaffWorkbench {

	private WebDriver driver;
	private String user;
	private static String appUSERNAME = System.getenv("USERNAME_APP");
	private static String appPASSWORD = System.getenv("PASSWORD_APP");

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

		DisputeTicketOptionsPickerByMail dispute = new DisputeTicketOptionsPickerByMail();
		dispute.test();

		CommonUtils.loginStaffWorkbench();

		SubmitToStaffWorkbench loginStaff = new SubmitToStaffWorkbench();
		loginStaff.loginToStaffWorkbenchWithUser(driverWait, driver, appUSERNAME);

		validateTicketOnStaffWOrkbench(driverWait, driver);
	}

	public void validateTicketOnStaffWOrkbench(WebDriverWait driverWait, WebDriver driver) throws Exception {
		WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("ticketNumber")));
		element.sendKeys(E_TICKET_NUMBER);

		// Click first entry and verify the new created user is present

		setUser(DisputeTicketOptionsPickerByMail.getUser());

		System.out.println("Name to be checked in Staff Workbench: " + DisputeTicketOptionsPickerByMail.getUser());

		Thread.sleep(1000);

		new WebDriverWait(driver, Duration.ofSeconds(50)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), '" + DisputeTicketOptionsPickerByMail.getUser() + "')]")));

		new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//a[contains(text(), '" + E_TICKET_NUMBER + "')]")));

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"//a[contains(text(), '" + E_TICKET_NUMBER + "')]")));
		element.click();

		new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), '" + E_TICKET_COUNT_1 + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '$" + E_TICKET_AMOUNT_1 + ".00')]")));
		new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), '" + E_TICKET_COUNT_2 + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '$" + E_TICKET_AMOUNT_2 + ".00')]")));
		new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), '" + E_TICKET_COUNT_3 + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '$" + E_TICKET_AMOUNT_3 + ".00')]")));
	}

	public String getUser() {
		return user;
	}

	public void setUser(String user) {
		this.user = user;
	}

	public void loginToStaffWorkbench(WebDriverWait driverWait, WebDriver driver) throws Exception {

		Thread.sleep(1000);

		WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("username")));
		element.sendKeys(appUSERNAME);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("password")));
		element.sendKeys(appPASSWORD);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.name("login")));
		element.click();

		new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), 'Sign out')]")));
	}

	public void loginToStaffWorkbenchWithUser(WebDriverWait driverWait, WebDriver driver, String appUSERNAME) throws Exception {

		Thread.sleep(1000);

		WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("username")));
		element.sendKeys(appUSERNAME);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("password")));
		element.sendKeys(appPASSWORD);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.name("login")));
		element.click();

		new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), 'Sign out')]")));
	}

}
