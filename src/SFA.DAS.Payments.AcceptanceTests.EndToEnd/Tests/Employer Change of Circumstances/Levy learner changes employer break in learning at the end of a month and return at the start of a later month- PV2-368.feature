Feature: Levy learner changes employer break in learning at the end of a month and return at the start of a later month- PV2-368
		As a provider,
		I want earnings and payments for a levy learner, levy available, and they have a break in learning at the end of a month and return at the start of a later month with a different employer, to be paid the correct amount
		So that I am accurately paid my apprenticeship provision.

Scenario Outline: Levy learner changes employer break in learning at the end of a month and return at the start of a later month- PV2-368
	Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
	And  the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	And the following commitments exist
      | Identifier        | Employer   | start date                   | end date                  | agreed price | status    | effective from               | effective to                 | stop effective from          |
      | Apprenticeship 1   | employer 1 | 01/Aug/Current Academic Year | 31/Aug/Next Academic Year | 15000        | cancelled | 01/Aug/Current Academic Year | 31/Oct/Current Academic Year | 01/Nov/Current Academic Year |
      | Apprenticeship 2   | employer 2 | 01/Jan/Current Academic Year | 31/Oct/Next Academic Year | 5625         | active    | 01/Jan/Current Academic Year |                              |                              |
	And the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          | 3 months        | planned break     | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
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
		| 01/Jan/Current Academic Year | 9 months         |                      |                                     |                        |                                       |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
        | 1st price details | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |
        | 2nd price details |                      |                                     |                        |                                       | 5000                    | 01/Jan/Current Academic Year           | 625                       | 01/Jan/Current Academic Year             | 90%                         | Act1          |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
		| Sep/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
		| Oct/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
		| Nov/Current Academic Year | 0            | 0          | 0         | 1st price details        |
		| Dec/Current Academic Year | 0            | 0          | 0         | 1st price details        |
		| Jan/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| Feb/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| Mar/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| Apr/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| May/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| Jun/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
		| Jul/Current Academic Year | 500          | 0          | 0         | 2nd price details        |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R04/Current Academic Year | Nov/Current Academic Year | 0            | 0          | 0         |
        | R05/Current Academic Year | Dec/Current Academic Year | 0            | 0          | 0         |
        | R06/Current Academic Year | Jan/Current Academic Year | 500          | 0          | 0         |
        | R07/Current Academic Year | Feb/Current Academic Year | 500          | 0          | 0         |
        | R08/Current Academic Year | Mar/Current Academic Year | 500          | 0          | 0         |
        | R09/Current Academic Year | Apr/Current Academic Year | 500          | 0          | 0         |
        | R10/Current Academic Year | May/Current Academic Year | 500          | 0          | 0         |
        | R11/Current Academic Year | Jun/Current Academic Year | 500          | 0          | 0         |
        | R12/Current Academic Year | Jul/Current Academic Year | 500          | 0          | 0         |
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer   |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | Learning         | employer 1 |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | Learning         | employer 1 |
        | R06/Current Academic Year | Jan/Current Academic Year | 500           | Learning         | employer 2 |
        | R07/Current Academic Year | Feb/Current Academic Year | 500           | Learning         | employer 2 |
        | R08/Current Academic Year | Mar/Current Academic Year | 500           | Learning         | employer 2 |
        | R09/Current Academic Year | Apr/Current Academic Year | 500           | Learning         | employer 2 |
        | R10/Current Academic Year | May/Current Academic Year | 500           | Learning         | employer 2 |
        | R11/Current Academic Year | Jun/Current Academic Year | 500           | Learning         | employer 2 |
        | R12/Current Academic Year | Jul/Current Academic Year | 500           | Learning         | employer 2 |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type | Employer   |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | Learning         | employer 1 |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | Learning         | employer 1 |
        | R06/Current Academic Year | Jan/Current Academic Year | 500           | Learning         | employer 2 |
        | R07/Current Academic Year | Feb/Current Academic Year | 500           | Learning         | employer 2 |
        | R08/Current Academic Year | Mar/Current Academic Year | 500           | Learning         | employer 2 |
        | R09/Current Academic Year | Apr/Current Academic Year | 500           | Learning         | employer 2 |
        | R10/Current Academic Year | May/Current Academic Year | 500           | Learning         | employer 2 |
        | R11/Current Academic Year | Jun/Current Academic Year | 500           | Learning         | employer 2 |
        | R12/Current Academic Year | Jul/Current Academic Year | 500           | Learning         | employer 2 |
