Feature: Non-levy learner completes maths and english early PV2-485

Scenario Outline: Non-levy learner completes maths and english early PV2-485
	Given the following learners
        | Learner Reference Number | Uln      |
        | abc123                   | 12345678 |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Programme        | ZPROG001      | 06/Aug/Current Academic Year | 12 months        |                 | 1                   | 403            | 1            | 2              | 16-18 Apprenticeship Non-Levy | continuing        |
		| Maths or English | 12345         | 06/Aug/Current Academic Year | 12 months        |                 | 2                   | 403            | 1            | 2              | 16-18 Apprenticeship Non-Levy | continuing        |
	And price details as follows		
        | Price details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | Act2          | 1                   | 90%                         |
        | 2nd price details | 0                    | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | Act2          | 2                   | 100%                        |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | Aug/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
        | Sep/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
        | Oct/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
        | Nov/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
        | Dec/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
        | Jan/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
        | Feb/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
        | Mar/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
        | Apr/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
        | May/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
        | Jun/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
        | Jul/Current Academic Year | 1000         | 0          | 0         | 39.25                      |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Programme        | ZPROG001      | 06/Aug/Current Academic Year | 12 months        |                 | 1                   | 403            | 1            | 2              | 16-18 Apprenticeship Non-Levy | continuing        |
		| Maths or English | 12345         | 06/Aug/Current Academic Year | 12 months        | 10 months       | 2                   | 403            | 1            | 2              | 16-18 Apprenticeship Non-Levy | completed         |
	And price details are changed as follows		
        | Price details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 3rd price details | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | Act2          | 1                   | 90%                         |
        | 4th price details | 0                    | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | Act2          | 2                   | 100%                        |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | BalancingMathsAndEnglish |
        | Aug/Current Academic Year | 1000         | 0          | 0         | 39.25                      | 0                        |
        | Sep/Current Academic Year | 1000         | 0          | 0         | 39.25                      | 0                        |
        | Oct/Current Academic Year | 1000         | 0          | 0         | 39.25                      | 0                        |
        | Nov/Current Academic Year | 1000         | 0          | 0         | 39.25                      | 0                        |
        | Dec/Current Academic Year | 1000         | 0          | 0         | 39.25                      | 0                        |
        | Jan/Current Academic Year | 1000         | 0          | 0         | 39.25                      | 0                        |
        | Feb/Current Academic Year | 1000         | 0          | 0         | 39.25                      | 0                        |
        | Mar/Current Academic Year | 1000         | 0          | 0         | 39.25                      | 0                        |
        | Apr/Current Academic Year | 1000         | 0          | 0         | 39.25                      | 0                        |
        | May/Current Academic Year | 1000         | 0          | 0         | 39.25                      | 0                        |
        | Jun/Current Academic Year | 1000         | 0          | 0         | 0                          | 78.50                    |
        | Jul/Current Academic Year | 1000         | 0          | 0         | 0                          | 0                        |
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | BalancingMathsAndEnglish |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         | 0                          | 78.50                    |
        | R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         | 0                          | 0                        |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type         |
        | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 78.50                     | BalancingMathsAndEnglish |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type         |
        | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | 0                         | Learning                 |
        | R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 78.50                     | BalancingMathsAndEnglish |

Examples: 
        | Collection_Period         |
        | R11/Current Academic Year |
        | R12/Current Academic Year |