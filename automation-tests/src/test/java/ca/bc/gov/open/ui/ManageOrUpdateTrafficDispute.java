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

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.WebDriverManager;

public class ManageOrUpdateTrafficDispute {

	private WebDriver driver;

//	@After
//	public void tearDown() {
//		driver.close();
//		driver.quit();
//	}
//
//	@AfterClass
//	public static void afterClass() {
//		WebDriverManager.instance = null;
//	}

	@Test
	public void test() throws Exception {
		driver = WebDriverManager.getDriver();
		WebDriverWait driverWait = WebDriverManager.getDriverWait();
		WebElement element = WebDriverManager.getElement();
		WebDriverManager.getElements();

		CommonUtils.login();
		
		
		JavascriptExecutor js = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Manage or update my traffic dispute ')]")));
		Thread.sleep(1000);
		js.executeScript("arguments[0].click();", element);
		
		
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-0")));
		element.sendKeys("EA03148599");

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(@placeholder,'HH')]")));
		element.sendKeys("17");

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(@placeholder,'MM')]")))
				.sendKeys("16");
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Find ticket ')]")))
				.click();

	}

}
