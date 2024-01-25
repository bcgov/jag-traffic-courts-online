package ca.bc.gov.open.ui;

import org.junit.runner.JUnitCore;
import org.junit.runner.Result;
import org.junit.runner.notification.Failure;

public class JUnitRunner {
	public static void main(String[] args) {
		Result result;

		Boolean genericResponse = true;

		// Process the command-line arguments
		for (String arg : args) {
			String[] pairs = arg.split(",");
			for (String pair : pairs) {
				String[] keyValue = pair.split("=");
				if (keyValue.length == 2) {
					String key = keyValue[0].trim();
					Boolean value = Boolean.valueOf(keyValue[1].trim());
					System.out.println("Key: " + key + ", Value: " + value);

					switch (key) {
						case "image":
							if (value) {
								result = JUnitCore.runClasses(SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndSubmitARCandCancel.class);
								printResults(result, "SubmitToStaffWorkbenchUploadPNGNoEmailValidateAndSubmitARCandCancel");
								if (!result.wasSuccessful()) {
									genericResponse = false;
								}
							} else {
								System.out.println("Image ticket skipped");
							}
							break;

						case "eticket":
							if (value) {
								result = JUnitCore.runClasses(SubmitToStaffWorkbenchReject.class);
								printResults(result, "SubmitToStaffWorkbenchReject");
								if (!result.wasSuccessful()) {
									genericResponse = false;
								}
							} else {
								System.out.println("Eticket skipped");
							}
							break;

						case "regression":
							if (value) {
								result = JUnitCore.runClasses(SubmitToStaffWorkbenchReject.class);
								printResults(result, "SubmitToStaffWorkbenchReject");
								if (!result.wasSuccessful()) {
									genericResponse = false;
								}
							} else {
								System.out.println("Regression skipped");
							}
							break;
					}
				}
			}
		}

		//Set the exist code based on all tests.
		if (genericResponse)
			System.exit(0);  // good
		else
			System.exit(1);  // fail
	}

	private static void printResults(Result result, String testName) {
		for (Failure failure : result.getFailures()) {
			System.out.println(failure.toString());
		}
		System.out.println("'" + testName + "' Test successful: " + result.wasSuccessful());
	}
}
