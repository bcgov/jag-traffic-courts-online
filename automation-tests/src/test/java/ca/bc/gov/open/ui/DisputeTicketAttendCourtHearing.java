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
import ca.bc.gov.open.cto.CustomWebDriverManager;

public class DisputeTicketAttendCourtHearing {

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
		element = driverWait.until(ExpectedConditions.visibilityOfElementLocated(
				By.xpath("//*[contains(text(), ' I intend to be represented by a lawyer in court. ')]")));
		jse21.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-10")));
		element.sendKeys("Test LTD");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-11")));
		element.sendKeys("Lawyer Lawyer");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-12")));
		element.sendKeys("3220 Douglas");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-13")));
		element.sendKeys("9999999999");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-14")));
		element.clear();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-14")));
		element.sendKeys("test@test.com");

		// Additional Info with lawyer, interpreter and witness
		JavascriptExecutor jse6 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.elementToBeClickable(
				By.xpath("//*[contains(text(), ' I require a language interpreter at the hearing. ')]")));
		jse6.executeScript("arguments[0].click();", element);
		JavascriptExecutor jse61 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.elementToBeClickable(By.id("mat-select-value-9")));
		jse61.executeScript("arguments[0].click();", element);
		element = driverWait
				.until(ExpectedConditions.elementToBeClickable(By.cssSelector("#mat-option-267 > .mat-option-text")));
		element.click();
		Thread.sleep(1000);
		// Additional Info with lawyer, interpreter and witness
		JavascriptExecutor jse62 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.elementToBeClickable(By.xpath("//*[contains(text(), ' I intend to call a witness(es). ')]")));
		jse62.executeScript("arguments[0].click();", element);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-15")));
		element.clear();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-15")));
		element.sendKeys("3");
		Thread.sleep(1000);

		// Check text
		new WebDriverWait(driver, Duration.ofSeconds(60)).until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"//*[contains(text(), 'If you are calling a witness please keep the following information in mind:')]")));

		new WebDriverWait(driver, Duration.ofSeconds(60)).until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"//*[contains(text(), 'If you want a lawyer to represent you at your hearing, you should hire a lawyer prior to submitting your Notice')]")));

		new WebDriverWait(driver, Duration.ofSeconds(60)).until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"//*[contains(text(), 'If you bring a witness, you are responsible for ensuring they are aware of the hearing date and know')]")));

		JavascriptExecutor jse7 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		jse7.executeScript("arguments[0].click();", element);
		String d = "Ticket request overview";
		// identify elements with text()
		List<WebElement> o = driver.findElements(By.xpath("//*[contains(text(),'Ticket request overview')]"));
		// verify list size
		if (o.size() > 0) {
			System.out.println("Text: " + d + " is present. ");
		} else {
			System.out.println("Text: " + d + " is not present. ");
		}
		Thread.sleep(1000);
		JavascriptExecutor jse8 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-checkbox-2-input")));
		jse8.executeScript("arguments[0].click();", element);
		System.out.println("Tick declare box checked");
		Thread.sleep(1000);
		JavascriptExecutor jse9 = (JavascriptExecutor) driver;
		element = driverWait.until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Submit request')]")));
		jse9.executeScript("arguments[0].click();", element);

		DisputeTicketOptionsPicker popup = new DisputeTicketOptionsPicker();
		popup.popupSubmitWindow(element, driverWait, driver);

	}

}
