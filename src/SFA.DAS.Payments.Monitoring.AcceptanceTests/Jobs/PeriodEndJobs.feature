Feature: PeriodEndJobs
	To allow Payments System Administrators and other interested stake-holders to know the current status of the payments infrastructure 
	the Payments Job Monitoring records the completion status of period end jobs.

Background:
	Given the payments are for the current collection year
	And the current collection period is R01



Scenario: Provider Period End Start Job Completed
	Given the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	And the final messages for the job are successfully processed
	Then the job monitoring service should update the status of the job to show that it has completed	
	And the monitoring service should notify other services that the period end start job has completed successfully

Scenario: Provider Period End Run  Job Completed
	Given the period end service has received a period end run job
	When the period end service notifies the job monitoring service to record run job
	And the final messages for the job are successfully processed
	Then the job monitoring service should update the status of the job to show that it has completed	
	And the monitoring service should notify other services that the period end run job has completed successfully

Scenario: Provider Period End Stop  Job Completed
	Given the period end service has received a period end stop job
	When the period end service notifies the job monitoring service to record stop job
	And the final messages for the job are successfully processed
	Then the job monitoring service should update the status of the job to show that it has completed	
	And the monitoring service should notify other services that the period end stop job has completed successfully