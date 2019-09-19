Feature: Clean Up Data Lock Events On Submission Success
	In order to generate data match report
	I want to the Payments V2 service to clear all old submissions

Scenario: Submission Succeeds and Data Lock Events Cleared
	Given the data lock service has generated the following events
	| Submission Time | Learner |
	| 2019-09-18      | a       |
	| 2019-09-18      | b       |
	| 2019-09-19      | c       |
	| 2019-09-19      | d       |
	| 2019-09-20      | e       |
	When submission succeeded event for ILR submitted at '2019-09-20' arrives
	Then only the following events stay in database
	| Submission Time | Learner |
	| 2019-09-20      | e       |
