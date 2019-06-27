@supports_dc_e2e
Feature: Change employer at end of month PV2-381
	As a provider,
	I want earnings and payments for a non-levy learner, and there is a change to the Negotiated Cost which happens at the end of the month, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.

#Feature: Non-levy learner changes employer with change to negotiated price at the end of month
#And the employment status in the ILR is
#| Employer   | Employment Status  | Employment Status Applies    | Small Employer |
#| employer 1 | in paid employment | 01/Aug/Current Academic Year |                |
#| employer 2 | in paid employment | 01/Nov/Current Academic Year |                |

# Differences from 255 
# "price details as follows" has additional residual fields

Scenario Outline: Non-levy learner changes employer with change to negotiated price at the end of month PV2-381
	Given the following learners
		| Learner Reference Number |
		| abc123                   |
	And the following aims
		| Aim Type  | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                               | SFA Contribution Percentage |
		| Programme | 06/Aug/Current Academic Year | 12 months        | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 90%                         |
	And price details as follows
		| Price Episode Id | Aim Sequence Number | Contract Type | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage |
		| pe-1             | 1                   | Act2          | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         |
	And the employment status in the ILR is
		| Employer   | Employment Status  | Employment Status Applies    |
		| employer 1 | in paid employment | 01/Aug/Current Academic Year |
	And the following earnings had been generated for the learner
		| Delivery Period           | On-Programme | Completion | Balancing | Aim Sequence Number | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Sep/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Oct/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Nov/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Dec/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Jan/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Feb/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Mar/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Apr/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| May/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Jun/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Jul/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
	And the following provider payments had been generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer   |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         | employer 1 |
		| R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         | employer 1 |
		| R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         | employer 1 |
	But aims details are changed as follows
		| Aim Type  | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                               |
		| Programme | 06/Aug/Current Academic Year | 12 months        | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 
	# additional residual fields
	And price details as follows
		| Price Episode Id | Aim Sequence Number | Contract Type | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage |
		| pe-1             | 1                   | Act2          | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         |
		| pe-2             | 1                   | Act2          | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | 5625                    | 01/Nov/Current Academic Year           | 0                         | 01/Nov/Current Academic Year             | 90%                         |
	And the employment status in the ILR is now
		| Employer   | Employment Status  | Employment Status Applies    |
		| employer 1 | in paid employment | 01/Aug/Current Academic Year |
		| employer 2 | in paid employment | 01/Nov/Current Academic Year |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Aim Sequence Number | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Sep/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Oct/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
		| Nov/Current Academic Year | 500          | 0          | 0         | 1                   | pe-2                     |
		| Dec/Current Academic Year | 500          | 0          | 0         | 1                   | pe-2                     |
		| Jan/Current Academic Year | 500          | 0          | 0         | 1                   | pe-2                     |
		| Feb/Current Academic Year | 500          | 0          | 0         | 1                   | pe-2                     |
		| Mar/Current Academic Year | 500          | 0          | 0         | 1                   | pe-2                     |
		| Apr/Current Academic Year | 500          | 0          | 0         | 1                   | pe-2                     |
		| May/Current Academic Year | 500          | 0          | 0         | 1                   | pe-2                     |
		| Jun/Current Academic Year | 500          | 0          | 0         | 1                   | pe-2                     |
		| Jul/Current Academic Year | 500          | 0          | 0         | 1                   | pe-2                     |
	And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R04/Current Academic Year | Nov/Current Academic Year | 500          | 0          | 0         |
		| R05/Current Academic Year | Dec/Current Academic Year | 500          | 0          | 0         |
		| R06/Current Academic Year | Jan/Current Academic Year | 500          | 0          | 0         |
		| R07/Current Academic Year | Feb/Current Academic Year | 500          | 0          | 0         |
		| R08/Current Academic Year | Mar/Current Academic Year | 500          | 0          | 0         |
		| R09/Current Academic Year | Apr/Current Academic Year | 500          | 0          | 0         |
		| R10/Current Academic Year | May/Current Academic Year | 500          | 0          | 0         |
		| R11/Current Academic Year | Jun/Current Academic Year | 500          | 0          | 0         |
		| R12/Current Academic Year | Jul/Current Academic Year | 500          | 0          | 0         |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | Learning         |
		| R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | Learning         |
		| R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | Learning         |
		| R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | Learning         |
		| R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | Learning         |
		| R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | Learning         |
		| R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | Learning         |
		| R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | Learning         |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | Learning         |
		| R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | Learning         |
		| R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | Learning         |
		| R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | Learning         |
		| R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | Learning         |
		| R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | Learning         |
		| R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | Learning         |
		| R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | Learning         |
	Examples:
		| Collection_Period         |
		| R04/Current Academic Year |
		| R05/Current Academic Year |
		| R06/Current Academic Year |
		| R07/Current Academic Year |
		| R08/Current Academic Year |
		| R09/Current Academic Year |
		| R10/Current Academic Year |
		| R11/Current Academic Year |
		| R12/Current Academic Year |
