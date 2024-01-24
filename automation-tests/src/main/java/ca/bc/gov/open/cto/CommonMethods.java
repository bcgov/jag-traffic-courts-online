package ca.bc.gov.open.cto;

import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

public class CommonMethods {

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
//        JavascriptExecutor js = (JavascriptExecutor) driver;
//        WebElement element = driverWait.until(ExpectedConditions
//                .presenceOfElementLocated(By.cssSelector("#mat-checkbox-4 .mat-checkbox-inner-container")));
//        js.executeScript("arguments[0].click();", element);
        clickOnElementByXpath(driver,driverWait,"//input[@id='mat-checkbox-2-input']");
        System.out.println("I would like to request a fine reduction on this count..- selected");
        Thread.sleep(1000);
    }

    public static void agreeCommitedOffence1stCountAttendCourt(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        JavascriptExecutor js = (JavascriptExecutor) driver;
//        WebElement element = driverWait.until(ExpectedConditions
//                .presenceOfElementLocated(By.cssSelector("#mat-radio-5 .mat-radio-button")));
        WebElement element = driverWait.until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//input[@id='mat-radio-5-input']")));

        js.executeScript("arguments[0].click();", element);

        System.out.println("I would like to plead guilty and request time to pay on this count.- selected");
        Thread.sleep(1000);
    }


    public static void skip2ndCount(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        JavascriptExecutor js = (JavascriptExecutor) driver;
        WebElement element = driverWait.until(ExpectedConditions
                .presenceOfElementLocated(By.cssSelector("#mat-checkbox-7 .mat-checkbox-inner-container")));
        js.executeScript("arguments[0].click();", element);

        System.out.println("I would like to plead guilty and request time to pay on this count.- selected for 2nd count");
    }

    public static void skip3rdCount(WebDriver driver, WebDriverWait driverWait) throws InterruptedException {
        JavascriptExecutor js = (JavascriptExecutor) driver;
        WebElement element = driverWait.until(ExpectedConditions
                .presenceOfElementLocated(By.cssSelector("#mat-checkbox-10 .mat-checkbox-inner-container")));
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
    public static void clickOnElementByXpath(WebDriver driver, WebDriverWait driverWait, String xpath)  {
        driverWait.until(ExpectedConditions.elementToBeClickable(By.xpath(xpath)));

        try {
            driver.findElement(By.xpath(xpath)).click();
        } catch (Exception e) {
            JavascriptExecutor executor = (JavascriptExecutor) driver;
            executor.executeScript("arguments[0].click();", driver.findElement(By.xpath(xpath)));
        }
    }
}
