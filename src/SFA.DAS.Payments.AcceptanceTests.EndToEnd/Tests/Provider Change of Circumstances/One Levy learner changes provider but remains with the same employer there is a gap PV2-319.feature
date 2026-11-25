Feature: Levy learner changes provider but remains with the same employer and there is a gap PV2-319
	As a provider,
	I want a levy learner, that changes provider but remains with the same employer and there is a gap between the two learning spells, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.

Scenario Outline: Levy learner changes provider but remains with the same employer and there is a gap PV2-319
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
		 | Identifier       | Provider   | start date                   | end date                  | agreed price | status    | effective from               | effective to                 | stop effective from          | Standard Code | Programme Type |
		 | Apprenticeship 1 | provider a | 01/Aug/2025-26 Academic Year | 01/Aug/2026-27 Academic Year | 7500         | cancelled | 01/Aug/2025-26 Academic Year | 04/Mar/2025-26 Academic Year | 05/Mar/2025-26 Academic Year | 51            | 25             |
		 | Apprenticeship 2 | provider b | 01/Jun/2025-26 Academic Year | 01/Nov/2026-27 Academic Year | 4500         | active    | 06/Jun/2025-26 Academic Year |                              |                              | 51            | 25             |
	And the "provider a" previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/2025-26 Academic Year | 12 months        | 6000                 | 06/Aug/2025-26 Academic Year        | 1500                   | 06/Aug/2025-26 Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 19-24 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And the following earnings had been generated for the learner for "provider a"
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/2025-26 Academic Year | 500          | 0          | 0         |
        | Sep/2025-26 Academic Year | 500          | 0          | 0         |
        | Oct/2025-26 Academic Year | 500          | 0          | 0         |
        | Nov/2025-26 Academic Year | 500          | 0          | 0         |
        | Dec/2025-26 Academic Year | 500          | 0          | 0         |
        | Jan/2025-26 Academic Year | 500          | 0          | 0         |
        | Feb/2025-26 Academic Year | 500          | 0          | 0         |
        | Mar/2025-26 Academic Year | 500          | 0          | 0         |
        | Apr/2025-26 Academic Year | 500          | 0          | 0         |
        | May/2025-26 Academic Year | 500          | 0          | 0         |
        | Jun/2025-26 Academic Year | 500          | 0          | 0         |
        | Jul/2025-26 Academic Year | 500          | 0          | 0         |
    And the following "provider a" payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/2025-26 Academic Year | Aug/2025-26 Academic Year | 500           | Learning         |
        | R02/2025-26 Academic Year | Sep/2025-26 Academic Year | 500           | Learning         |
        | R03/2025-26 Academic Year | Oct/2025-26 Academic Year | 500           | Learning         |
        | R04/2025-26 Academic Year | Nov/2025-26 Academic Year | 500           | Learning         |
        | R05/2025-26 Academic Year | Dec/2025-26 Academic Year | 500           | Learning         |
        | R06/2025-26 Academic Year | Jan/2025-26 Academic Year | 500           | Learning         |
        | R07/2025-26 Academic Year | Feb/2025-26 Academic Year | 500           | Learning         |
    
	But the "provider a" now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/2025-26 Academic Year | 12 months        | 6000                 | 06/Aug/2025-26 Academic Year        | 1500                   | 06/Aug/2025-26 Academic Year          | 7 months        | withdrawn         | Act1          | 1                   | ZPROG001      | 51            | 25             | 19-24 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	And the "provider b" is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Jun/2025-26 Academic Year | 5 months         | 3000                 | 06/Jun/2025-26 Academic Year        | 1500                   | 06/Jun/2025-26 Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 19-24 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 6000                 | 06/Aug/2025-26 Academic Year        | 1500                   | 06/Aug/2025-26 Academic Year          | Act1          | 1                   | 90%                         |
		| pe-2             | 3000                 | 06/Jun/2025-26 Academic Year        | 1500                   | 06/Jun/2025-26 Academic Year          | Act1          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in the collection period <Collection_Period> by "provider a"
	When the ILR file is submitted for the learners for the collection period <Collection_Period> by "provider b"
	Then the following learner earnings should be generated for "provider a"
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/2025-26 Academic Year | 500          | 0          | 0         | pe-1                     |
		| Sep/2025-26 Academic Year | 500          | 0          | 0         | pe-1                     |
		| Oct/2025-26 Academic Year | 500          | 0          | 0         | pe-1                     |
		| Nov/2025-26 Academic Year | 500          | 0          | 0         | pe-1                     |
		| Dec/2025-26 Academic Year | 500          | 0          | 0         | pe-1                     |
		| Jan/2025-26 Academic Year | 500          | 0          | 0         | pe-1                     |
		| Feb/2025-26 Academic Year | 500          | 0          | 0         | pe-1                     |
		| Mar/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Apr/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| May/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jun/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jul/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
	And the following learner earnings should be generated for "provider b"
        | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
        | Aug/2025-26 Academic Year | 0            | 0          | 0         | pe-2                     |
        | Sep/2025-26 Academic Year | 0            | 0          | 0         | pe-2                     |
        | Oct/2025-26 Academic Year | 0            | 0          | 0         | pe-2                     |
        | Nov/2025-26 Academic Year | 0            | 0          | 0         | pe-2                     |
        | Dec/2025-26 Academic Year | 0            | 0          | 0         | pe-2                     |
        | Jan/2025-26 Academic Year | 0            | 0          | 0         | pe-2                     |
        | Feb/2025-26 Academic Year | 0            | 0          | 0         | pe-2                     |
        | Mar/2025-26 Academic Year | 0            | 0          | 0         | pe-2                     |
        | Apr/2025-26 Academic Year | 0            | 0          | 0         | pe-2                     |
        | May/2025-26 Academic Year | 0            | 0          | 0         | pe-2                     |
        | Jun/2025-26 Academic Year | 720          | 0          | 0         | pe-2                     |
        | Jul/2025-26 Academic Year | 720          | 0          | 0         | pe-2                     |

    And at month end no payments will be calculated for "provider a"
    And at month end only the following payments will be calculated for "provider b"
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 720          | 0          | 0         |
        | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 720          | 0          | 0         |

	And Month end is triggered

	And no "provider a" payments will be recorded        
	And only the following "provider b" payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
		| R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 720           | Learning         |
		| R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 720           | Learning         |        

	And no "provider a" payments will be generated
	And only the following "provider b" payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
		| R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 720           | Learning         |
		| R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 720           | Learning         |        
Examples: 
        | Collection_Period         | Levy Balance |
        #| R01/2025-26 Academic Year | 12500        |
        #| R02/2025-26 Academic Year | 12000        |
        #| R03/2025-26 Academic Year | 11500        |
        #| R04/2025-26 Academic Year | 10000        |
        #| R05/2025-26 Academic Year | 9500         |
        #| R06/2025-26 Academic Year | 9000         |
        #| R07/2025-26 Academic Year | 8500         |
        | R08/2025-26 Academic Year | 8000         |
        | R09/2025-26 Academic Year | 8000         |
        | R10/2025-26 Academic Year | 8000         |
        | R11/2025-26 Academic Year | 8000         |
        | R12/2025-26 Academic Year | 7280         |