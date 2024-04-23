package ca.bc.gov.open.cto;

import java.util.logging.Logger;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;

public class CommonUtils {

	private static Logger log = Logger.getLogger("CommonUtils.class");

	public static void login() throws Exception {

		WebDriver driver = CustomWebDriverManager.getDriver();
		WebElement element = CustomWebDriverManager.getElement();

		if (Config.ENVIROMENT.equals(Constants.DEV)) {
			driver.get("https://dev.tickets.gov.bc.ca");
			driver.navigate().to("https://dev.tickets.gov.bc.ca");
			driver.navigate().refresh();

		} else if (Config.ENVIROMENT.equals(Constants.TST)) {
			driver.get("https://test.tickets.gov.bc.ca");
			driver.navigate().to("https://test.tickets.gov.bc.ca");
			driver.navigate().refresh();

		}
	}

	public static void loginStaffWorkbench() throws Exception {

		WebDriver driver = CustomWebDriverManager.getDriver();
		WebElement element = CustomWebDriverManager.getElement();

		if (Config.ENVIROMENT.equals(Constants.DEV)) {
			driver.get("https://oidc-0198bb-dev.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Fstaff-web-0198bb-dev.apps.silver.devops.gov.bc.ca%2Fticket&state=bcb55b04-5956-403d-8bf5-0273f34e5cdb&response_mode=fragment&response_type=code&scope=openid&nonce=5b0a9ef9-c066-47e7-8cf7-5b7dfd7f5f24");
			driver.navigate().to("https://oidc-0198bb-dev.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Fstaff-web-0198bb-dev.apps.silver.devops.gov.bc.ca%2Fticket&state=bcb55b04-5956-403d-8bf5-0273f34e5cdb&response_mode=fragment&response_type=code&scope=openid&nonce=5b0a9ef9-c066-47e7-8cf7-5b7dfd7f5f24");
			driver.navigate().refresh();

		} else if (Config.ENVIROMENT.equals(Constants.TST)) {
			driver.get("https://oidc-0198bb-test.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Ftco.test.jag.gov.bc.ca%2Ftco&state=7ee467b8-8215-416b-8fa0-23c61fd5a2e4&response_mode=fragment&response_type=code&scope=openid&nonce=242b4a6b-0af2-4a6a-8aae-783a02bb3433");
			driver.navigate().to("https://oidc-0198bb-test.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Ftco.test.jag.gov.bc.ca%2Ftco&state=7ee467b8-8215-416b-8fa0-23c61fd5a2e4&response_mode=fragment&response_type=code&scope=openid&nonce=242b4a6b-0af2-4a6a-8aae-783a02bb3433");
			driver.navigate().refresh();
		}

	}

	public static void loginJJWorkbench() throws Exception {

		WebDriver driver = CustomWebDriverManager.getDriver();
		WebElement element = CustomWebDriverManager.getElement();

		if (Config.ENVIROMENT.equals(Constants.DEV)) {
			driver.get("https://oidc-0198bb-dev.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Ftco.dev.jag.gov.bc.ca%2Fjj&state=f51e38d6-8800-43ba-87ec-6e264f0e89d6&response_mode=fragment&response_type=code&scope=openid&nonce=eb9b2f6e-6a82-4e9e-bba8-1c5e9c9c1b7b");
			driver.navigate().to("https://oidc-0198bb-dev.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Ftco.dev.jag.gov.bc.ca%2Fjj&state=f51e38d6-8800-43ba-87ec-6e264f0e89d6&response_mode=fragment&response_type=code&scope=openid&nonce=eb9b2f6e-6a82-4e9e-bba8-1c5e9c9c1b7b");
			driver.navigate().refresh();

		} else if (Config.ENVIROMENT.equals(Constants.TST)) {
			driver.get("https://oidc-0198bb-test.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Ftco.test.jag.gov.bc.ca%2Fjj&state=af10870f-3636-4ee1-8776-55243d11659e&response_mode=fragment&response_type=code&scope=openid&nonce=8c039e76-fe82-4093-89bc-749082726242");
			driver.navigate().to("https://oidc-0198bb-test.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Ftco.test.jag.gov.bc.ca%2Fjj&state=af10870f-3636-4ee1-8776-55243d11659e&response_mode=fragment&response_type=code&scope=openid&nonce=8c039e76-fe82-4093-89bc-749082726242");
			driver.navigate().refresh();
		}

	}

	// Construct jdbcUrl for Environment
	public static String getJdbcUrl() {
		String jdbcUrl = "";
		if (Config.ENVIROMENT.equals(Constants.DEV)) {
			jdbcUrl = "jdbc:oracle:thin:@devdb.bcgov:1521:devj";
		} else if (Config.ENVIROMENT.equals(Constants.TST)) {
			jdbcUrl = "jdbc:oracle:thin:@testdb.bcgov:1521:tstj";
		}

		String dbName = "";

		if (Config.ENVIROMENT.equals(Constants.DEV)) {
			dbName = "DatabaseName=";
		} else if (Config.ENVIROMENT.equals(Constants.TST)) {
			dbName = "DatabaseName=";
		}

		return jdbcUrl;
	}

	// Construct userName for Environment
	public static String getJdbcUserName() {
		String userName = "";
		if (Config.ENVIROMENT.equals(Constants.DEV)) {
			userName = "jdbcUserName.DEV";
		} else if (Config.ENVIROMENT.equals(Constants.TST)) {
			userName = "jdbcUserName.TST";
		}
		return userName;
	}

	// Construct password for Environment
	public static String getJdbcPassword() {
		String password = "";
		if (Config.ENVIROMENT.equals(Constants.DEV)) {
			password = "jdbcPassword.DEV";
		} else if (Config.ENVIROMENT.equals(Constants.TST)) {
			password = "jdbcPassword.TST";
		}
		return password;
	}

}
