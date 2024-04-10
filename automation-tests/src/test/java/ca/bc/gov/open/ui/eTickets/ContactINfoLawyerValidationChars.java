package ca.bc.gov.open.ui.eTickets;

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
import ca.bc.gov.open.cto.CustomWebDriverManager;

import static ca.bc.gov.open.cto.Constants.*;

public class ContactINfoLawyerValidationChars {

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

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(DISPUTANT_SURNAME_XPATH)));
        element.sendKeys("Test");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(DISPUTANT_NAME_XPATH)));
        element.sendKeys("User");
        //Select Lawyer
        JavascriptExecutor js = (JavascriptExecutor) driver;
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-select-value-1")));
        Thread.sleep(1000);
        js.executeScript("arguments[0].click();", element);
        JavascriptExecutor js1 = (JavascriptExecutor) driver;
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.cssSelector("#mat-option-1 > .mat-option-text")));
        Thread.sleep(1000);
        //500 chars
        js1.executeScript("arguments[0].click();", element);

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(CONTACT_LAW_FIRM_NAME_XPATH)));
        element.sendKeys("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(CONTACT_NAME_XPATH)));
        element.sendKeys("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede.");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(CONTACT_SURNAME_XPATH)));
        element.sendKeys("Surname Agent");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(ADDRESS_XPATH)));
        element.sendKeys("3220 Qadra");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(CITY_XPATH)));
        element.sendKeys("Kelowna");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(POSTAL_CODE_XPATH)));
        element.sendKeys("V8x1g6");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(EMAIL_XPATH)));
        element.sendKeys("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa@nttdata.com");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(PHONE_XPATH)));
        element.sendKeys("9999999999");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(LICENSE_XPATH)));
        element.sendKeys("999 999 1234");

        // blocked by ticket https://justice.gov.bc.ca/jira/browse/TCVP-2894
//		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
//				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Maximum length is 100')]")));
//		new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
//				.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Maximum length is 30')]")));

        new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//*[contains(text(), 'Must be a valid email address')]")));


    }

}
