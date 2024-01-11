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

Scenario: Provider Period End Start Job waits for submissions to complete before finishing
	Given the earnings event service has received and is processing a provider earnings job
	And the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	And the submission summary metrics are recorded
	And the final messages for the job are successfully processed for the Period End Start job
	Then the period end job should not complete
	And when the final messages for the job are successfully processed for the submission job
	Then the job monitoring service should update the status of the job to show that it has completed	
	And the monitoring service should notify other services that the period end start job has completed successfully

Scenario: Provider Period End Start Job only waits for latest submissions to complete before finishing
	Given the earnings event service has received and successfully processed a large provider earnings job
	And the earnings event service has received and is processing a provider earnings job
	And the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	And the submission summary metrics are recorded
	And the final messages for the job are successfully processed for the Period End Start job
	Then the period end job should not complete
	And when the final messages for the job are successfully processed for the submission job
	Then the job monitoring service should update the status of the job to show that it has completed	
	And the monitoring service should notify other services that the period end start job has completed successfully

Scenario: Provider Period End Start Job fails if outstanding submissions time out
	Given the earnings event service has received and is processing a provider earnings job
	And the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	And the submission summary metrics are recorded
	And the final messages for the job are successfully processed for the Period End Start job
	And outstanding submission job times out 
	Then the job monitoring service should update the status of the job to show that it has failed	
	And the monitoring service should notify other services that the period end start job has failed

Scenario: Provider Period End Start Job times out if metrics are not complete
	Given the earnings event service has received and successfully processed a provider earnings job
	And the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	And the final messages for the job are successfully processed
	Then the job monitoring service should update the status of the job to show that it has timed out
	And the monitoring service should notify other services that the period end start job has failed
	
Scenario: Provider Period End Start Job fails if latest submissions is taking longer to complete
	Given the earnings event service has received and successfully processed a small provider earnings job
	And the earnings event service has received and is processing a provider earnings job
	And the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	And the submission summary metrics are recorded
	And the final messages for the job are successfully processed for the Period End Start job
	Then the period end job should not complete
	And the job monitoring service should update the status of the job to show that it has failed
	And the monitoring service should notify other services that the period end start job has failed

Scenario: Provider Period End Start Job Completes if latest submissions is taking less time then average to complete
	Given the earnings event service has received and successfully processed a Large provider earnings job
	And the earnings event service has received and is processing a provider earnings job
	And the period end service has received a period end start job
	When the period end service notifies the job monitoring service to record the start job
	And the submission summary metrics are recorded
	And the final messages for the job are successfully processed for the Period End Start job
	Then the period end job should not complete
	And when the final messages for the job are successfully processed for the submission job
	And the job monitoring service should update the status of the job to show that it has completed
	And the monitoring service should notify other services that the period end start job has completed successfully
