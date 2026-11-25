Feature: PV2-296 - One Levy learner, goes on a planned break which is recorded in ILR
	I want a levy learner, that goes on a planned break which is recorded in ILR, to be paid the correct amount
	So that I am accurately paid my apprenticeship PV2-296

Scenario Outline: PV2-296 - One Levy learner, goes on a planned break which is recorded in ILR

Given the employer levy account balance in collection period R02/2025-26 Academic Year is 17000

And the following apprenticeships exist
	| start date                   | end date                  | agreed price | Standard Code | Programme Type | status | Effective From               | Effective To              |
	| 01/Sep/2025-26 Academic Year | 30/Sep/2026-27 Academic Year | 15000        | 55            | 25             | Active | 01/Sep/2025-26 Academic Year | 30/Sep/2026-27 Academic Year |

And the apprenticeships status changes as follows 
	| Collection Period         | Status |
	| R04/2025-26 Academic Year | Paused |
	| R06/2025-26 Academic Year | Active |

And the provider previously submitted the following learner details
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
    | 01/Sep/2025-26 Academic Year | 12 months        | 12000                | 01/Sep/2025-26 Academic Year        | 3000                   | 01/Sep/2026-27 Academic Year             | 2 months        | planned break     | Act1          | 1                   | ZPROG001      | 55            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

And price details as follows		
    | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
    | pe-1             | 12000                | 01/Sep/2025-26 Academic Year        |                        | 01/Sep/2025-26 Academic Year          | Act1          | 1                   | 90%                         |

And the following earnings had been generated for the learner
    | Delivery Period           | On-Programme | Completion | Balancing | Aim Sequence Number | Price Episode Identifier |
    | Aug/2025-26 Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Sep/2025-26 Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
    | Oct/2025-26 Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
    | Nov/2025-26 Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Dec/2025-26 Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Jan/2025-26 Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Feb/2025-26 Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Mar/2025-26 Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Apr/2025-26 Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | May/2025-26 Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Jun/2025-26 Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Jul/2025-26 Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |

And the following provider payments had been generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R02/2025-26 Academic Year | Sep/2025-26 Academic Year | 1000          | Learning         |
    | R03/2025-26 Academic Year | Oct/2025-26 Academic Year | 1000          | Learning         |
	
And the Provider now changes the Learner details as follows
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Completion Status | Contract Type | Aim Sequence Number | Standard Code | Programme Type |Aim Reference | Funding Line Type                                  | 
	| 03/Jan/2025-26 Academic Year | 10 months        | 12000                | 03/Jan/2025-26 Academic Year        | 3000                   | 03/Jan/2025-26 Academic Year          | continuing        | Act1          | 1                   | 55            | 25             |ZPROG001      | 16-18 Apprenticeship (From May 2017) Levy Contract | 

And price details as follows
    | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
    | pe-1             | 12000                | 01/Sep/2025-26 Academic Year        | 3000                   | 01/Sep/2025-26 Academic Year          | 90%                         | Act1          |
    | pe-2             | 12000                | 03/Jan/2025-26 Academic Year        | 3000                   | 03/Jan/2025-26 Academic Year          | 90%                         | Act1          |

When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>

Then the following learner earnings should be generated
    | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
    | Aug/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
    | Sep/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
    | Oct/2025-26 Academic Year | 1000         | 0          | 0         | pe-1                     |
    | Nov/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
    | Dec/2025-26 Academic Year | 0            | 0          | 0         | pe-1                     |
    | Jan/2025-26 Academic Year | 1000         | 0          | 0         | pe-2                     |
    | Feb/2025-26 Academic Year | 1000         | 0          | 0         | pe-2                     |
    | Mar/2025-26 Academic Year | 1000         | 0          | 0         | pe-2                     |
    | Apr/2025-26 Academic Year | 1000         | 0          | 0         | pe-2                     |
    | May/2025-26 Academic Year | 1000         | 0          | 0         | pe-2                     |
    | Jun/2025-26 Academic Year | 1000         | 0          | 0         | pe-2                     |
    | Jul/2025-26 Academic Year | 1000         | 0          | 0         | pe-2                     |

And at month end only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | R06/2025-26 Academic Year | Jan/2025-26 Academic Year | 1000         | 0          | 0         |
    | R07/2025-26 Academic Year | Feb/2025-26 Academic Year | 1000         | 0          | 0         |
    | R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 1000         | 0          | 0         |
    | R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 1000         | 0          | 0         |
    | R10/2025-26 Academic Year | May/2025-26 Academic Year | 1000         | 0          | 0         |
    | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 1000         | 0          | 0         |
    | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 1000         | 0          | 0         |

And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R06/2025-26 Academic Year | Jan/2025-26 Academic Year | 1000          | Learning         |
    | R07/2025-26 Academic Year | Feb/2025-26 Academic Year | 1000          | Learning         |
    | R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 1000          | Learning         |
    | R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 1000          | Learning         |
    | R10/2025-26 Academic Year | May/2025-26 Academic Year | 1000          | Learning         |
    | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 1000          | Learning         |
    | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 1000          | Learning         |

And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R06/2025-26 Academic Year | Jan/2025-26 Academic Year | 1000          | Learning         |
    | R07/2025-26 Academic Year | Feb/2025-26 Academic Year | 1000          | Learning         |
    | R08/2025-26 Academic Year | Mar/2025-26 Academic Year | 1000          | Learning         |
    | R09/2025-26 Academic Year | Apr/2025-26 Academic Year | 1000          | Learning         |
    | R10/2025-26 Academic Year | May/2025-26 Academic Year | 1000          | Learning         |
    | R11/2025-26 Academic Year | Jun/2025-26 Academic Year | 1000          | Learning         |
    | R12/2025-26 Academic Year | Jul/2025-26 Academic Year | 1000          | Learning         |

Examples: 
        | Collection_Period         |
        | R04/2025-26 Academic Year |
        | R05/2025-26 Academic Year |
        | R06/2025-26 Academic Year |
        | R07/2025-26 Academic Year |
        | R08/2025-26 Academic Year |
        | R09/2025-26 Academic Year |
        | R10/2025-26 Academic Year |
        | R11/2025-26 Academic Year |
        | R12/2025-26 Academic Year |


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