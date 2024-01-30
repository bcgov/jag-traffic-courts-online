package ca.bc.gov.open.ui.eTickets;

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

public class SubmitToStaffWorkbenchCancel {

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

        SubmitToStaffWorkbenchCancel cancel = new SubmitToStaffWorkbenchCancel();
        cancel.cancelRequest(driverWait, driver);

    }

    public void cancelRequest(WebDriverWait driverWait, WebDriver driver) throws Exception {

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Cancel ')]")))
                .click();

        WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//p[contains(text(), 'Please enter ')]/following-sibling::*//textarea")));
        element.sendKeys("Test Cancel of ticket");

        new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//*[contains(text(), ' Send cancellation notification ')]")))
                .click();

        new WebDriverWait(driver, Duration.ofSeconds(50)).until(
                ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Ticket Validation')]")));

        new WebDriverWait(driver, Duration.ofSeconds(50)).until(ExpectedConditions.invisibilityOfElementLocated(
                By.xpath("//*[contains(text(), '" + DisputeTicketOptionsPickerByMail.getUser() + "')]")));

        Thread.sleep(1000);

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//mat-select")))
                .click();

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
                        .presenceOfElementLocated(By.xpath("//*[contains(text(), '" + DisputeTicketOptionsPickerByMail.getUser() + "')]")))
                .click();

    }

}
