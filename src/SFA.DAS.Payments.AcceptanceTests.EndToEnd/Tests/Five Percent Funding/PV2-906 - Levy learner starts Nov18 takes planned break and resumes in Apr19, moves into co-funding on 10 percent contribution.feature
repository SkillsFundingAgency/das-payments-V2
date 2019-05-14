@ignore
Feature: 5% Contribution from April 2019 PV2-906
As a provider,
I want a Levy learner, starting prior to Apr 2019, where learner takes a planned break and resumes in Apr 2019, and moves into co-funding on 10% contribution
So that I am paid the correct apprenticeship funding by SFA

Scenario Outline: Levy Learner, started learning before Apr19 with Levy balance, takes a planned break and resumes from Apr19, moves into co-funding on 10% contribution PV2-906

Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
		
And the following apprenticeships exist
	| Standard code | programme type | agreed price | start date                   | end date                  | status        | effective from               |
	| 17            | 25             | 15000        | 01/Nov/Current Academic Year | 01/Nov/Next Academic Year | planned break | 01/Nov/Current Academic Year |

And the provider previously submitted the following learner details
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Standard Code | Programme Type | Aim Reference | Funding Line Type                                  | SFA Contribution Percentage |
    | 01/Nov/Current Academic Year | 12 months        | 12000                | 01/Nov/Current Academic Year        | 3000                   | 01/Nov/Next Academic Year             | 4 months        | planned break     | Act1          | 1                   | 17            | 25             | ZPROG001      | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

And the following earnings had been generated for the learner
    | Delivery Period           | On-Programme | Completion | Balancing |
    | Aug/Current Academic Year | 0            | 0          | 0         |
    | Sep/Current Academic Year | 0            | 0          | 0         |
    | Oct/Current Academic Year | 0            | 0          | 0         |
    | Nov/Current Academic Year | 1000         | 0          | 0         |
    | Dec/Current Academic Year | 1000         | 0          | 0         |
    | Jan/Current Academic Year | 1000         | 0          | 0         |
    | Feb/Current Academic Year | 1000         | 0          | 0         |
    | Mar/Current Academic Year | 0            | 0          | 0         |
    | Apr/Current Academic Year | 0            | 0          | 0         |
    | May/Current Academic Year | 0            | 0          | 0         |
    | Jun/Current Academic Year | 0            | 0          | 0         |
    | Jul/Current Academic Year | 0            | 0          | 0         |

And the following provider payments had been generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R04/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         |
    | R05/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         |
    | R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         |
    | R07/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         |

But the Apprenticeship details are changed as follows

	| Apprenticeship | Provider   | Learner ID | Standard code | programme type | agreed price | start date                   | end date                  | status     | effective from               |
	| Apprentice a   | Provider a | learner a  | 17            | 25             | 15000        | 01/Apr/Current Academic Year | 01/Apr/Next Academic Year | continuing | 01/Apr/Current Academic Year |
	
	
And the Provider now changes the Learner details as follows
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Completion Status | Contract Type | Aim Sequence Number | Standard Code | Programme Type |Aim Reference | Funding Line Type                                  | 
	| 01/Apr/Current Academic Year | 12 months        | 12000                | 01/Apr/Current Academic Year        | 3000                   | 01/Apr/Current Academic Year          | continuing        | Act1          | 1                   | 17            | 25             |ZPROG001      | 16-18 Apprenticeship (From May 2017) Levy Contract | 

And price details as follows
    | Price details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | SFA Contribution Percentage | 
    | 1st price details | 12000                | 01/Nov/Current Academic Year        | 3000                   | 01/Nov/Current Academic Year          | 90%                         | 
    | 2nd price details | 12000                | 01/Apr/Current Academic Year        | 3000                   | 01/Apr/Current Academic Year          | 90%                         | 

When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>

Then the following learner earnings should be generated
    | Delivery Period           | On-Programme | Completion | Balancing |
    | Aug/Current Academic Year | 0            | 0          | 0         |
    | Sep/Current Academic Year | 0            | 0          | 0         |
    | Oct/Current Academic Year | 0            | 0          | 0         |
    | Nov/Current Academic Year | 1000         | 0          | 0         |
    | Dec/Current Academic Year | 1000         | 0          | 0         |
    | Jan/Current Academic Year | 1000         | 0          | 0         |
    | Feb/Current Academic Year | 1000         | 0          | 0         |
    | Mar/Current Academic Year | 0            | 0          | 0         |
    | Apr/Current Academic Year | 1000         | 0          | 0         |
    | May/Current Academic Year | 1000         | 0          | 0         |
    | Jun/Current Academic Year | 1000         | 0          | 0         |
    | Jul/Current Academic Year | 1000         | 0          | 0         |

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