Feature: Levy learner deletes aim reference for eng and maths aim and payments are reconciled - PV2-286
		As a provider,
		I want a levy learner, where aim reference for English and Maths is deleted and payments are reconciled,
		So that I am accurately paid my apprenticeship provision - PV2-286

Scenario Outline: Levy learner deletes aim reference for eng and maths aim and payments are reconciled - PV2-286

	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>

	And the following commitments exist
        | start date                   | end date                  | agreed price | status | Framework Code | Pathway Code | Programme Type |
        | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 9000         | active | 593            | 1            | 20             |

	And the following aims
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Maths or English | 50086832      | 01/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
		| Programme        | ZPROG001      | 01/Aug/Current Academic Year | 12 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
		
	And price details as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        |                  | 471                  | 01/Aug/Current Academic Year        | Act1          | 1                   |                             |
        | pe-1             | 9000                 | 01/Aug/Current Academic Year        | Act1          | 2                   | 90%                         |

    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		# Maths and English
        | Aug/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 39.25                      | 1                   |                          |
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
		#p1
        | Aug/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Sep/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Oct/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Nov/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Dec/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Jan/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Feb/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Mar/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Apr/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |
        | May/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Jun/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Jul/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-1                     |

    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R01/Current Academic Year | Aug/Current Academic Year | 600           | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 600           | 0                         | Learning                   |

    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Programme        | ZPROG001      | 01/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
																																																																			                 
	And price details are changed as follows																																																								                  
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 9000                 | 01/Aug/Current Academic Year        | Act1          | 1                   | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>

    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
        | Aug/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Sep/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Oct/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Nov/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Dec/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jan/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Feb/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Mar/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Apr/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | May/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jun/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jul/Current Academic Year | 600          | 0          | 0         | 0                          | 1                   | pe-1                     |

    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R03/Current Academic Year | Aug/Current Academic Year | 0             | -39.25                    | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Sep/Current Academic Year | 0             | -39.25                    | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 600           | 0                         | Learning                   |
		| R04/Current Academic Year | Nov/Current Academic Year | 600           | 0                         | Learning                   |
		| R05/Current Academic Year | Dec/Current Academic Year | 600           | 0                         | Learning                   |
		| R06/Current Academic Year | Jan/Current Academic Year | 600           | 0                         | Learning                   |

	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R03/Current Academic Year | Aug/Current Academic Year | 0             | -39.25                    | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Sep/Current Academic Year | 0             | -39.25                    | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 600           | 0                         | Learning                   |
		| R04/Current Academic Year | Nov/Current Academic Year | 600           | 0                         | Learning                   |
		| R05/Current Academic Year | Dec/Current Academic Year | 600           | 0                         | Learning                   |
		| R06/Current Academic Year | Jan/Current Academic Year | 600           | 0                         | Learning                   |

Examples: 
        | Collection_Period         | Levy Balance |
        | R03/Current Academic Year | 7800         |
		| R04/Current Academic Year | 7200         |
		| R05/Current Academic Year | 6600         |
		| R06/Current Academic Year | 6000         |



#Scenario: Levy apprentice, deleted aim reference for English/maths aims and payments are refunded for the aim 
#
#		Given The learner is programme only DAS
#        And levy balance > agreed price for all months
#        And the apprenticeship funding band maximum is 9000
#
#        And the following commitments exist:
#			| commitment Id | version Id | ULN       | employer   | provider   | start date | end date   | framework code | programme type | pathway code | agreed price | status    | effective from | effective to |
#			| 1             | 1          | learner a | employer 0 | provider A | 01/08/2018 | 01/08/2019 | 403            | 2              | 1            | 9000         | Active    | 01/08/2018     |              |
#        
#        When an ILR file is submitted for period R01 with the following data:
#            | ULN       | learner type       | aim sequence number | aim type         | aim reference | aim rate | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code |
#            | learner a | programme only DAS | 2                   | programme        | ZPROG001      |          | 9000         | 06/08/2018 | 20/08/2019       |                 | continuing        | 403            | 2              | 1            |
#            | learner a | programme only DAS | 1                   | maths or English | 50086832      | 471      |              | 06/08/2018 | 20/08/2019       |                 | continuing        | 403            | 2              | 1            |
#  
#        And an ILR file is submitted for period R03 with the following data:
#            | ULN       | learner type       | aim sequence number | aim type         | aim reference | aim rate | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code |
#            | learner a | programme only DAS | 1                   | programme        | ZPROG001      |          | 9000         | 06/08/2018 | 20/08/2019       |                 | continuing        | 403            | 2              | 1            |
#  
#        Then the provider earnings and payments break down as follows:
#            | Type                                    | 08/18  | 09/18  | 10/18  | 11/18  | 12/18 | 01/19 |
#            | Provider Earned Total                   | 639.25 | 639.25 | 600    | 600    | 600   | 600   |
#            | Provider Earned from SFA                | 639.25 | 639.25 | 600    | 600    | 600   | 600   |
#            | Provider Earned from Employer           | 0      | 0      | 0      | 0      | 0     | 0     |
#            | Provider Paid by SFA                    | 0      | 639.25 | 639.25 | 600    | 600   | 600   |
#            | Refund taken by SFA                     | 0      | 0      | 0      | -78.50 | 0     | 0     |
#            | Payment due from Employer               | 0      | 0      | 0      | 0      | 0     | 0     |
#            | Refund due to employer                  | 0      | 0      | 0      | 0      | 0     | 0     |
#            | Levy account debited                    | 0      | 600    | 600    | 600    | 600   | 600   |
#            | Levy account credited                   | 0      | 0      | 0      | 0      | 0     | 0     |
#            | SFA Levy employer budget                | 600    | 600    | 600    | 600    | 600   | 600   |
#            | SFA Levy co-funding budget              | 0      | 0      | 0      | 0      | 0     | 0     |
#            | SFA Levy additional payments budget     | 0      | 0      | 0      | 0      | 0     | 0     |
#            | SFA non-Levy co-funding budget          | 0      | 0      | 0      | 0      | 0     | 0     |
#            | SFA non-Levy additional payments budget | 0      | 0      | 0      | 0      | 0     | 0     |

