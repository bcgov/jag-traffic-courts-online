package ca.bc.gov.open.ui.imageTickets;

import java.time.Duration;

import ca.bc.gov.open.cto.CommonMethods;
import ca.bc.gov.open.ui.imageTickets.DisputeTicketUploadPNG;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.JavascriptExecutor;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.CustomWebDriverManager;

import static ca.bc.gov.open.cto.ApiClient.generateImageTicket;
import static ca.bc.gov.open.cto.TicketInfo.*;

public class MoreThan500CharsOnAdditionalInfoNegTest {

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

        generateImageTicket();

        DisputeTicketUploadPNG upload = new DisputeTicketUploadPNG();
        upload.uploadPNG(element, driverWait, driver);

        upload.validateImageTicket(driver);

        Thread.sleep(1000);
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

        CommonMethods.addressInputForImageDispute(driverWait);

        DisputeTicketUploadPNG review = new DisputeTicketUploadPNG();
        review.review(element, driverWait, driver);

        JavascriptExecutor jse33 = (JavascriptExecutor) driver;
        // Click Next
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
        jse33.executeScript("arguments[0].click();", element);
        Thread.sleep(1000);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//input[@formcontrolname='fine_reduction_reason']")));
        element.sendKeys(
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus");
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//input[@formcontrolname='time_to_pay_reason']")));
        element.sendKeys(
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus");
        // Click on page
        Thread.sleep(1000);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//html")));
        element.click();
        Thread.sleep(1000);

        new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//*[contains(text(), 'Maximum length is 500')]")));

    }
}
