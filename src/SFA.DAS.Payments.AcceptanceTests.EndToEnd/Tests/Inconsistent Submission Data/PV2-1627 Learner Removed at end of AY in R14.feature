Feature: Inconsistent Submissions Data PV2-1627

Scenario: A learner was in the ILR correctly for R01 & R02, was withdrawn in R14 for previous AY, clawback R01 & R02 payments in R03
	Given the following learners
        | Learner Reference Number | Uln      |
        | abc123                   | 12345678 |
	And the following aims
		| Aim Type  | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type           | Completion Status |
		| Programme | ZPROG001      | 06/Jan/Last Academic Year | 12 months        |                 | 1                   | 403            | 1            | 2              | 19+ Apprenticeship Non-Levy | continuing        |
	And price details as follows
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 9000                 | 06/Jan/Last Academic Year           | 0                      | 06/Jan/Last Academic Year             | Act2          | 1                   | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
        | Jan/Last Academic Year    | 600          | 0          | 0         | 1st price details        |
        | Feb/Last Academic Year    | 600          | 0          | 0         | 1st price details        |
        | Mar/Last Academic Year    | 600          | 0          | 0         | 1st price details        |
        | Apr/Last Academic Year    | 600          | 0          | 0         | 1st price details        |
        | May/Last Academic Year    | 600          | 0          | 0         | 1st price details        |
        | Jun/Last Academic Year    | 600          | 0          | 0         | 1st price details        |
        | Jul/Last Academic Year    | 600          | 0          | 0         | 1st price details        |
        | Aug/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | Sep/Current Academic Year | 600          | 0          | 0         | 1st price details        |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type |
        | R06/Last Academic Year    | Jan/Last Academic Year    | 540                    | 60                          | 0                         | Learning         |
        | R07/Last Academic Year    | Feb/Last Academic Year    | 540                    | 60                          | 0                         | Learning         |
        | R08/Last Academic Year    | Mar/Last Academic Year    | 540                    | 60                          | 0                         | Learning         |
        | R09/Last Academic Year    | Apr/Last Academic Year    | 540                    | 60                          | 0                         | Learning         |
        | R10/Last Academic Year    | May/Last Academic Year    | 540                    | 60                          | 0                         | Learning         |
        | R11/Last Academic Year    | Jun/Last Academic Year    | 540                    | 60                          | 0                         | Learning         |
        | R12/Last Academic Year    | Jul/Last Academic Year    | 540                    | 60                          | 0                         | Learning         |
        | R01/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | 0                         | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | 0                         | Learning         |
    But aims details are changed as follows
		| Aim Type  | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type           | Completion Status |
		| Programme | ZPROG001      | 06/Jan/Last Academic Year |                  | 7 Months        | 1                   | 403            | 1            | 2              | 19+ Apprenticeship Non-Levy | withdrawn         |
	When the amended ILR file is re-submitted for the learners in collection period R03/Current Academic Year
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | Aim Sequence Number | Price Episode Identifier | Contract Type |
    #    | Aug/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    #    | Sep/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    #    | Oct/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    #    | Nov/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    #    | Dec/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    #    | Jan/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    #    | Feb/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    #    | Mar/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    #    | Apr/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    #    | May/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    #    | Jun/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    #    | Jul/Current Academic Year | 0            | 0          | 0         | 1                   | 1st price details        | Act2          |
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R03/Current Academic Year | Oct/Current Academic Year | -1200        | 0          | 0         |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type |
        | R03/Current Academic Year | Oct/Current Academic Year | -1080                  | -120                        | 0                         | On-Programme     |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type |
        | R03/Current Academic Year | Oct/Current Academic Year | -1080                  | -120                        | 0                         | On-Programme     |