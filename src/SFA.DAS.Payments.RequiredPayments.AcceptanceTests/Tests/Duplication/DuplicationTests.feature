Feature: Required Payments Service prevents the creation of duplicate removed learner events


Scenario: Duplicate Removed Learner Aim Event only generates one set of results
	Given a learner has a payment from a previous submission
	When an idenitfied removed learning aim event is handled by the required payments service 
	And the event is duplicated
	Then the duplicate event is ignored
	And only one set of earning events is generated for the learner 
	
