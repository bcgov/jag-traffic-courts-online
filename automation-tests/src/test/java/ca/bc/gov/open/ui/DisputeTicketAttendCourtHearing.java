package ca.bc.gov.open.ui;

import java.time.Duration;
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

public class DisputeTicketAttendCourtHearing {

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
		// Scroll down till the bottom of the page
		JavascriptExecutor jse3 = (JavascriptExecutor) driver;
		// Click Next
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		jse3.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
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
		Thread.sleep(1000);
		jse1.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[2]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/form/section/div[3]/mat-checkbox[1]/label/span[1]")));
		element.click();
		JavascriptExecutor jse8 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[2]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/app-stepper-footer/div/div[2]/button/span[1]/strong")));
		Thread.sleep(1000);
		jse8.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
		String a = "Count 1: Review";
		// identify elements with text()
		List<WebElement> l = driver.findElements(By.xpath("//*[contains(text(),'Count 1: Review')]"));
		// verify list size
		if (l.size() > 0) {
			System.out.println("Text: " + a + " is present. ");
		} else {
			System.out.println("Text: " + a + " is not present. ");
		}
		JavascriptExecutor jse22 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[3]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/form/section/div[3]/mat-checkbox[1]/label/span[1]")));
		Thread.sleep(1000);
		jse22.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
		JavascriptExecutor jse2 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[3]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/app-stepper-footer/div/div[2]/button")));
		Thread.sleep(1000);
		jse2.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
		String b = "Count 2: Review";
		// identify elements with text()
		List<WebElement> m = driver.findElements(By.xpath("//*[contains(text(),'Count 2: Review')]"));
		// verify list size
		if (m.size() > 0) {
			System.out.println("Text: " + b + " is present. ");
		} else {
			System.out.println("Text: " + b + " is not present. ");
		}
		JavascriptExecutor jse4 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[4]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/app-stepper-footer/div/div[2]/button")));
		Thread.sleep(1000);
		jse4.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
		String c = "Count 3: Review";
		// identify elements with text()
		List<WebElement> n = driver.findElements(By.xpath("//*[contains(text(),'Count 3: Review')]"));
		// verify list size
		if (n.size() > 0) {
			System.out.println("Text: " + c + " is present. ");
		} else {
			System.out.println("Text: " + c + " is not present. ");
		}

		// Additional Info with lawyer, interpreter and witness
		element = driverWait.until(ExpectedConditions
				.elementToBeClickable(By.cssSelector("#mat-checkbox-13 .mat-checkbox-inner-container")));
		element.click();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-11")));
		element.sendKeys("Test LTD");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-12")));
		element.sendKeys("Lawyer Lawyer");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-13")));
		element.sendKeys("3220 Douglas");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-14")));
		element.sendKeys("9999999999");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-15")));
		element.sendKeys("test@test.com");
		element = driverWait.until(ExpectedConditions
				.elementToBeClickable(By.cssSelector("#mat-checkbox-14 .mat-checkbox-inner-container")));
		element.click();
		element = driverWait.until(ExpectedConditions.elementToBeClickable(By.id("mat-select-value-7")));
		element.click();
		element = driverWait
				.until(ExpectedConditions.elementToBeClickable(By.cssSelector("#mat-option-264 > .mat-option-text")));
		element.click();
		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions
				.elementToBeClickable(By.cssSelector("#mat-checkbox-15 .mat-checkbox-inner-container")));
		element.click();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-16")));
		element.clear();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-16")));
		element.sendKeys("3");
		Thread.sleep(1000);
		JavascriptExecutor js1 = (JavascriptExecutor) driver;
		// Scroll down till the bottom of the page
		js1.executeScript("window.scrollBy(0,document.body.scrollHeight)");
		Thread.sleep(1000);
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[5]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/app-stepper-footer/div/div[2]/button")))
				.click();
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
		JavascriptExecutor js15 = (JavascriptExecutor) driver;
		// Scroll down till the bottom of the page
		js15.executeScript("window.scrollBy(0,document.body.scrollHeight)");

		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-checkbox-16")));
		element.click();
		System.out.println("Tick declare box checked");
		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[6]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/app-stepper-footer/div/div[2]/button")));
		element.click();

		DisputeTicketOptionsPicker popup = new DisputeTicketOptionsPicker();
		popup.popupSubmitWindow(element, driverWait, driver);

	}

}
