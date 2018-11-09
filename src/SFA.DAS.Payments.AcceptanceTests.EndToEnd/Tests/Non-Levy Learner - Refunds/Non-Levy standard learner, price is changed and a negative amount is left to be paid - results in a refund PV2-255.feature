Feature: Non-Levy standard learner, price is changed and a negative amount is left to be paid - results in a refund PV2-255
	As a Provider
	I would like TODO
	So that TODO

Scenario Outline: Non-Levy standard learner, price is changed and a negative amount is left to be paid - results in a refund PV2-255
    Given the provider previously submitted the following learner details
        | ULN       | Priority | Start Date             | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assesment Price | Total Assesment Price Effective Date | Actual Duration | Completion Status | SFA Contribution Percentage | Contract Type        | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
        | learner a | 1        | start of academic year | 12 months        | 9000                 | Aug/Current Academic Year           | 2250                  | Aug/Current Academic Year            |                 | continuing        | 90%                         | ContractWithEmployer | 1                   | ZPROG001      | 403            | 1            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
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
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments |
        | R01/Current Academic Year | Aug/Current Academic Year | 675                    | 75                          |
        | R02/Current Academic Year | Sep/Current Academic Year | 675                    | 75                          |

    But the Provider now changes the Learner details as follows
        | ULN       | Priority | Start Date             | Planned Duration | Actual Duration | Programme Type | Completion Status | SFA Contribution Percentage |
        | learner a | 1        | start of academic year | 12 months        | 12 months       | 25             | continuing        | 90%                         |
	And price details as follows
        | Price details     | Total Training Price | Total Training Price Effective Date | Total Assesment Price | Total Assesment Price Effective Date |
        | 1st price details | 9000                 | Aug/Current Academic Year           | 2250                  | Aug/Current Academic Year            |
        | 2nd price details | 1200                 | Oct/Current Academic Year           | 200                   | Oct/Current Academic Year            |

    When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 750          | 0          | 0         |
        | Sep/Current Academic Year | 750          | 0          | 0         |
        | Oct/Current Academic Year | -100         | 0          | 0         |
        | Nov/Current Academic Year | 0            | 0          | 0         |
        | Dec/Current Academic Year | 0            | 0          | 0         |
        | Jan/Current Academic Year | 0            | 0          | 0         |
        | Feb/Current Academic Year | 0            | 0          | 0         |
        | Mar/Current Academic Year | 0            | 0          | 0         |
        | Apr/Current Academic Year | 0            | 0          | 0         |
        | May/Current Academic Year | 0            | 0          | 0         |
        | Jun/Current Academic Year | 0            | 0          | 0         |
        | Jul/Current Academic Year | 0            | 0          | 0         |
    And the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R03/Current Academic Year | Oct/Current Academic Year | -100         | 0          | 0         |
	And no payments will be calculated for following collection periods
		| Collection Period         |
		| R04/Current Academic Year |
		| R05/Current Academic Year |
		| R06/Current Academic Year |
		| R07/Current Academic Year |
		| R08/Current Academic Year |
		| R09/Current Academic Year |
		| R10/Current Academic Year |
		| R11/Current Academic Year |
		| R12/Current Academic Year |
    And the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R03/Current Academic Year | Oct/Current Academic Year | -90                    | -10                         | Learning         |
    And at month end the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R03/Current Academic Year | Oct/Current Academic Year | -90                    | -10                         | Learning         |

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