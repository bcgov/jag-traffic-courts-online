Feature: User initiates and submits dispute in BC Traffic Courts online website

Scenario:  User initiates dispute in BC Traffic Courts online website
	Given User has successfully launched the web browser
	When User navigates to BC Traffic Courts online website
	And User clicks on Initiate Dispute option
	And User enters the violation ticket details
	And User signs and clicks on Complete option
	Then The violation ticket should be successfully submitted
	