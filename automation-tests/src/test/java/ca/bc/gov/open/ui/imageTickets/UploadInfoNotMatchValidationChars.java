package ca.bc.gov.open.ui.imageTickets;

import java.time.Duration;

import ca.bc.gov.open.ui.imageTickets.DisputeTicketUploadPNG;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.*;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.CustomWebDriverManager;

import static ca.bc.gov.open.cto.ApiClient.generateImageTicket;

public class UploadInfoNotMatchValidationChars {

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

        // Tick there are differences box

        new WebDriverWait(driver, Duration.ofSeconds(120)).until(
                ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Ticket details')]")));
        JavascriptExecutor js6 = (JavascriptExecutor) driver;
        element = driverWait
                .until(ExpectedConditions.presenceOfElementLocated(By.className("mat-checkbox-inner-container")));
        js6.executeScript("arguments[0].click();", element);

        System.out.println("box ticked");

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-2")));
        element.sendKeys(
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibus");

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//html")));
        element.sendKeys(Keys.TAB);

        new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//*[contains(text(), 'Maximum length is 500')]")));
    }

}
