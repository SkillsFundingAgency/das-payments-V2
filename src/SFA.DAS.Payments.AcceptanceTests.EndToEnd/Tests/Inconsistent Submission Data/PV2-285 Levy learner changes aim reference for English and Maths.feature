@ignore
Feature: Levy learner changes aim reference for eng and maths aim and payments are reconciled - PV2-285
		As a provider,
		I want a levy learner, where aim reference for English and Maths is changed and payments are reconciled,
		So that I am accurately paid my apprenticeship provision - PV2-285

Scenario Outline: Levy learner changes aim reference for eng and maths aim and payments are reconciled - PV2-285
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                | end date                     | agreed price | status |
        | 01/May/Last Academic Year | 01/May/Current Academic Year | 9000         | active |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Programme        | ZPROG001      | 01/May/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
		| Maths or English | 60001227      | 01/May/Last Academic Year | 12 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
	And price details as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 9000                 | 01/May/Last Academic Year           | Act1          | 1                   | 90%                         |
        |                  | 471                  | 01/May/Last Academic Year           | Act1          | 2                   |                             |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
        | Aug/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Sep/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Oct/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Nov/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Dec/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jan/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Feb/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Mar/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Apr/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | May/Last Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jun/Last Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jul/Last Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
		#p2 - Maths and English
        | Aug/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Sep/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Oct/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Nov/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Dec/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Jan/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Feb/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Mar/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Apr/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | May/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Jun/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Jul/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |

    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R10/Last Academic Year | May/Last Academic Year | 600           | 0                         | Learning                   |
        | R11/Last Academic Year | Jun/Last Academic Year | 600           | 0                         | Learning                   |
        | R10/Last Academic Year | May/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R11/Last Academic Year | Jun/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |

    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Maths or English | 50086832      | 01/May/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
		| Programme        | ZPROG001      | 01/May/Last Academic Year | 12 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
																																																																			                 
	And price details are changed as follows																																																								                  
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        |                  | 471                  | 01/May/Last Academic Year           | Act1          | 1                   |                             |
        | pe-1             | 9000                 | 01/May/Last Academic Year           | Act1          | 2                   | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	# New technical task is needed to hanndle Learning Aim Reference column
    Then the following learner earnings should be generated
        | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier | Learning Aim Reference |
		#p1
        | Aug/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
        | Sep/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
        | Oct/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
        | Nov/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
        | Dec/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
        | Jan/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
        | Feb/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
        | Mar/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
        | Apr/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
        | May/Last Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
        | Jun/Last Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
        | Jul/Last Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     | ZPROG001               |
		#p2
        | Aug/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 60001227               |
        | Sep/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 60001227               |
        | Oct/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 60001227               |
        | Nov/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 60001227               |
        | Dec/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 60001227               |
        | Jan/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 60001227               |
        | Feb/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 60001227               |
        | Mar/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 60001227               |
        | Apr/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 60001227               |
        | May/Last Academic Year | 0            | 0          | 0         | -39.25                     | 1                   |                          | 60001227               |
        | Jun/Last Academic Year | 0            | 0          | 0         | -39.25                     | 1                   |                          | 60001227               |
        | Jul/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 60001227               |
		#p3
        | Aug/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 50086832               |
        | Sep/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 50086832               |
        | Oct/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 50086832               |
        | Nov/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 50086832               |
        | Dec/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 50086832               |
        | Jan/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 50086832               |
        | Feb/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 50086832               |
        | Mar/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 50086832               |
        | Apr/Last Academic Year | 0            | 0          | 0         | 0                          | 1                   |                          | 50086832               |
        | May/Last Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          | 50086832               |
        | Jun/Last Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          | 50086832               |
        | Jul/Last Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          | 50086832               |

	## Option 1
	## -ve and +ve resulted in 0 values - check if we need price episodes here
 #   And at month end only the following payments will be calculated
 #       | Collection Period      | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
 #       | R12/Last Academic Year | May/Last Academic Year | 0            | 0          | 0         | 0                          |
 #       | R12/Last Academic Year | Jun/Last Academic Year | 0            | 0          | 0         | 0                          |
 #       | R12/Last Academic Year | Jul/Last Academic Year | 600          | 0          | 0         | 39.25                      |
	
	# Option 2
    And at month end only the following payments will be calculated
        | Collection Period      | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | May/Last Academic Year | 0            | 0          | 0         | -39.25                     |
        | R12/Last Academic Year | May/Last Academic Year | 0            | 0          | 0         | 39.25                      |
        | R12/Last Academic Year | Jun/Last Academic Year | 0            | 0          | 0         | -39.25                     |
        | R12/Last Academic Year | Jun/Last Academic Year | 0            | 0          | 0         | 39.25                      |
        | R12/Last Academic Year | Jul/Last Academic Year | 600          | 0          | 0         | 39.25                      |

    And only the following provider payments will be recorded
        | Collection Period      | Delivery Period        | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R12/Last Academic Year | Jul/Last Academic Year | 600           | 0                         | Learning                   |
        | R12/Last Academic Year | May/Last Academic Year | 0             | -39.25                    | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | Jun/Last Academic Year | 0             | -39.25                    | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | May/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | Jun/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | Jul/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |

	And only the following provider payments will be generated
        | Collection Period      | Delivery Period        | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R12/Last Academic Year | Jul/Last Academic Year | 600           | 0                         | Learning                   |
        | R12/Last Academic Year | May/Last Academic Year | 0             | -39.25                    | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | Jun/Last Academic Year | 0             | -39.25                    | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | May/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | Jun/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | Jul/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |

