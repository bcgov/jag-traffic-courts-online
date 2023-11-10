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

import ca.bc.gov.open.cto.WebDriverManager;

public class SubmitToStaffWorkbenchReject {

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

		SubmitToStaffWorkbench dispute = new SubmitToStaffWorkbench();
		dispute.test();

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Reject ')]")))
				.click();

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-17")));
		element.sendKeys("Test Rejection of ticket");

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions
						.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Send rejection notification ')]")))
				.click();

		new WebDriverWait(driver, Duration.ofSeconds(50)).until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), '" + DisputeTicketOptionsPickerByMail.getUser() + "')]")));

		new WebDriverWait(driver, Duration.ofSeconds(20))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'REJECTED')]")));

	}

}
