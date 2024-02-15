package ca.bc.gov.open.ui.imageTickets;

import java.time.Duration;
import java.util.List;

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

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.CustomWebDriverManager;

import static ca.bc.gov.open.cto.ApiClient.*;
import static ca.bc.gov.open.cto.TicketInfo.*;

public class DisputeTicketUploadPNG {

	private WebDriver driver;

	
	  @After public void tearDown() { driver.close(); driver.quit(); }

	  @AfterClass public static void afterClass() { CustomWebDriverManager.instance =
	  null; }
	 

	@Test
	public void test() throws Exception {
		driver = CustomWebDriverManager.getDriver();
		WebDriverWait driverWait = CustomWebDriverManager.getDriverWait();
		WebElement element = CustomWebDriverManager.getElement();
		CustomWebDriverManager.getElements();

		generateImageTicket();

		DisputeTicketUploadPNG upload = new DisputeTicketUploadPNG();
		upload.uploadPNG(element, driverWait, driver);

		validateImageTicket(driver);

		submitPNGticket(driverWait, driver);

	}

	public void submitPNGticket(WebDriverWait driverWait, WebDriver driver) throws Exception {
		Thread.sleep(1000);
		JavascriptExecutor js = (JavascriptExecutor) driver;
		WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(
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

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-4")));
		element.sendKeys("3220 Qadra");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-5")));
		element.sendKeys("Victoria");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-7")));
		element.sendKeys("V8X1G3");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-6")));
		element.sendKeys(TICKET_EMAIL);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-8")));
		element.sendKeys("9999999999");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-9")));
		element.sendKeys("999999999");
		Thread.sleep(1000);
		
		DisputeTicketUploadPNG review = new DisputeTicketUploadPNG();
		review.review(element, driverWait, driver);
		
		Thread.sleep(1000);
		JavascriptExecutor jse33 = (JavascriptExecutor) driver;
		// Click Next
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		jse33.executeScript("arguments[0].click();", element);

		Thread.sleep(1000);

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-12")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-13")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");

		// Click Next
		JavascriptExecutor js5 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		js5.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);

		JavascriptExecutor jse31 = (JavascriptExecutor) driver;

		// Click Next
//		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-checkbox-3")));
//		jse31.executeScript("arguments[0].scrollIntoView();", element);
//		Thread.sleep(1000);
//		element.click();

		element = driverWait.until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Please review and ensure details are correct before submission. You may update your dispute online up to 5 business days prior to a set Hearing Date.')]")));

		System.out.println("Click Submit button");
		Thread.sleep(1000);
		JavascriptExecutor js7 = (JavascriptExecutor) driver;
		element = driverWait.until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Submit request')]")));
		js7.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();

		Thread.sleep(1000);

		DisputeTicketOptionsPicker popup = new DisputeTicketOptionsPicker();
		popup.popupSubmitWindow(element, driverWait, driver);

	}

	public void uploadPNG(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {

		CommonUtils.login();

		Thread.sleep(1000);
		JavascriptExecutor jse = (JavascriptExecutor) driver;
		jse.executeScript("scroll(0, 450);");
		Thread.sleep(1000);
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Dispute your ticket ')]")))
				.click();

		Thread.sleep(1000);
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Traffic Ticket ')]")))
				.click();

		Thread.sleep(1000);
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Upload ticket image ')]")));

		Thread.sleep(1000);
		WebElement upload = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("file")));
		upload.sendKeys(System.getProperty("user.dir") + '/' + "TestUpload.png");

	}

	public void review(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {


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
		Thread.sleep(3000);
		// Select rdo button
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-radio-2")));
		element.click();
		System.out.println("RDO selected");

		// select disputant
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-radio-5")));
		element.click();
		System.out.println("Disputant selected");

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//input[@formcontrolname='disputant_signatory_name']")));
		element.sendKeys(IMAGE_TICKET_NAME);

		JavascriptExecutor js = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.presenceOfElementLocated(By.cssSelector("#mat-checkbox-5 .mat-checkbox-inner-container")));
		Thread.sleep(1000);
		js.executeScript("arguments[0].click();", element);

		JavascriptExecutor js1 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.presenceOfElementLocated(By.cssSelector("#mat-checkbox-6 .mat-checkbox-inner-container")));
		js1.executeScript("arguments[0].click();", element);
		JavascriptExecutor js2 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.presenceOfElementLocated(By.cssSelector("#mat-checkbox-8 .mat-checkbox-inner-container")));
		js2.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);
		JavascriptExecutor js3 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.visibilityOfElementLocated(By.cssSelector("#mat-checkbox-9 .mat-checkbox-inner-container")));
		js3.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);
		JavascriptExecutor js4 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.visibilityOfElementLocated(By.cssSelector("#mat-checkbox-10 .mat-checkbox-inner-container")));
		js4.executeScript("arguments[0].click();", element);

		Thread.sleep(1000);
		JavascriptExecutor jse34 = (JavascriptExecutor) driver;
		// Click Next
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		jse34.executeScript("arguments[0].scrollIntoView();", element);

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

	public void validateImageTicket(WebDriver driver) throws Exception {

		new WebDriverWait(driver, Duration.ofSeconds(120)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Ticket details')]")));
		new WebDriverWait(driver, Duration.ofSeconds(60))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '" + TICKET_NUMBER + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(60))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '" + TICKET_DATE_M_D_Y + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '" + IMAGE_TICKET_SURNAME + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '" + IMAGE_TICKET_NAME + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))

				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '" + IMAGE_TICKET_PROVINCE + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '" + IMAGE_TICKET_TIME_HOURS + ":" + IMAGE_TICKET_TIME_MINUTES + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Given name(s)')]/following-sibling::*[contains(text(), '" + IMAGE_TICKET_NAME + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Province / State of DL')]/following-sibling::*[contains(text(), '" + IMAGE_TICKET_STATE_OF_DL + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'licence number')]/following-sibling::*[contains(text(), '" + IMAGE_TICKET_DL_NUMBER + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '" + IMAGE_TICKET_COUNT_1 + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '" + IMAGE_TICKET_COUNT_2 + "')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '" + IMAGE_TICKET_COUNT_3 + "')]")));

		System.out.println("File uploaded properly");

	}
}
