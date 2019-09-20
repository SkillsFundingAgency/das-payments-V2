@basic_day
@supports_dc_e2e
Feature: One Non-Levy Learner Withdraws After Qualifying Period PV2-277


Scenario Outline: A non-levy learner withdraws after qualifying period
	Given the provider is providing training for the following learners
        | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                               | SFA Contribution Percentage |
        | 01/Aug/Current Academic Year | 12 months        | 15000                | 01/Aug/Current Academic Year        |                        |                                       | 4 months        | withdrawn         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 90%                         |
    And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| pe-1             | 15000                | 01/Aug/Current Academic Year        |                        |                                       | 0                       |                                        | 0                         |                                          | 90%                         | Act2          | 1                   |
	When the ILR file is submitted for the learners for collection period <collection_period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    And only the following payments will be calculated
		| Collection Period			| Delivery Period           | On-Programme | Completion | Balancing |
		| R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
		| R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
		| R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         |
		| R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         |
		| R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         |

	Examples:
		| collection_period			|
		| R01/Current Academic Year |
		| R02/Current Academic Year |
		| R03/Current Academic Year |
		| R04/Current Academic Year |
		| R05/Current Academic Year |