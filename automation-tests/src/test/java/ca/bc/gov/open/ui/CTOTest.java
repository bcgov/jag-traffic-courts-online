package ca.bc.gov.open.ui;

import java.time.Duration;

import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.interactions.Actions;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.WebDriverManager;


public class CTOTest {
	
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
		driver.manage().window().maximize();


		CommonUtils.login();
		
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Dispute your ticket')]")));
		
		/*element = driverWait.until(ExpectedConditions
				.presenceOfElementLocated(By.cssSelector("[class='mat-button-focus-overlay']")));
		Actions actions = new Actions(driver);
		((JavascriptExecutor) driver).executeScript("arguments[0].scrollIntoView();", element);
		actions.moveToElement(element).click().build().perform();*/
		
		driver.get("https://dev.tickets.gov.bc.ca/ticket");
		
		
		
		
		
		
	}

}


