#@supports_dc_e2e
Feature: Non-levy standard learner, payments made then price is changed retrospectively from beginning PV2-258

As a provider,
I want a non-levy learner, that when payments are made then price is changed retrospectively from beginning
So that I am accurately paid my apprenticeship provision.

Scenario Outline:  non-levy learner price changed retrospectively PV2-258
Non-Levy standard learner, payments made then price is changed retrospectively from beginning

    Given the provider previously submitted the following learner details
        | Priority | Start Date             | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
        | 1        | start of academic year | 12 months        | 11250                | Aug/Current Academic Year           | 0                      | Aug/Current Academic Year             | 12 months       | continuing        | 90%                         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
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
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 675                    | 75                          | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 675                    | 75                          | Learning         |

    But the Provider now changes the Learner details as follows
        | Priority | Start Date             | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
        | 1        | start of academic year | 12 months        | 10                   | Aug/Current Academic Year           | 0                      | Aug/Current Academic Year             | 12 months       | continuing        | 90%                         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
   
  	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| pe-1             | 10                   | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act2          | 1                   |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
        | Aug/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
        | Sep/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
        | Oct/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
        | Nov/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
        | Dec/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
        | Jan/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
        | Feb/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
        | Mar/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
        | Apr/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
        | May/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
        | Jun/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
        | Jul/Current Academic Year | 0.66667      | 0          | 0         | pe-1                     |
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R03/Current Academic Year | Aug/Current Academic Year | -749.33333   | 0          | 0         |
        | R03/Current Academic Year | Sep/Current Academic Year | -749.33333   | 0          | 0         |
        | R03/Current Academic Year | Oct/Current Academic Year | 0.66667      | 0          | 0         |
        | R04/Current Academic Year | Nov/Current Academic Year | 0.66667      | 0          | 0         |
        | R05/Current Academic Year | Dec/Current Academic Year | 0.66667      | 0          | 0         |
        | R06/Current Academic Year | Jan/Current Academic Year | 0.66667      | 0          | 0         |
        | R07/Current Academic Year | Feb/Current Academic Year | 0.66667      | 0          | 0         |
        | R08/Current Academic Year | Mar/Current Academic Year | 0.66667      | 0          | 0         |
        | R09/Current Academic Year | Apr/Current Academic Year | 0.66667      | 0          | 0         |
        | R10/Current Academic Year | May/Current Academic Year | 0.66667      | 0          | 0         |
        | R11/Current Academic Year | Jun/Current Academic Year | 0.66667      | 0          | 0         |
        | R12/Current Academic Year | Jul/Current Academic Year | 0.66667      | 0          | 0         |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R03/Current Academic Year | Aug/Current Academic Year | -674.40                | -74.93333                   | Learning         |
        | R03/Current Academic Year | Sep/Current Academic Year | -674.40                | -74.93333                   | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 0.60                   | 0.06667                     | Learning         |

	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R03/Current Academic Year | Aug/Current Academic Year | -674.40                | -74.93333                   | Learning         |
        | R03/Current Academic Year | Sep/Current Academic Year | -674.40                | -74.93333                   | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R11/Current Academic Year | Jun/Current Academic Year | 0.60                   | 0.06667                     | Learning         |
        | R12/Current Academic Year | Jul/Current Academic Year | 0.60                   | 0.06667                     | Learning         |

	Examples: 
        | Collection_Period         |
        | R03/Current Academic Year |
        | R04/Current Academic Year |
        | R05/Current Academic Year |
        | R06/Current Academic Year |
        | R07/Current Academic Year |
        | R08/Current Academic Year |
        | R09/Current Academic Year |
        | R10/Current Academic Year |
        | R11/Current Academic Year |
        | R12/Current Academic Year |