Feature: Open a sample website
Description: Test that Google.com is opening successfully

Scenario:  Open a Sample Website
	Given User had successfully launched the web browser
	When User navigates to Google.com
	Then User searches for BC Traffic Courts
	And Expected Search Results are Displayed