package runners;

import org.junit.runner.RunWith;
import io.cucumber.junit.Cucumber;
import io.cucumber.junit.CucumberOptions;
 
@RunWith(Cucumber.class)
@CucumberOptions(
	 features = {"src/test/resources/frontend","src/test/resources/backend"},
	 glue= {"stepDefinitions.backendstepdefinitions","stepDefinitions.frontendstepdefinitions"},
	 publish = true
 )
public class TestRunner {
}
