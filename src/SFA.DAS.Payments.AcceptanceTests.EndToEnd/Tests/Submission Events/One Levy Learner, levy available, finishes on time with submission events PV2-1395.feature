@ignore
Feature: Job submission succeeded 
	As a provider,
	I would like my payments and reports to to be based on my most recent successful ILR submission

Scenario: Successful submission removes previous submission data - PV2-1395
	Given the provider has already submitted an ILR in the collection period
	When the amended ILR file is re-submitted
	And the Payments service records the completion of the job
	And the Data-Collections system confirms successful completion of processing the job
	Then the data for the previous submission should be removed