package ca.bc.gov.open.ui;

import java.time.Duration;
import java.util.List;

import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.WebDriverManager;
import junit.framework.Assert;

public class SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndSubmitARCandCancel {

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
				.until(ExpectedConditions
						.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Approve and submit to ARC ')]")))
				.click();

		new WebDriverWait(driver, Duration.ofSeconds(10))
				.until(ExpectedConditions
						.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Approve and send request ')]")))
				.click();

		new WebDriverWait(driver, Duration.ofSeconds(50))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Kent')]")));

		List<WebElement> searchTextBoxes = driver.findElements(
				By.xpath("//*[@id=\"mat-tab-content-1-0\"]/div/app-ticket-inbox/div[2]/table/tbody/tr[1]/td[6]/span"));
		for (WebElement searchTextBox : searchTextBoxes) {
			String typeValue = searchTextBox.getText();
			String expectTitle = "PROCESSING";
			System.out.println("Value of type attribute: " + typeValue);
			Assert.assertEquals(expectTitle, typeValue);
		}

		new WebDriverWait(driver, Duration.ofSeconds(50))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath(
						"//*[@id=\"mat-tab-content-1-0\"]/div/app-ticket-inbox/div[2]/table/tbody/tr[1]/td[3]/span/a")))
				.click();
		new WebDriverWait(driver, Duration.ofSeconds(50))
				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Cancel ')]")))
				.click();

		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-58")));
		element.sendKeys("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec.");

		new WebDriverWait(driver, Duration.ofSeconds(50)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Send cancellation notification ')]")))
				.click();
		//Enter ticket no
		
		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.name("searchInput")));
		element.sendKeys("AN00893391");

		new WebDriverWait(driver, Duration.ofSeconds(50)).until(
				ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Ticket Validation')]")));
		
		
		new WebDriverWait(driver, Duration.ofSeconds(50)).until(ExpectedConditions
				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Email Address Not Verified')]")));
		
		
	}

}
