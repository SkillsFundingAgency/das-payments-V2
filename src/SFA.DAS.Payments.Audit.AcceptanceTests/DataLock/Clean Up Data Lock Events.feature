Feature: Clean Up Data Lock Events On Submission Failure
	In order to generate data match report
	I want to the Payments V2 service to clear the failed submission

Scenario: Submission Fails and Data Lock Events Cleared
	Given the data lock service has generated the following events
	| Submission Time | Learner |
	| 2019-09-18      | a       |
	| 2019-09-18      | b       |
	| 2019-09-19      | c       |
	| 2019-09-19      | d       |
	| 2019-09-20      | e       |
	When submission failed event for ILR submitted at '2019-09-19' arrives
	Then only the following events stay in database
	| Submission Time | Learner |
	| 2019-09-18      | a       |
	| 2019-09-18      | b       |
	| 2019-09-20      | e       |
