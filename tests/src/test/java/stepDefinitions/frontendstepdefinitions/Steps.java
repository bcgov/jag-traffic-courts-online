package stepDefinitions.frontendstepdefinitions;

import java.util.concurrent.TimeUnit;
import io.github.bonigarcia.wdm.WebDriverManager;

import org.openqa.selenium.PageLoadStrategy;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.chrome.ChromeOptions;

import io.cucumber.java.en.Given;
import io.cucumber.java.en.Then;
import io.cucumber.java.en.When;

public class Steps {
	WebDriver driver;
	
	@Given("User has successfully launched the web browser")
	public void user_had_successfully_launched_the_web_browser() {
	    // Write code here that turns the phrase above into concrete actions
		String browser = System.getProperty("BROWSER");
		WebDriverManager.chromedriver().setup();
		ChromeOptions options = new ChromeOptions();
		options.addArguments("enable-automation");
		options.addArguments("--no-sandbox");
		options.addArguments("--disable-dev-shm-usage");
		options.addArguments("--headless");
		options.addArguments("--window-size=1920,1080");
		options.addArguments("--disable-extensions");
		options.addArguments("--dns-prefetch-disable");
		options.addArguments("--disable-gpu");
		options.setPageLoadStrategy(PageLoadStrategy.NONE);
        driver = new ChromeDriver(options);
        
        driver.manage().timeouts().implicitlyWait(120, TimeUnit.SECONDS);
        driver.manage().timeouts().setScriptTimeout(120, TimeUnit.SECONDS);
        driver.manage().deleteAllCookies();
	}
	
	@When("User navigates to BC Traffic Courts Online Website")
	public void user_navigates_to_bc_traffic_courts_online_website() {
	    // Write code here that turns the phrase above into concrete actions
	    driver.get("http://localhost:4200/");
	}
	@When("User clicks on Initiate Dispute Option")
	public void user_clicks_on_initiate_dispute_option() {
	    // Write code here that turns the phrase above into concrete actions
	    
	}
	@When("User Enters the Violation Ticket Details")
	public void user_enters_violation_ticket_number_plus_time_of_ticket() {
	    // Write code here that turns the phrase above into concrete actions
	}
	@When("User Signs and Clicks on Complete Option")
	public void user_signs_and_clicks_on_complete_option() {
	    // Write code here that turns the phrase above into concrete actions
	}
	@Then("The Violation Ticket should be Successfully Submitted")
	public void the_violation_ticket_should_be_successfully_submitted() {
	    // Write code here that turns the phrase above into concrete actions
		driver.quit();
	}
}
