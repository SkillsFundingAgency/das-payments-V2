# DC Integration
#| learning support code | learning support date from | learning support date to |
#| 1                     | 06/08/2018                 | 06/10/2019               |
#| 1                     | 06/08/2018                 | 06/10/2019               |

Feature: Non Levy learner, takes an English qualification that has a planned end date that exceeds the actual end date of the programme aim and learning support is applicable to all learning PV2-802
		As a provider,
		I want a Non Levy learner, with Learning Support, where English & Maths exceeds end date of programme aim
		So that I am paid Learning Support by SFA until English & Maths completes

Scenario Outline: Non Levy learner, Eng aim planned end date exceeds the actual end date of the programme aim and learning support is applicable to all learning PV2-802
	Given the following aims
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        |
		| Maths or English | 12345         | 06/Aug/Last Academic Year | 14 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        |
	And price details as follows	
        | Price Details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 1                   | 90%                         |
        | 2nd price details | 0                    | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act2          | 2                   | 100%                        |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | Aim Sequence Number |
        | Aug/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | Sep/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | Oct/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | Nov/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | Dec/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | Jan/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | Feb/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | Mar/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | Apr/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | May/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | Jun/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | Jul/Last Academic Year | 1000         | 0          | 0         | 0               | 0                          | 1                   |
        | Aug/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Sep/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Oct/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Nov/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Dec/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Jan/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Feb/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Mar/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Apr/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | May/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Jun/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Jul/Last Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
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
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year | 12 months        | 12 months       | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | completed         |
		| Maths or English | 12345         | 06/Aug/Last Academic Year | 14 months        | 14 months       | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | completed         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | Aim Sequence Number |
        | Aug/Current Academic Year | 0            | 3000       | 0         | 0               | 0                          | 1                   |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   |
        | May/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   |
        | Aug/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Sep/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |
        | May/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 2                   |
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
        | R01/Current Academic Year | Aug/Current Academic Year | 2700                   | 300                         | 0                         | Learning                   |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 2700                   | 300                         | 0                         | Learning                   |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 150                       | LearningSupport            |
        | R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 33.64                     | OnProgrammeMathsAndEnglish |
Examples: 
        | Collection_Period         |
        | R01/Current Academic Year |
        | R02/Current Academic Year |
        | R03/Current Academic Year |



#Feature: Learning Support	
#
#Scenario: Non Levy learner, takes an English qualification that has a planned end date that exceeds the actual end date of the programme aim and learning support is applicable to all learning
#
#	When an ILR file is submitted with the following data:
#		| ULN       | learner type           | aim type         | agreed price | aim rate | start date | planned end date | actual end date | completion status | learning support code | learning support date from | learning support date to | employer contribution |
#		| learner a | programme only non-DAS | programme        | 15000        |          | 06/08/2018 | 08/08/2019       | 08/08/2019      | completed         | 1                     | 06/08/2018                 | 06/10/2019               | 1200                  |
#		| learner a | programme only non-DAS | maths or english |              | 471      | 06/08/2018 | 06/10/2019       | 06/10/2019      | completed         | 1                     | 06/08/2018                 | 06/10/2019               | 1200                  |
#	
#	Then the provider earnings and payments break down as follows:
#		| Type                                    | 08/18   | 09/18   | 10/18   | ... | 05/19   | 06/19   | 07/19   | 08/19   | 09/19   | 10/19  | 11/19 |
#		| Provider Earned Total                   | 1183.64 | 1183.64 | 1183.64 | ... | 1183.64 | 1183.64 | 1183.64 | 3183.64 | 183.64  | 0      | 0     |
#		| Provider Paid by SFA                    | 0       | 1083.64 | 1083.64 | ... | 1083.64 | 1083.64 | 1083.64 | 1083.64 | 2883.64 | 183.64 | 0     |
#		| Payment due from Employer               | 0       | 100     | 100     | ... | 100     | 100     | 100     | 100     | 300     | 0      | 0     |
#		| Levy account debited                    | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0      | 0     |
#		| SFA Levy employer budget                | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0      | 0     |
#		| SFA Levy co-funding budget              | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0      | 0     |
#		| SFA non-Levy co-funding budget          | 900     | 900     | 900     | ... | 900     | 900     | 900     | 2700    | 0       | 0      | 0     |
#		| SFA Levy additional payments budget     | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0      | 0     |
#		| SFA non-Levy additional payments budget | 183.64  | 183.64  | 183.64  | ... | 183.64  | 183.64  | 183.64  | 183.64  | 183.64  | 0      | 0     | 
#    
#	And the transaction types for the payments are:
#		| Payment type                   | 09/18 | 10/18 | ... | 05/19 | 06/19 | 07/19 | 08/19 | 09/19 | 10/19 | 11/19 |
#		| On-program                     | 900   | 900   | ... | 900   | 900   | 900   | 900   | 0     | 0     | 0     |
#		| Completion                     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 2700  | 0     | 0     |
#		| Balancing                      | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#		| English and maths on programme | 33.64 | 33.64 | ... | 33.64 | 33.64 | 33.64 | 33.64 | 33.64 | 33.64 | 0     |
#		| English and maths Balancing    | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#		| Provider learning support      | 150   | 150   | ... | 150   | 150   | 150   | 150   | 150   | 150   | 0     |