Examples: 
        | Collection_Period      | Levy Balance |
        | R12/Last Academic Year | 7800         |




#Scenario:852-AC02 Levy apprentice, changes aim reference for English/maths aims and payments are reconciled 
#
#		Given The learner is programme only DAS
#        And levy balance > agreed price for all months
#        And the apprenticeship funding band maximum is 9000
#
#        And the following commitments exist:
#			| commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status    | effective from | effective to |
#			| 1             | 1          | learner a | 01/05/2018 | 01/05/2019 | 403            | 2              | 1            | 9000         | Active    | 01/05/2018     |              |
#        
#        And following learning has been recorded for previous payments:
#            | ULN       | start date | aim sequence number | aim type         | aim reference | framework code | programme type | pathway code | completion status |
#            | learner a | 06/05/2018 | 1                   | programme        | ZPROG001      | 403            | 2              | 1            | continuing        |
#  
#        And the following programme earnings and payments have been made to the provider A for learner a:
#            | Type                                | 05/18 | 06/18 | 07/18 | 08/18 |
#            | Provider Earned Total               | 600   | 600   | 0     | 0     |
#            | Provider Earned from SFA            | 600   | 600   | 0     | 0     |
#            | Provider Earned from Employer       | 0     | 0     | 0     | 0     |
#            | Provider Paid by SFA                | 0     | 600   | 600   | 0     |
#            | Payment due from Employer           | 0     | 0     | 0     | 0     |
#            | Levy account debited                | 0     | 600   | 600   | 0     |
#            | SFA Levy employer budget            | 600   | 600   | 0     | 0     |
#            | SFA Levy co-funding budget          | 0     | 0     | 0     | 0     |
#            | SFA Levy additional payments budget | 0     | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget      | 0     | 0     | 0     | 0     | 
#
#		And following learning has been recorded for previous payments:
#            | ULN       | start date | aim sequence number | aim type         | aim reference | framework code | programme type | pathway code | completion status |
#            | learner a | 06/05/2018 | 2                   | maths or English | 60001227      | 403            | 2              | 1            | continuing        |
#  
#        And the following maths or english earnings and payments have been made to the provider A for learner a:
#            | Type                                | 05/18 | 06/18 | 07/18 | 08/18 |
#            | Provider Earned Total               | 39.25 | 39.25 | 0     | 0     |
#            | Provider Earned from SFA            | 39.25 | 39.25 | 0     | 0     |
#            | Provider Earned from Employer       | 0     | 0     | 0     | 0     |
#            | Provider Paid by SFA                | 0     | 39.25 | 39.25 | 0     |
#            | Payment due from Employer           | 0     | 0     | 0     | 0     |
#            | Levy account debited                | 0     | 0     | 0     | 0     |
#            | SFA Levy employer budget            | 0     | 0     | 0     | 0     |
#            | SFA Levy co-funding budget          | 0     | 0     | 0     | 0     |
#            | SFA Levy additional payments budget | 39.25 | 39.25 | 0     | 0     |
#            | SFA non-Levy co-funding budget      | 0     | 0     | 0     | 0     | 
#        
#        When an ILR file is submitted for the first time on 31/07/18 with the following data:
#            | ULN       | learner type       | aim sequence number | aim type         | aim reference | aim rate | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code |
#            | learner a | programme only DAS | 2                   | programme        | ZPROG001      |          | 9000         | 06/05/2018 | 20/05/2019       |                 | continuing        | 403            | 2              | 1            |
#            | learner a | programme only DAS | 1                   | maths or English | 50086832      | 471      |              | 06/05/2018 | 20/05/2019       |                 | continuing        | 403            | 2              | 1            |
#  
#        Then the provider earnings and payments break down as follows:
#            | Type                                    | 05/18  | 06/18  | 07/18  | 08/18   | 09/18  | 10/18  |
#            | Provider Earned Total                   | 639.25 | 639.25 | 639.25 | 639.25  | 639.25 | 639.25 |
#            | Provider Earned from SFA                | 639.25 | 639.25 | 639.25 | 639.25  | 639.25 | 639.25 |
#            | Provider Earned from Employer           | 0      | 0      | 0      | 0       | 0      | 0      |
#            | Provider Paid by SFA                    | 0      | 639.25 | 639.25 | 717.75  | 639.25 | 639.25 |
#            | Refund taken by SFA                     | 0      | 0      | 0      | -78.50  | 0      | 0      |
#            | Payment due from Employer               | 0      | 0      | 0      | 0       | 0      | 0      |
#            | Refund due to employer                  | 0      | 0      | 0      | 0       | 0      | 0      |
#            | Levy account debited                    | 0      | 600    | 600    | 600     | 600    | 600    |
#            | Levy account credited                   | 0      | 0      | 0      | 0       | 0      | 0      |
#            | SFA Levy employer budget                | 600    | 600    | 600    | 600     | 600    | 600    |
#            | SFA Levy co-funding budget              | 0      | 0      | 0      | 0       | 0      | 0      |
#            | SFA Levy additional payments budget     | 39.25  | 39.25  | 39.25  | 39.25   | 39.25  | 39.25  |
#            | SFA non-Levy co-funding budget          | 0      | 0      | 0      | 0       | 0      | 0      |
#            | SFA non-Levy additional payments budget | 0      | 0      | 0      | 0       | 0      | 0      |   


