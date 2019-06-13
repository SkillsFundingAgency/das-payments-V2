@ignore
# failing due to incorrect handling of maths and english contract types
# DC Integration
#| learning support code | learning support date from | learning support date to |
#| 1                     | 06/08/2018                 | 06/10/2019               |
#| 1                     | 06/08/2018                 | 06/10/2019               |

Feature: Non Levy learner, takes an English qualification that has a planned end date that exceeds the actual end date of the programme aim and learning support is applicable to all learning PV2-802
		As a provider,
		I want a Non Levy learner, with Learning Support, where English & Maths exceeds end date of programme aim
		So that I am paid Learning Support by SFA until English & Maths completes

Scenario Outline: Non Levy learner, Eng aim planned end date exceeds the actual end date of the programme aim and learning support is applicable to all learning PV2-802
    Given the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Reference | SFA Contribution Percentage | Funding Line Type             | Framework Code | Pathway Code | Programme Type |
		| 06/Aug/Last Academic Year | 12 months        | 12000                | 06/Aug/Last Academic Year           | 3000                   | 06/Aug/Last Academic Year             |                 | continuing        | Act2          | ZPROG001      | 90%                         | 19-24 Apprenticeship Non-Levy | 593            | 1            | 20             |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status | Price Episode Id  |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        | 1st price details |
		| Maths or English | 12345         | 06/Aug/Last Academic Year | 14 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        | 2nd price details | 
	And price details as follows	
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage | 
        | 1st price details | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 1                   | 90%                         | 
        | 2nd price details | 0                    | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 2                   | 100%                        | 
 And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier | 
        | Aug/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | Sep/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | Oct/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | Nov/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | Dec/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | Jan/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | Feb/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | Mar/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | Apr/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | May/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | Jun/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | Jul/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   | 1st price details        | 
        | Aug/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        | Sep/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        | Oct/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        | Nov/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        | Dec/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        | Jan/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        | Feb/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        | Mar/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        | Apr/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        | May/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        | Jun/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        | Jul/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        | 
        
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Last Academic Year | Aug/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R02/Last Academic Year | Sep/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R03/Last Academic Year | Oct/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R04/Last Academic Year | Nov/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R05/Last Academic Year | Dec/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R06/Last Academic Year | Jan/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R07/Last Academic Year | Feb/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R08/Last Academic Year | Mar/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R09/Last Academic Year | Apr/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R10/Last Academic Year | May/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R11/Last Academic Year | Jun/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R12/Last Academic Year | Jul/Last Academic Year | 900                    | 100                         | 0                         | Learning                   |
        | R01/Last Academic Year | Aug/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R02/Last Academic Year | Sep/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R03/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R05/Last Academic Year | Dec/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R06/Last Academic Year | Jan/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R07/Last Academic Year | Feb/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R08/Last Academic Year | Mar/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R09/Last Academic Year | Apr/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R10/Last Academic Year | May/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R11/Last Academic Year | Jun/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R12/Last Academic Year | Jul/Last Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R01/Last Academic Year | Aug/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R02/Last Academic Year | Sep/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R03/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R05/Last Academic Year | Dec/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R06/Last Academic Year | Jan/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R07/Last Academic Year | Feb/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R08/Last Academic Year | Mar/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R09/Last Academic Year | Apr/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R10/Last Academic Year | May/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R11/Last Academic Year | Jun/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | Jul/Last Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status | Price Episode Id  |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year | 12 months        | 12 months       | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | completed         | 1st price details |
		| Maths or English | 12345         | 06/Aug/Last Academic Year | 14 months        | 14 months       | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | completed         | 2nd price details |
	And price details as follows	
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 1                   | 90%                         |
        | 2nd price details | 0                    | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 2                   | 100%                        |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
        | Aug/Current Academic Year | 0            | 3000       | 0         | 0               | 0                          | 1                   | 1st price details        |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 1st price details        |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 1st price details        |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 1st price details        |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 1st price details        |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 1st price details        |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 1st price details        |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 1st price details        |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 1st price details        |
        | May/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 1st price details        |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 1st price details        |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | 1st price details        |
        | Aug/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        |
        | Sep/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | 2nd price details        |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   | 2nd price details        |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   | 2nd price details        |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   | 2nd price details        |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   | 2nd price details        |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   | 2nd price details        |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   | 2nd price details        |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   | 2nd price details        |
        | May/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   | 2nd price details        |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   | 2nd price details        |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   | 2nd price details        |
       
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish |
        | R01/Current Academic Year | Aug/Current Academic Year | 0            | 3000       | 0         | 150             | 33.64                      |
        | R02/Current Academic Year | Sep/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      |
        | R03/Current Academic Year | Oct/Current Academic Year | 0            | 0          | 0         | 0               | 0                          |
        | R04/Current Academic Year | Nov/Current Academic Year | 0            | 0          | 0         | 0               | 0                          |
        | R05/Current Academic Year | Dec/Current Academic Year | 0            | 0          | 0         | 0               | 0                          |
        | R06/Current Academic Year | Jan/Current Academic Year | 0            | 0          | 0         | 0               | 0                          |
        | R07/Current Academic Year | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 0                          |
        | R08/Current Academic Year | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 0                          |
        | R09/Current Academic Year | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 0                          |
        | R10/Current Academic Year | May/Current Academic Year | 0            | 0          | 0         | 0               | 0                          |
        | R11/Current Academic Year | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 0                          |
        | R12/Current Academic Year | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 0                          |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 2700                   | 300                         | 0                         | Completion                 |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 2700                   | 300                         | 0                         | Completion                 |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
Examples: 
        | Collection_Period         |
        | R01/Current Academic Year |
        | R02/Current Academic Year |
        | R03/Current Academic Year |