package ca.bc.gov.open.ui.eTickets;

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

import static ca.bc.gov.open.cto.TicketInfo.TICKET_EMAIL;
public class DisputeTicketOptionsPickerDiffCountry {

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

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-2")));
		element.sendKeys("Test");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-3")));
		element.sendKeys("User");
		Thread.sleep(1000);

		JavascriptExecutor jse = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-select-value-1")));
		jse.executeScript("scroll(0, 450);");
		Thread.sleep(1000);
		element.click();

		Thread.sleep(1000);
		element = driverWait
				.until(ExpectedConditions.presenceOfElementLocated(By.cssSelector("#mat-option-1 > .mat-option-text")));
		element.click();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-4")));
		element.sendKeys("9999 Oak Street");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-5")));
		element.sendKeys("Chicago");
		Thread.sleep(1000);
		JavascriptExecutor jse22 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-select-value-3")));
		jse22.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);
		JavascriptExecutor jse62 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.elementToBeClickable(By.xpath("//*[contains(text(), 'USA')]")));
		jse62.executeScript("arguments[0].click();", element);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-11")));
		element.sendKeys("98001");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-9")));
		element.sendKeys("9999999999");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-6")));
		element.sendKeys(TICKET_EMAIL);

		JavascriptExecutor jse2 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-select-value-5")));
		jse2.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();
		JavascriptExecutor jse63 = (JavascriptExecutor) driver;
		element = driverWait
				.until(ExpectedConditions.elementToBeClickable(By.xpath("//*[contains(text(), 'Arizona ')]")));
		jse63.executeScript("arguments[0].click();", element);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-10")));
		element.sendKeys("test Law firm");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-12")));
		element.sendKeys("test Given");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-13")));
		element.sendKeys("12345");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-8")));
		element.sendKeys("999 999 1234");

		DisputeTicketOptionsPicker review = new DisputeTicketOptionsPicker();
		review.reviewProcess(element, driverWait, driver);

		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//div[contains(text(), 'Count 1')]/..//*[contains(text(),'fine reduction')]/../../../..//input")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, ven");
		// Click Next
		JavascriptExecutor js5 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		js5.executeScript("arguments[0].click();", element);

		DisputeTicketOptionsPicker overview = new DisputeTicketOptionsPicker();
		overview.ticketRequestOverview(element, driverWait, driver);

	}

}
