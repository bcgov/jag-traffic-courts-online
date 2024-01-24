package ca.bc.gov.open.ui;
import org.junit.runner.JUnitCore;
import org.junit.runner.Result;
import org.junit.runner.notification.Failure;

public class JUnitRunner {

	public static void main(String[] args) {
		Result result = JUnitCore.runClasses(TestExample.class);

		for (Failure failure : result.getFailures()) {
			System.out.println(failure.toString());
		}

		System.out.println("Tests successful: " + result.wasSuccessful());
	}
}
