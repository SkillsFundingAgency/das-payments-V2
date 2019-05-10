﻿Feature: Levy learner changes course and there is a change in price - PV2-290
	As a provider,
	I want a levy learner, that changes standard with change to negotiated price, to be paid correct amount
	So that I am accurately paid my apprenticeship provision. PV2-290

Scenario Outline: Levy learner change to standard at the end of a month along with change in price PV2-290

	Given the employer levy account balance in collection period R01/Current Academic Year is 15500

	And the following commitments exist
		| Identifier      | standard code | start date                   | end date                  | agreed price | effective from               | effective to                 |
		| Apprenticeship 1 | 51            | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 15000        | 01/Aug/Current Academic Year | 31/Oct/Current Academic Year |
		| Apprenticeship 2 | 52            | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 5625         | 03/Nov/Current Academic Year |                              |

	And the following aims
		| Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Programme Type | Standard Code | Funding Line Type             | Completion Status |
		| ZPROG001      | 03/Aug/Current Academic Year | 12 months        | 3 months        | 1                   | 25             | 51            | 16-18 Apprenticeship Non-Levy | withdrawn         |
		| ZPROG001      | 03/Nov/Current Academic Year | 9 months         |                 | 2                   | 25             | 52            | 16-18 Apprenticeship Non-Levy | continuing        |

	And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          | Act1          | 1                   | 90%                         |
        | pe-2             | 4500                 | 03/Nov/Current Academic Year        | 1125                   | 03/Nov/Current Academic Year          | Act1          | 2                   | 90%                         |

    When the ILR file is submitted for the learners for collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | Aim Sequence Number | Price Episode Identifier |
        #p1
        | Aug/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
        | Sep/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
        | Oct/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
        | Nov/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Dec/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Jan/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Feb/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Mar/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Apr/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | May/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Jun/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        | Jul/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
        #p2
        | Aug/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
        | Sep/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
        | Oct/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
        | Nov/Current Academic Year | 500          | 0          | 0         | 2                   | pe-2                     |
        | Dec/Current Academic Year | 500          | 0          | 0         | 2                   | pe-2                     |
        | Jan/Current Academic Year | 500          | 0          | 0         | 2                   | pe-2                     |
        | Feb/Current Academic Year | 500          | 0          | 0         | 2                   | pe-2                     |
        | Mar/Current Academic Year | 500          | 0          | 0         | 2                   | pe-2                     |
        | Apr/Current Academic Year | 500          | 0          | 0         | 2                   | pe-2                     |
        | May/Current Academic Year | 500          | 0          | 0         | 2                   | pe-2                     |
        | Jun/Current Academic Year | 500          | 0          | 0         | 2                   | pe-2                     |
        | Jul/Current Academic Year | 500          | 0          | 0         | 2                   | pe-2                     |

	And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
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
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 500           | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 500           | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 500           | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 500           | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 500           | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 500           | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 500           | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 500           | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 500           | Learning         |

    And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
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
        | Collection_Period         |
        | R01/Current Academic Year |
        | R02/Current Academic Year |
        | R03/Current Academic Year |
        | R04/Current Academic Year |
        | R05/Current Academic Year |
        | R06/Current Academic Year |
        | R07/Current Academic Year |
        | R08/Current Academic Year |
        | R09/Current Academic Year |
        | R10/Current Academic Year |
        | R11/Current Academic Year |
        | R12/Current Academic Year |


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