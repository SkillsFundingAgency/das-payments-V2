@ignore
Feature: Levy learner changes employer with change to negotiated price in mid month with gap PV2-364
		As a provider,
		I want earnings and payments for a levy learner, levy available, commitment entered for a new employer in the middle of the month with gap, and there is a change to the employer and negotiated cost in the middle of a month in the ILR, to be paid the correct amount
		So that I am accurately paid my apprenticeship provision.

Scenario Outline: Levy learner changes employer with change to negotiated price in mid month with gap PV2-364
	Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
	And  the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the following commitments exist
        | Employer   | start date                   | end date                  | agreed price | status    | effective from               | effective to                 | stop effective from          |
        | employer 1 | 01/Aug/Current Academic Year | 28/Aug/Next Academic Year | 15000        | cancelled | 01/Aug/Current Academic Year | 14/Nov/Current Academic Year | 15/Nov/Current Academic Year |
        | employer 2 | 15/Nov/Current Academic Year | 28/Aug/Next Academic Year | 5625         | active    | 15/Nov/Current Academic Year |                              |                              |
	And the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
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
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer   |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         | employer 1 |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         | employer 1 |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         | employer 1 |
    But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows
        | Price details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date |
        | 1st price details | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          |
        | 2nd price details | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          | 5000                    | 25/Nov/Current Academic Year           | 625                       | 25/Nov/Current Academic Year             |
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
    And at month end only the following payments will be calculated
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
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer   |
        | R04/Current Academic Year | Nov/Current Academic Year | 500           | Learning         | employer 2 |
        | R05/Current Academic Year | Dec/Current Academic Year | 500           | Learning         | employer 2 |
        | R06/Current Academic Year | Jan/Current Academic Year | 500           | Learning         | employer 2 |
        | R07/Current Academic Year | Feb/Current Academic Year | 500           | Learning         | employer 2 |
        | R08/Current Academic Year | Mar/Current Academic Year | 500           | Learning         | employer 2 |
        | R09/Current Academic Year | Apr/Current Academic Year | 500           | Learning         | employer 2 |
        | R10/Current Academic Year | May/Current Academic Year | 500           | Learning         | employer 2 |
        | R11/Current Academic Year | Jun/Current Academic Year | 500           | Learning         | employer 2 |
        | R12/Current Academic Year | Jul/Current Academic Year | 500           | Learning         | employer 2 |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R04/Current Academic Year | Nov/Current Academic Year | 500           | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 500           | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 500           | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 500           | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 500           | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 500           | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 500           | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 500           | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 500           | Learning         |
Examples: 
        | Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 |
        | R01/Current Academic Year | 15500                       | 7125                        |
        | R02/Current Academic Year | 14500                       | 7125                        |
        | R03/Current Academic Year | 13500                       | 7125                        |
        | R04/Current Academic Year | 13500                       | 5625                        |
        | R05/Current Academic Year | 13500                       | 5125                        |
		| R06/Current Academic Year | 13500                       | 4625                        |
		| R07/Current Academic Year | 13500                       | 4125                        |
		| R08/Current Academic Year | 13500                       | 3525                        |
		| R09/Current Academic Year | 13500                       | 3125                        |
		| R10/Current Academic Year | 13500                       | 2625                        |
		| R11/Current Academic Year | 13500                       | 2125                        |
		| R12/Current Academic Year | 13500                       | 1625                        |

	    #Scenario:  Earnings and payments for a levy learner, levy available, commitment entered for a new employer in the middle of the month with gap, and there is a change to the employer and negotiated cost in the middle of a month in the ILR
    #    
    #    Given The learner is programme only DAS
    #    And the employer 1 has a levy balance > agreed price for all months
    #    And the employer 2 has a levy balance > agreed price for all months
    #    And the learner changes employers
    #        | Employer   | Type | ILR employment start date |
    #        | employer 1 | DAS  | 01/08/2018                |
    #        | employer 2 | DAS  | 15/11/2018                |
    #    
    #    And the following commitments exist on 03/12/2018:
    #        | Employer   | commitment Id | version Id | ULN       | start date | end date   | agreed price | status    | effective from | effective to | stop effective from |
    #        | employer 1 | 1             | 1-001      | learner a | 01/08/2018 | 28/08/2019 | 15000        | cancelled | 01/08/2018     | 14/11/2018   | 15/11/2018   |
    #        | employer 2 | 2             | 1-001      | learner a | 15/11/2018 | 28/08/2019 | 5625         | active    | 15/11/2018     |              |              |
    #    
    #    When an ILR file is submitted on 03/12/2018 with the following data:
    #        | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | Residual training price | Residual training price effective date | Residual assessment price | Residual assessment price effective date |
    #        | learner a | 01/08/2018 | 28/08/2019       |                 | continuing        | 12000                | 01/08/2018                          | 3000                   | 01/08/2018                            | 5000                    | 25/11/2018                             | 625                       | 25/11/2018                               |
    #    
    #    #Then the data lock status of the ILR in 03/12/2018 is:
    #    #    | Payment type | 08/18               | 09/18               | 10/18               | 11/18               | 12/18               |
    #    #    | On-program   | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-001 | commitment 2 v1-001 | commitment 2 v1-001 |
    #    
    #    Then the provider earnings and payments break down as follows:
    #        | Type                            | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 |
    #        | Provider Earned Total           | 1000  | 1000  | 1000  | 500   | 500   |
    #        | Provider Earned from SFA        | 1000  | 1000  | 1000  | 500   | 500   |
    #        | Provider Earned from Employer 1 | 0     | 0     | 0     | 0     | 0     |
    #        | Provider Earned from Employer 2 | 0     | 0     | 0     | 0     | 0     |
    #        | Provider Paid by SFA            | 0     | 1000  | 1000  | 1000  | 500   |
    #        | Payment due from employer 1     | 0     | 0     | 0     | 0     | 0     |
    #        | Payment due from employer 2     | 0     | 0     | 0     | 0     | 0     |
    #        | Employer 1 Levy account debited | 0     | 1000  | 1000  | 1000  | 0     |
    #        | Employer 2 Levy account debited | 0     | 0     | 0     | 0     | 500   |
    #        | SFA Levy employer budget        | 1000  | 1000  | 1000  | 500   | 500   |
    #        | SFA Levy co-funding budget      | 0     | 0     | 0     | 0     | 0     |

# For DC integration
    #    And the learner changes employers
    #        | Employer   | Type | ILR employment start date |
    #        | employer 1 | DAS  | 01/08/2018                |
    #        | employer 2 | DAS  | 15/11/2018                |