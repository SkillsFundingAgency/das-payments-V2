Feature: Earnings and payments for a levy learner, switches from levy to non levy employer at the end of month- PV2-362
As a provider,
I want earnings and payments for a levy learner, levy available, where a learner switches from levy to non-levy employer at the end of month, to be paid the correct amount
So that I am accurately paid my apprenticeship provision.

Scenario Outline: Earnings and payments for a levy learner, switches from levy to non levy employer at the end of month- PV2-362
Given the "employer 1" levy account balance in collection period R01/Current Academic Year is 15000
And the following commitments exist 
	| Employer   | start date                   | end date                     | agreed price | status | effective from            | effective to |
	| employer 1 | 01/Aug/Current Academic Year | 04/Aug/Current Academic Year | 15000        | active | 01/Aug/Last Academic Year |              |

And the provider previously submitted the following learner details
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
	| 03/Aug/Current Academic Year | 12 months        | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

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
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                      |
	| 03/Aug/Current Academic Year | 12 months        | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract |

And price details as follows
	| Price details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage |
	| 1st price details | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         |
	| 2nd price details | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          | 4500                    | 03/Nov/Current Academic Year           | 1125                      | 03/Nov/Current Academic Year             | 90%                         |

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
	| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer   |
	| R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |

And only the following provider payments will be generated
	| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer   |
	| R04/Current Academic Year | Nov/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R05/Current Academic Year | Dec/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R06/Current Academic Year | Jan/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R07/Current Academic Year | Feb/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R08/Current Academic Year | Mar/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R09/Current Academic Year | Apr/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R10/Current Academic Year | May/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R11/Current Academic Year | Jun/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
	| R12/Current Academic Year | Jul/Current Academic Year | 450                    | 50                          | Learning         | employer 2 |
Examples: 
	| Collection_Period         |
	| R04/Current Academic Year |
	| R05/Current Academic Year |
	| R06/Current Academic Year |
	| R07/Current Academic Year |
	| R08/Current Academic Year |
	| R09/Current Academic Year |
	| R10/Current Academic Year |
	| R11/Current Academic Year |
	| R12/Current Academic Year |

		#Scenario: Earnings and payments for a levy learner, levy available, where a learner switches from DAS to Non Das employer at the end of month
#        Given The learner is programme only DAS
#        And the employer 1 has a levy balance > agreed price for all months
#        
#        And the learner changes employers
#            | Employer   | Type    | ILR employment start date |
#            | employer 1 | DAS     | 03/08/2018                |
#            | employer 2 | Non DAS | 03/11/2018                |
#        
#        And the following commitments exist on 03/12/2018:
#            | Employer   | commitment Id | version Id | ULN       | start date | end date   | agreed price | status    | effective from | effective to | stop effective from |
#            | employer 1 | 1             | 1-001      | learner a | 03/08/2018 | 04/08/2019 | 15000        | Cancelled | 03/08/2018     | 02/11/2018   | 03/11/2018   |
#        
#        When an ILR file is submitted on 03/12/2018 with the following data:
#            | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | Residual training price | Residual training price effective date | Residual assessment price | Residual assessment price effective date |
#            | learner a | 03/08/2018 | 04/08/2019       |                 | continuing        | 12000                | 03/08/2018                          | 3000                   | 03/08/2018                            | 4500                    | 03/11/2018                             | 1125                      | 03/11/2018                               |
#        
#        And the Contract type in the ILR is:
#            | contract type | date from  | date to    |
#            | DAS           | 03/08/2018 | 02/11/2018 |
#            | Non-DAS       | 03/11/2018 | 04/08/2019 |
#        
#        #Then the data lock status of the ILR in 03/12/2018 is:
#        #    | Payment type | 08/18               | 09/18               | 10/18               | 11/18 | 12/18 |
#        #    | On-program   | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-001 |       |       |
#        
#        Then the provider earnings and payments break down as follows:
#            | Type                            | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 |
#            | Provider Earned Total           | 1000  | 1000  | 1000  | 500   | 500   |
#            | Provider Earned from SFA        | 1000  | 1000  | 1000  | 450   | 450   |
#            | Provider Earned from Employer 1 | 0     | 0     | 0     | 0     | 0     |
#            | Provider Earned from Employer 2 | 0     | 0     | 0     | 50    | 50    |
#            | Provider Paid by SFA            | 0     | 1000  | 1000  | 1000  | 450   |
#            | Payment due from employer 1     | 0     | 0     | 0     | 0     | 0     |
#            | Payment due from employer 2     | 0     | 0     | 0     | 0     | 50    |
#            | Employer 1 Levy account debited | 0     | 1000  | 1000  | 1000  | 0     |
#            | SFA Levy employer budget        | 1000  | 1000  | 1000  | 0     | 0     |
#            | SFA Levy co-funding budget      | 0     | 0     | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget  | 0     | 0     | 0     | 450   | 450   |

# For DC integration
    #    And the learner changes employers
        #| Employer   | Type    | ILR employment start date |
        #| employer 1 | DAS     | 03/08/2018                |
        #| employer 2 | Non DAS | 03/11/2018                |