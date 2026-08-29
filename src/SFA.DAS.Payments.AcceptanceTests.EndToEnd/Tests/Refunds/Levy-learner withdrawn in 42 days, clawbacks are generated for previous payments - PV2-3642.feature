@basic_refund
Feature: Levy Learner is withdrawn retrospectively PV2-3642
	As a provider,
	I want a levy learner, where levy is available the provider retrospectively notifies a withdrawal before 42 days and previously paid monthly instalments are refunded
	So that I am accurately paid my apprenticeship provision.

Scenario:  Provider retrospectively notifies of a withdrawal before 42 days for a levy learner after payments have already been made PV2-3642

Given the employer levy account balance in collection period R02/Current Academic Year is 15000

And the following commitments exist

	| start date                   | end date                     | agreed price |Standard Code | Programme Type |
	| 01/Aug/Current Academic Year | 31/Jul/Current Academic Year | 11250        |17            | 25             |

And the provider previously submitted the following learner details
    | Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assesment Price | Total Assesment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
    | 04/Aug/Current Academic Year | 12 months        | 9000                 | 01/Aug/Current Academic Year        | 2250                  | 01/Aug/Current Academic Year         |                 | continuing        | 90%                         | Act1          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |

And the following earnings had been generated for the learner
    | Delivery Period           | On-Programme | Completion | Balancing |
    | Aug/Current Academic Year | 750          | 0          | 0         |
    | Sep/Current Academic Year | 750          | 0          | 0         |
    | Oct/Current Academic Year | 750          | 0          | 0         |
    | Nov/Current Academic Year | 750          | 0          | 0         |
    | Dec/Current Academic Year | 750          | 0          | 0         |
    | Jan/Current Academic Year | 750          | 0          | 0         |
    | Feb/Current Academic Year | 750          | 0          | 0         |
    | Mar/Current Academic Year | 750          | 0          | 0         |
    | Apr/Current Academic Year | 750          | 0          | 0         |
    | May/Current Academic Year | 750          | 0          | 0         |
    | Jun/Current Academic Year | 750          | 0          | 0         |
    | Jul/Current Academic Year | 750          | 0          | 0         |

And the following provider payments had been generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R01/Current Academic Year | Aug/Current Academic Year | 750           | Learning         |
        
But the Provider now changes the Learner details as follows
    | Start Date                   | Planned Duration | Total Training Price | Total Assessment Price | Total Training Price Effective Date | Total Assesment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
    | 04/Aug/Current Academic Year | 12 months        | 9000                 | 2250                   | 01/Aug/Current Academic Year        | 01/Aug/Current Academic Year         | 42 days         | withdrawn         | 90%                         | Act1          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |

And price details as follows
    | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
    | pe-1             | 9000                 | 01/Aug/Current Academic Year        | 2250                   | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | Act1          | 1                   | 90%                         |
		 
When the amended ILR file is re-submitted for the learners in collection period R06/Current Academic Year

Then the following learner earnings should be generated
    | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
    | Aug/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Sep/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Apr/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | May/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Jun/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    | Jul/Current Academic Year | 0            | 0          | 0         | pe-1                     |

And at month end only the following payments will be calculated
    | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
    | R06/Current Academic Year | Aug/Current Academic Year | -750         | 0          | 0         |

And only the following provider payments will be recorded
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R06/Current Academic Year | Aug/Current Academic Year | -750          | Learning         |

And only the following provider payments will be generated
    | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
    | R06/Current Academic Year | Aug/Current Academic Year | -750          | Learning         |