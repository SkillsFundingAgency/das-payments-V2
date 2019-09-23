﻿Feature: Jobs
	To allow Payments System Administrators and other interested stake-holders to know the current status of the payments infrastructure 
	the Payments Job Monitoring records the completion status of jobs.

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
#	
#Scenario: Provider Earnings Job Finished With Errors
#	Given a provider earnings job has already been recorded		
#	When the final messages for the job are failed to be processed
#	Then the job monitoring service should update the status of the job to show that it has completed with errors
#		
#Scenario: Period End Started Job
#	Given the period end service has received a period end start job
#	When the earnings event service notifies the job monitoring service to record the job
#	Then the job monitoring service should record the job
#	And the job monitoring service should also record the period end job messages
#
#Scenario: Period End Running Job
#	Given the period end service has received a period end run job
#	When the earnings event service notifies the job monitoring service to record the job
#	Then the job monitoring service should record the job
#	And the job monitoring service should also record the period end job messages
#
#Scenario: Period End Stopped Job
#	Given the period end service has received a period end stop job
#	When the earnings event service notifies the job monitoring service to record the job
#	Then the job monitoring service should record the job
#	And the job monitoring service should also record the period end job messages
