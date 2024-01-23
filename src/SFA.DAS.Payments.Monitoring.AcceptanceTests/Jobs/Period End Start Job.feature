Feature: Period End Start Job
	To enable deterministic and repeatable payment calculations,
	the reference data used by the payment system should not change during the period end pay run.

Background:
	Given the payments are for the current collection year
	And the current collection period is R01

Scenario: Records successful completion when the Approvals Reference Data Service is disbaled
	Given tha collection window has now been closed and period end has started
	And the monitoring service has recorded that the period end start job has started
	When the Approvals Reference Data Service is disabled
	Then the job monitoring service should update the status of the job to show that it has completed	
	And the monitoring service should notify other services that the period end start job has completed successfully
	