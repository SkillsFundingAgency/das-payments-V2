Feature: 5% Contribution from April 2019 PV2-896
As a provider,
I want a Non Levy learner, starting prior to Apr 2019, where learner changes Employer from Apr 2019 but stays on same course and price, remains at 10% contribution
So that I am paid the correct apprenticeship funding by SFA

Scenario Outline: Earnings and payments for a levy learner, switches from levy to non levy employer at the end of month- PV2-896

Given the provider previously submitted the following learner details
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                      | SFA Contribution Percentage |
	| 03/Aug/Current Academic Year | 12 months        | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract | 90%                         |

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
	| R04/Current Academic Year | Nov/Current Academic Year | 1000          | Learning         | employer 1 |
	| R05/Current Academic Year | Dec/Current Academic Year | 1000          | Learning         | employer 1 |
	| R06/Current Academic Year | Jan/Current Academic Year | 1000          | Learning         | employer 1 |
	| R05/Current Academic Year | Feb/Current Academic Year | 1000          | Learning         | employer 1 |
	| R06/Current Academic Year | Mar/Current Academic Year | 1000          | Learning         | employer 1 |

But the Provider now changes the Learner details as follows
	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                      |
	| 03/Aug/Current Academic Year | 12 months        | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract |

And price details as follows
	| Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
	| 1st price details | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act2          |
	| 2nd price details | 12000                | 03/Aug/Current Academic Year        | 3000                   | 03/Aug/Current Academic Year          | 3200                    | 03/Apr/Current Academic Year           | 800                       | 03/Apr/Current Academic Year             | 90%                         | Act2          |

When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>

Then the following learner earnings should be generated
	| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
	| Aug/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
	| Sep/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
	| Oct/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
	| Nov/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
	| Dec/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
	| Jan/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
	| Feb/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
	| Mar/Current Academic Year | 1000         | 0          | 0         | 1st price details        |
	| Apr/Current Academic Year | 800          | 0          | 0         | 2nd price details        |
	| May/Current Academic Year | 800          | 0          | 0         | 2nd price details        |
	| Jun/Current Academic Year | 800          | 0          | 0         | 2nd price details        |
	| Jul/Current Academic Year | 800          | 0          | 0         | 2nd price details        |

And at month end only the following payments will be calculated
	| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
	| R09/Current Academic Year | Apr/Current Academic Year | 800          | 0          | 0         |
	| R10/Current Academic Year | May/Current Academic Year | 800          | 0          | 0         |
	| R11/Current Academic Year | Jun/Current Academic Year | 800          | 0          | 0         |
	| R12/Current Academic Year | Jul/Current Academic Year | 800          | 0          | 0         |

And only the following provider payments will be recorded
	| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer   |
	| R09/Current Academic Year | Apr/Current Academic Year | 720                    | 80                          | Learning         | employer 2 |
	| R10/Current Academic Year | May/Current Academic Year | 720                    | 80                          | Learning         | employer 2 |
	| R11/Current Academic Year | Jun/Current Academic Year | 720                    | 80                          | Learning         | employer 2 |
	| R12/Current Academic Year | Jul/Current Academic Year | 720                    | 80                          | Learning         | employer 2 |

And only the following provider payments will be generated
	| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Employer   |
	| R09/Current Academic Year | Apr/Current Academic Year | 720                    | 80                          | Learning         | employer 2 |
	| R10/Current Academic Year | May/Current Academic Year | 720                    | 80                          | Learning         | employer 2 |
	| R11/Current Academic Year | Jun/Current Academic Year | 720                    | 80                          | Learning         | employer 2 |
	| R12/Current Academic Year | Jul/Current Academic Year | 720                    | 80                          | Learning         | employer 2 |
Examples: 
	| Collection_Period         |
	| R09/Current Academic Year |
	| R10/Current Academic Year |
	| R11/Current Academic Year |
	| R12/Current Academic Year |