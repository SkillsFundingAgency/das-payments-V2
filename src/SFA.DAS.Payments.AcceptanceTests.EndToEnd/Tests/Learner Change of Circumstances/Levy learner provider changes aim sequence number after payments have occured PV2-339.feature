﻿#		Scenario:822-AC01- Levy apprentice, provider changes aim sequence numbers in ILR after payments have already occurred
#
#        Given The learner is programme only DAS
#        And levy balance > agreed price for all months
#        And the apprenticeship funding band maximum is 9000
#
#        And the following commitments exist:
#			| commitment Id | version Id | ULN       | start date | end date   | framework code | programme type | pathway code | agreed price | status    | effective from | effective to |
#			| 1             | 1          | learner a | 01/08/2018 | 01/08/2019 | 593            | 2              | 1            | 9000         | Active    | 01/08/2018     |              |
#        
#		When an ILR file is submitted for period R01 with the following data:
#			| ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | aim rate | framework code | programme type | pathway code |
#			| learner a | programme only DAS | 9000         | 06/08/2018 | 20/08/2019       |                 | continuing        | programme        | 1                   |          | 593            | 2              | 1            |
#        
#        And an ILR file is submitted for period R03 with the following data:
#			| ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | aim type         | aim sequence number | aim rate | framework code | programme type | pathway code |
#			| learner a | programme only DAS | 9000         | 06/08/2018 | 20/08/2019       |                 | continuing        | programme        | 2                   |          | 593            | 2              | 1            |
#			| learner a | programme only DAS |              | 06/08/2018 | 20/08/2019       |                 | continuing        | maths or english | 1                   | 471      | 593            | 2              | 1            |
#  
#        Then the provider earnings and payments break down as follows:
#			| Type                                    | 08/18 | 09/18 | 10/18  | 11/18  |
#			| Provider Earned Total                   | 600   | 600   | 639.25 | 639.25 |
#			| Provider Earned from SFA                | 600   | 600   | 639.25 | 0      |
#			| Provider Earned from Employer           | 0     | 0     | 0      | 0      |
#			| Provider Paid by SFA                    | 0     | 600   | 600    | 717.75 |
#			| Refund taken by SFA                     | 0     | 0     | 0      | 0      |
#			| Payment due from Employer               | 0     | 0     | 0      | 0      |
#			| Refund due to employer                  | 0     | 0     | 0      | 0      |
#			| Levy account debited                    | 0     | 600   | 600    | 600    |
#			| Levy account credited                   | 0     | 0     | 0      | 0      |
#			| SFA Levy employer budget                | 600   | 600   | 600    | 600    |
#			| SFA Levy co-funding budget              | 0     | 0     | 0      | 0      |
#			| SFA Levy additional payments budget     | 39.25 | 39.25 | 39.25  | 39.25  |
#			| SFA Levy co-funding budget          | 0     | 0     | 0      | 0      |
#			| SFA Levy additional payments budget | 0     | 0     | 0      | 0      |

Feature: Levy - Learner change of circumstances - change in aim sequence number PV2-339
As a provider,
I want a levy learner, where provider changes aim sequence number after payments have occurred, to be paid the correct amount
So that I am accurately paid my apprenticeship provision.

Scenario Outline: Levy apprentice, provider changes aim sequence numbers in ILR after payments have already occurred PV2-339

	Given the employer levy account balance in collection period <Collection_Period> is <Levy_Balance>
	# Commitment lines
	And the following commitments exist
		| start date                   | end date                     | framework code | programme type | pathway code | agreed price | status | effective from               | effective to |
		| 01/Aug/Current Academic Year | 31/Jul/Current Academic Year | 593            | 20             | 1            | 9000         | Active | 01/Aug/Current Academic Year |              |

	#And the provider is providing training for the following learners
	#	| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type         | SFA Contribution Percentage |
	#	| 01/Aug/Current Academic Year | 12 months        | 9000                 | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship Levy | 90%                         |

	And the following aims
		| Aim Type  | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Contract Type | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Programme | ZPROG001      | 06/Aug/Current Academic Year | 12 months        |                 | Act1          | 1                   | 593            | 1            | 20             | 16-18 Apprenticeship Levy | continuing        |      
	And price details as follows	
	# Price details
        | Price Details     | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 9000                 | 06/Aug/Current Academic Year        | Act1          | 1                   | 90%                         |

    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 600          | 0          | 0         |
        | Sep/Current Academic Year | 600          | 0          | 0         |
        | Oct/Current Academic Year | 600          | 0          | 0         |
        | Nov/Current Academic Year | 600          | 0          | 0         |
        | Dec/Current Academic Year | 600          | 0          | 0         |
        | Jan/Current Academic Year | 600          | 0          | 0         |
        | Feb/Current Academic Year | 600          | 0          | 0         |
        | Mar/Current Academic Year | 600          | 0          | 0         |
        | Apr/Current Academic Year | 600          | 0          | 0         |
        | May/Current Academic Year | 600          | 0          | 0         |
        | Jun/Current Academic Year | 600          | 0          | 0         |
        | Jul/Current Academic Year | 600          | 0          | 0         |

    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 600           | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 600           | Learning         |

    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Contract Type | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Maths or English | 12345         | 06/Aug/Current Academic Year | 12 months        |                 | Act1          | 1                   | 593            | 1            | 20             | 16-18 Apprenticeship Levy | continuing        |
		| Programme        | ZPROG001      | 06/Aug/Current Academic Year | 12 months        |                 | Act1          | 2                   | 593            | 1            | 20             | 16-18 Apprenticeship Levy | continuing        |
	
	And price details are changed as follows		
        | Price Details     | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 9000                 | 06/Aug/Current Academic Year        | Act1          | 2                   | 90%                         |
        | 2nd price details | 0                    | 06/Aug/Current Academic Year        | Act1          | 1                   | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | Aug/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Sep/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Oct/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Nov/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Dec/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Jan/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Feb/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Mar/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Apr/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | May/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Jun/Current Academic Year | 600          | 0          | 0         | 39.25                      |
        | Jul/Current Academic Year | 600          | 0          | 0         | 39.25                      |

    And only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Aug/Current Academic Year | 0            | 0          | 0         | 39.25                      |
        | R03/Current Academic Year | Sep/Current Academic Year | 0            | 0          | 0         | 39.25                      |
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
        | R03/Current Academic Year | Aug/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Sep/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
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

	And at month end only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R03/Current Academic Year | Oct/Current Academic Year | 600                    | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 600                    | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 600                    | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 600                    | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 600                    | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 600                    | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 600                    | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 600                    | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 600                    | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 600                    | 0                         | Learning                   |
        | R03/Current Academic Year | Aug/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Sep/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
        | R11/Current Academic Year | Jun/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Current Academic Year | Jul/Current Academic Year | 0                      | 39.25                     | OnProgrammeMathsAndEnglish |
Examples: 
        | Collection_Period         | Levy_Balance |
        | R03/Current Academic Year | 1200         |
        | R04/Current Academic Year | 1200         |
        | R05/Current Academic Year | 1200         |
        | R06/Current Academic Year | 1200         |
        | R07/Current Academic Year | 1200         |
        | R08/Current Academic Year | 1200         |
        | R09/Current Academic Year | 1200         |
        | R10/Current Academic Year | 1200         |
        | R11/Current Academic Year | 1200         |
        | R12/Current Academic Year | 1200         |