package stepDefinitions;

import java.util.concurrent.TimeUnit;

import org.openqa.selenium.By;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;

import io.cucumber.java.en.Given;
import io.cucumber.java.en.Then;
import io.cucumber.java.en.When;

public class Steps {
	WebDriver driver;
	
	@Given("User had successfully launched the web browser")
	public void user_had_successfully_launched_the_web_browser() {
	    // Write code here that turns the phrase above into concrete actions
        System.setProperty("webdriver.chrome.driver","C:\\chromedriver_win32\\chromedriver.exe");
        driver = new ChromeDriver();
        driver.manage().window().maximize();
        driver.manage().timeouts().implicitlyWait(10, TimeUnit.SECONDS);
        
	}
	
	@When("User navigates to Google.com")
	public void user_navigates_to_google_com() {
	    // Write code here that turns the phrase above into concrete actions
		driver.get("https://www.google.com");
	}
	
	@Then("User searches for BC Traffic Courts")
	public void user_searches_for_bc_traffic_courts() {
	    // Write code here that turns the phrase above into concrete actions
		WebElement searchBar = driver.findElement(By.name("q"));
		searchBar.sendKeys("BC Traffic Courts");
		
        WebElement searchButton = driver.findElement(By.name("btnK"));
        searchButton.click();
	}
	
	@Then("Expected Search Results are Displayed")
	public void expected_search_results_are_displayed() {
	    // Write code here that turns the phrase above into concrete actions
		driver.quit();
	}



}
