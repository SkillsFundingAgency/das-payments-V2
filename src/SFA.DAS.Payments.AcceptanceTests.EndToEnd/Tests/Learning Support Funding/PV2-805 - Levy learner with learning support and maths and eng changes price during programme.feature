# DC Integration
# | learning support code | learning support date from | learning support date to |
# | 1                     | 06/08/2018                 | 06/10/2019               |
# | 1                     | 06/08/2018                 | 06/10/2019               |

Feature: Levy learner with learning support and maths and eng changes price during programme PV2-805
		As a provider,
		I want a Levy learner, with Learning Support, where the price changes during learning
		So that I am paid Learning Support by SFA for the duration of the learning

Scenario Outline: Levy learner with learning support and maths and eng changes price during programme PV2-805
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                   | end date                     | agreed price | status | 
        | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year | 11250        | active | 
	And the following aims
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status | LearningSupportIsFunctionalSkill | Price Episode Id |
		| Programme        | ZPROG001      | 04/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |                                  | pe-1             |
		| Maths or English | 12345         | 04/Aug/Current Academic Year | 14 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        | true                             |                  |
	And price details as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 11250                | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act1          | 1                   | 90%                         |
        | pe-2             | 0                    | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act1          | 2                   | 100%                        |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
        | Aug/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Sep/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Oct/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Nov/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Dec/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Jan/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Feb/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Mar/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Apr/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | May/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Jun/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Jul/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        #p2
        | Aug/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
        | Oct/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
        | Nov/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
        | Jan/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
        | Feb/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
        | Mar/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
        | Apr/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
        | May/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
        | Jun/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
        | Jul/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |

    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 750           | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 750           | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 750           | 0                         | Learning                   |
        | R01/Current Academic Year | Aug/Current Academic Year | 0             | 150                       | LearningSupport            |
        | R02/Current Academic Year | Sep/Current Academic Year | 0             | 150                       | LearningSupport            |
        | R03/Current Academic Year | Oct/Current Academic Year | 0             | 150                       | LearningSupport            |
        | R01/Current Academic Year | Aug/Current Academic Year | 0             | 33.64                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0             | 33.64                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 0             | 33.64                     | OnProgrammeMathsAndEnglish |
    And aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status | LearningSupportIsFunctionalSkill | Price Episode Id |
		| Programme        | ZPROG001      | 04/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |                                  | pe-3             |
		| Maths or English | 12345         | 04/Aug/Current Academic Year | 14 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        | true                             |                  |
	
	But price details are changed as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 11250                | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act1          | 1                   | 90%                         |
        |                  | 0                    | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act1          | 2                   | 100%                        |
        | pe-3             | 6750                 | 11/Nov/Current Academic Year        | 0                      | 11/Nov/Current Academic Year          | Act1          | 1                   | 90%                         |
	
	And the Commitment details are changed as follows
        | start date                   | end date                     | agreed price | status | effective from               | effective to                 | Framework Code | Pathway Code | Programme Type |
        | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year | 11250        | active | 01/Aug/Current Academic Year | 10/Nov/Current Academic Year | 593            | 1            | 20             |
        | 01/Aug/Current Academic Year | 01/Aug/Current Academic Year | 6750         | active | 11/Nov/Current Academic Year |                              | 593            | 1            | 20             |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
         | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
         | Aug/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
         | Sep/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
         | Oct/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
         | Nov/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
         | Dec/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
         | Jan/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
         | Feb/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
         | Mar/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
         | Apr/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
         | May/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
         | Jun/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
         | Jul/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        #p2
         | Aug/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
         | Sep/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
         | Oct/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
         | Nov/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
         | Dec/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
         | Jan/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
         | Feb/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
         | Mar/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
         | Apr/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
         | May/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
         | Jun/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
         | Jul/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |                          |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 350          | 0          | 0         | 150             | 33.64                      |
        | R05/Current Academic Year | Dec/Current Academic Year | 350          | 0          | 0         | 150             | 33.64                      |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R04/Current Academic Year | Nov/Current Academic Year | 350           | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 350           | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 150                       | LearningSupport            |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | 150                       | LearningSupport            |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 33.64                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | 33.64                     | OnProgrammeMathsAndEnglish |

	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R04/Current Academic Year | Nov/Current Academic Year | 350           | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 350           | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 150                       | LearningSupport            |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | 150                       | LearningSupport            |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 33.64                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | 33.64                     | OnProgrammeMathsAndEnglish |

Examples: 
        | Collection_Period         | Levy Balance |
        | R04/Current Academic Year | 6750         |
        | R05/Current Academic Year | 6400         |