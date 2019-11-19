Feature: Inconsistent Submissions Data PV2-1627

Scenario: A learner was in the ILR correctly for R01 & R02, was withdrawn in R14 for previous AY, clawback R01 & R02 payments in R03
	Given the following learners
        | Learner Reference Number | Uln      |
        | abc123                   | 12345678 |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                   | Completion Status |
		| Programme        | ZPROG001      | 01/Jan/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        |
		| Maths or English | 5012689       | 01/Jan/Last Academic Year | 12 months        |                 | 2                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        |
	And price details as follows
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 9000                 | 01/Jan/Last Academic Year           | 0                      | 01/Jan/Last Academic Year             | Act2          | 1                   | 90%                         |
        |                   | 0                    | 01/Jan/Last Academic Year           | 0                      | 01/Jan/Last Academic Year             | Act2          | 2                   | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Price Episode Identifier |
        | Jan/Last Academic Year    | 600          | 0          | 0         |                            | 1st price details        |
        | Feb/Last Academic Year    | 600          | 0          | 0         |                            | 1st price details        |
        | Mar/Last Academic Year    | 600          | 0          | 0         |                            | 1st price details        |
        | Apr/Last Academic Year    | 600          | 0          | 0         |                            | 1st price details        |
        | May/Last Academic Year    | 600          | 0          | 0         |                            | 1st price details        |
        | Jun/Last Academic Year    | 600          | 0          | 0         |                            | 1st price details        |
        | Jul/Last Academic Year    | 600          | 0          | 0         |                            | 1st price details        |
        | Aug/Current Academic Year | 600          | 0          | 0         |                            | 1st price details        |
        | Sep/Current Academic Year | 600          | 0          | 0         |                            | 1st price details        |
        | Jan/Last Academic Year    | 0            | 0          | 0         | 39.25                      |                          |
        | Feb/Last Academic Year    | 0            | 0          | 0         | 39.25                      |                          |
        | Mar/Last Academic Year    | 0            | 0          | 0         | 39.25                      |                          |
        | Apr/Last Academic Year    | 0            | 0          | 0         | 39.25                      |                          |
        | May/Last Academic Year    | 0            | 0          | 0         | 39.25                      |                          |
        | Jun/Last Academic Year    | 0            | 0          | 0         | 39.25                      |                          |
        | Jul/Last Academic Year    | 0            | 0          | 0         | 39.25                      |                          |
        | Aug/Current Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 39.25                      |                          |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R06/Last Academic Year    | Jan/Last Academic Year    | 540                    | 60                          | 0                         | Learning                   |
        | R07/Last Academic Year    | Feb/Last Academic Year    | 540                    | 60                          | 0                         | Learning                   |
        | R08/Last Academic Year    | Mar/Last Academic Year    | 540                    | 60                          | 0                         | Learning                   |
        | R09/Last Academic Year    | Apr/Last Academic Year    | 540                    | 60                          | 0                         | Learning                   |
        | R10/Last Academic Year    | May/Last Academic Year    | 540                    | 60                          | 0                         | Learning                   |
        | R11/Last Academic Year    | Jun/Last Academic Year    | 540                    | 60                          | 0                         | Learning                   |
        | R12/Last Academic Year    | Jul/Last Academic Year    | 540                    | 60                          | 0                         | Learning                   |
        | R01/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R06/Last Academic Year    | Jan/Last Academic Year    | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R07/Last Academic Year    | Feb/Last Academic Year    | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Last Academic Year    | Mar/Last Academic Year    | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Last Academic Year    | Apr/Last Academic Year    | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Last Academic Year    | May/Last Academic Year    | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R11/Last Academic Year    | Jun/Last Academic Year    | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year    | Jul/Last Academic Year    | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration  | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                   | Completion Status |
		| Programme        | ZPROG001      | 01/Jan/Last Academic Year |                  | 7 months - 1 day | 1                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | withdrawn         |
		| Maths or English | 5012689       | 01/Jan/Last Academic Year |                  | 7 months - 1 day | 2                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | withdrawn         |
	And price details are changed as follows
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 9000                 | 01/Jan/Last Academic Year           | 0                      | 01/Jan/Last Academic Year             | Act2          | 1                   | 90%                         |
        |                   | 0                    | 01/Jan/Last Academic Year           | 0                      | 01/Jan/Last Academic Year             | Act2          | 2                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period R03/Current Academic Year
    Then the following learner earnings should be generated
        | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier | Contract Type |
        | Aug/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | Sep/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | Oct/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | Nov/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | Dec/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | Jan/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | Feb/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | Mar/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | Apr/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | May/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | Jun/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | Jul/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
        | Aug/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
        | Sep/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
        | Oct/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
        | Nov/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
        | Dec/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
        | Jan/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
        | Feb/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
        | Mar/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
        | Apr/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
        | May/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
        | Jun/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
        | Jul/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Aug/Current Academic Year | -600         | 0          | 0         | -39.25                     |
        | R03/Current Academic Year | Sep/Current Academic Year | -600         | 0          | 0         | -39.25                     |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           | Price Episode Identifier |
        | R03/Current Academic Year | Aug/Current Academic Year | -540                   | -60                         | 0                         | Learning                   | 1st price details        |
        | R03/Current Academic Year | Sep/Current Academic Year | -540                   | -60                         | 0                         | Learning                   | 1st price details        |
        | R03/Current Academic Year | Aug/Current Academic Year |                        |                             | -39.25                    | OnProgrammeMathsAndEnglish |                          |
        | R03/Current Academic Year | Sep/Current Academic Year |                        |                             | -39.25                    | OnProgrammeMathsAndEnglish |                          |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           | Price Episode Identifier |
        | R03/Current Academic Year | Aug/Current Academic Year | -540                   | -60                         | 0                         | Learning                   | 1st price details        |
        | R03/Current Academic Year | Sep/Current Academic Year | -540                   | -60                         | 0                         | Learning                   | 1st price details        |
        | R03/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | -39.25                    | OnProgrammeMathsAndEnglish |                          |
        | R03/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | -39.25                    | OnProgrammeMathsAndEnglish |                          |