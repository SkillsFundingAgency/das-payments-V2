@ignore - at the moment, there is an issue in the 1395 branch with the first message returning and hence the second one doesn't get consumed.
Feature: Job submission succeeded 
	As a provider,
	I would like my payments and reports to to be based on my most recent successful ILR submission

Scenario: Successful submission removes previous submission data - PV2-1395
	Given the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                               | SFA Contribution Percentage |
		| 03/Aug/Current Academic Year | 12 months        | 15000                | 03/Aug/Current Academic Year        |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 90%                         |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| pe-1             | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act2          | 1                   |
	And the ILR file is submitted for the learners for collection period <Collection_Period>
	And the following learner earnings were generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
	When the amended ILR file is re-submitted for the learners in collection period <collection_period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |

	Examples:
		| Collection_Period         |
		| R01/Current Academic Year |