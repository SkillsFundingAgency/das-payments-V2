 Feature: 5% Contribution from April 2019 PV2-906
As a provider,
I want a Levy learner, starting prior to Apr 2019, where learner takes a planned break and resumes in Apr 2019, and moves into co-funding on 10% contribution
So that I am paid the correct apprenticeship funding by SFA

Scenario Outline: Levy Learner, started learning before Apr19 with Levy balance, takes a planned break and resumes from Apr19, moves into co-funding on 10% contribution PV2-906

Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
		
And the following apprenticeships exist
	| Identifier       | start date                   | end date                  | agreed price | status    | effective from               | effective to                 | stop effective from          | Standard Code | Programme Type |
	| Apprenticeship 1 | 01/Nov/Current Academic Year | 01/Nov/Next Academic Year | 15000        | cancelled | 01/Nov/Current Academic Year | 28/Feb/Current Academic Year | 01/Mar/Current Academic Year | 17            | 25             |
	| Apprenticeship 2 | 01/Apr/Current Academic Year | 01/Apr/Next Academic Year | 15000        | active    | 01/Apr/Current Academic Year |                              |                              | 17            | 25             |

And the following aims
	| Aim Type  | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Standard Code | Programme Type | Funding Line Type                                  | Completion Status | Contract Type | Aim Sequence Number |
	| Programme | ZPROG001      | 01/Nov/Current Academic Year | 12 months        | 4 months        | 1                   | 17            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | planned break     | Act1          | 1                   |
																														   
And price details as follows
    | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Aim Sequence Number | Contract Type |
    | pe-1             | 12000                | 01/Nov/Current Academic Year        | 3000                   | 01/Nov/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | 1                   | Act1          |

And the following earnings had been generated for the learner
    | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
    | Aug/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Sep/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Nov/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
    | Dec/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
    | Jan/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
    | Feb/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
    | Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Apr/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | May/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Jun/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Jul/Current Academic Year | 0            | 0          | 0         | pe-1                     |

And the following provider payments had been generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R04/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         |
    | R05/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         |
    | R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
    | R07/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         |

But aims details are changed as follows
	| Aim Type  | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Standard Code | Programme Type | Funding Line Type                                  | Completion Status | Contract Type |
	| Programme | ZPROG001      | 01/Apr/Current Academic Year | 12 months        |                 | 1                   | 17            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | continuing        | Act1          |
	
And price details as follows
    | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Aim Sequence Number | Contract Type |
    | pe-1             | 12000                | 01/Nov/Current Academic Year        | 3000                   | 01/Nov/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | 1                   | Act1          |
    | pe-2             | 12000                | 01/Apr/Current Academic Year        | 3000                   | 01/Apr/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | 1                   | Act1          | 

When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>

Then the following learner earnings should be generated
    | Delivery Period           | On-Programme | Completion | Balancing | Aim Sequence Number | Price Episode Identifier |
    | Aug/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Sep/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Oct/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Nov/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
    | Dec/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
    | Jan/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
    | Feb/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
    | Mar/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
    | Apr/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-2                     |
    | May/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-2                     |
    | Jun/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-2                     |
    | Jul/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-2                     |

And at month end only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
    | R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
    | R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
    | R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |

And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
    | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | 0             | Learning         |
    | R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | 0             | Learning         |
    | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | 0             | Learning         |
    | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | 0             | Learning         |

And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
    | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | 0             | Learning         |
    | R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | 0             | Learning         |
    | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | 0             | Learning         |
    | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | 0             | Learning         |
    
 Examples: 
        | Collection_Period         | Levy Balance |
        | R09/Current Academic Year | 0            |
        | R10/Current Academic Year | 0            |
        | R11/Current Academic Year | 0            |
        | R12/Current Academic Year | 0            |