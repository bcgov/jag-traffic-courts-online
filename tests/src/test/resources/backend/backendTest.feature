Feature: API Test for Traffic Courts Online

Scenario:  User is able to get ticket details from API
	Given A list of tickets are available.
	When User makes a Request to get the List of Tickets
	Then The Request is successfully processed