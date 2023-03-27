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

public class MoreThan500CharsOnAdditionalInfoNegTest {
	
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

		new WebDriverWait(driver, Duration.ofSeconds(20)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Ticket details')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'AN00893391')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'May 24, 2023')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Kent')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'BC')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '12:44')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'clark')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '3139264')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Pass On Right')]")));
		System.out.println("File uploaded properly");

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

		DisputeTicketOptionsPicker review = new DisputeTicketOptionsPicker();
		review.reviewProcess(element, driverWait, driver);
		
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-10")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus");
		// Click on page
		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"//html")));
		element.click();
		new WebDriverWait(driver, Duration.ofSeconds(20)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Maximum length is 500')]")));

	}
}
