package ca.bc.gov.open.ui.eTickets;

import java.time.Duration;

import ca.bc.gov.open.cto.CommonMethods;
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

import static ca.bc.gov.open.cto.Constants.*;
import static ca.bc.gov.open.cto.Constants.LICENSE_XPATH;

public class ContactInfoValidationChars {

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

        CommonUtils.login();

        DisputeTicketOptionsPicker disputeTicketExisting = new DisputeTicketOptionsPicker();
        disputeTicketExisting.startDisputeTicket(element, driverWait, driver);

        Thread.sleep(1000);

        // Required
        CommonMethods.clickOnElementByXpath(driver, driverWait, "//input[@id='mat-input-2']");

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-3")));
        element.click();

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Required')]")));

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(DISPUTANT_SURNAME_XPATH)));
        element.sendKeys("Lorem ipsum dolor sit amet, coq");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(DISPUTANT_NAME_XPATH)));
        element.sendKeys(
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aeneqan commodo ligula eget dolor. ");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(ADDRESS_XPATH)));
        element.sendKeys(
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec p");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(CITY_XPATH)));
        element.sendKeys("Lorem ipsum dolor sit amet, con");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(POSTAL_CODE_XPATH)));
        element.sendKeys("V8X1G3");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(EMAIL_XPATH)));
        element.sendKeys("aaaaaaaaaaaeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeegggggggggggggggggggaaaaaaa@a.com");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(PHONE_XPATH)));
        element.sendKeys("9999999999");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(LICENSE_XPATH)));
        element.sendKeys("999 999 1234");

        Thread.sleep(1000);
        new WebDriverWait(driver, Duration.ofSeconds(10)).until(
                ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Maximum length is 30')]")));

        new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//*[contains(text(), 'Maximum length is 30 per name')]")));

        new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//*[contains(text(), 'Must be a valid email address')]")));

    }

}
