Feature: PV2-1583 - 1 Non-Levy learner, paid for via transfer, on ACT1 contract paid through DAS
Levy-paying employers are able to transfer up to 10% of the annual value of funds in their Levy account to any employer, including for Non-Levy Learners. 
Non-Levy learners funded by a transfer do not get Data Locked with a DLOCK_11 when getting paid through DAS.

Scenario Outline: Transfers - PV2-1583 - 1 Non-Levy learner, paid for via transfer, on ACT1 contract paid through DAS
	Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
	And the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the remaining transfer allowance for "employer 2" is <Employer 2 Remaining Transfer Allowance>
	Given the "employer 1" IsLevyPayer flag is false
	And the following apprenticeships exist
		| Employer   | Sending Employer | Start Date                   | End Date                  | Agreed Price | Standard Code | Programme Type | Status | Effective From               | Non Levy Flag |
		| employer 1 | employer 2       | 01/May/Current Academic Year | 06/May/Next Academic Year | 15000        | 50            | 25             | active | 01/May/Current Academic Year | Non Levy      |
	And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/May/Current Academic Year | 12 months        | 15000                | 06/May/Current Academic Year        | 0                      | 06/May/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 50            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 15000                | 01/May/Current Academic Year        | 0                      | 01/May/Current Academic Year          | 0                       |                                        | 0                         |                                          | Act1          | 1                   | 90%                         |
	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
	And a DLOCK_11 is not flagged
	And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | Levy Payments | Transfer Payments | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer   | Sending Employer |
		| R10/Current Academic Year | May/Current Academic Year | 1000          | 0                 | 0                      | 0                           | Learning         | employer 1 | employer 2       |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000          | 0                 | 0                      | 0                           | Learning         | employer 1 | employer 2       |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000          | 0                 | 0                      | 0                           | Learning         | employer 1 | employer 2       |

	Examples:
		| Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 | Employer 2 Remaining Transfer Allowance |
		| R10/Current Academic Year | 4000                        | 60000                       | 2000                                    |
		| R11/Current Academic Year | 4000                        | 59600                       | 2000                                    |
		| R12/Current Academic Year | 4000                        | 59200                       | 2000                                    |