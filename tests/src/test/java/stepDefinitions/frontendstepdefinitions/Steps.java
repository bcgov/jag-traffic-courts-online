package stepDefinitions.frontendstepdefinitions;

import java.util.concurrent.TimeUnit;
import io.github.bonigarcia.wdm.WebDriverManager;

import org.openqa.selenium.By;
import org.openqa.selenium.PageLoadStrategy;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.chrome.ChromeDriver;
import org.openqa.selenium.chrome.ChromeOptions;
import org.openqa.selenium.interactions.Action;
import org.openqa.selenium.interactions.Actions;

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
        
        driver.manage().timeouts().implicitlyWait(10, TimeUnit.SECONDS);
        driver.manage().timeouts().setScriptTimeout(10, TimeUnit.SECONDS);
        driver.manage().deleteAllCookies();
	}
	
	@When("User navigates to BC Traffic Courts Online Website")
	public void user_navigates_to_bc_traffic_courts_online_website() {
	    driver.get("http://localhost:4200/");
	}
	@When("User clicks on Initiate Dispute Option")
	public void user_clicks_on_initiate_dispute_option() {
	    driver.findElement(By.xpath("//button[@ng-reflect-router-link='/survey/home']")).click();
	}
	@When("User Enters the Violation Ticket Details")
	public void user_enters_violation_ticket_number_plus_time_of_ticket() {
	    // Write code here that turns the phrase above into concrete actions
		driver.findElement(By.xpath("//input[@id='sq_102i']")).sendKeys("ABCD1234");
		driver.findElement(By.xpath("//input[@id='sq_103i']")).sendKeys("01022");
		driver.findElement(By.xpath("//input[@type='button' and @value='Next']")).click();
		
		driver.findElement(By.xpath("//input[@type='radio' and @id='sq_110i_0']")).click();
		
		
		driver.findElement(By.xpath("//input[@type='button' and @value='Next']")).click();
		
		driver.findElement(By.xpath("//input[@type='radio' and @id='sq_112i_2']")).click();
		driver.findElement(By.xpath("//input[@type='button' and @value='Next']")).click();
		driver.findElement(By.xpath("//input[@type='radio' and @id='sq_118i_2']")).click();
		driver.findElement(By.xpath("//input[@type='button' and @value='Next']")).click();
		
		driver.findElement(By.xpath("//input[@type='radio' and @id='sq_130i_0']")).click();
		driver.findElement(By.xpath("//input[@type='radio' and @id='sq_131i_1']")).click();
		driver.findElement(By.xpath("//input[@type='radio' and @id='sq_133i_0']")).click();
		
		
		driver.findElement(By.xpath("//input[@type='button' and @value='Next']")).click();
		
	}
	@When("User Signs and Clicks on Complete Option")
	public void user_signs_and_clicks_on_complete_option() {
	    // Write code here that turns the phrase above into concrete actions
		
		WebElement canvasElement = driver.findElement(By.xpath("//canvas[@tabindex='0']"));

	    Actions builder = new Actions(driver);
	    Action signature = builder.contextClick(canvasElement) //start points x axis and y axis. 
	              .clickAndHold()
                  .clickAndHold()
                  .moveToElement(canvasElement,20,-50)
                  .moveByOffset(50, 50)
                  .moveByOffset(80,-50)
                  .moveByOffset(100,50)
                  .release(canvasElement)
                   .build();
	    signature.perform();
	    
		
	    driver.findElement(By.xpath("//input[@type='button' and @value='Complete']")).click();
	}
	@Then("The Violation Ticket should be Successfully Submitted")
	public void the_violation_ticket_should_be_successfully_submitted() {
	    // Write code here that turns the phrase above into concrete actions
		
		driver.quit();
	}
	
	private void sleep() {
		try {
			Thread.sleep(2000);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
}
