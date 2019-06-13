@ignore
# failing due to incorrect handling of maths and english contract types
#@supports_dc_e2e
Feature: Non-levy learner, learner moves start date forward, on prog payments and english/maths will be refunded PV2-465

Scenario Outline: Non-levy learner provider changes aim sequence numbers after payments have already occurred PV2-465
	Given the following learners
        | Learner Reference Number | Uln      |
        | abc123                   | 12345678 |
	And the following aims
		| Aim Type  | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Programme | ZPROG001      | 06/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | continuing        |
	And price details as follows	
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 9000                 | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | Act2          | 1                   | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
        | Aug/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | Sep/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | Oct/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | Nov/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | Dec/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | Jan/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | Feb/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | Mar/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | Apr/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | May/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | Jun/Current Academic Year | 600          | 0          | 0         | 1st price details        |
        | Jul/Current Academic Year | 600          | 0          | 0         | 1st price details        |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | 0                         | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | 0                         | Learning         |
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Maths or English | 12345         | 06/Oct/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | continuing        |
		| Programme        | ZPROG001      | 06/Oct/Current Academic Year | 12 months        |                 | 2                   | 593            | 1            | 20             | 16-18 Apprenticeship Non-Levy | continuing        |
	And price details are changed as follows		
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        |                   | 0                    | 06/Oct/Current Academic Year        | 0                      | 06/Oct/Current Academic Year          | Act2          | 1                   | 100%                        |
        | 1st price details | 9000                 | 06/Oct/Current Academic Year        | 0                      | 06/Oct/Current Academic Year          | Act2          | 2                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
        | Aug/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | 1st price details        |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | 1st price details        |
        | Oct/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | 1st price details        |
        | Nov/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | 1st price details        |
        | Dec/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | 1st price details        |
        | Jan/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | 1st price details        |
        | Feb/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | 1st price details        |
        | Mar/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | 1st price details        |
        | Apr/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | 1st price details        |
        | May/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | 1st price details        |
        | Jun/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | 1st price details        |
        | Jul/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | 1st price details        |
        | Aug/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          |
        | Oct/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
        | Nov/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
        | Jan/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
        | Feb/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
        | Mar/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
        | Apr/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
        | May/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
        | Jun/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
        | Jul/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Aug/Current Academic Year | -600         | 0          | 0         | 0                          |
        | R03/Current Academic Year | Sep/Current Academic Year | -600         | 0          | 0         | 0                          |
        | R03/Current Academic Year | Oct/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | R04/Current Academic Year | Nov/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | R05/Current Academic Year | Dec/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | R06/Current Academic Year | Jan/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | R07/Current Academic Year | Feb/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | R08/Current Academic Year | Mar/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | R09/Current Academic Year | Apr/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | R10/Current Academic Year | May/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | R11/Current Academic Year | Jun/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | R12/Current Academic Year | Jul/Current Academic Year | 600          | 0          | 0         | 39.25                      |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R03/Current Academic Year | Aug/Current Academic Year | -540                   | -60                         | 0                         | Learning                   |
        | R03/Current Academic Year | Sep/Current Academic Year | -540                   | -60                         | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R03/Current Academic Year | Aug/Current Academic Year | -540                   | -60                         | 0                         | Learning                   |
        | R03/Current Academic Year | Sep/Current Academic Year | -540                   | -60                         | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 540                    | 60                          | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
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