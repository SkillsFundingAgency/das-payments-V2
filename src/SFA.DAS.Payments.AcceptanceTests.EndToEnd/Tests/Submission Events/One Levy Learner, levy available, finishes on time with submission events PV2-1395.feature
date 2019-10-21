@ignore
Feature: Job submission succeeded 
	As a provider,
	I would like my payments and reports to to be based on my most recent successful ILR submission

Scenario: Successful submission removes previous submission data - PV2-1395
	Given the provider has already submitted an ILR in the collection period
	#Given the provider is providing training for the following learners
	#	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                               | SFA Contribution Percentage |
	#	| 03/Aug/Current Academic Year | 12 months        | 15000                | 03/Aug/Current Academic Year        |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 90%                         |
	#And price details as follows
	#	| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
	#	| pe-1             | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act2          | 1                   |
	#And the ILR file is submitted for the learners for collection period R01/Current Academic Year
	#And the learner earnings were generated
	When the amended ILR file is re-submitted
	#And the learner earnings are generated
	And the Payments service records the completion of the job
	And the Data-Collections system confirms successful completion of processing the job
	Then the data for the previous submission should be removed