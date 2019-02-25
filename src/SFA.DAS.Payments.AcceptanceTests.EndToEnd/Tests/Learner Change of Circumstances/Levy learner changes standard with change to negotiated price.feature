#Background:
#        Given The learner is programme only DAS
#        And the apprenticeship funding band maximum is 17000
#        And levy balance > agreed price for all months
#
#    Scenario: Earnings and payments for a DAS learner, levy available, where the apprenticeship standard changes
#        Given the following commitments exist on 03/12/2018:
#            | commitment Id | version Id | ULN       | standard code | start date | end date   | agreed price | effective from | effective to |
#            | 1             | 1-001      | learner a | 51            | 01/08/2018 | 01/08/2019 | 15000        | 01/08/2018     | 31/10/2018   |
#            | 1             | 1-002      | learner a | 52            | 01/08/2018 | 01/08/2019 | 5625         | 03/11/2018     |              |
#        When an ILR file is submitted with the following data:
#            | ULN       | standard code | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#            | learner a | 51            | 03/08/2018 | 01/08/2019       | 31/10/2018      | withdrawn         | 12000                | 03/08/2018                          | 3000                   | 03/08/2017                            |
#            | learner a | 52            | 03/11/2018 | 01/08/2019       |                 | continuing        | 4500                 | 03/11/2018                          | 1125                   | 03/11/2017                            |
#        #Then the data lock status of the ILR in 03/12/2018 is:
#        #    | Payment type | 08/18               | 09/18               | 10/18               | 11/18               | 12/18               |
#        #    | On-program   | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-002 | commitment 1 v1-002 |
#        Then the provider earnings and payments break down as follows:
#            | Type                       | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 |
#            | Provider Earned Total      | 1000  | 1000  | 1000  | 500   | 500   |
#            | Provider Earned from SFA   | 1000  | 1000  | 1000  | 500   | 500   |
#            | Provider Paid by SFA       | 0     | 1000  | 1000  | 1000  | 500   |
#            | Levy account debited       | 0     | 1000  | 1000  | 1000  | 500   |
#            | SFA Levy employer budget   | 1000  | 1000  | 1000  | 500   | 500   |
#            | SFA Levy co-funding budget | 0     | 0     | 0     | 0     | 0     |

Feature: Levy learner changes course and there is a change in price
	As a provider,
	I want a levy learner, that changes standard with change to negotiated price, to be paid correct amount
	So that I am accurately paid my apprenticeship provision. PV2-290

Scenario Outline: Levy learner change to standard at the end of a month along with change in price PV2-290

Given the employer levy account balance in collection period Current Academic Year/R01 is 15500
And the following commitments exist
	| version Id | standard code | start date                   | end date                  | agreed price | effective from               | effective to                 |
	| 1-001      | 51            | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 15000        | 01/Aug/Current Academic Year | 31/Oct/Current Academic Year |
	| 1-002      | 52            | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 5625         | 03/Nov/Current Academic Year |                              |
And the provider previously submitted the following learner details
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
	| 03/Aug/Current Academic Year | 12 months        | 12000                | 03/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          | 3 months        | withdrawn         | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
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
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
    | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
    | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |        
But the Provider now changes the Learner details as follows
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
	| 03/Nov/Current Academic Year | 9 months         | 4500                 | 03/Nov/Current Academic Year        | 1125                   | 03/Nov/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 52            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

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
And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | SFA Levy Payments | Transaction Type |
    | R04/Current Academic Year | Nov/Current Academic Year | 500               | Learning         |
    | R05/Current Academic Year | Dec/Current Academic Year | 500               | Learning         |
And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | SFA Levy Payments | Transaction Type |
    | R04/Current Academic Year | Nov/Current Academic Year | 500               | Learning         |
    | R05/Current Academic Year | Dec/Current Academic Year | 500               | Learning         |

Examples: 
    | Collection_Period         |
    | R04/Current Academic Year |
    | R05/Current Academic Year |