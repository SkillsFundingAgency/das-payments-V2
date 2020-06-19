Feature: Required Payments Service prevents the creation of duplicate removed learner events


Scenario: Duplicate Removed Learner Aim Event only generates one set of results
	Given a learner has a payment from a previous submission
	When an idenitfied removed learning aim event is handled by the required payments service 
	And the event is duplicated
	Then only one set of events is generated for the learner
	And after waiting a second
	And there is only a single event produced
	
