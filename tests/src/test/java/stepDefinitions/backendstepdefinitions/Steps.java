package stepDefinitions.backendstepdefinitions;

import org.junit.Assert;

import io.cucumber.java.en.Given;
import io.cucumber.java.en.Then;
import io.cucumber.java.en.When;
import io.restassured.RestAssured;
import io.restassured.response.Response;
import io.restassured.specification.RequestSpecification;

public class Steps {
	private static final String BASE_URL = "http://localhost:8080/";
	private static Response response;
	
	@Given("A list of tickets are available.")
	public void a_list_of_tickets_are_available() {
	}
	@When("User makes a request to get the list of tickets")
	public void user_makes_a_request_to_get_the_list_of_tickets() {
	    // Write code here that turns the phrase above into concrete actions
	}
	@Then("The request is successfully processed")
	public void the_request_is_successfully_processed() {
	    // Write code here that turns the phrase above into concrete actions
	}

}
