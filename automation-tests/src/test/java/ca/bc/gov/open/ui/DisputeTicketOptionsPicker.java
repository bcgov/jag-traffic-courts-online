package ca.bc.gov.open.ui;

import java.time.Duration;
import java.util.Iterator;
import java.util.List;
import java.util.Set;

import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.Dimension;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.WebDriverManager;

public class DisputeTicketOptionsPicker {

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

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-find-ticket/app-page/app-busy-overlay/div/div/div/div/div/div[2]/div[1]/div/form/div[2]/mat-form-field/div/div[1]/div/ngx-timepicker-field/div/ngx-timepicker-time-control[1]/div/input")));
		element.sendKeys("17");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-find-ticket/app-page/app-busy-overlay/div/div/div/div/div/div[2]/div[1]/div/form/div[2]/mat-form-field/div/div[1]/div/ngx-timepicker-field/div/ngx-timepicker-time-control[2]/div/input")));
		element.sendKeys("16");
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
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-2")));
		element.sendKeys("Test");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-3")));
		element.sendKeys("User");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-4")));
		element.sendKeys("3220 Qadra");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-5")));
		element.sendKeys("Victoria");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-8")));
		element.sendKeys("V8X1G3");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-10")));
		element.sendKeys("9999999999");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-6")));
		element.sendKeys("claudiu.vlasceanu@nttdata.com");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-9")));
		element.sendKeys("999 999 1234");

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
		// Select rdo button
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-radio-2")));
		element.click();
		
		JavascriptExecutor jse68 = (JavascriptExecutor) driver;
		element = driverWait.until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[2]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/form/section/div[2]/div[1]/mat-checkbox[1]/label/span[1]")));
		jse68.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
		JavascriptExecutor jse6 = (JavascriptExecutor) driver;
		element = driverWait.until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[2]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/form/section/div[2]/div[1]/mat-checkbox[2]/label/span[1]")));
		jse6.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
		JavascriptExecutor jse7 = (JavascriptExecutor) driver;
		element = driverWait.until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[2]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/form/section/div[3]/mat-checkbox[1]/label/span[1]")));
		jse7.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
		Thread.sleep(1000);
		// Click Next
		JavascriptExecutor jse8 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.visibilityOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[2]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/app-stepper-footer/div/div[2]/button")));
		Thread.sleep(1000);
		jse8.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
		String a = "Count 2: Review";
		// identify elements with text()
		List<WebElement> l = driver.findElements(By.xpath("//*[contains(text(),'Count 2: Review')]"));
		// verify list size
		if (l.size() > 0) {
			System.out.println("Text: " + a + " is present. ");
		} else {
			System.out.println("Text: " + a + " is not present. ");
		}

		// Click Next
		Thread.sleep(1000);
		
		JavascriptExecutor js11 = (JavascriptExecutor) driver;
		js11.executeScript("window.scrollBy(0,document.body.scrollHeight)");
		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[3]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/app-stepper-footer/div/div[2]/button")));		
		element.click();

		String b = "Count 3: Review";
		// identify elements with text()
		List<WebElement> k = driver.findElements(By.xpath("//*[contains(text(),'Count 3: Review')]"));
		// verify list size
		if (k.size() > 0) {
			System.out.println("Text: " + b + " is present. ");
		} else {
			System.out.println("Text: " + b + " is not present. ");
		}
		Thread.sleep(1000);
		JavascriptExecutor js12 = (JavascriptExecutor) driver;
		js12.executeScript("window.scrollBy(0,document.body.scrollHeight)");
		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[4]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/app-stepper-footer/div/div[2]/button")));
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
		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-11")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, ven");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-12")));
		element.sendKeys(
				"semper nisi. Aenean vulputate eleifend tellus. Aenean leo ligula, porttitor eu, consequat vitae, eleifend ac, enim. Aliquam lorem ante, dapibus in, viverra quis, feugiat a, tellus. Phasellus viverra nulla ut metus varius laoreet. Quisque rutrum. Aenean imperdiet. Etiam ultricies nisi vel augue. Curabitur ullamcorper ultricies nisi. Nam eget dui. Etiam rhoncus. Maecenas tempus, tellus eget condimentum rhoncus, sem quam semper libero, sit amet adipiscing sem neque sed ipsum. Nam quam nunc, blandit vel,");
		// Click Next
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[5]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/app-stepper-footer/div/div[2]/button")));
		element.click();

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
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-checkbox-13")));
		element.click();
		System.out.println("Tick declare box checked");
		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/app-root/div/app-tco-page/div/div[2]/app-create-notice-of-dispute/app-page/app-busy-overlay/div/div/div/div/div/app-dispute-stepper/mat-vertical-stepper/div[6]/div/div/div/app-page/app-busy-overlay/div/div/div/div/div/app-stepper-footer/div/div[2]/button")));
		element.click();

		// Switch to pop-up window
		String parentWindowHandler = driver.getWindowHandle(); // Store your parent window
		String subWindowHandler = null;

		Set<String> handles = driver.getWindowHandles(); // get all window handles
		Iterator<String> iterator = handles.iterator();
		while (iterator.hasNext()) {
			subWindowHandler = iterator.next();
		}
		driver.switchTo().window(subWindowHandler); // switch to popup window

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
				"/html/body/div[3]/div[2]/div/mat-dialog-container/app-confirm-dialog/mat-dialog-actions/button[2]/span[1]")));
		element.click();
		System.out.println("Submit in pop-up clicked");

		driver.switchTo().window(parentWindowHandler); // switch back to parent window

	}

}
