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
import ca.bc.gov.open.cto.WebDriverManager;
import junit.framework.Assert;

public class SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject {

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

	@SuppressWarnings("deprecation")
	@Test
	public void test() throws Exception {
		driver = WebDriverManager.getDriver();
		WebDriverWait driverWait = WebDriverManager.getDriverWait();
		WebElement element = WebDriverManager.getElement();
		WebDriverManager.getElements();

		DisputeTicketUploadPNG upload = new DisputeTicketUploadPNG();
		upload.uploadPNG(element, driverWait, driver);

		SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject submit = new SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject();
		submit.submitPNG(element, driverWait, driver);

		// Switch to pop-up window
		DisputeTicketOptionsPickerByMail popupWindowHandle = new DisputeTicketOptionsPickerByMail();
		popupWindowHandle.popup(element, driverWait, driver);

		// Switch to Staff Workbench

		CommonUtils.loginStaffWorkbench();

		SubmitToStaffWorkbench loginStaff = new SubmitToStaffWorkbench();
		loginStaff.loginToStaffWorkbench(element, driverWait, driver);

		SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject checkEditStaffTCO = new SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject();
		checkEditStaffTCO.staffWorkCheckAndEdit(element, driverWait, driver);

		Thread.sleep(5000);
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Reject ')]")))
				.click();

		// Reject reason
		Thread.sleep(1000);
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-29")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium.");

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions
						.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Send rejection notification ')]")))
				.click();

		new WebDriverWait(driver, Duration.ofSeconds(50))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Kent')]")));

		List<WebElement> searchTextBoxes = driver.findElements(
				By.xpath("//*[@id=\"mat-tab-content-1-0\"]/div/app-ticket-inbox/div[2]/table/tbody/tr[1]/td[6]/span"));
		for (WebElement searchTextBox : searchTextBoxes) {
			String typeValue = searchTextBox.getText();
			String expectTitle = "REJECTED";
			System.out.println("Value of type attribute: " + typeValue);
			Assert.assertEquals(expectTitle, typeValue);
		}
	}

	public void submitPNG(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {
		
		new WebDriverWait(driver, Duration.ofSeconds(50))
		.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Save information and create online ticket ')]")));

		JavascriptExecutor js = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(
				By.xpath("//*[contains(text(), ' Save information and create online ticket ')]")));
		js.executeScript("arguments[0].click();", element);

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

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-4")));
		element.sendKeys("3220 Qadra");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-5")));
		element.sendKeys("Victoria");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-7")));
		element.sendKeys("V8X1G3");
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-8")));
		element.sendKeys("9999999999");

		Thread.sleep(1000);
		JavascriptExecutor js2 = (JavascriptExecutor) driver;
		element = driverWait
				.until(ExpectedConditions.presenceOfElementLocated(By.cssSelector(".mat-checkbox-inner-container")));
		Thread.sleep(1000);
		js2.executeScript("arguments[0].click();", element);

		DisputeTicketUploadPNG review = new DisputeTicketUploadPNG();
		review.review(element, driverWait, driver);

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-10")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-11")));
		element.sendKeys(
				"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");

		// Click Next
		JavascriptExecutor js5 = (JavascriptExecutor) driver;
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
		js5.executeScript("arguments[0].click();", element);
		Thread.sleep(1000);

		JavascriptExecutor jse3 = (JavascriptExecutor) driver;
		// Click Next
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-checkbox-3")));
		jse3.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();

		System.out.println("Click Submit button");
		Thread.sleep(1000);
		JavascriptExecutor js7 = (JavascriptExecutor) driver;
		element = driverWait.until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Submit request')]")));
		js7.executeScript("arguments[0].scrollIntoView();", element);
		Thread.sleep(1000);
		element.click();

	}

	public void staffWorkCheckAndEdit(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.name("searchInput")));
		element.sendKeys("AN00893391");
		
		// Click first entry and verify the new created user is present

		new WebDriverWait(driver, Duration.ofSeconds(50))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'AN00893391')]")));
		new WebDriverWait(driver, Duration.ofSeconds(50))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Kent')]")));

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By
				.xpath("//*[@id=\"mat-tab-content-0-0\"]/div/app-ticket-inbox/div[2]/table/tbody/tr[1]/td[3]/span/a")));
		element.click();

		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Invalid statute selection.')]")));

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Pass on right')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Registration and Licence')]")));
		new WebDriverWait(driver, Duration.ofSeconds(10)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Municipal Powers')]")));

		// Edit Red Flags

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-13")));
		element.clear();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-13")));
		element.sendKeys("MVA 10(1) Special licence for tractors, etc.");
		
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-15")));
		element.clear();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-15")));
		element.sendKeys("MVA 10(1) Special licence for tractors, etc.");

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-17")));
		element.clear();
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-17")));
		element.sendKeys("MVA 10(1) Special licence for tractors, etc.");
		
		Thread.sleep(1000);
		driver.findElement(By.xpath("//html")).click();

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Save ')]")))
				.click();

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Validate ')]")))
				.click();
		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' None ')]")));

	}

}
