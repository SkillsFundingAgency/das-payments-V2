Feature: Period End Submission Window Validation
	As a Payments Administrator
	I would like to know that all submissions have been processed correctly
	So that it is safe to perform the calculation of the actual payments for all Apprenticeship training providers

Background:
	Given the payments are for the current collection year
	And the current collection period is R01

@ignore 
Scenario: Provider Validate Submission Window Job Completed
	Given the period end service has received a Validate Submission Window Job
	When the period end service notifies the job monitoring service to record the Validate Submission Window job
	And the submission summary metrics are recorded
	And the final messages for the job are successfully processed
	Then the job monitoring service should update the status of the job to show that it has completed
