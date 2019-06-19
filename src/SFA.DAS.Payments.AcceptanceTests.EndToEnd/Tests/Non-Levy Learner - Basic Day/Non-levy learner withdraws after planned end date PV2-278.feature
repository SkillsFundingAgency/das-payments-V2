﻿@supports_dc_e2e
Feature:  PV2-278 A non-levy learner withdraws after planned end date


Scenario Outline: PV2-278 A non-levy learner withdraws after planned end date
    Given the provider previously submitted the following learner details in collection period "R01/Last Academic Year"
		| ULN       | Priority | Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                               | SFA Contribution Percentage |
		| learner a | 1        | 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year | 0            | 0          | 0         |
        | Sep/Last Academic Year | 1000         | 0          | 0         |
        | Oct/Last Academic Year | 1000         | 0          | 0         |
        | Nov/Last Academic Year | 1000         | 0          | 0         |
        | Dec/Last Academic Year | 1000         | 0          | 0         |
        | Jan/Last Academic Year | 1000         | 0          | 0         |
        | Feb/Last Academic Year | 1000         | 0          | 0         |
        | Mar/Last Academic Year | 1000         | 0          | 0         |
        | Apr/Last Academic Year | 1000         | 0          | 0         |
        | May/Last Academic Year | 1000         | 0          | 0         |
        | Jun/Last Academic Year | 1000         | 0          | 0         |
        | Jul/Last Academic Year | 1000         | 0          | 0         |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R02/Last Academic Year | Sep/Last Academic Year | 900                    | 100                         | Learning         |
        | R03/Last Academic Year | Oct/Last Academic Year | 900                    | 100                         | Learning         |
        | R04/Last Academic Year | Nov/Last Academic Year | 900                    | 100                         | Learning         |
        | R05/Last Academic Year | Dec/Last Academic Year | 900                    | 100                         | Learning         |
        | R06/Last Academic Year | Jan/Last Academic Year | 900                    | 100                         | Learning         |
        | R07/Last Academic Year | Feb/Last Academic Year | 900                    | 100                         | Learning         |
        | R08/Last Academic Year | Mar/Last Academic Year | 900                    | 100                         | Learning         |
        | R09/Last Academic Year | Apr/Last Academic Year | 900                    | 100                         | Learning         |
        | R10/Last Academic Year | May/Last Academic Year | 900                    | 100                         | Learning         |
        | R11/Last Academic Year | Jun/Last Academic Year | 900                    | 100                         | Learning         |
        | R12/Last Academic Year | Jul/Last Academic Year | 900                    | 100                         | Learning         |
    And the provider previously submitted the following learner details in collection period "R01/Current Academic Year" 
		| ULN       | Priority | Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                               | SFA Contribution Percentage |
		| learner a | 1        | 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship Non-Levy Contract (procured) | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | 
        | Aug/Current Academic Year | 1000         | 0          | 0         | 
        | Sep/Current Academic Year | 0            | 0          | 0         |
        | Oct/Current Academic Year | 0            | 0          | 0         |
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
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |

    But the Provider now changes the Learner details as follows
		| ULN       | Priority | Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                   |
		| learner a | 1        | 01/Sep/Last Academic Year | 12 months        | 15000                | 01/Sep/Last Academic Year           |                        |                                       | 15 months       | withdrawn         | 90%                         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 15000                | 01/Sep/Last Academic Year           |                        |                                       | 0                       |                                        | 0                         |                                          | Act2          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <collection_period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
        | Aug/Current Academic Year | 1000         | 0          | 0         | pe-1                     |
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

	And no provider payments will be recorded

	Examples:
		| collection_period			|
		| R05/Current Academic Year |
		| R06/Current Academic Year |
		| R07/Current Academic Year |
		| R08/Current Academic Year |
		| R09/Current Academic Year |
		| R10/Current Academic Year |
		| R11/Current Academic Year |
		| R12/Current Academic Year |