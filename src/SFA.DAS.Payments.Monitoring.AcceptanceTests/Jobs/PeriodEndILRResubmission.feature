Feature: Period End ILR Resubmission
	As a Payments Administrator
	I would like to know when the system has finished re-processing all ILRs that were submitted late, 
	have a mismatching success status or were not submitted at all during the collection window that has just closed 
	So that it is safe to perform the calculation of the actual payments for all Apprenticeship training providers

Background:
	Given the payments are for the current collection year
	And the current collection period is R01

Scenario: ILR Reprocessing Job Started
	Given the period end service has received a period end Ilr reprocessing job
	When the period end service notifies the job monitoring service to record the Ilr reprocessing job
	Then the job monitoring service should record the job

Scenario: ILR Reprocessing Records Successful Completion of Earning Jobs
	Given all earnings jobs have finished processing successfully
	When the ILR Reprocessing job is initiated
	Then the job monitoring service should update the status of the job to show that it has completed

Scenario: ILR Reprocessing Notifies Other Services When Completed
	Given all earnings jobs have finished processing successfully
	When the ILR Reprocessing job is initiated
	Then the job monitoring service should update the status of the job to show that it has completed
	And the monitoring service should notify other services that the period end Ilr Reprocessing job has completed successfully

Scenario: ILR Reprocessing Waits For Submissions to Complete
	Given earnings jobs are still being processed
	And the ILR Reprocessing Job has been initiated
	When the earnings job completes processing
	Then the job monitoring service should update the status of the job to show that it has completed

Scenario: ILR Reprocessing Waits For Late Submissions to Complete
	Given the ILR Reprocessing Job has been initiated
	And earnings jobs are still being processed
	When the earnings job completes processing
	Then the job monitoring service should update the status of the job to show that it has completed

Scenario: Monitors Timed-out ILR submissions
	Given the ILR Reprocessing Job has been initiated
	When the earnings job times-out
	Then the job monitoring service should update the status of the job to show that it has failed
