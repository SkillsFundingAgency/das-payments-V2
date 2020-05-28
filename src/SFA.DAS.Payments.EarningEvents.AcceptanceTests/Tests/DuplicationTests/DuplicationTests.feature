Feature: Earnings Events Service prevents the creation of duplicate earning events


Scenario: Duplicate Process Learner Command only generates one set of earning events 
	When a process learner command is handled by the process learner service 
	And the event is duplicated
	Then the duplicate event is ignored
	And only one set of earning events is generated for the learner 
	
Scenario: Duplicate Process Learner Command with different CommandId only generates one set of earning events
	When a process learner command is handled by the process learner service 
	And the event is duplicated but with a different commandId on the process learner command
	Then the duplicate event is ignored
	And only one set of earning events is generated for the learner 

Scenario: Process Learner Command with same uln but different ukprn generated earnings for each learner
	When a process learner command is handled by the process learner service 
	And the same learner is submitted but with a different ukprn
	Then two sets of earning events is generated for each learner 