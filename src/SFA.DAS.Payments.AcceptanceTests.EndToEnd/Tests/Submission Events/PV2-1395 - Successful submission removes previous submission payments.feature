Feature: Successful submission removes previous submission payments - PV2-1395
	As a provider,
	I would like my payments and reports to to be based on my most recent successful ILR submission

Scenario: Successful submission removes previous submission payments - PV2-1395
	Given the provider has already submitted an ILR in the collection period
	When the amended ILR file is re-submitted
	And the payments service has notified Data-Collections that the Data-Locks process has finished
	And the Data-Collections system confirms successful completion of processing the job
	And the Payments service records the completion of the job
	Then the payments for the previous submission should be removed