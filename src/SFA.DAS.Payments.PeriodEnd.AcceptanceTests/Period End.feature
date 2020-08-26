Feature: Period End
	To allow Payments System Administrators and other interested stake-holders to know the current status of the payments infrastructure 
	the Payments Job Monitoring records the completion status of jobs.

Background:
	Given the payments are for the current collection year
	And the current collection period is R01

Scenario: Period End Started Job
	When the period end service is notified the the period end has started
	Then the period end service should publish a period end started event
	And the period end started job is persisted to the database

Scenario: Period End Request Validate Submission Window Job
	When the period end service is notified that a period end request validate submission window job has been requested 
	Then the period end service should publish a period end request validate submission window event

Scenario: Period End Running Job
	When the period end service is notified the the period end is running
	Then the period end service should publish a period end running event
	And the period end running job is persisted to the database

Scenario: Period End Request Reports Job
	When the period end service is notified that period end reports have been requested 
	Then the period end service should publish a period end request reports event

Scenario: Period End Stopped Job
	When the period end service is notified the the period end has stopped
	Then the period end service should publish a period end stopped event
	And the period end stopped job is persisted to the database

Scenario: Period End Running Job Not Published Twice
	When the period end service is notified the the period end is running twice
	Then the period end service should publish a period end running event
	And not publish one for the duplicate notification
	And the period end running job is persisted to the database