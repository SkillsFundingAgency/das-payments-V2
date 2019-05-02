Feature: One Levy learner changes employer with change to negotiated price earlier than expected PV2-361
Change in employer and change in negotiated price in mid-month
As a provider,
I want earnings and payments for a levy learner, levy available, and there is a change to the Negotiated Cost earlier than expected to be paid the correct amount
So that I am accurately paid my apprenticeship provision.

Scenario Outline: Levy learner changes employer with change to negotiated price earlier than expected PV2-361
Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
And  the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
And the following commitments exist 
		| Identifier              | Employer   | start date                   | end date                     | agreed price | status    | effective from               | effective to                 | stop effective from          |
		| Apprentiiship 1         | employer 1 | 01/Aug/Current Academic Year | 31/Jul/Current Academic Year | 15000        | cancelled | 01/Aug/Current Academic Year | 31/Oct/Current Academic Year | 01/Nov/Current Academic Year |  
		| Apprentiiship 2         | employer 2 | 01/Nov/Current Academic Year | 31/Jul/Current Academic Year | 5625         | active    | 01/Nov/Current Academic Year |                              |                              |

And the provider previously submitted the following learner details
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
	| 04/Aug/Current Academic Year | 12 months        | 12000                | 04/Aug/Current Academic Year        | 3000                   | 04/Aug/Current Academic Year          |                 | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |

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
 
But the Provider now changes the Learner details as follows
	| Start Date                   | Planned Duration | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
	| 01/Aug/Current Academic Year | 12 months        |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |

And price details as follows
	| Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage |
	| pe-1          | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         |
	| pe-2          | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          | 5000                    | 25/Oct/Current Academic Year           | 625                       | 25/Oct/Current Academic Year             | 90%                         |

When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	
Then the following learner earnings should be generated
	| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
	| Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
	| Sep/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
	| Oct/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
	| Nov/Current Academic Year | 450          | 0          | 0         | pe-2                     |
	| Dec/Current Academic Year | 450          | 0          | 0         | pe-2                     |
	| Jan/Current Academic Year | 450          | 0          | 0         | pe-2                     |
	| Feb/Current Academic Year | 450          | 0          | 0         | pe-2                     |
	| Mar/Current Academic Year | 450          | 0          | 0         | pe-2                     |
	| Apr/Current Academic Year | 450          | 0          | 0         | pe-2                     |
	| May/Current Academic Year | 450          | 0          | 0         | pe-2                     |
	| Jun/Current Academic Year | 450          | 0          | 0         | pe-2                     |
	| Jul/Current Academic Year | 450          | 0          | 0         | pe-2                     |
	
And Month end is triggered
And no provider payments will be recorded
And no provider payments will be generated

Examples: 
        | Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 |
        | R03/Current Academic Year | 13500                       | 7500                        |
        | R04/Current Academic Year | 13500                       | 7500                        |









  #		Scenario: Earnings and payments for a levy learner, levy available, and there is a change to the Negotiated Cost earlier than expected
  #      Given The learner is programme only DAS
  #      And the employer 1 has a levy balance > agreed price for all months
  #      And the employer 2 has a levy balance > agreed price for all months
  #      
  #      And the learner changes employers
  #          | Employer   | Type | ILR employment start date |
  #          | employer 1 | DAS  | 04/08/2018                |
  #          | employer 2 | DAS  | 10/11/2018                |
  #      
  #      And the following commitments exist on 03/12/2018:
  #          | Employer   | commitment Id | version Id | ULN       | start date | end date   | agreed price | status    | effective from | effective to | stop effective from |
  #          | employer 1 | 1             | 1-001      | learner a | 01/08/2018 | 31/08/2019 | 15000        | cancelled | 01/08/2018     | 31/10/2018   | 01/11/2018   |
  #          | employer 2 | 2             | 1-001      | learner a | 01/11/2018 | 31/08/2019 | 5625         | active    | 01/11/2018     |              |              |
  #      
  #      When an ILR file is submitted on 03/12/2018 with the following data:
  #          | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | Residual training price | Residual training price effective date | Residual assessment price | Residual assessment price effective date |
  #          | learner a | 04/08/2018 | 04/08/2019       |                 | continuing        | 12000                | 04/08/2018                          | 3000                   | 04/08/2018                            | 5000                    | 25/10/2018                             | 625                       | 25/10/2018                               |
  #      
  #      #Then the data lock status of the ILR in 03/12/2018 is:
  #      #    | Payment type | 08/18               | 09/18               | 10/18 | 11/18 | 12/18 |
  #      #    | On-program   | commitment 1 v1-001 | commitment 1 v1-001 |       |       |       |
  #      
  #      Then the provider earnings and payments break down as follows:
  #          | Type                            | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 |
  #          | Provider Earned Total           | 1000  | 1000  | 450   | 450   | 450   |
  #          | Provider Earned from SFA        | 1000  | 1000  | 450   | 450   | 450   |
  #          | Provider Earned from Employer 1 | 0     | 0     | 0     | 0     | 0     |
  #          | Provider Earned from Employer 2 | 0     | 0     | 0     | 0     | 0     |
  #          | Provider Paid by SFA            | 0     | 1000  | 1000  | 0     | 0     |
  #          | Payment due from employer 1     | 0     | 0     | 0     | 0     | 0     |
  #          | Payment due from employer 2     | 0     | 0     | 0     | 0     | 0     |
  #          | Employer 1 Levy account debited | 0     | 1000  | 1000  | 0     | 0     |
  #          | Employer 2 Levy account debited | 0     | 0     | 0     | 0     | 0     |
  #          | SFA Levy employer budget        | 1000  | 1000  | 0     | 0     | 0     |
  #          | SFA Levy co-funding budget      | 0     | 0     | 0     | 0     | 0     |

  # For DC Integration
  #      And the learner changes employers
  #          | Employer   | Type | ILR employment start date |
  #          | employer 1 | DAS  | 04/08/2018                |
  #          | employer 2 | DAS  | 10/11/2018                |

