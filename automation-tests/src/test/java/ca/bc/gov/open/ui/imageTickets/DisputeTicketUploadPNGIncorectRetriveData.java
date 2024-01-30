package ca.bc.gov.open.ui.imageTickets;

import java.time.Duration;

import ca.bc.gov.open.ui.eTickets.DisputeTicketOptionsPicker;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.CustomWebDriverManager;

import static ca.bc.gov.open.cto.ApiClient.generateImageTicket;
import static ca.bc.gov.open.cto.TicketInfo.TICKET_EMAIL;
public class DisputeTicketUploadPNGIncorectRetriveData {

	private WebDriver driver;

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

		// Tick there are differences box

		new WebDriverWait(driver, Duration.ofSeconds(100)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Ticket details')]")));
		JavascriptExecutor js6 = (JavascriptExecutor) driver;
		element = driverWait
				.until(ExpectedConditions.presenceOfElementLocated(By.className("mat-checkbox-inner-container")));
		js6.executeScript("arguments[0].click();", element);

		System.out.println("box ticked");

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-2")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium.");

		Thread.sleep(1000);
		JavascriptExecutor js = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), ' Save information and create online ticket ')]")));
		js.executeScript("arguments[0].click();", element);

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

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-5")));
		element.sendKeys("3220 Qadra");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-6")));
		element.sendKeys("Victoria");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-8")));
		element.sendKeys("V8X1G3");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-7")));
		element.sendKeys(TICKET_EMAIL);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-9")));
		element.sendKeys("9999999999");

		DisputeTicketUploadPNG review = new DisputeTicketUploadPNG();
		review.review(element, driverWait, driver);

		Thread.sleep(1000);
		JavascriptExecutor jse33 = (JavascriptExecutor) driver;
		// Click Next
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		jse33.executeScript("arguments[0].click();", element);

		Thread.sleep(1000);

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-13")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-14")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");

		// Click Next
		JavascriptExecutor js5 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		js5.executeScript("arguments[0].click();", element);

		System.out.println("Click Submit button");

		// This will scroll the web page till end.
		js.executeScript("window.scrollTo(0, document.body.scrollHeight)");

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Please review and ensure details are correct before submission. You may update your dispute online up to 5 business days prior to a set Hearing Date.')]")));

		System.out.println("Click Submit button");

		Thread.sleep(1000);
		JavascriptExecutor js7 = (JavascriptExecutor) driver;
		element = driverWait.until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Submit request')]")));

		js7.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(2000);
		element.click();

		DisputeTicketOptionsPicker popup = new DisputeTicketOptionsPicker();
		popup.popupSubmitWindow(element, driverWait, driver);
	}

}
