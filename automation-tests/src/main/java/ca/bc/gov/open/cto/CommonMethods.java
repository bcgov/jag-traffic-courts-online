package ca.bc.gov.open.cto;

import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import java.util.Calendar;

import static ca.bc.gov.open.cto.Constants.*;
import static ca.bc.gov.open.cto.TicketInfo.*;

public class CommonMethods {

    public static String user;

    public static void clickOnNextButton(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        // Click Next
        JavascriptExecutor js = (JavascriptExecutor) driver;
        WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
        js.executeScript("arguments[0].click();", element);
        System.out.println("Click Next");
        Thread.sleep(1000);
    }

    public static void pleadGuilty1stCountDoNotAttendCourt(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        // Click Next
        JavascriptExecutor js = (JavascriptExecutor) driver;
        WebElement element = driverWait.until(ExpectedConditions
                .presenceOfElementLocated(By.cssSelector("#mat-radio-5 .mat-radio-outer-circle")));
        js.executeScript("arguments[0].click();", element);

        System.out.println("I would like to plead guilty and request time to pay on this count.- selected");
        Thread.sleep(1000);
    }

    public static void pleadGuilty1stCountAttendCourt(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        clickOnElementByXpath(driver, driverWait, "//input[@id='mat-checkbox-3-input']");
        System.out.println("I would like to request a fine reduction on this count..- selected");
        Thread.sleep(1000);
    }

    public static void agreeCommitedOffence1stCountAttendCourt(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        JavascriptExecutor js = (JavascriptExecutor) driver;
        WebElement element = driverWait.until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//input[@id='mat-radio-5-input']")));

        js.executeScript("arguments[0].click();", element);

        System.out.println("I would like to plead guilty and request time to pay on this count.- selected");
        Thread.sleep(1000);
    }


    public static void skip2ndCount(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        JavascriptExecutor js = (JavascriptExecutor) driver;
        WebElement element = driverWait.until(ExpectedConditions
                .presenceOfElementLocated(By.cssSelector("#mat-checkbox-5 .mat-checkbox-inner-container")));
        js.executeScript("arguments[0].click();", element);

        System.out.println("I would like to plead guilty and request time to pay on this count.- selected for 2nd count");
    }

    public static void skip2ndCountV2(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        JavascriptExecutor js = (JavascriptExecutor) driver;
        WebElement element = driverWait.until(ExpectedConditions
                .presenceOfElementLocated(By.cssSelector("#mat-checkbox-5 .mat-checkbox-inner-container")));
        js.executeScript("arguments[0].click();", element);

        System.out.println("I would like to plead guilty and request time to pay on this count.- selected for 2nd count");
    }

    public static void skip3rdCount(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        JavascriptExecutor js = (JavascriptExecutor) driver;
        WebElement element = driverWait.until(ExpectedConditions
                .presenceOfElementLocated(By.cssSelector("#mat-checkbox-8 .mat-checkbox-inner-container")));
        js.executeScript("arguments[0].click();", element);

        System.out.println("Skip this count, no action required.- selected for 3rd count");
        Thread.sleep(1000);
    }

    public static void skip3rdCountV2(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        JavascriptExecutor js = (JavascriptExecutor) driver;
        WebElement element = driverWait.until(ExpectedConditions
                .presenceOfElementLocated(By.cssSelector("#mat-checkbox-8 .mat-checkbox-inner-container")));
        js.executeScript("arguments[0].click();", element);

        System.out.println("Skip this count, no action required.- selected for 3rd count");
        Thread.sleep(1000);
    }

    public static void scrollToBottom(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        JavascriptExecutor js1 = (JavascriptExecutor) driver;
        // Scroll down till the bottom of the page
        js1.executeScript("window.scrollBy(0,document.body.scrollHeight)");
        System.out.println("Scroll down till the bottom of the page");
        JavascriptExecutor js2 = (JavascriptExecutor) driver;
        Thread.sleep(1000);
    }

    public static void selectAttendCourtHearingRBO(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-radio-3")));
        element.click();
        Thread.sleep(1000);
    }

    public static void clickOnElementByXpath(WebDriver driver, WebDriverWait driverWait, String xpath) {
        driverWait.until(ExpectedConditions.elementToBeClickable(By.xpath(xpath)));

        try {
            driver.findElement(By.xpath(xpath)).click();
        } catch (Exception e) {
            JavascriptExecutor executor = (JavascriptExecutor) driver;
            executor.executeScript("arguments[0].click();", driver.findElement(By.xpath(xpath)));
        }
    }

    public static void addressInputForImageDispute(WebDriverWait driverWait) {

        // temp solution until ticket https://justice.gov.bc.ca/jira/browse/TCVP-2830 is resolved
        WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(DISPUTANT_SURNAME_XPATH)));
        element.clear();
        element.sendKeys(IMAGE_TICKET_SURNAME);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(DISPUTANT_NAME_XPATH)));
        element.clear();
        element.sendKeys(IMAGE_TICKET_NAME);
        //

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(ADDRESS_XPATH)));
        element.sendKeys("3220 Qadra");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(CITY_XPATH)));
        element.sendKeys("Kelowna");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(POSTAL_CODE_XPATH)));
        element.sendKeys("V8x1g6");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(EMAIL_XPATH)));
        element.sendKeys(TICKET_EMAIL);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(PHONE_XPATH)));
        element.sendKeys("9999999999");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(LICENSE_XPATH)));
        element.sendKeys("999 999 1234");
    }

    public static void addressInputForETicketDispute(WebDriverWait driverWait) {

        user = Calendar.getInstance().getTimeInMillis() + "Test";

        System.out.println("New created user is: " + user);

        WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(DISPUTANT_SURNAME_XPATH)));
        element.clear();
        element.sendKeys(user);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(DISPUTANT_NAME_XPATH)));
        element.clear();
        element.sendKeys("User");

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(ADDRESS_XPATH)));
        element.sendKeys("3220 Qadra");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(CITY_XPATH)));
        element.sendKeys("Kelowna");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(POSTAL_CODE_XPATH)));
        element.sendKeys("V8x1g6");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(EMAIL_XPATH)));
        element.sendKeys(TICKET_EMAIL);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(PHONE_XPATH)));
        element.sendKeys("9999999999");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(LICENSE_XPATH)));
        element.sendKeys("999 999 1234");
    }

}
