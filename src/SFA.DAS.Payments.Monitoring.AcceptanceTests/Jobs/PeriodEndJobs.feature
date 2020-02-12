Feature: PeriodEndJobs
	To allow Payments System Administrators and other interested stake-holders to know the current status of the payments infrastructure 
	the Payments Job Monitoring records the completion status of period end jobs.

Background:
	Given the payments are for the current collection year
	And the current collection period is R01

Scenario: Provider Period End Start Job Started
	Given the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	Then the job monitoring service should record the job

Scenario: Provider Period End Start Job Completed
	Given the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	And the final messages for the job are successfully processed
	Then the job monitoring service should update the status of the job to show that it has completed	
	And the monitoring service should notify other services that the start job has completed successfully


Scenario: Provider Period End  Run Job Started
	Given the period end service has received a period end run job
	When the period end service notifies the job monitoring service to record run job
	Then the job monitoring service should record the job

Scenario: Provider Period End Run  Job Completed
	Given the period end service has received a period end run job
	When the period end service notifies the job monitoring service to record run job
	And the final messages for the job are successfully processed
	Then the job monitoring service should update the status of the job to show that it has completed	
	And the monitoring service should notify other services that the run job has completed successfully


Scenario: Provider Period End  Stop Job Started
	Given the period end service has received a period end stop job
	When the period end service notifies the job monitoring service to record stop job
	Then the job monitoring service should record the job

Scenario: Provider Period End Stop  Job Completed
	Given the period end service has received a period end stop job
	When the period end service notifies the job monitoring service to record stop job
	And the final messages for the job are successfully processed
	Then the job monitoring service should update the status of the job to show that it has completed	
	And the monitoring service should notify other services that the stop job has completed successfully


#emulate steps below for period end messages

#Scenario: Data-Collections confirms failure of Provider Earnings Job
#	Given the monitoring service has recorded the completion of an earnings job
#	When Data-Collections confirms the failure of the job
#	Then the monitoring service should record the failure of the Data-Collections processes
#	And the monitoring service should notify other services that the job has failed
#
##Scenario: Monitoring service records the completion time of data-locks processing
##	Given the earnings event service has received a provider earnings job
##	When the earnings event service notifies the job monitoring service to record the job
##	And the earnig event service processes the learners in the ILR
##	And the data-locks service processes the ACT1 Earning events
##	Then the job monitoring service should have recorded the completion time of the data-locks processing
#
#	
#Scenario: Large Provider Earnings Job Finished
#	Given the earnings event service has received a large provider earnings job
#	When the earnings event service notifies the job monitoring service to record the job
#	And the final messages for the job are successfully processed
#	#Then the job monitoring service should update the status of the job to show that it has completed
	

