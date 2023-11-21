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
	 private static String  bceidUSERNAME = System.getenv("USERNAME_BCEID");
	 private static String bceidPASSWORD = System.getenv("PASSWORD_BCEID");


		
		  @After public void tearDown() { driver.close(); driver.quit(); }
		  
		  @AfterClass public static void afterClass() { WebDriverManager.instance =
		  null; }
		 

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

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.name("searchInput")));
		element.sendKeys("EA03148599");

		// Click first entry and verify the new created user is present

		setUser(DisputeTicketOptionsPickerByMail.getUser());

		System.out.println("Name to be checked in Staff Workbench: " + DisputeTicketOptionsPickerByMail.getUser());

		new WebDriverWait(driver, Duration.ofSeconds(50)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), '" + DisputeTicketOptionsPickerByMail.getUser() + "')]")));

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By
				.xpath("//*[@id=\"mat-tab-content-0-0\"]/div/app-ticket-inbox/div[2]/table/tbody/tr[1]/td[3]/span/a")));
		element.click();

		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), 'Fail to signal stop or decrease in speed')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '$121.00')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Change lanes without signal')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '$109.00')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Drive without due care')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '$368.00')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"//*[contains(text(), 'I want to attend a court hearing to request a fine reduction and/or time to pay.')]")));

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

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("user")));
		element.sendKeys(bceidUSERNAME);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("password")));
		element.sendKeys(bceidPASSWORD);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.name("btnSubmit")));
		element.click();

	}

}