Examples: 
        | Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 |
        | R04/Current Academic Year | 13500                       | 7125                        |
        | R05/Current Academic Year | 13500                       | 7125                        |
        | R06/Current Academic Year | 13500                       | 5625                        |
        | R07/Current Academic Year | 13500                       | 5125                        |
        | R08/Current Academic Year | 13500                       | 4525                        |
        | R09/Current Academic Year | 13500                       | 4125                        |
        | R10/Current Academic Year | 13500                       | 3625                        |
        | R11/Current Academic Year | 13500                       | 3125                        |
        | R12/Current Academic Year | 13500                       | 2625                        |


    #Scenario: Earnings and payments for a levy learner, levy available, and they have a break in learning at the end of a month and return at the start of a later month with a different employer
    #
    #    Given the apprenticeship funding band maximum is 17000
    #    Given The learner is programme only DAS
    #    And the employer 1 has a levy balance > agreed price for all months
    #    And the employer 2 has a levy balance > agreed price for all months
    #    And the learner changes employers
    #        | Employer   | Type | ILR employment start date |
    #        | employer 1 | DAS  | 01/08/2018                |
    #        | employer 2 | DAS  | 01/01/2019                |
    #    
    #    And the following commitments exist on 03/12/2018:
    #        | Employer   | commitment Id | version Id | ULN       | start date | end date   | agreed price | status    | effective from | effective to | stop effective from |
    #        | employer 1 | 1             | 1-001      | learner a | 01/08/2018 | 31/08/2019 | 15000        | cancelled | 01/08/2018     | 31/10/2018   | 01/11/2018   |
    #        | employer 2 | 2             | 1-001      | learner a | 01/01/2019 | 31/10/2019 | 5625         | active    | 01/01/2019     |              |              |
    #    
    #    When an ILR file is submitted with the following data:
    #        | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | Residual training price | Residual training price effective date | Residual assessment price | Residual assessment price effective date |
    #        | learner a | 01/08/2018 | 04/08/2019       | 31/10/2018      | planned break     | 12000                | 01/08/2018                          | 3000                   | 01/08/2018                            |                         |                                        |                           |                                          |
    #        | learner a | 01/01/2019 | 04/10/2019       |                 | continuing        |                      |                                     |                        |                                       | 5000                    | 01/01/2019                             | 625                       | 01/01/2019                               |
    #       
    #    #Then the data lock status of the ILR in 03/12/2018 is:
    #    #    | Payment type | 08/18               | 09/18               | 10/18               | 11/18 | 12/18 | 01/19               | 02/19               | 03/19               |
    #    #    | On-program   | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-001 |       |       | commitment 2 v1-001 | commitment 2 v1-001 | commitment 2 v1-001 |
    #    
    #    Then the provider earnings and payments break down as follows:
    #        | Type                            | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 | 02/19 | 03/19 |
    #        | Provider Earned Total           | 1000  | 1000  | 1000  | 0     | 0     | 500   | 500   | 500   |
    #        | Provider Earned from SFA        | 1000  | 1000  | 1000  | 0     | 0     | 500   | 500   | 500   |
    #        | Provider Earned from Employer 1 | 0     | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
    #        | Provider Earned from Employer 2 | 0     | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
    #        | Provider Paid by SFA            | 0     | 1000  | 1000  | 1000  | 0     | 0     | 500   | 500   |
    #        | Payment due from employer 1     | 0     | 0     | 0     | 0     | 0     | 0     | 0     | 0     | 
    #        | Payment due from employer 2     | 0     | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
    #        | Employer 1 Levy account debited | 0     | 1000  | 1000  | 1000  | 0     | 0     | 0     | 0     |
    #        | Employer 2 Levy account debited | 0     | 0     | 0     | 0     | 0     | 0     | 500   | 500   |
    #        | SFA Levy employer budget        | 1000  | 1000  | 1000  | 0     | 0     | 500   | 500   | 500   |
    #        | SFA Levy co-funding budget      | 0     | 0     | 0     | 0     | 0     | 0     | 0     | 0     |


# For DC integration
    #    And the learner changes employers
        #| Employer   | Type | ILR employment start date |
        #| employer 1 | DAS  | 01/08/2018                |
        #| employer 2 | DAS  | 01/01/2019                |

# Restart indicator will be possibly needed for integration in case of planned break.
