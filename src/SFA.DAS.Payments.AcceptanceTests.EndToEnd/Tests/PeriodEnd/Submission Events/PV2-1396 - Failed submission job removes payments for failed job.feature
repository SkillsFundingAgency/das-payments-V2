Feature: Failed submission removes submission data - PV2-1396
	As a provider,
	I would like my payments and reports to to be based on my most recent successful ILR submission

Scenario: Failed submission removes submission data - PV2-1395
	Given the provider has already submitted an ILR in the collection period
	When the amended ILR file is re-submitted
	And the payments service has notified Data-Collections that the Data-Locks process has finished
	But the payments service is notified that the subsequent Data-Collections processes failed to process the job
	And the Payments service records the completion of the job
	Then the payments for the current submission should be removed