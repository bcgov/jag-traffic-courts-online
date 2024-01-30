package ca.bc.gov.open.ui.eTickets;

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

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.CustomWebDriverManager;

import static ca.bc.gov.open.cto.TicketInfo.*;

public class ManageOrUpdateTrafficDispute {

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

		SubmitToStaffWorkbench dispute = new SubmitToStaffWorkbench();
		dispute.test();

		CommonUtils.login();

		JavascriptExecutor js = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Manage or update my dispute')]")));
		Thread.sleep(1000);
		js.executeScript("arguments[0].click();", element);

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-0")));
		element.sendKeys(E_TICKET_NUMBER);

		element = driverWait
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(@placeholder,'HH')]")));
		element.sendKeys(E_TICKET_TIME_HOURS);

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(@placeholder,'MM')]")))
				.sendKeys(E_TICKET_TIME_MINUTES);
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Find ticket ')]")))
				.click();
		Thread.sleep(1000);
		// Start Traffic ticket dispute request
		JavascriptExecutor js1 = (JavascriptExecutor) driver;
		// Scroll down till the bottom of the page
		js1.executeScript("window.scrollBy(0,document.body.scrollHeight)");
		System.out.println("Scroll down till the bottom of the page");

		driverWait.until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'What would you like to do?')]")));

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//b[contains(text(), 'Check Status')]")))
				.click();


		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//div[contains(text(), 'Submitted')]")));
	}

}
