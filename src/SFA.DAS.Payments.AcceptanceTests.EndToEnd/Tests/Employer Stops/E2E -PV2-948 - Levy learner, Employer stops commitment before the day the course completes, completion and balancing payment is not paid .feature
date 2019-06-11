#Feature: Employer Stops
#
#Scenario: Employer stops commitment before the day that the course is completed, but in the same month, completes course early. Completion and Balancing payment not paid
#
#    Given the learner is programme only Levy
#    And levy balance > agreed price for all months
#	
#    And the following commitments exist:
#		| commitment Id | version Id | ULN       | start date | end date   | status    | agreed price | effective from | effective to | stop effective from |
#		| 1             | 1          | learner a | 01/08/2018 | 01/10/2019 | cancelled | 15000        | 01/08/2018     | 01/10/2019   | 05/08/2019          |
#		
#	When an ILR file is submitted with the following data:
#        | ULN       | agreed price | learner type       | start date | planned end date | actual end date   |completion status|
#        | learner a | 15000        | programme only DAS | 01/08/2018 | 09/10/2019       | 09/08/2019        |completed        |
#       
#	Then the provider earnings and payments break down as follows:
#        | Type                           | 08/18 | 09/18 | ... | 07/19 | 08/19 | 09/19 |
#        | Provider Earned Total          | 1000  | 1000  | ... | 1000  | 4500  | 0     |
#        | Provider Earned from SFA       | 1000  | 1000  | ... | 1000  | 4500  | 0     |
#        | Provider Earned from Employer  | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider Paid by SFA           | 0     | 1000  | ... | 1000  | 1000  | 0     |
#        | Payment due from Employer      | 0     | 0     | ... | 0     | 0     | 0     |
#        | Levy account debited           | 0     | 1000  | ... | 1000  | 1000  | 0     |
#        | SFA Levy employer budget       | 1000  | 1000  | ... | 1000  | 0     | 0     |
#        | SFA Levy co-funding budget     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA non-Levy co-funding budget | 0     | 0     | ... | 0     | 0     | 0     |
#
#	And the transaction types for the payments are:
#        | Payment type                 | 09/18 | ... | 07/19 | 08/19 | 09/19 |
#        | On-program                   | 1000  | ... | 1000  | 1000  | 0     |
#        | Completion                   | 0     | ... | 0     | 0     | 0     |
#        | Balancing                    | 0     | ... | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | ... | 0     | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | ... | 0     | 0     | 0     |
#        | Framework uplift on-program  | 0     | ... | 0     | 0     | 0     |
#        | Framework uplift completion  | 0     | ... | 0     | 0     | 0     |
#        | Framework uplift balancing   | 0     | ... | 0     | 0     | 0     |
#        | Provider disadvantage uplift | 0     | ... | 0     | 0     | 0     |

Feature: Employer Stops  PV2-948

Scenario Outline: Employer stops commitment before the day that the course is completed, but in the same month, completes course early. Completion and Balancing payment not paid

Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following apprenticeships exist
		| agreed price | start date                | end date                     | status  | effective from            | stop effective from          |
		| 17500        | 01/Aug/Last Academic Year | 01/Oct/Current Academic Year | stopped | 01/Aug/Last Academic Year | 30/Aug/Current Academic Year |		
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| 01/Aug/Last Academic Year | 14 months        | 15500                | 01/Aug/Last Academic Year           | 2000                   | 01/Aug/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 17             | 25             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year    | 1000         | 0          | 0         |
        | Sep/Last Academic Year    | 1000         | 0          | 0         |
        | Oct/Last Academic Year    | 1000         | 0          | 0         |
        | Nov/Last Academic Year    | 1000         | 0          | 0         |
        | Dec/Last Academic Year    | 1000         | 0          | 0         |
        | Jan/Last Academic Year    | 1000         | 0          | 0         |
        | Feb/Last Academic Year    | 1000         | 0          | 0         |
        | Mar/Last Academic Year    | 1000         | 0          | 0         |
        | Apr/Last Academic Year    | 1000         | 0          | 0         |
        | May/Last Academic Year    | 1000         | 0          | 0         |
        | Jun/Last Academic Year    | 1000         | 0          | 0         |
        | Jul/Last Academic Year    | 1000         | 0          | 0         |
        | Aug/Current Academic Year | 1000         | 0          | 0         |
        | Sep/Current Academic Year | 1000         | 3500       | 0         |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R02/Last Academic Year    | Sep/Last Academic Year    | 1000          | Learning         |
        | R03/Last Academic Year    | Oct/Last Academic Year    | 1000          | Learning         |
        | R04/Last Academic Year    | Nov/Last Academic Year    | 1000          | Learning         |
        | R05/Last Academic Year    | Dec/Last Academic Year    | 1000          | Learning         |
        | R06/Last Academic Year    | Jan/Last Academic Year    | 1000          | Learning         |
        | R07/Last Academic Year    | Feb/Last Academic Year    | 1000          | Learning         |
        | R08/Last Academic Year    | Mar/Last Academic Year    | 1000          | Learning         |
        | R09/Last Academic Year    | Apr/Last Academic Year    | 1000          | Learning         |
        | R10/Last Academic Year    | May/Last Academic Year    | 1000          | Learning         |
        | R11/Last Academic Year    | Jun/Last Academic Year    | 1000          | Learning         |
        | R12/Last Academic Year    | Jul/Last Academic Year    | 1000          | Learning         |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
			
	But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                | SFA Contribution Percentage |
		| 01/Aug/Last Academic Year | 14 months        | 15500                | 01/Aug/Last Academic Year           | 2000                   | 01/Aug/Last Academic Year             | 13 months       | completed         | Act1          | 1                   | ZPROG001      | 17            | 25             | 19+ Apprenticeship (From May 2017) Levy Contract | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>

	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 1000         | 0          | 0         |
		| Sep/Current Academic Year | 0            | 3500       | 1000      |
		| Oct/Current Academic Year | 0            | 0          | 0         |
		| Nov/Current Academic Year | 0            | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 0            | 0          | 0         |
		| Feb/Current Academic Year | 0            | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 0            | 0          | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |

 # Please check if the next two steps need to be changed
    And the following non-payable earnings were generated
        | Learner ID | ILR Start Date            | Standard code | programme type |
        | learner a  | 01/Aug/Last Academic Year | 17            | 25             |

	And the following data lock failures were generated
        | Apprentice   | Learner ID | Delivery Period           | ILR Start Date            | Transaction Type | Error Description |
        | apprentice a | learner a  | Sep/Current Academic Year | 01/Aug/Last Academic Year | Learning         | DLOCK 10          |

	And Month end is triggered
	And no provider payments will be generated
	And no provider payments will be recorded
Examples: 
        | Collection_Period         | Levy Balance |
        | R02/Current Academic Year | 5000         |