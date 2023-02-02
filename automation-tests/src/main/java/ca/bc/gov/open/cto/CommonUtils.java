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
			driver.get("");
			driver.navigate().to("");
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
