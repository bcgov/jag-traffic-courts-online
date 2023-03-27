package ca.bc.gov.open.ui;

import java.time.Duration;
import java.util.Calendar;
import java.util.Iterator;
import java.util.List;
import java.util.Set;

import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.WebDriverManager;

public class DisputeTicketOptionsPickerByMail {
	
	
	private WebDriver driver;

	private static String user;

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

		CommonUtils.login();

		DisputeTicketOptionsPicker disputeTicketExisting = new DisputeTicketOptionsPicker();
		disputeTicketExisting.startDisputeTicket(element, driverWait, driver);

		DisputeTicketOptionsPickerByMail persInfo = new DisputeTicketOptionsPickerByMail();
		persInfo.addressInput(element, driverWait, driver);

		// Add DOB
		/*
		 * new WebDriverWait(driver, Duration.ofSeconds(10))
		 * .until(ExpectedConditions.presenceOfElementLocated(By.
		 * xpath("//*[@aria-label='Open calendar']"))) .click(); new
		 * WebDriverWait(driver, Duration.ofSeconds(10))
		 * .until(ExpectedConditions.presenceOfElementLocated(By.
		 * xpath("//*[contains(text(), ' 2004 ')]"))) .click(); Thread.sleep(1000); new
		 * WebDriverWait(driver, Duration.ofSeconds(10))
		 * .until(ExpectedConditions.presenceOfElementLocated(By.
		 * xpath("//*[contains(text(), ' JAN ')]"))).click(); Thread.sleep(1000); new
		 * WebDriverWait(driver, Duration.ofSeconds(10))
		 * .until(ExpectedConditions.presenceOfElementLocated(By.
		 * xpath("//*[contains(text(), ' 31 ')]"))).click();
		 */

		DisputeTicketOptionsPicker review = new DisputeTicketOptionsPicker();
		review.reviewProcess(element, driverWait, driver);

		// Additional info

		DisputeTicketOptionsPicker info = new DisputeTicketOptionsPicker();
		info.additionalInfo(element, driverWait, driver);

		DisputeTicketOptionsPicker overview = new DisputeTicketOptionsPicker();
		overview.ticketRequestOverview(element, driverWait, driver);
		
		// Switch to pop-up window
		String parentWindowHandler = driver.getWindowHandle(); // Store your parent window
		String subWindowHandler = null;

		Set<String> handles = driver.getWindowHandles(); // get all window handles
		Iterator<String> iterator = handles.iterator();
		while (iterator.hasNext()) {
			subWindowHandler = iterator.next();
		}
		driver.switchTo().window(subWindowHandler); // switch to popup window

		JavascriptExecutor js = (JavascriptExecutor) driver;
		element = driverWait
				.until(ExpectedConditions.presenceOfElementLocated(By.cssSelector(".me-1 > .mat-button-wrapper")));
		js.executeScript("arguments[0].click();", element);
		System.out.println("Submit in pop-up clicked");
		driver.switchTo().window(parentWindowHandler); // switch back to parent window

		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Dispute your ticket ')]")));
		System.out.println("Ticket submitted without email");
	}

	public static String getUser() {
		return user;
	}

	public static void user(String user) {
		DisputeTicketOptionsPickerByMail.user = user;

	}

	public void addressInput(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {

		// String user;

		user = Calendar.getInstance().getTimeInMillis() + "Test";

		System.out.println("New created user is: " + user);

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-2")));
		element.sendKeys(user);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-3")));
		element.sendKeys("User");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-4")));
		element.sendKeys("3220 Qadra");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-5")));
		element.sendKeys("Victoria");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-7")));
		element.sendKeys("V8X1G3");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-8")));
		element.sendKeys("9999999999");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-9")));
		element.sendKeys("999 999 1234");
		Thread.sleep(1000);
		JavascriptExecutor js = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.presenceOfElementLocated(By.cssSelector(".mat-checkbox-inner-container")));
		Thread.sleep(1000);
		js.executeScript("arguments[0].click();", element);
		
	}


}
