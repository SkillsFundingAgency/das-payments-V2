Feature: Period End
	To allow Payments System Administrators and other interested stake-holders to know the current status of the payments infrastructure 
	the Payments Job Monitoring records the completion status of jobs.

Background:
	Given the payments are for the current collection year
	And the current collection period is R01

Scenario: Period End Started Job
	When the period end service is notified the the period end has started
	Then the period end service should publish a period end started event