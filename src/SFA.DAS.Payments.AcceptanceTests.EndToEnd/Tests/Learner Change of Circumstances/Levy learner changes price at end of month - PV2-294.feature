Feature: Levy learner, and there is a change to the negotiated price which happens at the end of the month - PV2-294
	As a provider,
	I want a levy learner, changes to the agreed price at the end of the month, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.

Scenario Outline: Levy learner changes to the agreed price at the end of the month PV2-294
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
		| version Id | start date                   | end date                  | agreed price | effective from               | effective to                 |
		| 1          | 01/Aug/Current Academic Year | 31/Aug/Next Academic Year | 15000        | 01/Aug/Current Academic Year | 31/Oct/Current Academic Year |
		| 2          | 01/Aug/Current Academic Year | 31/Aug/Next Academic Year | 9375         | 01/Nov/Current Academic Year |                              |

	And the following aims
		| Aim Reference 	| Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Programme Type | Standard Code | Funding Line Type         | Completion Status |
		| ZPROG001      	| 01/Aug/Current Academic Year | 12 months        |                 | 1                   | 25             | 51            | 19-24 Apprenticeship Levy | continuing        |

	And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          | Act1          | 1                   | 90%                         |
        | pe-2             | 7500                 | 01/Nov/Current Academic Year        | 1875                   | 01/Nov/Current Academic Year          | Act1          | 1                   | 90%                         |

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

    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 500           | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 500           | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 500           | Learning         |

    And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |
		| R04/Current Academic Year | Nov/Current Academic Year | 500           | Learning         |
		| R05/Current Academic Year | Dec/Current Academic Year | 500           | Learning         |
		| R06/Current Academic Year | Jan/Current Academic Year | 500           | Learning         |

Examples: 
		| Collection_Period         | Levy Balance |
		| R01/Current Academic Year | 15500        |
		| R02/Current Academic Year | 14500        |
		| R03/Current Academic Year | 13500        |
		| R04/Current Academic Year | 12500        |
		| R05/Current Academic Year | 12000        |
		| R05/Current Academic Year | 11500        |

 #Scenario: Earnings and payments for a DAS learner, levy available, and there is a change to the Negotiated Cost which happens at the end of the month
 #       Given the following commitments exist:
 #           | commitment Id | version Id | ULN       | start date | end date   | status | agreed price | effective from | effective to |
 #           | 1             | 1-001      | learner a | 01/08/2018 | 31/08/2019 | active | 15000        | 01/08/2018     | 31/10/2018   |
 #           | 1             | 1-002      | learner a | 01/08/2018 | 31/08/2019 | active | 9375         | 01/11/2018     |              |
 #       When an ILR file is submitted with the following data:
 #           | ULN       | start date | planned end date | actual end date | completion status | Total training price 1 | Total training price 1 effective date | Total assessment price 1 | Total assessment price 1 effective date | Total training price 2 | Total training price 2 effective date | Total assessment price 2 | Total assessment price 2 effective date |
 #           | learner a | 01/08/2018 | 04/08/2019       |                 | continuing        | 12000                  | 01/08/2018                            | 3000                     | 01/08/2018                              | 7500                   | 01/11/2018                            | 1875                     | 01/11/2018                              |
 #       #Then the data lock status will be as follows:
 #       #    | Payment type | 08/18               | 09/18               | 10/18               | 11/18               | 12/18               | ... | 07/19               |
 #       #    | On-program   | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-002 | commitment 1 v1-002 | ... | commitment 1 v1-002 | 
 #       Then the provider earnings and payments break down as follows:
 #           | Type                          | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 07/19 | 08/19 |
 #           | Provider Earned Total         | 1000  | 1000  | 1000  | 500   | 500   | ... | 500   | 0     |
 #           | Provider Earned from SFA      | 1000  | 1000  | 1000  | 500   | 500   | ... | 500   | 0     |
 #           | Provider Earned from Employer | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
 #           | Provider Paid by SFA          | 0     | 1000  | 1000  | 1000  | 500   | ... | 500   | 500   |
 #           | Payment due from Employer     | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
 #           | Levy account debited          | 0     | 1000  | 1000  | 1000  | 500   | ... | 500   | 500   |
 #           | SFA Levy employer budget      | 1000  | 1000  | 1000  | 500   | 500   | ... | 500   | 0     |
 #           | SFA Levy co-funding budget    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |