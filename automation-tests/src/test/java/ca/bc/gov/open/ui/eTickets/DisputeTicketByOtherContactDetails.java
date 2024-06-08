package ca.bc.gov.open.ui.eTickets;

import ca.bc.gov.open.cto.CommonMethods;
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
import ca.bc.gov.open.cto.CustomWebDriverManager;

import static ca.bc.gov.open.cto.Constants.*;
import static ca.bc.gov.open.cto.Constants.LICENSE_XPATH;
import static ca.bc.gov.open.cto.TicketInfo.TICKET_EMAIL;

public class DisputeTicketByOtherContactDetails {

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

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//input[@formcontrolname='disputant_surname']")));
        element.sendKeys("Test");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//input[@formcontrolname='disputant_given_names']")));
        element.sendKeys("User");
        // Select Lawyer
        JavascriptExecutor js = (JavascriptExecutor) driver;
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-select-value-1")));
        Thread.sleep(1000);
        js.executeScript("arguments[0].click();", element);
        JavascriptExecutor js1 = (JavascriptExecutor) driver;
        element = driverWait
                .until(ExpectedConditions.presenceOfElementLocated(By.cssSelector("#mat-option-2 > .mat-option-text")));
        Thread.sleep(1000);
        js1.executeScript("arguments[0].click();", element);

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(CONTACT_NAME_XPATH)));
        element.sendKeys("Test Agent");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(CONTACT_SURNAME_XPATH)));
        element.sendKeys("Surname Agent");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(ADDRESS_XPATH)));
        element.sendKeys("3220 Qadra");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(CITY_XPATH)));
        element.sendKeys("Victoria");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(POSTAL_CODE_XPATH)));
        element.sendKeys("V8x1g6");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(EMAIL_XPATH)));
        element.sendKeys(TICKET_EMAIL);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(PHONE_XPATH)));
        element.sendKeys("9999999999");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(LICENSE_XPATH)));
        element.sendKeys("999 999 1234");

        DisputeTicketOptionsPicker review = new DisputeTicketOptionsPicker();
        review.reviewProcess(element, driverWait, driver);

        Thread.sleep(1000);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(TIME_TO_PAY_REASON_XPATH)));
        element.sendKeys(
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");
        // Click Next
        JavascriptExecutor js5 = (JavascriptExecutor) driver;
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
        js5.executeScript("arguments[0].click();", element);

        DisputeTicketOptionsPicker overview = new DisputeTicketOptionsPicker();
        overview.ticketRequestOverview(element, driverWait, driver);

        DisputeTicketOptionsPicker popup = new DisputeTicketOptionsPicker();
        popup.popupSubmitWindow(element, driverWait, driver);

    }

}
