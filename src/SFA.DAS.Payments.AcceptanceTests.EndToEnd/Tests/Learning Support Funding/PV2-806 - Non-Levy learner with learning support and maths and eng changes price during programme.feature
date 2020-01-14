# DC Integration
# | learning support code | learning support date from | learning support date to |
# | 1                     | 06/08/2018                 | 06/10/2019               |
# | 1                     | 06/08/2018                 | 06/10/2019               |

Feature: Non-Levy learner with learning support and maths and eng changes price during programme PV2-806
		As a provider,
		I want a Non-Levy learner, with Learning Support, where the price changes during learning
		So that I am paid Learning Support by SFA for the duration of the learning

Scenario Outline: Non-Levy learner with learning support and maths and eng changes price during programme PV2-806
	Given the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Reference | SFA Contribution Percentage | Funding Line Type             | Framework Code | Pathway Code | Programme Type |
		| 06/Aug/Current Academic Year | 12 months        | 12000                | 04/Aug/Current Academic Year        | 3000                   | 04/Aug/Current Academic Year          |                 | continuing        | Act2          | ZPROG001      | 90%                         | 19-24 Apprenticeship Non-Levy | 593            | 1            | 20             |
		| 06/Aug/Current Academic Year | 12 months        | 12000                | 04/Aug/Current Academic Year        | 3000                   | 04/Aug/Current Academic Year          |                 | continuing        | Act2          | 12345         | 100%                        | 19-24 Apprenticeship Non-Levy | 593            | 1            | 20             |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status | Price Episode Id  |
		| Programme        | ZPROG001      | 04/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        | 1st Price Details |
		| Maths or English | 12345         | 04/Aug/Current Academic Year | 14 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        |                   |
	And price details as follows	
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st Price Details | 11250                | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act2          | 1                   | 90%                         |
        |                   | 0                    | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act2          | 2                   | 100%                        |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
        | Aug/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        | Sep/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        | Oct/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        | Nov/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        | Dec/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        | Jan/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        | Feb/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        | Mar/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        | Apr/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        | May/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        | Jun/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        | Jul/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        |
        #p2
        | Aug/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
        | May/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           | 
        | R01/Current Academic Year | Aug/Current Academic Year | 675                    | 75                          | 0                         | Learning                   | 
        | R02/Current Academic Year | Sep/Current Academic Year | 675                    | 75                          | 0                         | Learning                   | 
        | R03/Current Academic Year | Oct/Current Academic Year | 675                    | 75                          | 0                         | Learning                   | 
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            | 
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            | 
        | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            | 
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish | 
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish | 
        | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish | 
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status | Price Episode Id  |
		| Programme        | ZPROG001      | 04/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        | 3rd Price Details |
		| Maths or English | 12345         | 04/Aug/Current Academic Year | 14 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        |                   |
	And price details are changed as follows	
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st Price Details | 11250                | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act2          | 1                   | 90%                         |
        |                   | 0                    | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act2          | 2                   | 100%                        |
        | 3rd Price Details | 6750                 | 11/Nov/Current Academic Year        | 0                      | 11/Nov/Current Academic Year          | Act2          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
         | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier | Contract Type |
		#p1																																									   
         | Aug/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        | Act2          |
         | Sep/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        | Act2          |
         | Oct/Current Academic Year | 750          | 0          | 0         | 150             | 0                          | 1                   | 1st Price Details        | Act2          |
         | Nov/Current Academic Year | 350          | 0          | 0         | 150             | 0                          | 1                   | 3rd Price Details        | Act2          |
         | Dec/Current Academic Year | 350          | 0          | 0         | 150             | 0                          | 1                   | 3rd Price Details        | Act2          |
         | Jan/Current Academic Year | 350          | 0          | 0         | 150             | 0                          | 1                   | 3rd Price Details        | Act2          |
         | Feb/Current Academic Year | 350          | 0          | 0         | 150             | 0                          | 1                   | 3rd Price Details        | Act2          |
         | Mar/Current Academic Year | 350          | 0          | 0         | 150             | 0                          | 1                   | 3rd Price Details        | Act2          |
         | Apr/Current Academic Year | 350          | 0          | 0         | 150             | 0                          | 1                   | 3rd Price Details        | Act2          |
         | May/Current Academic Year | 350          | 0          | 0         | 150             | 0                          | 1                   | 3rd Price Details        | Act2          |
         | Jun/Current Academic Year | 350          | 0          | 0         | 150             | 0                          | 1                   | 3rd Price Details        | Act2          |
         | Jul/Current Academic Year | 350          | 0          | 0         | 150             | 0                          | 1                   | 3rd Price Details        | Act2          |
        #p2																																										  
         | Aug/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
         | Sep/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
         | Oct/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
         | Nov/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
         | Dec/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
         | Jan/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
         | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
         | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
         | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
         | May/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
         | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
         | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 33.64                      | 2                   |                          | Act2          |
	
    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 350          | 0          | 0         | 150             | 33.64                      |
        | R05/Current Academic Year | Dec/Current Academic Year | 350          | 0          | 0         | 150             | 33.64                      |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R04/Current Academic Year | Nov/Current Academic Year | 315                    | 35                          | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 315                    | 35                          | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |

	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R04/Current Academic Year | Nov/Current Academic Year | 315                    | 35                          | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 315                    | 35                          | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |

Examples: 
        | Collection_Period         |
        | R04/Current Academic Year |
        | R05/Current Academic Year |