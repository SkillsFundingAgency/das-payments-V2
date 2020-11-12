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

Scenario: Provider Validate Submission Window Job Completed
	Given the period end service has received a Validate Submission Window Job
	When the period end service notifies the job monitoring service to record the Validate Submission Window job
	And the final messages for the job are successfully processed
	Then the job monitoring service should update the status of the job to show that it has completed
	And the monitoring service should notify other services that the Validate Submission Window job has completed successfully

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


Scenario: Provider Period End Start Job waits for submissions to complete before finishing
	Given the earnings event service has received and is processing a provider earnings job
	And the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	And the final messages for the job are successfully processed for the Period End Start job
	Then the period end job should not complete
	And when the final messages for the job are successfully processed for the submission job
	Then the job monitoring service should update the status of the job to show that it has completed	
	And the monitoring service should notify other services that the period end start job has completed successfully

Scenario: Provider Period End Start Job fails if outstanding submissions time out
	Given the earnings event service has received and is processing a provider earnings job
	And the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	And the final messages for the job are successfully processed for the Period End Start job
	And outstanding submission job times out 
	Then the job monitoring service should update the status of the job to show that it has failed	
	And the monitoring service should notify other services that the period end start job has failed