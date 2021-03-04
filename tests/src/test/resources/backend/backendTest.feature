Feature: API Test for Traffic Courts Online

Scenario:  User is able to get ticket details from API
	Given A list of tickets are available.
	When User makes a request to get the list of tickets
	Then The request is successfully processed
