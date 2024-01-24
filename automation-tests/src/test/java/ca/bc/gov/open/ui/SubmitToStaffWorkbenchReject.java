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

public class SubmitToStaffWorkbenchReject {

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

		SubmitToStaffWorkbench dispute = new SubmitToStaffWorkbench();
		dispute.test();

		rejectRequest(driverWait, driver);
	}

	public void rejectRequest(WebDriverWait driverWait, WebDriver driver) throws Exception {

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Reject ')]")))
				.click();

		WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//p[contains(text(), 'Please enter ')]/following-sibling::*//textarea")));
		element.sendKeys("Test Rejection of ticket");

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions
						.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Send rejection notification ')]")))
				.click();

		Thread.sleep(1000);

		new WebDriverWait(driver, Duration.ofSeconds(30))
				.until(ExpectedConditions
						.presenceOfElementLocated(By.xpath("//mat-select")))
				.click();

		Thread.sleep(1000);

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions
						.presenceOfElementLocated(By.xpath("//span[contains(text(), 'REJECTED')]")))
				.click();

		new WebDriverWait(driver, Duration.ofSeconds(50))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//td/span[contains(text(), 'REJECTED')]")));

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions
						.presenceOfElementLocated(By.xpath("//*[contains(text(), '" + DisputeTicketOptionsPickerByMail.getUser() + "')]")));

	}

}
