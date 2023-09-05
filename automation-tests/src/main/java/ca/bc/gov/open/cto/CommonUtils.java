package ca.bc.gov.open.cto;

import java.util.logging.Logger;

import org.openqa.selenium.WebDriver;
import org.openqa.selenium.WebElement;

public class CommonUtils {

	private static Logger log = Logger.getLogger("CommonUtils.class");

	public static void login() throws Exception {

		WebDriver driver = WebDriverManager.getDriver();
		WebElement element = WebDriverManager.getElement();

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

		WebDriver driver = WebDriverManager.getDriver();
		WebElement element = WebDriverManager.getElement();

		if (Config.ENVIROMENT.equals(Constants.DEV)) {
			driver.get("https://oidc-0198bb-dev.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Fstaff-web-0198bb-dev.apps.silver.devops.gov.bc.ca%2Fticket&state=bcb55b04-5956-403d-8bf5-0273f34e5cdb&response_mode=fragment&response_type=code&scope=openid&nonce=5b0a9ef9-c066-47e7-8cf7-5b7dfd7f5f24");
			driver.navigate().to("https://oidc-0198bb-dev.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Fstaff-web-0198bb-dev.apps.silver.devops.gov.bc.ca%2Fticket&state=bcb55b04-5956-403d-8bf5-0273f34e5cdb&response_mode=fragment&response_type=code&scope=openid&nonce=5b0a9ef9-c066-47e7-8cf7-5b7dfd7f5f24");
			driver.navigate().refresh();

		} else if (Config.ENVIROMENT.equals(Constants.TST)) {
			driver.get("https://oidc-0198bb-test.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Fstaff-web-0198bb-test.apps.silver.devops.gov.bc.ca%2Fticket&state=0a427dd8-7e88-46c2-87fc-83dc05e38821&response_mode=fragment&response_type=code&scope=openid&nonce=cc795bb1-7e20-415c-961c-2b4ac717bb0a");
			driver.navigate().to("https://oidc-0198bb-test.apps.silver.devops.gov.bc.ca/realms/traffic-court/protocol/openid-connect/auth?client_id=staff-portal&redirect_uri=https%3A%2F%2Fstaff-web-0198bb-test.apps.silver.devops.gov.bc.ca%2Fticket&state=0a427dd8-7e88-46c2-87fc-83dc05e38821&response_mode=fragment&response_type=code&scope=openid&nonce=cc795bb1-7e20-415c-961c-2b4ac717bb0a");
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
