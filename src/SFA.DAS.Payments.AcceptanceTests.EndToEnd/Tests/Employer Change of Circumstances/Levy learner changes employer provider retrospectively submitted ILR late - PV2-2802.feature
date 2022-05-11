Feature: Levy learner changes employer provider retrospectively submitted ILR late - PV2-2802
		As a provider,
		I want earnings and payments for a levy learner, levy available, when there is a change of employer during the programme and ILR is submitted late, to be paid the correct amount
		So that I am accurately paid my apprenticeship provision

Scenario Outline: Levy learner changes employer provider retrospectively submitted ILR late - PV2-2802
	Given the "test employer" levy account balance in collection period <Collection_Period> is <Levy Balance for test employer>
	And the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the following commitments exist
		| Identifier       | Employer      | start date                   | end date                     | agreed price | status  | effective from               | effective to                 | stop effective from          | Standard Code | Programme Type |
		| Apprenticeship 1 | test employer | 01/Aug/Current Academic Year | 31/Aug/Next Academic Year    | 15000        | stopped | 01/Aug/Current Academic Year | 01/Feb/Current Academic Year | 01/Feb/Current Academic Year | 51            | 25             |
		| Apprenticeship 2 | employer 2    | 01/Feb/Current Academic Year | 31/Aug/Current Academic Year | 5625         | active  | 01/Feb/Current Academic Year |                              |                              | 51            | 25             |
	
	And the provider previously submitted the following learner details
		| Employer id   | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| test employer | 01/Aug/Current Academic Year | 12 months        | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          |                 | withdrawn         | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
		| pe-1             | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |
	
	And the following earnings had been generated for the learner
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
	
	And the following provider payments had been generated
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Price Episode Identifier | Employer      |
		| R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R04/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R05/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         | pe-1                     | test employer |
		| R07/Current Academic Year | Feb/Current Academic Year | -1000         | Learning         | pe-1                     | test employer |
		| R08/Current Academic Year | Mar/Current Academic Year | -1000         | Learning         | pe-1                     | test employer |
		| R08/Current Academic Year | Mar/Current Academic Year | 2000          | Learning         | pe-2                     | employer 2    |
	
	But the Provider now changes the Learner details as follows
		| Employer id | Start Date                   | Planned Duration | Actual Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| employer 2  | 01/Feb/Current Academic Year | 6 months         | 6 months        | 12000                | 01/Feb/Current Academic Year        | 3000                   | 01/Feb/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
		| pe-1             | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |
		| pe-2             |                      |                                     |                        |                                       | 5000                    | 01/Feb/Current Academic Year           | 625                       | 01/Feb/Current Academic Year             | 90%                         | Act1          |
	
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 750          | 0          | 0         | pe-2                     |
		| Mar/Current Academic Year | 750          | 0          | 0         | pe-2                     |
		| Apr/Current Academic Year | 750          | 0          | 0         | pe-2                     |
		| May/Current Academic Year | 750          | 0          | 0         | pe-2                     |
		| Jun/Current Academic Year | 750          | 0          | 0         | pe-2                     |
		| Jul/Current Academic Year | 750          | 0          | 0         | pe-2                     |
        
	And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| R09/Current Academic Year | Feb/Current Academic Year | -1000        | 0          | 0         | pe-1                     |
		| R09/Current Academic Year | Mar/Current Academic Year | -1000        | 0          | 0         | pe-1                     |
		| R09/Current Academic Year | Feb/Current Academic Year | 750          | 0          | 0         | pe-2                     |
		| R09/Current Academic Year | Mar/Current Academic Year | 750          | 0          | 0         | pe-2                     |
		| R09/Current Academic Year | Apr/Current Academic Year | 750          | 0          | 0         | pe-2                     |
		| R10/Current Academic Year | May/Current Academic Year | 750          | 0          | 0         | pe-2                     |
		| R11/Current Academic Year | Jun/Current Academic Year | 750          | 0          | 0         | pe-2                     |
		| R12/Current Academic Year | Jul/Current Academic Year | 750          | 0          | 0         | pe-2                     |

	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer      |
		| R09/Current Academic Year | Feb/Current Academic Year | -1000         | Learning         | test employer |
		| R09/Current Academic Year | Mar/Current Academic Year | -1000         | Learning         | test employer |
		| R09/Current Academic Year | Feb/Current Academic Year | 750           | Learning         | employer 2    |
		| R09/Current Academic Year | Mar/Current Academic Year | 750           | Learning         | employer 2    |
		| R09/Current Academic Year | Apr/Current Academic Year | 750           | Learning         | employer 2    |
		| R10/Current Academic Year | May/Current Academic Year | 750           | Learning         | employer 2    |
		| R11/Current Academic Year | Jun/Current Academic Year | 750           | Learning         | employer 2    |
		| R12/Current Academic Year | Jul/Current Academic Year | 750           | Learning         | employer 2    |

	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer      |
		| R09/Current Academic Year | Feb/Current Academic Year | -1000         | Learning         | test employer |
		| R09/Current Academic Year | Mar/Current Academic Year | -1000         | Learning         | test employer |
		| R09/Current Academic Year | Feb/Current Academic Year | 750           | Learning         | employer 2    |
		| R09/Current Academic Year | Mar/Current Academic Year | 750           | Learning         | employer 2    |
		| R09/Current Academic Year | Apr/Current Academic Year | 750           | Learning         | employer 2    |
		| R10/Current Academic Year | May/Current Academic Year | 750           | Learning         | employer 2    |
		| R11/Current Academic Year | Jun/Current Academic Year | 750           | Learning         | employer 2    |
		| R12/Current Academic Year | Jul/Current Academic Year | 750           | Learning         | employer 2    |

Examples:
	| Collection_Period         | Levy Balance for test employer | Levy Balance for employer 2 |
	| R09/Current Academic Year | 999999                         | 999999                      |
	| R10/Current Academic Year | 999999                         | 999999                      |
	| R11/Current Academic Year | 999999                         | 999999                      |
	| R12/Current Academic Year | 999999                         | 999999                      |
        