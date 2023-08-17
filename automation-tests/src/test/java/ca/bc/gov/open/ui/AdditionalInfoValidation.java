package ca.bc.gov.open.ui;

import java.time.Duration;
import java.util.List;

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

public class AdditionalInfoValidation {
	
	
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

		CommonUtils.login();

		DisputeTicketOptionsPicker disputeTicketExisting = new DisputeTicketOptionsPicker();
		disputeTicketExisting.startDisputeTicket(element, driverWait, driver);

		DisputeTicketOptionsPicker persInfo = new DisputeTicketOptionsPicker();
		persInfo.addressInput(element, driverWait, driver);

		Thread.sleep(1000);
		// Click Next
		JavascriptExecutor js = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		js.executeScript("arguments[0].click();", element);
		System.out.println("Click Next");
		System.out.println("Count review 1");
		Thread.sleep(1000);
		// Select rdo button attend court hearing
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-radio-3")));
		element.click();
		Thread.sleep(1000);
		JavascriptExecutor jse1 = (JavascriptExecutor) driver;
		element = driverWait
				.until(ExpectedConditions.elementToBeClickable(By.cssSelector("#mat-radio-5 .mat-radio-outer-circle")));
		jse1.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);
		JavascriptExecutor jse2 = (JavascriptExecutor) driver;
		element = driverWait
				.until(ExpectedConditions.elementToBeClickable(By.cssSelector("#mat-radio-6 .mat-radio-outer-circle")));
		jse2.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);
		JavascriptExecutor jse3 = (JavascriptExecutor) driver;
		element = driverWait
				.until(ExpectedConditions.elementToBeClickable(By.cssSelector("#mat-radio-9 .mat-radio-outer-circle")));
		jse3.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);
		JavascriptExecutor jse4 = (JavascriptExecutor) driver;
		element = driverWait
				.until(ExpectedConditions.elementToBeClickable(By.cssSelector("#mat-radio-12 .mat-radio-outer-circle")));
		jse4.executeScript("arguments[0].click();", element);

		JavascriptExecutor jse22 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		jse22.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);
		String a = "Additional information";
		// identify elements with text()
		List<WebElement> l = driver.findElements(By.xpath("//*[contains(text(),'Additional information')]"));
		// verify list size
		if (l.size() > 0) {
			System.out.println("Text: " + a + " is present. ");
		} else {
			System.out.println("Text: " + a + " is not present. ");
		}
		JavascriptExecutor jse21 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.visibilityOfElementLocated(By.id("mat-checkbox-12-input")));
		jse21.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);

		// Additional Info with lawyer, interpreter and witness
		element = driverWait.until(ExpectedConditions
				.elementToBeClickable(By.cssSelector("#mat-checkbox-13 .mat-checkbox-inner-container")));
		element.click();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-10")));
		element.sendKeys("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec qua");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-11")));
		element.sendKeys("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean m");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-12")));
		element.sendKeys("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-13")));
		element.sendKeys("9999999999");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-14")));
		element.sendKeys("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa@a.com");
		JavascriptExecutor jse6 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.elementToBeClickable(By.id("mat-select-value-9")));
		jse6.executeScript("arguments[0].click();", element);
		element = driverWait
				.until(ExpectedConditions.elementToBeClickable(By.cssSelector("#mat-option-267 > .mat-option-text")));
		element.click();
		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions
				.elementToBeClickable(By.cssSelector("#mat-checkbox-14 .mat-checkbox-inner-container")));
		element.click();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-15")));
		element.clear();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-15")));
		element.sendKeys("9999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999");
		
		Thread.sleep(1000);
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Maximum length is 200')]")));

		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Maximum length is 100')]")));

		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Must be a valid email address')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'must be a number less than 100')]")));
		
	}

}
