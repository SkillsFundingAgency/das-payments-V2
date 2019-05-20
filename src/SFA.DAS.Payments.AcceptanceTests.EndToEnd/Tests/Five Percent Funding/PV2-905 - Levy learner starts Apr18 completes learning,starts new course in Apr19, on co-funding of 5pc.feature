Feature: 5% Contribution from April 2019 PV2-905
As a provider,
I want a Levy learner, starting prior to Apr 2019, where learner completes learning and starts a new course on new Pathway code in Apr 2019, and moves into co-funding on 5% contribution
So that I am paid the correct apprenticeship funding by SFA

Scenario Outline: Levy Learner, started learning before Apr19, completes learning and starts new course on new pathway code from Apr19, moves into co-funding and second pathway code on 5% contribution PV2-905

Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
		
	And the following apprenticeships exist
		| Identifier       | Framework code | programme type | pathway code | agreed price | start date                   | end date                     | status    | effective from               |
		| Apprenticeship a | 503            | 20             | 1            | 15000        | 01/Apr/Last Academic Year    | 01/Apr/Current Academic Year | completed | 01/Apr/Last Academic Year    |
		| Apprenticeship b | 503            | 20             | 2            | 15000        | 02/Apr/Current Academic Year | 02/Apr/Next Academic Year    | active    | 02/Apr/Current Academic Year |
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Apr/Last Academic Year | 12 months        | 15000                | 01/Apr/Current Academic Year        |                        |                                       |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	And the following earnings had been generated for the learner
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Last Academic Year    | 0            | 0          | 0         |
		| Sep/Last Academic Year    | 0            | 0          | 0         |
		| Oct/Last Academic Year    | 0            | 0          | 0         |
		| Nov/Last Academic Year    | 0            | 0          | 0         |
		| Dec/Last Academic Year    | 0            | 0          | 0         |
		| Jan/Last Academic Year    | 0            | 0          | 0         |
		| Feb/Last Academic Year    | 0            | 0          | 0         |
		| Mar/Last Academic Year    | 0            | 0          | 0         |
		| Apr/Last Academic Year    | 1000         | 0          | 0         |
		| May/Last Academic Year    | 1000         | 0          | 0         |
		| Jun/Last Academic Year    | 1000         | 0          | 0         |
		| Jul/Last Academic Year    | 1000         | 0          | 0         |
		| Aug/Current Academic Year | 1000         | 0          | 0         |
		| Sep/Current Academic Year | 1000         | 0          | 0         |
		| Oct/Current Academic Year | 1000         | 0          | 0         |
		| Nov/Current Academic Year | 1000         | 0          | 0         |
		| Dec/Current Academic Year | 1000         | 0          | 0         |
		| Jan/Current Academic Year | 1000         | 0          | 0         |
		| Feb/Current Academic Year | 1000         | 0          | 0         |
		| Mar/Current Academic Year | 1000         | 3000       | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 0            | 0          | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |

	And the following provider payments had been generated
	    | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
	    | R09/Last Academic Year    | Apr/Last Academic Year    | 900                    | 100                         | Learning         |
	    | R10/Last Academic Year    | May/Last Academic Year    | 900                    | 100                         | Learning         |
	    | R11/Last Academic Year    | Jun/Last Academic Year    | 900                    | 100                         | Learning         |
	    | R12/Last Academic Year    | Jul/Last Academic Year    | 900                    | 100                         | Learning         |
	    | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
	    | R01/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         |
	    | R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         |
	    | R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         |
	    | R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | Learning         |
	    | R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
	    | R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
	    | R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         |
	    | R08/Current Academic Year | Mar/Current Academic Year | 2700                   | 300                         | Completion       |

	But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 01/Apr/Last Academic Year    | 12 months        | 15000                | 01/Apr/Last Academic Year           |                        |                                       | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
		| 02/Apr/Current Academic Year | 12 months        | 15000                | 02/Apr/Current Academic Year        |                        |                                       |                 | continuing        | Act1          | 2                   | ZPROG001      | 593            | 2            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 95%                         |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 0            | 0          | 0         |
		| Sep/Current Academic Year | 0            | 0          | 0         |
		| Oct/Current Academic Year | 0            | 0          | 0         |
		| Nov/Current Academic Year | 0            | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 0            | 0          | 0         |
		| Feb/Current Academic Year | 0            | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 1000         | 0          | 0         |
		| May/Current Academic Year | 1000         | 0          | 0         |
		| Jun/Current Academic Year | 1000         | 0          | 0         |
		| Jul/Current Academic Year | 1000         | 0          | 0         |
	
	And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |

	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R09/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | Learning         |

	And  only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R09/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | Learning         |

	Examples:
	    | Collection_Period         | Levy Balance |
	    | R09/Current Academic Year | 0            |
	    | R10/Current Academic Year | 0            |