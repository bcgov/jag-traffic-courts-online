package ca.bc.gov.open.ui.imageTickets;

import java.time.Duration;

import ca.bc.gov.open.ui.eTickets.DisputeTicketOptionsPickerByMail;
import ca.bc.gov.open.ui.eTickets.SubmitToStaffWorkbench;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.CustomWebDriverManager;

import static ca.bc.gov.open.cto.ApiClient.generateImageTicket;
import static ca.bc.gov.open.cto.TicketInfo.*;

public class SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndSubmitARCandCancel {

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

    @SuppressWarnings("deprecation")
    @Test
    public void test() throws Exception {
        driver = CustomWebDriverManager.getDriver();
        WebDriverWait driverWait = CustomWebDriverManager.getDriverWait();
        WebElement element = CustomWebDriverManager.getElement();
        CustomWebDriverManager.getElements();

        generateImageTicket();

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
        loginStaff.loginToStaffWorkbench(driverWait, driver);

        SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject checkEditStaffTCO = new SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject();
        checkEditStaffTCO.staffWorkCheckAndEdit(element, driverWait, driver);

        Thread.sleep(5000);
        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//*[contains(text(), ' Approve and submit to ARC ')]")))
                .click();

        Thread.sleep(3000);

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//*[contains(text(), ' Approve and send request ')]")))
                .click();

        Thread.sleep(3000);

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//mat-select")))
                .click();

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//mat-option//span[contains(text(), 'PROCESSING')]")))
                .click();

        new WebDriverWait(driver, Duration.ofSeconds(50))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//td/span[contains(text(), 'PROCESSING')]")));

//		element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("ticketNumber")));
//		element.sendKeys(TICKET_NUMBER);

//		new WebDriverWait(driver, Duration.ofSeconds(50)).until(ExpectedConditions.presenceOfElementLocated(
//				By.xpath("//*[contains(text(), '" + IMAGE_TICKET_NAME + "')]")));

        new WebDriverWait(driver, Duration.ofSeconds(30))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath(
                        "//a[contains(text(), '" + TICKET_NUMBER + "')]")))
                .click();

        new WebDriverWait(driver, Duration.ofSeconds(50))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Cancel ')]")))
                .click();

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//p[contains(text(), 'Please enter ')]/following-sibling::*//textarea")));
        element.sendKeys("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec.");

        new WebDriverWait(driver, Duration.ofSeconds(50)).until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//*[contains(text(), ' Send cancellation notification ')]")))
                .click();
        Thread.sleep(1000);

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//mat-select")))
                .click();

        Thread.sleep(1000);

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//span[contains(text(), 'CANCELLED')]")))
                .click();

        new WebDriverWait(driver, Duration.ofSeconds(50))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//td/span[contains(text(), 'CANCELLED')]")));

        //check if should be unassigned
//		new WebDriverWait(driver, Duration.ofSeconds(50))
//				.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Unassigned')]")));

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//*[contains(text(), '" + TICKET_NUMBER + "')]")))
                .click();

        new WebDriverWait(driver, Duration.ofSeconds(50))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'CANCELLED')]")));
    }

}
