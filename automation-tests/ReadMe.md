Test Automation Project
The test automation is a subproject of the main jag-traffic-courts-online project. It provides a series of end to end tests which are run against the active TCO once deployed to OpenShift.

Usage
Clone the https://github.com/bcgov/jag-traffic-courts-online repo and load the AutomationTest project individually into InteliJ IDEA of another IDE of your choosing. A single Maven command is used to execute the series of Automation tests from your IDE.

Before this may be executed, environmental variables must be set:

Environment variables for common tests	Value
'USERNAME_APP'	userWithAllRoles
'PASSWORD_APP'	PasswordForUserWithAllRoles

-----
CURRENLY CAN BE IGNORED:
Environment variables for roles tests	Value
'USERNAME_APP1'	userWithVtcStaffRole
'PASSWORD_APP1'	PasswordForUserWithVtcStaffRole
'USERNAME_APP2'	userWithAdminVtcStaffRole
'PASSWORD_APP2'	PasswordForUserWithAdminVtcStaffRole
'USERNAME_APP3'	userWithJJRole
'USERNAME_APP3'	userWithJJRole
'PASSWORD_APP4'	userWithAdminJJRole
'PASSWORD_APP4'	PasswordForUserWithAdminJJRole
'PASSWORD_APP5'	PasswordForUserWithSupportStaffRole
'PASSWORD_APP5'	PasswordForUserWithSupportStaffRole
-----

Run the tests with command:
java -jar pathToJar "args", in args you should specify what kind of tests should be run: image,eticket,regression.
Add "true" value for tests that you want to run, "false is optional".

Command which will run one Eticket test:
java -jar target/TrafficCourtVirtualization-0.0.1-SNAPSHOT-test-jar-with-dependencies.jar "eticket=true"

Command which will run one Image ticket and Eticket tests:
java -jar target/TrafficCourtVirtualization-0.0.1-SNAPSHOT-test-jar-with-dependencies.jar "image=true,eticket=true,regression=false"

Command which will run full regression scope:
java -jar target/TrafficCourtVirtualization-0.0.1-SNAPSHOT-test-jar-with-dependencies.jar "regression=true"

