Feature: Levy learner changes employer provider retrospectively submitted ILR late bad previous payment History - PV2-2802-2
		As a provider,
		I want earnings and payments for a levy learner, levy available, when there is a change of employer during the programme and ILR is submitted late, to be paid the correct amount
		So that I am accurately paid my apprenticeship provision

Scenario Outline: Levy learner changes employer provider retrospectively submitted ILR late bad previous payment History - PV2-2802-2
	Given the "test employer" levy account balance in collection period <Collection_Period> is <Levy Balance for test employer>
	And the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the following commitments exist
		| Identifier       | Employer      | start date                   | end date                     | agreed price | status  | effective from               | effective to                 | stop effective from          | Standard Code | Programme Type |
		| Apprenticeship 1 | test employer | 01/Aug/2025-26 Academic Year | 31/Aug/2026-27 Academic Year    | 15000        | stopped | 01/Aug/2025-26 Academic Year | 01/Feb/2025-26 Academic Year | 01/Feb/2025-26 Academic Year | 51            | 25             |
		| Apprenticeship 2 | employer 2    | 01/Feb/2025-26 Academic Year | 31/Aug/2025-26 Academic Year | 5625         | active  | 01/Feb/2025-26 Academic Year |                              |                              | 51            | 25             |
	
	And the provider previously submitted the following learner details
		| Employer id   | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| test employer | 01/Aug/2025-26 Academic Year | 12 months        | 12000                | 01/Aug/2025-26 Academic Year        | 3000                   | 01/Aug/2025-26 Academic Year          |                 | withdrawn         | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
		| pe-1             | 12000                | 01/Aug/2025-26 Academic Year        | 3000                   | 01/Aug/2025-26 Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |

	And the following earnings had been generated for the learner
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Sep/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Oct/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Nov/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Dec/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jan/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Feb/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Mar/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Apr/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| May/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jun/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jul/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
	
	And the following provider payments had been generated
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Price Episode Identifier | Employer      |
		| R01/2025-26 Academic Year | Aug/2025-26 Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R02/2025-26 Academic Year | Sep/2025-26 Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R03/2025-26 Academic Year | Oct/2025-26 Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R04/2025-26 Academic Year | Nov/2025-26 Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R05/2025-26 Academic Year | Dec/2025-26 Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R06/2025-26 Academic Year | Jan/2025-26 Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R07/2025-26 Academic Year | Feb/2025-26 Academic Year | -1000         | Learning         | pe-1                     | test employer |
		| R08/2025-26 Academic Year | Mar/2025-26 Academic Year | -1000         | Learning         | pe-1                     | test employer |
		| R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 2000          | Learning         | pe-2                     | employer 2    |
	
	But the Provider now changes the Learner details as follows
		| Employer id | Start Date                   | Planned Duration | Actual Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| employer 2  | 01/Feb/2025-26 Academic Year | 6 months         | 6 months        | 12000                | 01/Feb/2025-26 Academic Year        | 3000                   | 01/Feb/2025-26 Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
		| pe-1             | 12000                | 01/Aug/2025-26 Academic Year        | 3000                   | 01/Aug/2025-26 Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |
		| pe-2             |                      |                                     |                        |                                       | 5000                    | 01/Feb/2025-26 Academic Year           | 625                       | 01/Feb/2025-26 Academic Year             | 90%                         | Act1          |
	
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Sep/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Oct/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Nov/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Dec/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jan/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Feb/2025-26 Academic Year | 750          | 0          | 0         | pe-2                     |
		| Mar/2025-26 Academic Year | 750          | 0          | 0         | pe-2                     |
		| Apr/2025-26 Academic Year | 750          | 0          | 0         | pe-2                     |
		| May/2025-26 Academic Year | 750          | 0          | 0         | pe-2                     |
		| Jun/2025-26 Academic Year | 750          | 0          | 0         | pe-2                     |
		| Jul/2025-26 Academic Year | 750          | 0          | 0         | pe-2                     |
        
	And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| R09/2025-26 Academic Year | Feb/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| R09/2025-26 Academic Year | Mar/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
		| R09/2025-26 Academic Year | Feb/2025-26 Academic Year | 750          | 0          | 0         | pe-2                     |
		| R09/2025-26 Academic Year | Mar/2025-26 Academic Year | -1250        | 0          | 0         | pe-2                     |
		| R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 750          | 0          | 0         | pe-2                     |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 750          | 0          | 0         | pe-2                     |
		| R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 750          | 0          | 0         | pe-2                     |
		| R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 750          | 0          | 0         | pe-2                     |

	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer      |
		| R09/2025-26 Academic Year | Feb/2025-26 Academic Year | 1000          | Learning         | test employer |
		| R09/2025-26 Academic Year | Mar/2025-26 Academic Year | 1000          | Learning         | test employer |
		| R09/2025-26 Academic Year | Feb/2025-26 Academic Year | 750           | Learning         | employer 2    |
		| R09/2025-26 Academic Year | Mar/2025-26 Academic Year | -1250         | Learning         | employer 2    |
		| R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 750           | Learning         | employer 2    |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 750           | Learning         | employer 2    |
		| R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 750           | Learning         | employer 2    |
		| R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 750           | Learning         | employer 2    |

	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer      |
		| R09/2025-26 Academic Year | Feb/2025-26 Academic Year | 1000          | Learning         | test employer |
		| R09/2025-26 Academic Year | Mar/2025-26 Academic Year | 1000          | Learning         | test employer |
		| R09/2025-26 Academic Year | Feb/2025-26 Academic Year | 750           | Learning         | employer 2    |
		| R09/2025-26 Academic Year | Mar/2025-26 Academic Year | -1250         | Learning         | employer 2    |
		| R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 750           | Learning         | employer 2    |
		| R10/2025-26 Academic Year | May/2025-26 Academic Year | 750           | Learning         | employer 2    |
		| R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 750           | Learning         | employer 2    |
		| R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 750           | Learning         | employer 2    |

Examples:
	| Collection_Period         | Levy Balance for test employer | Levy Balance for employer 2 |
	| R09/2025-26 Academic Year | 999999                         | 999999                      |
	| R10/2025-26 Academic Year | 999999                         | 999999                      |
	| R11/2025-26 Academic Year | 999999                         | 999999                      |
	| R12/2025-26 Academic Year | 999999                         | 999999                      |
        