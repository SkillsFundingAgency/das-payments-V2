@ignore
Feature: One Levy learner, goes on a planned break which is recorded in ILR  PV2-296
As a provider,
I want a levy learner, that goes on a planned break which is recorded in ILR, to be paid the correct amount
So that I am accurately paid my apprenticeship PV2-296

Scenario Outline: One Levy learner, goes on a planned break which is recorded in ILR  PV2-296

Given the employer levy account balance in collection period R02/Current Academic Year is 17000

And the following commitments exist
	| start date                   | end date                  | status | agreed price | effective from               | effective to                 |
	| 01/Sep/Current Academic Year | 30/Sep/Next Academic Year | Active | 15000        | 01/Sep/Current Academic Year | 31/Oct/Current Academic Year |
	| 01/Sep/Current Academic Year | 30/Sep/Next Academic Year | Paused | 15000        | 01/Nov/Current Academic Year | 02/Jan/Current Academic Year |
																							
And the provider previously submitted the following learner details
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
    | 01/Sep/Current Academic Year | 12 months        | 12000                | 01/Sep/Current Academic Year        | 3000                   | 01/Sep/Next Academic Year             | 2 months        | planned break     | Act1          | 1                   | ZPROG001      | 55            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

And the following earnings had been generated for the learner
    | Delivery Period           | On-Programme | Completion | Balancing |
    | Aug/Current Academic Year | 0            | 0          | 0         |
    | Sep/Current Academic Year | 1000         | 0          | 0         |
    | Oct/Current Academic Year | 1000         | 0          | 0         |
    | Nov/Current Academic Year | 0            | 0          | 0         |
    | Dec/Current Academic Year | 0            | 0          | 0         |
    | Jan/Current Academic Year | 0            | 0          | 0         |
    | Feb/Current Academic Year | 0            | 0          | 0         |
    | Mar/Current Academic Year | 0            | 0          | 0         |
    | Apr/Current Academic Year | 0            | 0          | 0         |
    | May/Current Academic Year | 0            | 0          | 0         |
    | Jun/Current Academic Year | 0            | 0          | 0         |
    | Jul/Current Academic Year | 0            | 0          | 0         |

And the following provider payments had been generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R02/Current Academic Year | Sep/Current Academic Year | 1000          | Learning         |
    | R03/Current Academic Year | Oct/Current Academic Year | 1000          | Learning         |

But the Commitment details are changed as follows

	| start date                   | end date                  | status | agreed price | effective from               | effective to                 |
	| 01/Sep/Current Academic Year | 30/Sep/Next Academic Year | Active | 15000        | 01/Sep/Current Academic Year | 31/Oct/Current Academic Year |
	| 01/Sep/Current Academic Year | 30/Sep/Next Academic Year | Paused | 15000        | 01/Nov/Current Academic Year | 02/Jan/Current Academic Year |
	| 01/Sep/Current Academic Year | 30/Sep/Next Academic Year | Active | 15000        | 03/Jan/Current Academic Year |                              |
		
And the Provider now changes the Learner details as follows
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Completion Status | Actual Duration | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
	| 03/Jan/Current Academic Year | 10 months        | 12000                | 03/Jan/Current Academic Year        | 3000                   | 03/Jan/Current Academic Year          | continuing        |                 | Act1          | 1                   | ZPROG001      | 55            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

And price details as follows
    | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date |SFA Contribution Percentage |
    | pe-1             | 12000                | 01/Sep/Current Academic Year        | 3000                   | 01/Sep/Current Academic Year          |90%                         |
    | pe-2             | 12000                | 03/Jan/Current Academic Year        | 3000                   | 03/Jan/Current Academic Year          |90%                         |

When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>

Then the following learner earnings should be generated
    | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
    | Aug/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
    | Oct/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
    | Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Jan/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
    | Feb/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
    | Mar/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
    | Apr/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
    | May/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
    | Jun/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
    | Jul/Current Academic Year | 1000         | 0          | 0         | pe-2                     |

And at month end only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |
    | R07/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         |
    | R08/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         |
    | R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
    | R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
    | R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
    | R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |

And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
    | R07/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         |
    | R08/Current Academic Year | Mar/Current Academic Year | 1000          | Learning         |
    | R09/Current Academic Year | Apr/Current Academic Year | 1000          | Learning         |
    | R10/Current Academic Year | May/Current Academic Year | 1000          | Learning         |
    | R11/Current Academic Year | Jun/Current Academic Year | 1000          | Learning         |
    | R12/Current Academic Year | Jul/Current Academic Year | 1000          | Learning         |

And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
    | R07/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         |
    | R08/Current Academic Year | Mar/Current Academic Year | 1000          | Learning         |
    | R09/Current Academic Year | Apr/Current Academic Year | 1000          | Learning         |
    | R10/Current Academic Year | May/Current Academic Year | 1000          | Learning         |
    | R11/Current Academic Year | Jun/Current Academic Year | 1000          | Learning         |
    | R12/Current Academic Year | Jul/Current Academic Year | 1000          | Learning         |

Examples: 
        | Collection_Period         |
        | R06/Current Academic Year |
        | R07/Current Academic Year |
        | R08/Current Academic Year |
        | R09/Current Academic Year |
        | R10/Current Academic Year |
        | R11/Current Academic Year |
        | R12/Current Academic Year |



 #Scenario: Apprentice goes on a planned break midway through the learning episode and this is notified through the ILR
 #       Given the following commitments exist on 03/12/2018:
 #           | commitment Id | version Id | ULN       | start date | end date   | status | agreed price | effective from | effective to |
 #           | 1             | 1-001      | learner a | 01/09/2018 | 08/09/2019 | active | 15000        | 01/09/2018     | 31/10/2018   |
 #           | 1             | 1-002      | learner a | 01/09/2018 | 08/09/2019 | paused | 15000        | 01/11/2018     | 02/01/2019   |
 #           | 1             | 1-003      | learner a | 01/09/2018 | 08/09/2019 | active | 15000        | 03/01/2019     |              |
 #       When an ILR file is submitted on 03/12/2018 with the following data:
 #           | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
 #           | learner a | 01/09/2018 | 08/09/2019       | 31/10/2018      | planned break     | 12000                | 01/09/2018                          | 3000                   | 01/09/2018                            |
 #           | learner a | 03/01/2019 | 08/11/2019       |                 | continuing        | 12000                | 03/01/2019                          | 3000                   | 03/01/2019                            |
 #       Then the provider earnings and payments break down as follows:
 #           | Type                     | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 | 02/19 | ... | 10/19 | 11/19 |
 #           | Provider Earned from SFA | 1000  | 1000  | 0     | 0     | 1000  | 1000  | ... | 1000  | 0     |
 #           | Provider Paid by SFA     | 0     | 1000  | 1000  | 0     | 0     | 1000  | ... | 1000  | 1000  |
 #           | Levy account debited     | 0     | 1000  | 1000  | 0     | 0     | 1000  | ... | 1000  | 1000  |
 #           | SFA Levy employer budget | 1000  | 1000  | 0     | 0     | 1000  | 1000  | ... | 1000  | 0     |
