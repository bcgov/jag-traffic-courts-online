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

public class DisputeTicketOptionsPicker {

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

		DisputeTicketOptionsPicker persInfo = new DisputeTicketOptionsPicker();
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

		DisputeTicketOptionsPicker popup = new DisputeTicketOptionsPicker();
		popup.popupSubmitWindow(element, driverWait, driver);
	}

	public static String getUser() {
		return user;
	}

	public static void setAppNumScreen(String user) {
		DisputeTicketOptionsPicker.user = user;

	}

	public void startDisputeTicket(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {

		Thread.sleep(1000);
		JavascriptExecutor jse = (JavascriptExecutor) driver;
		jse.executeScript("scroll(0, 450);");
		Thread.sleep(1000);
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Dispute your ticket ')]")))
				.click();

		new WebDriverWait(driver, Duration.ofSeconds(10)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Traffic Ticket ')]")))
				.click();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-0")));
		element.sendKeys("EA03148599");

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(@placeholder,'HH')]")));
		element.sendKeys("17");

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(@placeholder,'MM')]")))
				.sendKeys("16");
		
		System.out.println("Ticket found");
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Find ticket ')]")))
				.click();
		Thread.sleep(1000);
		// Start Traffic ticket dispute request
		JavascriptExecutor js1 = (JavascriptExecutor) driver;
		// Scroll down till the bottom of the page
		js1.executeScript("window.scrollBy(0,document.body.scrollHeight)");
		System.out.println("Scroll down till the bottom of the page");
		Thread.sleep(1000);
		element = driverWait
				.until(ExpectedConditions.presenceOfElementLocated(By.cssSelector(".mat-button-wrapper > strong")));
		element.click();
		System.out.println("Start dispute ticket");
	}

	public void reviewProcess(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {

		Thread.sleep(1000);
		// Scroll down till the bottom of the page
		JavascriptExecutor jse3 = (JavascriptExecutor) driver;
		// Click Next
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		jse3.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
		System.out.println("Click Next");
		String a = "Counts: Review";
		// identify elements with text()
		List<WebElement> l = driver.findElements(By.xpath("//*[contains(text(),'Counts: Review')]"));
		// verify list size
		if (l.size() > 0) {
			System.out.println("Text: " + a + " is present. ");
		} else {
			System.out.println("Text: " + a + " is not present. ");
		}
		Thread.sleep(1000);
		// Select rdo button
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-radio-2")));
		element.click();

		JavascriptExecutor js = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.presenceOfElementLocated(By.cssSelector("#mat-checkbox-4 .mat-checkbox-inner-container")));
		Thread.sleep(1000);
		js.executeScript("arguments[0].click();", element);

		JavascriptExecutor js1 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.presenceOfElementLocated(By.cssSelector("#mat-checkbox-5 .mat-checkbox-inner-container")));
		js1.executeScript("arguments[0].click();", element);
		JavascriptExecutor js2 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.presenceOfElementLocated(By.cssSelector("#mat-checkbox-7 .mat-checkbox-inner-container")));
		js2.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);
		JavascriptExecutor js3 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.visibilityOfElementLocated(By.cssSelector("#mat-checkbox-8 .mat-checkbox-inner-container")));
		js3.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);
		JavascriptExecutor js4 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.visibilityOfElementLocated(By.cssSelector("#mat-checkbox-9 .mat-checkbox-inner-container")));
		js4.executeScript("arguments[0].click();", element);
//		Thread.sleep(1000);
//		element = driverWait.until(ExpectedConditions
//				.presenceOfElementLocated(By.cssSelector("#mat-checkbox-7 .mat-checkbox-inner-container")));
//		element.click();
		Thread.sleep(1000);
		JavascriptExecutor js12 = (JavascriptExecutor) driver;
		js12.executeScript("window.scrollBy(0,document.body.scrollHeight)");
		Thread.sleep(1000);
		element = driverWait.until(
				ExpectedConditions.presenceOfElementLocated(By.cssSelector(".ng-star-inserted > .row #primaryButton")));
		element.click();

		String c = "Additional information";
		// identify elements with text()
		List<WebElement> m = driver.findElements(By.xpath("//*[contains(text(),'Additional information')]"));
		// verify list size
		if (m.size() > 0) {
			System.out.println("Text: " + c + " is present. ");
		} else {
			System.out.println("Text: " + c + " is not present. ");
		}

	}

	public void additionalInfo(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {

		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-10")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-11")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");
		// Click Next
		JavascriptExecutor js5 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		js5.executeScript("arguments[0].click();", element);

	}

	public void ticketRequestOverview(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {

		String d = "Ticket request overview";
		// identify elements with text()
		List<WebElement> n = driver.findElements(By.xpath("//*[contains(text(),'Ticket request overview')]"));
		// verify list size
		if (n.size() > 0) {
			System.out.println("Text: " + d + " is present. ");
		} else {
			System.out.println("Text: " + d + " is not present. ");
		}
		Thread.sleep(1000);
		JavascriptExecutor js15 = (JavascriptExecutor) driver;
		// Scroll down till the bottom of the page
		js15.executeScript("window.scrollBy(0,document.body.scrollHeight)");

		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-checkbox-2")));
		element.click();
		System.out.println("Tick declare box checked");
		System.out.println("Click Submit button");
		Thread.sleep(1000);
		JavascriptExecutor js = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[4]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/app-stepper-footer/div/div[2]/button")));
		js.executeScript("arguments[0].click();", element);

	}

	public void popupSubmitWindow(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {

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
		Thread.sleep(1000);
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Email verification required')]")));
		System.out.println("Email verification required is present on page");

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
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-6")));
		element.sendKeys("claudiu.vlasceanu@nttdata.com");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-8")));
		element.sendKeys("9999999999");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-9")));
		element.sendKeys("999 999 1234");
	}

}
