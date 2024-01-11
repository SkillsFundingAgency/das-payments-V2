Feature: Period End Start Job
	To enable deterministic and repeatable payment calculations,
	the reference data used by the payment system should not change during the period end pay run.

Background:
	Given the payments are for the current collection year
	And the current collection period is R01

Scenario: Provider Period End Start Job Completed
	Given the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	And the submission summary metrics are recorded
	And the final messages for the job are successfully processed
	Then the job monitoring service should update the status of the job to show that it has completed	
	And the monitoring service should notify other services that the period end start job has completed successfully