package ca.bc.gov.open.ui.eTickets;

import ca.bc.gov.open.cto.CommonMethods;
import ca.bc.gov.open.cto.CommonUtils;
import ca.bc.gov.open.cto.CustomWebDriverManager;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Test;
import org.openqa.selenium.By;

import org.openqa.selenium.Keys;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.support.ui.WebDriverWait;

import java.time.Duration;

import static ca.bc.gov.open.cto.TicketInfo.*;

public class SubmitToStaffWorkbench {

    private WebDriver driver;
    private String user;
    private static String appUSERNAME = System.getenv("USERNAME_APP");
    private static String appPASSWORD = System.getenv("PASSWORD_APP");

    String xpath = "//a[contains(text(), '" + E_TICKET_NUMBER + "')]"; // XPath pattern for the element
    int timeoutSeconds = 300; // Timeout in seconds (5 minutes)
    int searchIntervalSeconds = 5; // Interval in seconds between search attempts

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

        DisputeTicketOptionsPickerByMail dispute = new DisputeTicketOptionsPickerByMail();
        dispute.test();

        CommonUtils.loginStaffWorkbench();

        SubmitToStaffWorkbench loginStaff = new SubmitToStaffWorkbench();
        loginStaff.loginToStaffWorkbenchWithUser(driverWait, driver, appUSERNAME);

        validateTicketOnStaffWOrkbench(driverWait, driver);
    }

    public void validateTicketOnStaffWOrkbench(WebDriverWait driverWait, WebDriver driver) throws Exception {

        xpath = "//a[contains(text(), '" + E_TICKET_NUMBER + "')]"; // XPath pattern for the element
        WebElement element = waitForElementWithRefresh(driver, xpath, E_TICKET_NUMBER, timeoutSeconds, searchIntervalSeconds);
        if (element != null) {
            element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(xpath)));
            element.click();

        } else {
            System.out.println("Dispute not found.");
        }

        new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
                By.xpath("//*[contains(text(), '" + E_TICKET_COUNT_1 + "')]")));
        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '$" + E_TICKET_AMOUNT_1 + ".00')]")));
        new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
                By.xpath("//*[contains(text(), '" + E_TICKET_COUNT_2 + "')]")));
        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '$" + E_TICKET_AMOUNT_2 + ".00')]")));
        new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
                By.xpath("//*[contains(text(), '" + E_TICKET_COUNT_3 + "')]")));
        new WebDriverWait(driver, Duration.ofSeconds(10))
                .until(ExpectedConditions.presenceOfElementLocated(By.xpath("//*[contains(text(), '$" + E_TICKET_AMOUNT_3 + ".00')]")));
    }

    public String getUser() {
        return user;
    }

    public void setUser(String user) {
        this.user = user;
    }

    public void loginToStaffWorkbench(WebDriverWait driverWait, WebDriver driver) throws Exception {

        Thread.sleep(1000);

        WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("username")));
        element.sendKeys(appUSERNAME);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("password")));
        element.sendKeys(appPASSWORD);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.name("login")));
        element.click();

        new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
                By.xpath("//*[contains(text(), 'Sign out')]")));
    }

    public void loginToStaffWorkbenchWithUser(WebDriverWait driverWait, WebDriver driver, String appUSERNAME) throws Exception {

        Thread.sleep(1000);

        WebElement element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("username")));
        element.sendKeys(appUSERNAME);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.id("password")));
        element.sendKeys(appPASSWORD);
        element = driverWait.until(ExpectedConditions.presenceOfElementLocated(By.name("login")));
        element.click();

        new WebDriverWait(driver, Duration.ofSeconds(20)).until(ExpectedConditions.presenceOfElementLocated(
                By.xpath("//*[contains(text(), 'Sign out')]")));
    }

    public static WebElement waitForElementWithRefresh(WebDriver driver, String xpath, String searchText, int timeoutSeconds, int searchIntervalSeconds) {
        WebDriverWait wait = new WebDriverWait(driver, Duration.ofSeconds(20));
        WebElement element = null;
        boolean found = false;

        long startTime = System.currentTimeMillis();
        while (!found && (System.currentTimeMillis() - startTime) < (timeoutSeconds * 1000)) {
            try {
                WebElement searchElement = wait.until(ExpectedConditions.presenceOfElementLocated(By.id("ticketNumber")));
                searchElement.clear();
                searchElement.sendKeys(searchText);
                searchElement.sendKeys(Keys.RETURN);

                element = wait.until(ExpectedConditions.presenceOfElementLocated(By.xpath(xpath)));
                found = true;
            } catch (Exception e) {
                // If element is not found, refresh the page and perform the search again
                driver.navigate().refresh();
            }
            // Wait for the specified interval before retrying
            try {
                Thread.sleep(searchIntervalSeconds * 1000);
            } catch (InterruptedException ex) {
                Thread.currentThread().interrupt();
            }
        }

        // If element is found, return it
        if (found) {
            return element;
        } else {
            System.out.println("Dispute ticket wasn't created. Not found.");
            return null;
        }
    }

}
