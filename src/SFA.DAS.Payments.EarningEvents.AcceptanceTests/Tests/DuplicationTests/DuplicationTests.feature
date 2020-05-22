Feature: Earnings Events Prevents the creation of duplicate earning events


Scenario: Duplicate Process Learner Command Only generates one set of earning events 
	When a process learner command is handled by the process learner service 
	And if the event is duplicated
	Then the duplicate event is ignored
	And only one set of earning events is generated for the learner 
	
