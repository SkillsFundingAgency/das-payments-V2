#Feature:  Maths and English
#	
#Scenario: Levy learner, funding agreed within band maximum, planned duration is same as programme (assumes both start and finish at same time), other funding adjutsment is 75%
#	
#	
#	Given levy balance > agreed price for all months
#		
#    And the following commitments exist:
#		  | ULN       | start date | end date   | agreed price | status |
#		  | learner a | 06/08/2018 | 08/08/2019 | 15000        | active |
#
#	When an ILR file is submitted with the following data:
#		  | ULN       | learner type             | agreed price | start date | planned end date | actual end date | completion status | funding adjustment for prior learning | other funding adjustment | aim type         |
#		  | learner a | 19-24 programme only DAS | 15000        | 06/08/2018 | 08/08/2019       |                 | continuing        | n/a                                   | n/a                      | programme        |
#		  | learner a | 19-24 programme only DAS | 471          | 06/08/2018 | 08/08/2019       | 08/08/2019      | completed         | n/a                                   | 75%                      | maths or english |
#		  	  
#		  
##	The English or maths aim is submitted with the same start and planned end date
#      
#    Then the provider earnings and payments break down as follows:
#		  | Type                                    | 08/18   | 09/18   | 10/18   | 11/18   | 12/18   | ... | 07/19   | 08/19   |
#		  | Provider Earned Total                   | 1029.44 | 1029.44 | 1029.44 | 1029.44 | 1029.44 | ... | 1029.44 | 0       |
#		  | Provider Earned from SFA           	   	| 1029.44 | 1029.44 | 1029.44 | 1029.44 | 1029.44 | ... | 1029.44 | 0       |
#		  | Provider Earned from Employer          	| 0       | 0       | 0       | 0       | 0       | ... | 0       | 0       |
#		  | Provider Paid by SFA                    | 0       | 1029.44 | 1029.44 | 1029.44 | 1029.44 | ... | 1029.44 | 1029.44 |
#		  | Payment due from Employer               | 0       | 0       | 0       | 0       | 0       | ... | 0       | 0       |
#		  | Levy account debited                    | 0       | 1000    | 1000    | 1000    | 1000    | ... | 1000    | 1000    |
#		  | SFA Levy employer budget                | 1000    | 1000    | 1000    | 1000    | 1000    | ... | 1000    | 0       |
#		  | SFA Levy co-funding budget              | 0       | 0       | 0       | 0       | 0       | ... | 0       | 0       |
#		  | SFA non-Levy co-funding budget          | 0       | 0       | 0       | 0       | 0       | ... | 0       | 0       |
#		  | SFA non-levy additional payments budget | 0       | 0       | 0       | 0       | 0       | ... | 0       | 0       |
#		  | SFA levy additional payments budget     | 29.44   | 29.44   | 29.44   | 29.44   | 29.44   | ... | 29.44   | 0       |
#		  
#    And the transaction types for the payments are:
#		  | Payment type                   | 09/18 | 10/18 | 11/18 | 12/18 | ... | 07/19 | 08/19 |
#		  | On-program                     | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 1000  |
#		  | Completion                     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#		  | Balancing                      | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#          | English and maths on programme | 29.44 | 29.44 | 29.44 | 29.44 | ... | 29.44 | 29.44 |
#		  | English and maths Balancing    | 0     | 0     | 0     | 0     | ... | 0     | 0     |		

# DC Integration 
# 19-24 learner
#| funding adjustment for prior learning | other funding adjustment |
#| n/a                                   | n/a                      |
#| n/a                                   | 75%                      |

Feature: Levy learner with English & Maths and other funding adjustment - PV2-541
		As a provider,
		I want a Levy learner with English & Maths aim, where the planned duration is the same as the core program and there is other funding adjustment for English & Maths, and the learner completes English & Maths aim on time
		So that I am accurately paid my apprenticeship provision

Scenario Outline: Levy learner with English & Maths and other funding adjustment PV2-541
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                   | end date                  | agreed price | status | Framework Code | Pathway Code | Programme Type |
        | 06/Aug/Current Academic Year | 08/Aug/Next Academic Year | 15000        | active | 593            | 1            | 20             |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Programme        | ZPROG001      | 06/Aug/Current Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
		| Maths or English | 12345         | 06/Aug/Current Academic Year | 12 months        | 12 months       | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | completed         |
	And price details as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | Act1          | 1                   | 90%                         |
        |                  | 0                    | 06/Aug/Current Academic Year        | 0                      | 06/Aug/Current Academic Year          | Act1          | 2                   | 100%                        |

	When the ILR file is submitted for the learners for collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
		| Aug/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Sep/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Oct/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Nov/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Dec/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Jan/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Feb/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Mar/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Apr/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		| May/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Jun/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		| Jul/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		#p2
		| Aug/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
		| Sep/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
		| Oct/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
		| Nov/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
		| Dec/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
		| Jan/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
		| Feb/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
		| Mar/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
		| Apr/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
		| May/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
		| Jun/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
		| Jul/Current Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |

    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
		| R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         | 29.44                      |
		| R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         | 29.44                      |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         | 29.44                      |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         | 29.44                      |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         | 29.44                      |
		| R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         | 29.44                      |
		| R07/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         | 29.44                      |
		| R08/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         | 29.44                      |
		| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         | 29.44                      |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         | 29.44                      |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         | 29.44                      |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         | 29.44                      |

	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000          | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000          | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 1000          | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 1000          | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 1000          | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 1000          | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 1000          | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000          | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 1000          | 0                         | Learning                   |
        | R01/Current Academic Year | Aug/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R11/Current Academic Year | Jun/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R12/Current Academic Year | Jul/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |

	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000          | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000          | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 1000          | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 1000          | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 1000          | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 1000          | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 1000          | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 1000          | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 1000          | 0                         | Learning                   |
        | R01/Current Academic Year | Aug/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R06/Current Academic Year | Jan/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R07/Current Academic Year | Feb/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R08/Current Academic Year | Mar/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R09/Current Academic Year | Apr/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R10/Current Academic Year | May/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R11/Current Academic Year | Jun/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |
        | R12/Current Academic Year | Jul/Current Academic Year | 0             | 29.44                     | OnProgrammeMathsAndEnglish |

Examples: 
        | Collection_Period         | Levy Balance |
        | R01/Current Academic Year | 15500        |
        | R02/Current Academic Year | 14500        |
        | R03/Current Academic Year | 13500        |
        | R04/Current Academic Year | 12500        |
        | R05/Current Academic Year | 11500        |
        | R06/Current Academic Year | 10500        |
        | R07/Current Academic Year | 9500         |
        | R08/Current Academic Year | 8500         |
        | R09/Current Academic Year | 7500         |
        | R10/Current Academic Year | 6500         |
        | R11/Current Academic Year | 5500         |
        | R12/Current Academic Year | 4500         |