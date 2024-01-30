package ca.bc.gov.open.cto;

import io.github.bonigarcia.wdm.WebDriverManager;
import org.apache.commons.io.FileUtils;
import org.junit.Assert;
import org.openqa.selenium.OutputType;
import org.openqa.selenium.TakesScreenshot;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.chrome.ChromeOptions;

import java.io.File;
import java.time.Duration;
import java.util.HashMap;


public class WebDriverFactory {
    public static WebDriverFactory instance = null;



    public static WebDriverFactory getInstance() {
        if (instance == null) {
            instance = new WebDriverFactory();
        }
        return instance;
    }

    private static final HashMap<Long, WebDriver> driversMap = new HashMap<>();

    public static WebDriver getDriver() {
        if (!driverExists()) {
            WebDriver driver = createWebDriver();
            driversMap.put(Thread.currentThread().getId(), driver);
        }
        return driversMap.get(Thread.currentThread().getId());
    }

    public static void quit() {
        if (driverExists()) {
            driversMap.get(Thread.currentThread().getId()).quit();
            resetDriver();
        }
    }

    public static void resetDriver() {
        driversMap.remove(Thread.currentThread().getId());
        driversMap.put(Thread.currentThread().getId(), null);
    }

    public static boolean driverExists() {
        return driversMap.get(Thread.currentThread().getId()) != null;
    }

    public static WebDriver createWebDriver() {
        String webdriver = System.getProperty("browser", "chrome");
        System.setProperty("webdriver.chrome.whitelistedIps", "");
        switch (webdriver) {
            case "firefox":
// return new FirefoxDriver();
            case "chrome":

                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.addArguments("--disable-notifications"); //disables 3rd party notifications
                chromeOptions.addArguments("--disable-extensions"); //disables 3rd party extensions
                chromeOptions.addArguments("--no-sandbox");
                chromeOptions.addArguments("--headless");
                chromeOptions.addArguments("--window-size=1920,1080");
                chromeOptions.addArguments("--disable-dev-shm-usage");
                chromeOptions.addArguments("--verbose");
                chromeOptions.addArguments("--remote-allow-origins=*");
                chromeOptions.addArguments("--ignore-ssl-errors=yes");
                chromeOptions.addArguments("--ignore-certificate-errors");
                chromeOptions.addArguments("--disable-dev-shm-usage");

// default behaviour via Web Driver Manager
                String cachePath = "/usr/local/wd_thread_" + Thread.currentThread().getId() + "/cache/";
                String resolutionCachePath = "/usr/local/wd_thread_" + Thread.currentThread().getId() + "/rcache/";
                WebDriverManager.chromedriver().cachePath(cachePath).resolutionCachePath(resolutionCachePath).setup();

// manual chrome driver settings can be used if webdriver manager does not work. Uncomment below line only for docker container
                System.setProperty("webdriver.chrome.driver", "/chromedriver/chromedriver-linux64/chromedriver");

// manual chrome driver settings can be used if webdriver manager does not work. Uncomment below line
//                System.setProperty("webdriver.chrome.driver", "./chromeDrivers/WindowsDrivers/chromedriver_116W.exe");

// for mac download compatible chrome driver and replace the location of driver in 2nd parameter.
// System.setProperty("webdriver.chrome.driver", "chromeDrivers/MacOS/chromedriver");

                WebDriver driver = new ChromeDriver(chromeOptions);
                driver.manage().timeouts().implicitlyWait(Duration.ofSeconds(10));
                driver.manage().window().maximize();
                return driver;
            default:
                Assert.fail("Unsupported webdriver: " + webdriver);
                return null;
        }
    }
}