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
	Given the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 15000                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
	And the following earnings had been generated for the learner
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 1000         | 0          | 0         |
		| Sep/Current Academic Year | 1000         | 0          | 0         |
		| Oct/Current Academic Year | 1000         | 0          | 0         |
		| Nov/Current Academic Year | 1000         | 0          | 0         |
		| Dec/Current Academic Year | 1000         | 0          | 0         |
		| Jan/Current Academic Year | 1000         | 0          | 0         |
		| Feb/Current Academic Year | 1000         | 0          | 0         |
		| Mar/Current Academic Year | 1000         | 0          | 0         |
		| Apr/Current Academic Year | 1000         | 0          | 0         |
		| May/Current Academic Year | 1000         | 0          | 0         |
		| Jun/Current Academic Year | 1000         | 0          | 0         |
		| Jul/Current Academic Year | 1000         | 0          | 0         |
	And the following provider payments had been generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer   |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         | employer 1 |
		| R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         | employer 1 |
		| R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         | employer 1 |
	But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
		| 01/Aug/Current Academic Year | 12 months        | 15000                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 
	# additional residual fields
	And price details as follows
		| Price details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage |
		| 1st price details | 15000                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         |
		| 2nd price details | 15000                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          | 5625                    | 01/Nov/Current Academic Year           | 0                         | 01/Nov/Current Academic Year             | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 1000         | 0          | 0         |
		| Sep/Current Academic Year | 1000         | 0          | 0         |
		| Oct/Current Academic Year | 1000         | 0          | 0         |
		| Nov/Current Academic Year | 500          | 0          | 0         |
		| Dec/Current Academic Year | 500          | 0          | 0         |
		| Jan/Current Academic Year | 500          | 0          | 0         |
		| Feb/Current Academic Year | 500          | 0          | 0         |
		| Mar/Current Academic Year | 500          | 0          | 0         |
		| Apr/Current Academic Year | 500          | 0          | 0         |
		| May/Current Academic Year | 500          | 0          | 0         |
		| Jun/Current Academic Year | 500          | 0          | 0         |
		| Jul/Current Academic Year | 500          | 0          | 0         |
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
