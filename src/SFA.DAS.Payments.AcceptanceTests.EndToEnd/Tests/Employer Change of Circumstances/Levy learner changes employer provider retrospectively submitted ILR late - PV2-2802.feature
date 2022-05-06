Feature: Levy learner changes employer provider retrospectively submitted ILR late - PV2-2802
		As a provider,
		I want earnings and payments for a levy learner, levy available, when there is a change of employer during the programme and ILR is submitted late, to be paid the correct amount
		So that I am accurately paid my apprenticeship provision

Scenario Outline: Levy learner changes employer provider retrospectively submitted ILR late - PV2-2802
	Given the "test employer" levy account balance in collection period <Collection_Period> is <Levy Balance for test employer>
	And the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the following commitments exist
		| Identifier       | Employer      | start date                   | end date                     | agreed price | status  | effective from               | effective to                 | stop effective from          | Standard Code | Programme Type |
		| Apprenticeship 1 | test employer | 01/Apr/last Academic Year    | 01/Mar/Current Academic Year | 4200         | stopped | 01/Apr/last Academic Year    | 01/Feb/Current Academic Year | 01/Feb/Current Academic Year | 51            | 25             |
		| Apprenticeship 2 | employer 2    | 01/Feb/Current Academic Year | 01/Mar/Current Academic Year | 1446         | active  | 01/Feb/Current Academic Year |                              |                              | 51            | 25             |
	
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 07/Apr/last Academic Year | 12 months        | 12000                | 07/Apr/last Academic Year           | 3000                   | 07/Apr/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
		| pe-1             | 4200                 | 07/Apr/last Academic Year           | 0                      | 07/Apr/last Academic Year             | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |
	
	And the following earnings had been generated for the learner
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 280          | 0          | 0         | pe-1                     |
	
	And the following provider payments had been generated
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Price Episode Identifier |
		| R01/Current Academic Year | Aug/Current Academic Year | 280           | Learning         | pe-1                     |
		| R02/Current Academic Year | Sep/Current Academic Year | 280           | Learning         | pe-1                     |
		| R03/Current Academic Year | Oct/Current Academic Year | 280           | Learning         | pe-1                     |
		| R04/Current Academic Year | Nov/Current Academic Year | 280           | Learning         | pe-1                     |
		| R05/Current Academic Year | Dec/Current Academic Year | 280           | Learning         | pe-1                     |
		| R06/Current Academic Year | Jan/Current Academic Year | 280           | Learning         | pe-1                     |
		| R07/Current Academic Year | Feb/Current Academic Year | 280           | Learning         | pe-1                     |
		| R08/Current Academic Year | Mar/Current Academic Year | 280           | Learning         | pe-1                     |
	
	But the Provider now changes the Learner details as follows
		| Employer id   | Start Date                | Planned Duration | Actual Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| test employer | 07/Apr/last Academic Year | 10 months        | 6 months        | 12000                | 07/Apr/last Academic Year           | 3000                   | 07/Apr/Current Academic Year          |                 | withdrawn         | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
		| employer 2    | 07/Apr/last Academic Year | 2 months         | 2 months        | 12000                | 07/Apr/last Academic Year           | 3000                   | 07/Apr/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
		| pe-1             | 4200                 | 07/Apr/last Academic Year           | 0                      | 07/Apr/last Academic Year             | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |
		| pe-2             | 0                    | 01/Feb/Current Academic Year        | 0                      | 01/Feb/Current Academic Year          | 606                     | 01/Feb/Current Academic Year           | 840                       | 01/Feb/Current Academic Year             | 90%                         | Act1          |
	
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 280          | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 579          | 0          | 0         | pe-2                     |
		| Mar/Current Academic Year | 579          | 0          | 0         | pe-2                     |
		| Apr/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| May/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| Jun/Current Academic Year | 0            | 0          | 0         | pe-2                     |
		| Jul/Current Academic Year | 0            | 0          | 0         | pe-2                     |
        
	And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| R09/Current Academic Year | Feb/Current Academic Year | 579          | 0          | 0         | pe-2                     |
		| R09/Current Academic Year | Mar/Current Academic Year | 579          | 0          | 0         | pe-2                     |
		| R09/Current Academic Year | Feb/Current Academic Year | -280         | 0          | 0         | pe-1                     |
		| R09/Current Academic Year | Mar/Current Academic Year | -280         | 0          | 0         | pe-1                     |

	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer      |
		| R09/Current Academic Year | Feb/Current Academic Year | 579           | Learning         | employer 2    |
		| R09/Current Academic Year | Mar/Current Academic Year | 579           | Learning         | employer 2    |
		| R09/Current Academic Year | Feb/Current Academic Year | -280          | Learning         | test employer |
		| R09/Current Academic Year | Mar/Current Academic Year | -280          | Learning         | test employer |

	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer      |
		| R09/Current Academic Year | Feb/Current Academic Year | 579           | Learning         | employer 2    |
		| R09/Current Academic Year | Mar/Current Academic Year | 579           | Learning         | employer 2    |
		| R09/Current Academic Year | Feb/Current Academic Year | -280          | Learning         | test employer |
		| R09/Current Academic Year | Mar/Current Academic Year | -280          | Learning         | test employer |

Examples:
	| Collection_Period         | Levy Balance for test employer | Levy Balance for employer 2 |
	| R09/Current Academic Year | 999999                         | 999999                      |
        