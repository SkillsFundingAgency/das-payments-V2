@ignore this spec seems to need work
#Scenario: Levy apprentice, provider changes course details in ILR after payments have already occurred
#
#        Given The learner is programme only DAS
#        And levy balance > agreed price for all months
#        And the apprenticeship funding band maximum is 9000
#
#         And the following commitments exist for academic year 1819 in period R01:
#			| commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status |
#			| 1             | 1          | learner a | 01/08/2018 | 01/08/2019 | 401            | 2              | 1            | 9000         | Active |
#
#         And the following commitments exist for academic year 1819 in period R03:
#			| commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status    | stop effective from |
#			| 1             | 1          | learner a | 01/08/2018 | 01/08/2019 | 401            | 2              | 1            | 9000         | Cancelled | 01/08/2018          |
#			| 2             | 1          | learner a | 01/08/2018 | 01/08/2019 | 404            | 2              | 1            | 9000         | Active    |                     |
#        
#        When an ILR file is submitted for period R01 with the following data:
#			| ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | aim rate | framework code | programme type | pathway code |
#			| learner a | programme only DAS | 9000         | 06/08/2018 | 20/08/2019       |                 | continuing        | programme        | 1                   |          | 401            | 2              | 1            |
#        
#        And an ILR file is submitted for period R03 with the following data:
#			| ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | aim rate | framework code | programme type | pathway code |
#			| learner a | programme only DAS | 9000         | 06/08/2018 | 20/08/2019       |                 | continuing        | programme        | 2                   |          | 404            | 2              | 1            |
#			| learner a | programme only DAS |              | 06/08/2018 | 20/08/2019       |                 | continuing        | maths or english | 1                   | 471      | 404            | 2              | 1            |
#  
#        Then the provider earnings and payments break down as follows:
#			| Type                                    | 08/18 | 09/18 | 10/18  | 11/18   | 12/18  | 01/19  |
#			| Provider Earned Total                   | 600   | 600   | 639.25 | 639.25  | 639.25 | 639.25 |
#			| Provider Earned from SFA                | 600   | 600   | 639.25 | 639.25  | 639.25 | 639.25 |
#			| Provider Earned from Employer           | 0     | 0     | 0      | 0       | 0      | 0      |
#			| Provider Paid by SFA                    | 0     | 600   | 600    | 1917.75 | 639.25 | 639.25 |
#			| Refund taken by SFA                     | 0     | 0     | 0      | -1200   | 0      | 0      |
#			| Payment due from Employer               | 0     | 0     | 0      | 0       | 0      | 0      |
#			| Refund due to employer                  | 0     | 0     | 0      | 0       | 0      | 0      |
#			| Levy account debited                    | 0     | 600   | 600    | 1800    | 600    | 600    |
#			| Levy account credited                   | 0     | 0     | 0      | 1200    | 0      | 0      |
#			| SFA Levy employer budget                | 600   | 600   | 600    | 600     | 600    | 600    |
#			| SFA Levy co-funding budget              | 0     | 0     | 0      | 0       | 0      | 0      |
#			| SFA Levy additional payments budget     | 39.25 | 39.25 | 39.25  | 39.25   | 39.25  | 39.25  |
#			| SFA non-Levy co-funding budget          | 0     | 0     | 0      | 0       | 0      | 0      |
#			| SFA non-Levy additional payments budget | 0     | 0     | 0      | 0       | 0      | 0      |


# DC Integration
# Course details are key in this test

Feature: Levy learner changes course details in ILR after payments made and adds eng & maths - PV2-284
		As a provider,
		I want a levy learner, where the course details have changed after payments have been made, to be paid the correct amount,
		So that I am accurately paid my apprenticeship provision - PV2-284

Scenario Outline: Levy learner changes course details in ILR after payments made and adds eng & maths - PV2-284
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                   | end date                  | agreed price | status | Framework Code | Pathway Code | Programme Type |
        | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 9000         | active | 593            | 1            | 20             |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Programme        | ZPROG001      | 06/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
	And price details as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 9000                 | 06/Aug/Current Academic Year        | Act1          | 1                   | 90%                         |
    And the following earnings had been generated for the learner
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

    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 600           | 0                         | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 600           | 0                         | Learning         |
	# change the Framework code if not valid to any value other than 593
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Maths or English | 12345         | 06/Aug/Current Academic Year | 12 months        |                 | 1                   | 584            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
		| Programme        | ZPROG001      | 06/Aug/Current Academic Year | 12 months        |                 | 2                   | 584            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
	And price details are changed as follows																																																								                  
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 9000                 | 06/Aug/Current Academic Year        | Act1          | 2                   | 90%                         |
        |                  | 471                  | 06/Aug/Current Academic Year        | Act1          | 1                   | 100%                        |  
        | pe-3             | 9000                 | 06/Aug/Current Academic Year        | Act1          | 2                   | 90%                         |
	And the Commitment details are changed as follows
        | start date                   | end date                  | agreed price | status    | Framework Code | Pathway Code | Programme Type | stop effective from          |
        | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 9000         | cancelled | 593            | 1            | 20             | 01/Aug/Current Academic Year |
        | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 9000         | active    | 584            | 1            | 20             |                              |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
        | Aug/Current Academic Year | -600         | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Sep/Current Academic Year | -600         | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     |
        | May/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   | pe-1                     |
		#p2 - Maths and English
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
		#p3
        | Aug/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
        | Sep/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
        | Oct/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
        | Nov/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
        | Dec/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
        | Jan/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
        | Feb/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
        | Mar/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
        | Apr/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
        | May/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
        | Jun/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
        | Jul/Current Academic Year | 600          | 0          | 0         | 0                          | 2                   | pe-3                     |
    And at month end only the following payments will be calculated
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
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R03/Current Academic Year | Aug/Current Academic Year | -600          | 0                         | Learning                   |
        | R03/Current Academic Year | Sep/Current Academic Year | -600          | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 600           | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 600           | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 600           | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 600           | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 600           | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 600           | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 600           | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 600           | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 600           | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R11/Current Academic Year | Jun/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Current Academic Year | Jul/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |

	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R03/Current Academic Year | Aug/Current Academic Year | -600          | 0                         | Learning                   |
        | R03/Current Academic Year | Sep/Current Academic Year | -600          | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 600           | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 600           | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 600           | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 600           | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 600           | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 600           | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 600           | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 600           | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 600           | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 600           | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R11/Current Academic Year | Jun/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Current Academic Year | Jul/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |

Examples: 
        | Collection_Period         | Levy Balance |
        | R03/Current Academic Year | 8400         |
        | R04/Current Academic Year | 7800         |
        | R05/Current Academic Year | 7200         |
        | R06/Current Academic Year | 6600         |
        | R07/Current Academic Year | 6000         |
        | R08/Current Academic Year | 5400         |
        | R09/Current Academic Year | 4800         |
        | R10/Current Academic Year | 4200         |
        | R11/Current Academic Year | 3600         |
        | R12/Current Academic Year | 3000         |