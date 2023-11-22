package ca.bc.gov.open.ui;

import java.time.Duration;

import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.CustomWebDriverManager;

public class SubmitToStaffWorkbenchSubmitToARC {

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

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions
						.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Approve and submit to ARC ')]")))
				.click();

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions
						.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Approve and send request ')]")))
				.click();

		new WebDriverWait(driver, Duration.ofSeconds(50)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Ticket Validation')]")));

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.name("searchInput")));
		element.sendKeys("EA03148599");

		new WebDriverWait(driver, Duration.ofSeconds(50)).until(ExpectedConditions.invisibilityOfElementLocated(
				By.xpath("//*[contains(text(), '" + DisputeTicketOptionsPickerByMail.getUser() + "')]")));

		new WebDriverWait(driver, Duration.ofSeconds(50))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'PROCESSING')]")));

		// click first in queue

		new WebDriverWait(driver, Duration.ofSeconds(50))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
						"//*[@id=\"mat-tab-content-1-0\"]/div/app-ticket-inbox/div[2]/table/tbody/tr[1]/td[3]/span/a")))
				.click();

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Cancel ')]")))
				.click();

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-34")));
		element.sendKeys("Test Cancel of ticket");

		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Send cancellation notification ')]")))
				.click();

		new WebDriverWait(driver, Duration.ofSeconds(50)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Ticket Validation')]")));

		new WebDriverWait(driver, Duration.ofSeconds(50))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Unassigned')]")));

		new WebDriverWait(driver, Duration.ofSeconds(50)).until(ExpectedConditions.invisibilityOfElementLocated(
				By.xpath("//*[contains(text(), '" + DisputeTicketOptionsPickerByMail.getUser() + "')]")));

	}

}
