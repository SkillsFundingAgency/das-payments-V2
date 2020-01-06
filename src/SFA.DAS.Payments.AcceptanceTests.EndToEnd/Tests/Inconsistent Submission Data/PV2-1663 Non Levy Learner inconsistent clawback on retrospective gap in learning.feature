Feature: PV2-1663 Non Levy Learner inconsistent clawback when retrospective gap in learning
Scenario Outline: Non Levy Learner inconsistent clawback when retrospective gap in learning PV2-1663
    Given the following learners
        | Learner Reference Number | Uln      |
        | abc123                   | 12345678 |
    And the following aims
        | Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
        | Programme        | ZPROG001      | 06/Oct/Last Academic Year | 16 months        |                 | 1                   | 403            | 1            | 2              | 16-18 Apprenticeship Non-Levy | continuing        |
        | Maths or English | 12345         | 06/Oct/Last Academic Year | 16 months        |                 | 2                   | 403            | 1            | 2              | 16-18 Apprenticeship Non-Levy | continuing        |
    And price details as follows    
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 2nd price details | 3000                 | 06/Oct/Last Academic Year           | 0                      | 06/Oct/Last Academic Year             | Act2          | 1                   | 90%                         |
        |                   | 0                    | 06/Oct/Last Academic Year           | 0                      | 06/Oct/Last Academic Year             | Act2          | 2                   | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
        | Aug/Current Academic Year | 150          | 0          | 0         | 150             | 0                          | 1                   | 2nd price details        |
        | Sep/Current Academic Year | 150          | 0          | 0         | 150             | 0                          | 1                   | 2nd price details        |
        | Oct/Current Academic Year | 150          | 0          | 0         | 150             | 0                          | 1                   | 2nd price details        |
        | Nov/Current Academic Year | 150          | 0          | 0         | 150             | 0                          | 1                   | 2nd price details        |
        | Dec/Current Academic Year | 150          | 0          | 0         | 150             | 0                          | 1                   | 2nd price details        |
        | Jan/Current Academic Year | 150          | 0          | 0         | 150             | 0                          | 1                   | 2nd price details        |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | May/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Aug/Current Academic Year | 0            | 0          | 0         | 0               | 29.44                      | 2                   |                          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0               | 29.44                      | 2                   |                          |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0               | 29.44                      | 2                   |                          |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0               | 29.44                      | 2                   |                          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0               | 29.44                      | 2                   |                          |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0               | 29.44                      | 2                   |                          |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | May/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R02/Current Academic Year | Aug/Current Academic Year | 135                    | 15                          | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 135                    | 15                          | 0                         | Learning                   |
        | R02/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
    But aims details are changed as follows
        | Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
        | Programme        | ZPROG001      | 06/Oct/Last Academic Year | 16 months        | 11 months       | 1                   | 403            | 1            | 2              | 16-18 Apprenticeship Non-Levy | planned break     |
        | Maths or English | 12345         | 06/Oct/Last Academic Year | 16 months        | 11 months       | 2                   | 403            | 1            | 2              | 16-18 Apprenticeship Non-Levy | planned break     |
    And price details are changed as follows        
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 2nd price details | 3000                 | 06/Oct/Last Academic Year           | 0                      | 06/Oct/Last Academic Year             | Act2          | 1                   | 90%                         |
        |                   | 0                    | 06/Oct/Last Academic Year           | 0                      | 06/Oct/Last Academic Year             | Act2          | 2                   | 90%                         |
    When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
        | Aug/Current Academic Year | 150          | 0          | 0         | 150             | 0                          | 1                   | 2nd price details        |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | May/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 2nd price details        |
        | Aug/Current Academic Year | 0            | 0          | 0         | 0               | 29.44                      | 2                   |                          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | May/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |                          |
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | BalancingMathsAndEnglish |
        | R03/Current Academic Year | Sep/Current Academic Year | -150         | 0          | 0         | -150            | -29.44                     | 0                        |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R03/Current Academic Year | Sep/Current Academic Year | -135                   | -15                         | 0                         | Learning                   |
        | R03/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | -29.44                    | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | -150                      | LearningSupport            |
    And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R03/Current Academic Year | Sep/Current Academic Year | -135                   | -15                         | 0                         | Learning                   |
        | R03/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | -29.44                    | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | -150                      | LearningSupport            |
Examples: 
		| Collection_Period         |
		| R03/Current Academic Year |
