Feature: One Levy learner changes provider but remains with the same employer-PV2-317
	As a provider,
	I want a levy learner, that changes provider but remains with the same employer, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.

Scenario Outline: Levy learner changes provider but remains with the same employer PV2-317
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the "provider a" previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 6000                 | 06/Aug/Current Academic Year        | 1500                   | 06/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 19-24 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following commitments exist
		| Identifier       | Provider   | start date                   | end date                  | agreed price | status    | effective from               | effective to                 | stop effective from          | Standard Code | Programme Type |
		| Apprenticeship 1 | provider a | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 7500         | cancelled | 01/Aug/Current Academic Year | 04/Mar/Current Academic Year | 05/Mar/Current Academic Year | 51            | 25             |
		| Apprenticeship 2 | provider b | 05/Mar/Current Academic Year | 01/Aug/Next Academic Year | 4500         | active    | 05/Mar/Current Academic Year |                              |                              | 51            | 25             |
	And the following earnings had been generated for the learner for "provider a"
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 500          | 0          | 0         |
        | Sep/Current Academic Year | 500          | 0          | 0         |
        | Oct/Current Academic Year | 500          | 0          | 0         |
        | Nov/Current Academic Year | 500          | 0          | 0         |
        | Dec/Current Academic Year | 500          | 0          | 0         |
        | Jan/Current Academic Year | 500          | 0          | 0         |
        | Feb/Current Academic Year | 500          | 0          | 0         |
        | Mar/Current Academic Year | 500          | 0          | 0         |
        | Apr/Current Academic Year | 500          | 0          | 0         |
        | May/Current Academic Year | 500          | 0          | 0         |
        | Jun/Current Academic Year | 500          | 0          | 0         |
        | Jul/Current Academic Year | 500          | 0          | 0         |
    And the following "provider a" payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 500           | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 500           | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 500           | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 500           | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 500           | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 500           | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 500           | Learning         |
    But the "provider a" now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 6000                 | 06/Aug/Current Academic Year        | 1500                   | 06/Aug/Current Academic Year          | 7 months        | withdrawn         | Act1          | 1                   | ZPROG001      | 51            | 25             | 19-24 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	And the "provider b" is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 05/Mar/Current Academic Year | 5 months         | 3000                 | 05/Mar/Current Academic Year        | 1500                   | 05/Mar/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 19-24 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	When the amended ILR file is re-submitted for the learners in the collection period <Collection_Period> by "provider a"
	When the ILR file is submitted for the learners for the collection period <Collection_Period> by "provider b"

	Then the following learner earnings should be generated for "provider a"
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 500          | 0          | 0         |
		| Sep/Current Academic Year | 500          | 0          | 0         |
		| Oct/Current Academic Year | 500          | 0          | 0         |
		| Nov/Current Academic Year | 500          | 0          | 0         |
		| Dec/Current Academic Year | 500          | 0          | 0         |
		| Jan/Current Academic Year | 500          | 0          | 0         |
		| Feb/Current Academic Year | 500          | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 0            | 0          | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |
	And the following learner earnings should be generated for "provider b"
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 0            | 0          | 0         |
        | Sep/Current Academic Year | 0            | 0          | 0         |
        | Oct/Current Academic Year | 0            | 0          | 0         |
        | Nov/Current Academic Year | 0            | 0          | 0         |
        | Dec/Current Academic Year | 0            | 0          | 0         |
        | Jan/Current Academic Year | 0            | 0          | 0         |
        | Feb/Current Academic Year | 0            | 0          | 0         |
        | Mar/Current Academic Year | 720          | 0          | 0         |
        | Apr/Current Academic Year | 720          | 0          | 0         |
        | May/Current Academic Year | 720          | 0          | 0         |
        | Jun/Current Academic Year | 720          | 0          | 0         |
        | Jul/Current Academic Year | 720          | 0          | 0         |

    And at month end no payments will be calculated for "provider a"
    And at month end only the following payments will be calculated for "provider b"
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R08/Current Academic Year | Mar/Current Academic Year | 720          | 0          | 0         |
        | R09/Current Academic Year | Apr/Current Academic Year | 720          | 0          | 0         |
        | R10/Current Academic Year | May/Current Academic Year | 720          | 0          | 0         |
        | R11/Current Academic Year | Jun/Current Academic Year | 720          | 0          | 0         |
        | R12/Current Academic Year | Jul/Current Academic Year | 720          | 0          | 0         |

	And Month end is triggered

	And no "provider a" payments will be recorded        
	And only the following "provider b" payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R08/Current Academic Year | Mar/Current Academic Year | 720           | Learning         |
		| R09/Current Academic Year | Apr/Current Academic Year | 720           | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 720           | Learning         |
		| R11/Current Academic Year | Jun/Current Academic Year | 720           | Learning         |
		| R12/Current Academic Year | Jul/Current Academic Year | 720           | Learning         |        

	And no "provider a" payments will be generated
	And only the following "provider b" payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R08/Current Academic Year | Mar/Current Academic Year | 720           | Learning         |
		| R09/Current Academic Year | Apr/Current Academic Year | 720           | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 720           | Learning         |
		| R11/Current Academic Year | Jun/Current Academic Year | 720           | Learning         |
		| R12/Current Academic Year | Jul/Current Academic Year | 720           | Learning         |        
Examples: 
        | Collection_Period         | Levy Balance |
        #| R01/Current Academic Year | 12500        |
        #| R02/Current Academic Year | 12000        |
        #| R03/Current Academic Year | 11500        |
        #| R04/Current Academic Year | 10000        |
        #| R05/Current Academic Year | 9500         |
        #| R06/Current Academic Year | 9000         |
        #| R07/Current Academic Year | 8500         |
        | R08/Current Academic Year | 8000         |
        | R09/Current Academic Year | 7280         |
        | R10/Current Academic Year | 6560         |
        | R11/Current Academic Year | 5840         |
        | R12/Current Academic Year | 5120         |