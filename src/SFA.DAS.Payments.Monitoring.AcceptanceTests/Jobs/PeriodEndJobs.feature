Feature: PeriodEndJobs
	To allow Payments System Administrators and other interested stake-holders to know the current status of the payments infrastructure 
	the Payments Job Monitoring records the completion status of period end jobs.

Background:
	Given the payments are for the current collection year
	And the current collection period is R01

Scenario: Provider Earnings Job Started
	Given the earnings event service has received a provider earnings job
	When the earnings event service notifies the job monitoring service to record the job
	Then the job monitoring service should record the job

Scenario: Provider Earnings Job Finished
	Given the earnings event service has received a provider earnings job
	When the earnings event service notifies the job monitoring service to record the job
	And the final messages for the job are successfully processed
	Then the job monitoring service should update the status of the job to show that it has completed	

Scenario: Data-Collections confirms success of Provider Earnings Job
	Given the monitoring service has recorded the completion of an earnings job
	When Data-Collections confirms the successful completion of the job
	Then the monitoring service should record the successful completion of the Data-Collections processes
	And the monitoring service should notify other services that the job has completed successfully


Scenario: Data-Collections confirms failure of Provider Earnings Job
	Given the monitoring service has recorded the completion of an earnings job
	When Data-Collections confirms the failure of the job
	Then the monitoring service should record the failure of the Data-Collections processes
	And the monitoring service should notify other services that the job has failed

#Scenario: Monitoring service records the completion time of data-locks processing
#	Given the earnings event service has received a provider earnings job
#	When the earnings event service notifies the job monitoring service to record the job
#	And the earnig event service processes the learners in the ILR
#	And the data-locks service processes the ACT1 Earning events
#	Then the job monitoring service should have recorded the completion time of the data-locks processing

	
Scenario: Large Provider Earnings Job Finished
	Given the earnings event service has received a large provider earnings job
	When the earnings event service notifies the job monitoring service to record the job
	And the final messages for the job are successfully processed
	#Then the job monitoring service should update the status of the job to show that it has completed
	

