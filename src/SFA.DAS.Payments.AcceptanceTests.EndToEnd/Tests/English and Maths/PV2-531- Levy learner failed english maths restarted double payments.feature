#Feature:  Maths and English
#
#Background Info
##It is permissible for a Level 2 apprentice to fail level 2 English & Maths, retake the learning, but complete the programme because they have already met the policy.
#
#Scenario: Levy learner, takes single level 2 aim, fails, retakes beyond programme end, completes to time
#	Given levy balance > agreed price for all months
#
#	And the following commitments exist:
#		  | ULN       | start date  	      | end date           | agreed price | status   |
#		  | learner a | 06/08/2018            | 08/08/2019         | 15000        | active   |
#	When an ILR file is submitted with the following data:
#		  | ULN       | learner type             | agreed price | start date | planned end date | actual end date | completion status | restart indicator | aim type         |
#		  | learner a | 19-24 programme only DAS | 15000        | 06/08/2018 | 08/08/2019       | 08/08/2019      | completed         | NO                | programme        |
#		  | learner a | 19-24 programme only DAS | 471          | 06/08/2018 | 08/06/2019       | 08/05/2019      | withdrawn         | NO                | maths or english |
#		  | learner a | 19-24 programme only DAS | 471          | 09/06/2019 | 08/06/2020       | 				  | continuing        | YES               | maths or english |
#	
#	Then the provider earnings and payments break down as follows:
#		  | Type                                    | 08/18   | 09/18   | 10/18   | ... | 04/19   | 05/19   | 06/19   | 07/19   | 08/19   | 09/19   | ... | 05/20 | 06/20 |
#		  | Provider Earned Total                   | 1047.10 | 1047.10 | 1047.10 | ... | 1047.10 | 1000    | 1039.25 | 1039.25 | 3039.25 | 39.25   | ... | 39.25 | 0     |
#		  | Provider Earned from SFA           	    | 1047.10 | 1047.10 | 1047.10 | ... | 1047.10 | 1000    | 1039.25 | 1039.25 | 3039.25 | 39.25   | ... | 39.25 | 0     |
#          | Provider Earned from Employer           | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0       | ... | 0     | 0     |
#		  | Provider Paid by SFA                    | 0       | 1047.10 | 1047.10 | ... | 1047.10 | 1047.10 | 1000    | 1039.25 | 1039.25 | 3039.25 | ... | 39.25 | 39.25 |
#		  | Payment due from Employer               | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0       | ... | 0     | 0     |
#		  | Levy account debited                    | 0       | 1000    | 1000    | ... | 1000    | 1000    | 1000    | 1000    | 1000    | 3000    | ... | 0     | 0     |
#		  | SFA Levy employer budget                | 1000    | 1000    | 1000    | ... | 1000    | 1000    | 1000    | 1000    | 3000    | 0       | ... | 0     | 0     |
#		  | SFA levy co-funding budget              | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0       | ... | 0     | 0     |
#		  | SFA non-levy co-funding budget          | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0       | ... | 0     | 0     |
#		  | SFA non-Levy additional payments budget | 0       | 0       | 0       | ... | 0       | 0       | 0       | 0       | 0       | 0       | ... | 0     | 0     |
#		  | SFA levy additional payments budget     | 47.10   | 47.10   | 47.10   | ... | 47.10   | 0       | 39.25   | 39.25   | 39.25   | 39.25   | ... | 39.25 | 0     |
#		 
#	And the transaction types for the payments are:
#		  | Payment type                   | 08/18 | 09/18 | 10/18 | ... | 05/19 | 06/19 | 07/19 | 08/19 | 09/19 | ... | 05/20 | 06/20 |
#		  | On-program                     | 0     | 1000  | 1000  | ... | 1000  | 1000  | 1000  | 1000  | 0     | ... | 0     | 0     |
#		  | Completion                     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 3000  | ... | 0     | 0     |
#		  | Balancing                      | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#		  | English and maths on programme | 0     | 47.10 | 47.10 | ... | 47.10 | 0     | 39.25 | 39.25 | 39.25 | ... | 39.25 | 39.25 |
#		  | English and maths Balancing    | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 	  

# For DC integration
# 3rd ILR line has restart indicator as YES

