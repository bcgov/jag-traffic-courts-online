package ca.bc.gov.open.ui;

import java.time.Duration;

import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.WebDriverManager;

public class DisputeTicketUploadPNGIncorectRetriveData {

	private WebDriver driver;

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

		DisputeTicketUploadPNG upload = new DisputeTicketUploadPNG();
		upload.uploadPNG(element, driverWait, driver);

		// Tick there are differences box

		new WebDriverWait(driver, Duration.ofSeconds(20)).until(
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
		element.sendKeys("claudiu.vlasceanu@nttdata.com");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-9")));
		element.sendKeys("9999999999");

		DisputeTicketUploadPNG review = new DisputeTicketUploadPNG();
		review.review(element, driverWait, driver);

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-11")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-12")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");

		// Click Next
		JavascriptExecutor js5 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		js5.executeScript("arguments[0].click();", element);

		System.out.println("Click Submit button");

		// This will scroll the web page till end.
		js.executeScript("window.scrollTo(0, document.body.scrollHeight)");
		Thread.sleep(1000);
		// Tick i declare
		JavascriptExecutor js11 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-checkbox-3")));
		js11.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
		Thread.sleep(1000);
		JavascriptExecutor js7 = (JavascriptExecutor) driver;
		element = driverWait.until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Submit request')]")));
		js7.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();

		DisputeTicketOptionsPicker popup = new DisputeTicketOptionsPicker();
		popup.popupSubmitWindow(element, driverWait, driver);

	}

}
