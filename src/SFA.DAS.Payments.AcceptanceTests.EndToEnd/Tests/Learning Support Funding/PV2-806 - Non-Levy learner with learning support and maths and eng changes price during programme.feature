@ignore
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
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Reference | SFA Contribution Percentage | Funding Line Type             | Framework Code | Pathway Code | Programme Type |
		| 06/Aug/Last Academic Year | 12 months        | 12000                | 04/Aug/Current Academic Year        | 3000                   | 04/Aug/Current Academic Year          |                 | continuing        | Act2          | ZPROG001      | 90%                         | 19-24 Apprenticeship Non-Levy | 593            | 1            | 20             |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status | LearningSupportIsFunctionalSkill | Price Episode Id |
		| Programme        | ZPROG001      | 04/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        |                                  | pe-1             |
		| Maths or English | 12345         | 04/Aug/Current Academic Year | 14 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        | true                             | pe-2             |
	And price details as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 11250                | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act2          | 1                   | 90%                         |
        | pe-2             | 0                    | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act2          | 2                   | 100%                        |
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
        | Aug/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Sep/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Oct/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Nov/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Dec/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Jan/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Feb/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Mar/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Apr/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | May/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Jun/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Jul/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
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
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type             | Completion Status | LearningSupportIsFunctionalSkill | Price Episode Id |
		| Programme        | ZPROG001      | 04/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        |                                  | pe-1             |
		| Maths or English | 12345         | 04/Aug/Current Academic Year | 14 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Non-Levy | continuing        | true                             | pe-2             |
    And price details are changed as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 11250                | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act2          | 1                   | 90%                         |
        | pe-2             | 0                    | 04/Aug/Current Academic Year        | 0                      | 04/Aug/Current Academic Year          | Act2          | 2                   | 100%                        |
        | pe-3             | 6750                 | 11/Nov/Current Academic Year        | 0                      | 11/Nov/Current Academic Year          | Act2          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
        | Aug/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Sep/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Oct/Current Academic Year | 750          | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | May/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-1                     |
        #p2
        | Aug/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Sep/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Oct/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Nov/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Dec/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Jan/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Feb/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Mar/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Apr/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | May/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Jun/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
        | Jul/Current Academic Year | 0            | 0          | 0         | 150             | 33.64                      | 2                   | pe-2                     |
		#p3
        | Aug/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        | Nov/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        | Dec/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        | Jan/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        | Feb/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        | Mar/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        | Apr/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        | May/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        | Jun/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
        | Jul/Current Academic Year | 350          | 0          | 0         | 0               | 0                          | 1                   | pe-3                     |
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


#Feature: Learning Support
#
#Scenario: Non Levy learner, is taking an English or maths qualification, has learning support and the negotiated price changes during the programme
#    
#	Given the apprenticeship funding band maximum is 18000
#    
#    When an ILR file is submitted with the following data:
#        | ULN       | learner type           | aim type         | start date | planned end date | actual end date | completion status | aim rate | Total training price 1 | Total training price 1 effective date | Total assessment price 1 | Total assessment price 1 effective date | Total training price 2 | Total training price 2 effective date | Total assessment price 2 | Total assessment price 2 effective date | learning support code | learning support date from | learning support date to |
#        | learner a | programme only non-DAS | programme        | 04/08/2018 | 20/08/2019       |                 | continuing        |          | 9000                   | 04/08/2018                            | 2250                     | 04/08/2018                              | 5400                   | 11/11/2018                            | 1350                     | 11/11/2018                              | 1                     | 06/08/2018                 | 06/10/2019               |
#        | learner a | programme only non-DAS | maths or english | 04/08/2018 | 06/10/2019       |                 | continuing        | 471      |                        |                                       |                          |                                         |                        |                                       |                          |                                         | 1                     | 06/08/2018                 | 06/10/2019               |        
#    
#	Then the provider earnings and payments break down as follows: 
#        | Type                                    | 08/18   | 09/18  | 10/18   | 11/18   | 12/18  | 01/19  | 
#        | Provider Earned Total                   | 933.64  | 933.64 | 933.64  | 533.64  | 533.64 | 533.64 |       
#        | Provider Earned from SFA                | 858.64  | 858.64 | 858.64  | 498.64  | 498.64 | 498.64 |       
#        | Provider Earned from Employer           | 75      | 75     | 75      | 35      | 35     | 35     |       
#        | Provider Paid by SFA                    | 0       | 858.64 | 858.64  | 858.64  | 498.64 | 498.64 |        
#        | Payment due from Employer               | 0       | 75     | 75      | 75      | 35     | 35     |       
#        | Levy account debited                    | 0       | 0      | 0       | 0       | 0      | 0      |         
#        | SFA Levy employer budget                | 0       | 0      | 0       | 0       | 0      | 0      |
#        | SFA Levy co-funding budget              | 0       | 0      | 0       | 0       | 0      | 0      |
#        | SFA non-Levy co-funding budget          | 675     | 675    | 675     | 315     | 315    | 315    |               
#        | SFA Levy additional payments budget     | 0       | 0      | 0       | 0       | 0      | 0      |
#        | SFA non-Levy additional payments budget | 183.64  | 183.64 | 183.64  | 183.64  | 183.64 | 183.64 |         
#    
#	And the transaction types for the payments are:
#		| Payment type                   | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 |
#		| On-program                     | 675   | 675   | 675   | 315   | 315   | (see below calculation for new on-program payment)
#		| Completion                     | 0     | 0     | 0     | 0     | 0     |
#		| Balancing                      | 0     | 0     | 0     | 0     | 0     |
#        | English and maths on programme | 33.64 | 33.64 | 33.64 | 33.64 | 33.64 |
#		| English and maths Balancing    | 0     | 0     | 0     | 0     | 0     |
#        | Provider learning support      | 150   | 150   | 150   | 150   | 150   |
#
##On Program payment drops to £350 per month as a result of the change in Price episode to £6750
##New Agreed Price of £6750*0.8(on program payments) = £5400
##Subtract the On-Program payments from the first price episode which have already been paid £5400-(£750*3) = £3150
##New Price episode is over 9 months, so on-program payments from November onwards £3150/9 = £350 
##SFA contribution £350*0.9 = £315