Feature: Levy learner, takes single level 2 aim, fails, retakes beyond programme end, completes to time -  PV2-531
		As a provider,
		I want a Levy learner with English & Maths aim, where the learner takes a single level 2 aim, and fails, but retakes English & Maths aim beyond the core programme end which completes to time
		So that I am accurately paid my apprenticeship provision

Scenario Outline: Levy learner takes single level 2 aim, fails, retakes beyond programme end, completes to time PV2-531
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                | end date                     | agreed price | status |
        | 06/Aug/Last Academic Year | 08/Aug/Current Academic Year | 15000        | active |
	# New columns - Restart Indicator
	# Do we need it for payments service?
	And the following aims
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status | Restart Indicator |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        | No                |
		| Maths or English | 12345         | 06/Aug/Last Academic Year | 10 months        | 9 months        | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | withdrawn         | No                |
		| Maths or English | 12345         | 09/Jun/Last Academic Year | 12 months        |                 | 3                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        | Yes               |
	And price details as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act1          | 1                   | 90%                         |
        |                  | 0                    | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act1          | 2                   | 100%                        |
        |                  | 0                    | 09/Jun/Last Academic Year           | 0                      | 09/Jun/Last Academic Year             | Act1          | 3                   | 100%                        |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
        | Aug/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Sep/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Oct/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Nov/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Dec/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jan/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Feb/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Mar/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Apr/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | May/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jun/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jul/Last Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		#p2
        | Aug/Last Academic Year | 0            | 0          | 0         | 47.10                      | 2                   |                          |
        | Sep/Last Academic Year | 0            | 0          | 0         | 47.10                      | 2                   |                          |
        | Oct/Last Academic Year | 0            | 0          | 0         | 47.10                      | 2                   |                          |
        | Nov/Last Academic Year | 0            | 0          | 0         | 47.10                      | 2                   |                          |
        | Dec/Last Academic Year | 0            | 0          | 0         | 47.10                      | 2                   |                          |
        | Jan/Last Academic Year | 0            | 0          | 0         | 47.10                      | 2                   |                          |
        | Feb/Last Academic Year | 0            | 0          | 0         | 47.10                      | 2                   |                          |
        | Mar/Last Academic Year | 0            | 0          | 0         | 47.10                      | 2                   |                          |
        | Apr/Last Academic Year | 0            | 0          | 0         | 47.10                      | 2                   |                          |
		# Period 10 is 0 and not paid as withdrawn
        | May/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Jun/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Jul/Last Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
		#p3
        | Aug/Last Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |
        | Sep/Last Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |
        | Oct/Last Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |
        | Nov/Last Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |
        | Dec/Last Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |
        | Jan/Last Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |
        | Feb/Last Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |
        | Mar/Last Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |
        | Apr/Last Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |
        | May/Last Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |
        | Jun/Last Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |
        | Jul/Last Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |

    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Last Academic Year | Aug/Last Academic Year | 1000          | 0                         | Learning                   |
        | R02/Last Academic Year | Sep/Last Academic Year | 1000          | 0                         | Learning                   |
        | R03/Last Academic Year | Oct/Last Academic Year | 1000          | 0                         | Learning                   |
        | R04/Last Academic Year | Nov/Last Academic Year | 1000          | 0                         | Learning                   |
        | R05/Last Academic Year | Dec/Last Academic Year | 1000          | 0                         | Learning                   |
        | R06/Last Academic Year | Jan/Last Academic Year | 1000          | 0                         | Learning                   |
        | R07/Last Academic Year | Feb/Last Academic Year | 1000          | 0                         | Learning                   |
        | R08/Last Academic Year | Mar/Last Academic Year | 1000          | 0                         | Learning                   |
        | R09/Last Academic Year | Apr/Last Academic Year | 1000          | 0                         | Learning                   |
        | R10/Last Academic Year | May/Last Academic Year | 1000          | 0                         | Learning                   |
        | R11/Last Academic Year | Jun/Last Academic Year | 1000          | 0                         | Learning                   |
        | R12/Last Academic Year | Jul/Last Academic Year | 1000          | 0                         | Learning                   |
        | R01/Last Academic Year | Aug/Last Academic Year | 0             | 47.10                     | OnProgrammeMathsAndEnglish |
        | R02/Last Academic Year | Sep/Last Academic Year | 0             | 47.10                     | OnProgrammeMathsAndEnglish |
        | R03/Last Academic Year | Oct/Last Academic Year | 0             | 47.10                     | OnProgrammeMathsAndEnglish |
        | R04/Last Academic Year | Nov/Last Academic Year | 0             | 47.10                     | OnProgrammeMathsAndEnglish |
        | R05/Last Academic Year | Dec/Last Academic Year | 0             | 47.10                     | OnProgrammeMathsAndEnglish |
        | R06/Last Academic Year | Jan/Last Academic Year | 0             | 47.10                     | OnProgrammeMathsAndEnglish |
        | R07/Last Academic Year | Feb/Last Academic Year | 0             | 47.10                     | OnProgrammeMathsAndEnglish |
        | R08/Last Academic Year | Mar/Last Academic Year | 0             | 47.10                     | OnProgrammeMathsAndEnglish |
        | R09/Last Academic Year | Apr/Last Academic Year | 0             | 47.10                     | OnProgrammeMathsAndEnglish |
	#	| R10/Last Academic Year | May/Last Academic Year | 0             | 0                         | OnProgrammeMathsAndEnglish |
        | R11/Last Academic Year | Jun/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | Jul/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |

	# Updated main aim completion status to completed
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year | 12 months        | 12 months       | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | completed         |
		| Maths or English | 12345         | 06/Aug/Last Academic Year | 10 months        | 9 months        | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | withdrawn         |
		| Maths or English | 12345         | 09/Jun/Last Academic Year | 12 months        |                 | 3                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
	And price details as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act1          | 1                   | 90%                         |
        |                  | 0                    | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act1          | 2                   | 100%                        |
        |                  | 0                    | 09/Jun/Last Academic Year           | 0                      | 09/Jun/Last Academic Year             | Act1          | 3                   | 100%                        |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
        | Aug/Current Academic Year | 0            | 3000       | 0         | 0                          | 1                   | pe-1                     |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | May/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
		#p2 - Not sure if earnings calc will return 0 rows for aim 2 as it has been withdrawn
        | Aug/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Nov/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | May/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
		#p3
        | Aug/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |
        | Oct/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |
        | Nov/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |
        | Jan/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |
        | Feb/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |
        | Mar/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |
        | Apr/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |
        | May/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0                          | 3                   |                          |

    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | R01/Current Academic Year | Aug/Current Academic Year | 0            | 3000       | 0         | 39.25                      |
        | R02/Current Academic Year | Sep/Current Academic Year | 0            | 0          | 0         | 39.25                      |
        | R03/Current Academic Year | Oct/Current Academic Year | 0            | 0          | 0         | 39.25                      |
        | R04/Current Academic Year | Nov/Current Academic Year | 0            | 0          | 0         | 39.25                      |
        | R05/Current Academic Year | Dec/Current Academic Year | 0            | 0          | 0         | 39.25                      |
        | R06/Current Academic Year | Jan/Current Academic Year | 0            | 0          | 0         | 39.25                      |
        | R07/Current Academic Year | Feb/Current Academic Year | 0            | 0          | 0         | 39.25                      |
        | R08/Current Academic Year | Mar/Current Academic Year | 0            | 0          | 0         | 39.25                      |
        | R09/Current Academic Year | Apr/Current Academic Year | 0            | 0          | 0         | 39.25                      |
        | R10/Current Academic Year | May/Current Academic Year | 0            | 0          | 0         | 39.25                      |

    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 3000          | 0                         | Completion                 |
        | R01/Current Academic Year | Aug/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |

	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 3000          | 0                         | Completion                 |
        | R01/Current Academic Year | Aug/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |

Examples: 
        | Collection_Period         | Levy Balance |
        | R01/Current Academic Year | 3500         |
        | R02/Current Academic Year | 500          |
        | R03/Current Academic Year | 500          |
        | R04/Current Academic Year | 500          |
        | R05/Current Academic Year | 500          |
        | R06/Current Academic Year | 500          |
        | R07/Current Academic Year | 500          |
        | R08/Current Academic Year | 500          |
        | R09/Current Academic Year | 500          |
        | R10/Current Academic Year | 500          |
        | R11/Current Academic Year | 500          |