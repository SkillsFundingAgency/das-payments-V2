@supports_dc_e2e
Feature: Non-levy learner - on framework , Disadvantage Uplift 21-27% paid-PV2-437

Scenario Outline: Non-levy learner - on framework , Disadvantage Uplift 21-27% paid
# Notes for DC team : In the ILR  use post codes having deprivation of 21-27% 
	Given the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Programme Type | Pathway Code | Funding Line Type | SFA Contribution Percentage | Postcode Prior |
		| 06/Aug/Last Academic Year | 12 months        | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 20             | 1            | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         | AL10 0AA       |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | FirstDisadvantagePayment |
        | Aug/Last Academic Year | 1000         | 0          | 0         | 0                        |
        | Sep/Last Academic Year | 1000         | 0          | 0         | 0                        |
        | Oct/Last Academic Year | 1000         | 0          | 0         | 0                        |
        | Nov/Last Academic Year | 1000         | 0          | 0         | 100                      |
        | Dec/Last Academic Year | 1000         | 0          | 0         | 0                        |
        | Jan/Last Academic Year | 1000         | 0          | 0         | 0                        |
        | Feb/Last Academic Year | 1000         | 0          | 0         | 0                        |
        | Mar/Last Academic Year | 1000         | 0          | 0         | 0                        |
        | Apr/Last Academic Year | 1000         | 0          | 0         | 0                        |
        | May/Last Academic Year | 1000         | 0          | 0         | 0                        |
        | Jun/Last Academic Year | 1000         | 0          | 0         | 0                        |
        | Jul/Last Academic Year | 1000         | 0          | 0         | 0                        |

    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type         |
        | R01/Last Academic Year | Aug/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R02/Last Academic Year | Sep/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R03/Last Academic Year | Oct/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R04/Last Academic Year | Nov/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R05/Last Academic Year | Dec/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R06/Last Academic Year | Jan/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R07/Last Academic Year | Feb/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R08/Last Academic Year | Mar/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R09/Last Academic Year | Apr/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R10/Last Academic Year | May/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R11/Last Academic Year | Jun/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R12/Last Academic Year | Jul/Last Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 100                       | FirstDisadvantagePayment | 

    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Programme Type | Pathway Code | Funding Line Type | SFA Contribution Percentage | Postcode Prior |
		| 06/Aug/Last Academic Year | 12 months        | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 20             | 1            | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         | AL10 0AA       |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>

	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | SecondDisadvantagePayment |
		| Aug/Current Academic Year | 0            | 0          | 0         | 100                       |
		| Sep/Current Academic Year | 0            | 0          | 0         | 0                         |
		| Oct/Current Academic Year | 0            | 0          | 0         | 0                         |
		| Nov/Current Academic Year | 0            | 0          | 0         | 0                         |
		| Dec/Current Academic Year | 0            | 0          | 0         | 0                         |
		| Jan/Current Academic Year | 0            | 0          | 0         | 0                         |
		| Feb/Current Academic Year | 0            | 0          | 0         | 0                         |
		| Mar/Current Academic Year | 0            | 0          | 0         | 0                         |
		| Apr/Current Academic Year | 0            | 0          | 0         | 0                         |
		| May/Current Academic Year | 0            | 0          | 0         | 0                         |
		| Jun/Current Academic Year | 0            | 0          | 0         | 0                         |
		| Jul/Current Academic Year | 0            | 0          | 0         | 0                         |

    And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | SecondDisadvantagePayment |
		| R01/Current Academic Year | Aug/Current Academic Year | 0            | 0          | 0         | 100                       |

	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type          |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 100                       | SecondDisadvantagePayment |

	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type          |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 100                       | SecondDisadvantagePayment |

	Examples:
        | Collection_Period         |
		| R01/Current Academic Year |
		| R02/Current Academic Year |