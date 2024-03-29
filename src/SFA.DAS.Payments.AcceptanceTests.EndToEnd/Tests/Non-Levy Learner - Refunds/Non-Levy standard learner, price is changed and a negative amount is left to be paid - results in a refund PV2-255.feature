﻿@basic_refund
#B@ignore
#@supports_dc_e2e
Feature: Non-Levy standard learner, price is changed and a negative amount is left to be paid - results in a refund PV2-255
	As a Provider
	I would like TODO
	So that TODO

Scenario Outline: Non-Levy standard learner, price is changed and a negative amount is left to be paid - results in a refund PV2-255
	Given the provider previously submitted the following learner details
		| Priority | Start Date             | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
		| 1        | start of academic year | 12 months        | 11250                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          |                 | continuing        | 90%                         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
	And the following earnings had been generated for the learner
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 750          | 0          | 0         | 1st price details        |
		| Sep/Current Academic Year | 750          | 0          | 0         | 1st price details        |
		| Oct/Current Academic Year | 750          | 0          | 0         | 1nd price details        |
		| Nov/Current Academic Year | 750          | 0          | 0         | 1nd price details        |
		| Dec/Current Academic Year | 750          | 0          | 0         | 1nd price details        |
		| Jan/Current Academic Year | 750          | 0          | 0         | 1nd price details        |
		| Feb/Current Academic Year | 750          | 0          | 0         | 1nd price details        |
		| Mar/Current Academic Year | 750          | 0          | 0         | 1nd price details        |
		| Apr/Current Academic Year | 750          | 0          | 0         | 1nd price details        |
		| May/Current Academic Year | 750          | 0          | 0         | 1nd price details        |
		| Jun/Current Academic Year | 750          | 0          | 0         | 1nd price details        |
		| Jul/Current Academic Year | 750          | 0          | 0         | 1nd price details        |

	And the following provider payments had been generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 675                    | 75                          | Learning         |
		| R02/Current Academic Year | Sep/Current Academic Year | 675                    | 75                          | Learning         |

	But the Provider now changes the Learner details as follows
		| Priority | Start Date             | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
		| 1        | start of academic year | 12 months        | 1400                 | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          | 3 months        | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |

	And price details as follows
		| Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| 1st price details | 11250                | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          | 90%                         | Act2          | 1                   |
		| 2nd price details | 1400                 | 01/Oct/Current Academic Year        | 0                      | 01/Oct/Current Academic Year          | 90%                         | Act2          | 1                   |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>

	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 750          | 0          | 0         | 1st price details        |
		| Sep/Current Academic Year | 750          | 0          | 0         | 1st price details        |
		| Oct/Current Academic Year | -100         | 0          | 0         | 2nd price details        |
		| Nov/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
		| Dec/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
		| Jan/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
		| Feb/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
		| Mar/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
		| Apr/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
		| May/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
		| Jun/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
		| Jul/Current Academic Year | 0            | 0          | 0         | 2nd price details        |
	And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R03/Current Academic Year | Oct/Current Academic Year | -100         | 0          | 0         |
		| R04/Current Academic Year | Nov/Current Academic Year | 0            | 0          | 0         |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R03/Current Academic Year | Oct/Current Academic Year | -90                    | -10                         | Learning         |
		| R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | Learning         |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R03/Current Academic Year | Oct/Current Academic Year | -90                    | -10                         | Learning         |
		| R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | Learning         |

Examples:
	| Collection_Period         |
	| R03/Current Academic Year |
	| R04/Current Academic Year |