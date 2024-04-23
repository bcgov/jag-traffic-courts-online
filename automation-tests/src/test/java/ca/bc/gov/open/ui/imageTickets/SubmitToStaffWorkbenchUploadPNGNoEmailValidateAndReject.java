package ca.bc.gov.open.ui.imageTickets;

import java.time.Duration;

import ca.bc.gov.open.cto.CommonMethods;
import ca.bc.gov.open.ui.eTickets.DisputeTicketOptionsPicker;
import ca.bc.gov.open.ui.eTickets.SubmitToStaffWorkbench;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.*;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.CustomWebDriverManager;

import static ca.bc.gov.open.cto.ApiClient.generateImageTicket;
import static ca.bc.gov.open.cto.TicketInfo.*;
import static ca.bc.gov.open.ui.eTickets.SubmitToStaffWorkbench.waitForElementWithRefresh;

public class SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject {

    private WebDriver driver;
    String xpath = "//a[contains(text(), '" + TICKET_NUMBER + "')]"; // XPath pattern for the element
    int timeoutSeconds = 600; // (10 minutes) Timeout in seconds for order to appear in staff workbench
    int searchIntervalSeconds = 5; // Interval in seconds between search attempts

    @After
    public void tearDown() {
        driver.close();
        driver.quit();
    }

    @AfterClass
    public static void afterClass() {
        CustomWebDriverManager.instance =
                null;
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

        // Switch to Staff Workbench\
        CommonUtils.loginStaffWorkbench();

        SubmitToStaffWorkbench loginStaff = new SubmitToStaffWorkbench();
        loginStaff.loginToStaffWorkbench(driverWait, driver);

        SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject checkEditStaffTCO = new SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndReject();
        checkEditStaffTCO.staffWorkCheckAndEdit(element, driverWait, driver);

        Thread.sleep(3000);
        staffRejectImageTicket(driverWait, driver);
    }

    public void staffRejectImageTicket(WebDriverWait driverWait, WebDriver driver) throws Exception {
        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Reject ')]")))
                .click();
        Thread.sleep(1000);

        WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//p[contains(text(), 'Please enter ')]/following-sibling::*//textarea")));
        element.sendKeys(
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium.");

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//*[contains(text(), ' Send rejection notification ')]")))
                .click();

        Thread.sleep(3000);

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//mat-select")))
                .click();

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//mat-option//span[contains(text(), 'REJECTED')]")))
                .click();

        new WebDriverWait(driver, Duration.ofSeconds(50))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//td/span[contains(text(), 'REJECTED')]")));

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions
                        .presenceOfElementLocated(By.xpath("//*[contains(text(), '" + IMAGE_TICKET_SURNAME + "')]")))
                .click();

    }

    public void submitPNG(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {


        new WebDriverWait(driver, Duration.ofSeconds(200))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Save information and create online ticket ')]")));

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

        Thread.sleep(2000);
        JavascriptExecutor js2 = (JavascriptExecutor) driver;

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-select-6")));
        element.click();

        Thread.sleep(1000);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-select-6-panel")));
        element.click();

        Thread.sleep(1000);
        element = driverWait
                //I prefer to be contacted by regular mail
                .until(ExpectedConditions.presenceOfElementLocated(By.cssSelector(".mat-checkbox-inner-container")));
        Thread.sleep(1000);
        js2.executeScript("arguments[0].click();", element);

        DisputeTicketUploadPNG review = new DisputeTicketUploadPNG();
        review.review(element, driverWait, driver);

        Thread.sleep(1000);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));

        Thread.sleep(1000);
        js.executeScript("arguments[0].click();", element);

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Additional information')]")));

        Thread.sleep(1000);

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//input[@formcontrolname='fine_reduction_reason']")));
        element.sendKeys(
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//input[@formcontrolname='time_to_pay_reason']")));
        element.sendKeys(
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu");


        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("primaryButton")));
        Thread.sleep(1000);
        js.executeScript("arguments[0].click();", element);


        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Please review and ensure details are correct before submission. You may update your dispute online up to 5 business days prior to a set Hearing Date.')]")));


        Thread.sleep(1000);
        System.out.println("Click Submit button");

        Thread.sleep(1000);
        JavascriptExecutor js7 = (JavascriptExecutor) driver;
        element = driverWait.until(
                ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), 'Submit request')]")));

        js7.executeScript("arguments[0].scrollIntoView();", element);
        Thread.sleep(2000);
        element.click();

        DisputeTicketOptionsPicker popup = new DisputeTicketOptionsPicker();
        popup.popupSubmitWindow(element, driverWait, driver);


    }

    public void staffWorkCheckAndEdit(WebElement element, WebDriverWait driverWait, WebDriver driver) throws Exception {

        System.out.println("Started ticket search... ");
        xpath = "//a[contains(text(), '" + TICKET_NUMBER + "')]"; // XPath pattern for the element
        element = waitForElementWithRefresh(driver, xpath, TICKET_NUMBER, timeoutSeconds, searchIntervalSeconds);
        if (element != null) {
            element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(xpath)));
            element.click();
            System.out.println("Ticket is found.");
        } else {
            System.out.println("Ticket is not found.");
        }

        new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//*[contains(text(), '" + IMAGE_TICKET_SECTION_1 + "')]")));
        new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//*[contains(text(), '" + IMAGE_TICKET_SECTION_2 + "')]")));
        new WebDriverWait(driver, Duration.ofSeconds(10)).until(ExpectedConditions
                .presenceOfElementLocated(By.xpath("//*[contains(text(), '" + IMAGE_TICKET_SECTION_3 + "')]")));
        Thread.sleep(1000);
        // Edit Red Flags

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-14")));
        element.clear();
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-14")));
//        element.sendKeys("Excessive Speeding");

        element.sendKeys("MVA 10(1) Special licence for tractors");

//        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By
//                .xpath("//span[contains(text(), 'Excessive Speeding')]")));
//        element.click();

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-16")));
        element.clear();
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-16")));
        element.sendKeys("MVA 10(1) Special licence for tractors, etc.");
//
//        element.sendKeys("Driving Without Licence");
//        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By
//                .xpath("//span[contains(text(), 'Driving Without Licence')]")));
//        element.click();

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-18")));
        element.clear();
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("mat-input-18")));
        element.sendKeys("MVA 10(1) Special licence for tractors, etc.");

//        element.sendKeys("Driving With Burned Out Break Lights");
//        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By
//                .xpath("//span[contains(text(), 'Driving With Burned Out Break Lights')]")));
//        element.click();

//        Thread.sleep(1000);
//        driver.findElement(By.xpath("//html")).click();

        Thread.sleep(1000);
        driver.findElement(By.xpath("//html")).click();


        Thread.sleep(1000);
        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Save ')]")))
                .click();
//

        Thread.sleep(3000);

        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath("//p[contains(text(), 'Please describe the changes you made to the dispute')]/following-sibling::*//textarea")));
        element.sendKeys(
                "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium.");

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//mat-dialog-container//*[contains(text(), ' Save ')]")))
                .click();

        Thread.sleep(1000);

        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' Validate ')]")))
                .click();
        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), ' None ')]")));
    }
}
