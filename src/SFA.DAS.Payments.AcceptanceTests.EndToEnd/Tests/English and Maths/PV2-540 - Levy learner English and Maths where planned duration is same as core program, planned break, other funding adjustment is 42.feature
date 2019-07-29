#Feature:  Maths and English
#	  
#Scenario: Levy learner, funding agreed within band maximum, planned duration is same as programme (assumes both start and finish at same time) planned break, other funding adjustment is 42%
#
##	Tech Guide 102. If an adjustment is required due to prior learning, you must record data in the Funding adjustment for prior learning field on the ILR. 
#		  
#	Given levy balance > agreed price for all months
#		
#    And the following commitments exist:
#		  | ULN       | start date | end date   | agreed price | status |
#		  | learner a | 06/08/2018 | 08/08/2019 | 15000        | active |
#
#	When an ILR file is submitted with the following data:
#		  | ULN       | learner type             | agreed price | start date | planned end date | actual end date | completion status | funding adjustment for prior learning | other funding adjustment | restart indicator | aim type         |
#		  | learner a | 19-24 programme only DAS | 15000        | 06/08/2018 | 08/08/2019       | 08/01/2019      | planned break     | n/a                                   | n/a                      | NO                | programme        |
#		  | learner a | 19-24 programme only DAS | 471          | 06/08/2018 | 08/08/2019       | 08/01/2019      | planned break     | n/a                                   | n/a                      | NO                | maths or english |
#		  | learner a | 19-24 programme only DAS | 15000        | 06/08/2019 | 08/03/2020       |                 | continuing        | n/a                                   | n/a                      | YES               | programme        |
#		  | learner a | 19-24 programme only DAS | 471          | 06/08/2019 | 08/03/2020       | 08/03/2019      | completed         | 42%                                   | n/a                      | YES               | maths or english |
#		  
##	The English or maths aim is submitted with the same start and planned end date
#      
#    Then the provider earnings and payments break down as follows:
#		  | Type                                    | 08/18   | 09/18   | ... | 12/18   | 01/19   | ...   | 08/19   | 09/19   | 10/19   | ... | 02/20   | 03/20   |
#		  | Provider Earned Total                   | 1039.25 | 1039.25 | ... | 1039.25 | 0       | ...   | 1028.26 | 1028.26 | 1028.26 | ... | 1028.26 | 0       |
#		  | Provider Earned from SFA           	   	| 1039.25 | 1039.25 | ... | 1039.25 | 0       | ...   | 1028.26 | 1028.26 | 1028.26 | ... | 1028.26 | 0       |
#		  | Provider Earned from Employer          	| 0       | 0       | ... | 0       | 0       | ...   | 0       | 0       | 0       | ... | 0       | 0       |
#		  | Provider Paid by SFA                    | 0       | 1039.25 | ... | 1039.25 | 1039.25 | ...   | 0       | 1028.26 | 1028.26 | ... | 1028.26 | 1028.26 |
#		  | Payment due from Employer               | 0       | 0       | ... | 0       | 0       | ...   | 0       | 0       | 0       | ... | 0       | 0       |
#		  | Levy account debited                    | 0       | 1000    | ... | 1000    | 1000    | ...   | 0       | 1000    | 1000    | ... | 1000    | 1000    |
#		  | SFA Levy employer budget                | 1000    | 1000    | ... | 1000    | 0       | ...   | 1000    | 1000    | 1000    | ... | 1000    | 0       |
#		  | SFA Levy co-funding budget              | 0       | 0       | ... | 0       | 0       | ...   | 0       | 0       | 0       | ... | 0       | 0       |
#		  | SFA non-Levy co-funding budget          | 0       | 0       | ... | 0       | 0       | ...   | 0       | 0       | 0       | ... | 0       | 0       |
#		  | SFA non-levy additional payments budget | 0       | 0       | ... | 0       | 0       | ...   | 0       | 0       | 0       | ... | 0       | 0       |
#		  | SFA levy additional payments budget     | 39.25   | 39.25   | ... | 39.25   | 0       | ...   | 28.26   | 28.26   | 28.26   | ... | 28.26   | 0       |
#		  
#    And the transaction types for the payments are:
#		  | Payment type                   | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 | ... | 09/19 | 10/19 | 11/19 | ... | 02/20 | 03/20 |
#		  | On-program                     | 1000  | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 1000  | 1000  | ... | 1000  | 1000  |
#		  | Completion                     | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | ... | 0     | 0     |
#		  | Balancing                      | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | ... | 0     | 0     |
#		  | English and maths on programme | 39.25 | 39.25 | 39.25 | 39.25 | 39.25 | ... | 28.26 | 28.26 | 28.26 | ... | 28.26 | 28.26 |
#		  | English and maths Balancing    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | ... | 0     | 0     |	

# Remaining 42% (197.82) is divided into remaining 7 months - Hence 28.26
# Restart Indicator
#For DC Integration
 #| Funding Adjustment For Prior Learning |
 #| n/a                                   |
 #| 42% for remaining period|                                 | n/a                      |

Feature:  Levy learner with English & Maths and prior funding adjustment and planned break in learning - PV2-540
		As a provider,
		I want a Levy learner with English & Maths aim, where the planned duration is the same as the core program and 
		there is other funding adjustment for planned break for English & Maths, and the learner completes English & Maths aim
		So that I am accurately paid my apprenticeship provision PV2-540

Scenario Outline: Levy learner with English & Maths and prior funding adjustment and planned break in learning PV2-540
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                   | end date                  | agreed price | status |Framework Code | Pathway Code | Programme Type |
        | 06/Aug/Current Academic Year | 08/Aug/Next Academic Year | 15000        | active |593            | 1            | 20             |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Programme        | ZPROG001      | 06/Aug/Current Academic Year | 12 months        | 5 months        | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | planned break     |
		| Maths or English | 12345         | 06/Aug/Current Academic Year | 12 months        | 5 months        | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | planned break     |
	And price details as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 06/Aug/Current Academic Year        | Act1          | 1                   | 90%                         |
        |                  | 471                  | 06/Aug/Current Academic Year        | Act1          | 2                   | 100%                        |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
        | Aug/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Sep/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Oct/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Nov/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Dec/Current Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-1                     |
		# planned break - 0 earnings
        | Jan/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | May/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-1                     |
		#p2
        | Aug/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Oct/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Nov/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
		# planned break - 0 earnings
        | Jan/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | May/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000          | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000          | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000          | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000          | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000          | 0                         | Learning                   |
        | R01/Current Academic Year | Aug/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R02/Current Academic Year | Sep/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R03/Current Academic Year | Oct/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Current Academic Year | Dec/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status | funding adjustment for prior learning | restart indicator |
		| Programme        | ZPROG001      | 06/Aug/Next Academic Year | 7 months         |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        | n/a                                   | YES               |
		| Maths or English | 12345         | 06/Aug/Next Academic Year | 7 months         | 7 months        | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | completed         | 42%                                   | YES               |
																																																																			                 
	And price details are changed as follows																																																								                  
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-3             | 15000                | 06/Aug/Next Academic Year           | Act1          | 1                   | 90%                         |
        |                  | 471                  | 06/Aug/Next Academic Year           | Act1          | 2                   | 100%                        |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier | Contract Type |
		#p3
        | Aug/Next Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
        | Sep/Next Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
        | Oct/Next Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
        | Nov/Next Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
        | Dec/Next Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
        | Jan/Next Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
        | Feb/Next Academic Year | 1000         | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
        | Mar/Next Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
        | Apr/Next Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
        | May/Next Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
        | Jun/Next Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
        | Jul/Next Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-3                     | Act1          |
		#p4
        | Aug/Next Academic Year | 0            | 0          | 0         | 28.26                      | 2                   |                          | Act1          |
        | Sep/Next Academic Year | 0            | 0          | 0         | 28.26                      | 2                   |                          | Act1          |
        | Oct/Next Academic Year | 0            | 0          | 0         | 28.26                      | 2                   |                          | Act1          |
        | Nov/Next Academic Year | 0            | 0          | 0         | 28.26                      | 2                   |                          | Act1          |
        | Dec/Next Academic Year | 0            | 0          | 0         | 28.26                      | 2                   |                          | Act1          |
        | Jan/Next Academic Year | 0            | 0          | 0         | 28.26                      | 2                   |                          | Act1          |
        | Feb/Next Academic Year | 0            | 0          | 0         | 28.26                      | 2                   |                          | Act1          |
        | Mar/Next Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act1          |
        | Apr/Next Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act1          |
        | May/Next Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act1          |
        | Jun/Next Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act1          |
        | Jul/Next Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act1          |
    And at month end only the following payments will be calculated
        | Collection Period      | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | R01/Next Academic Year | Aug/Next Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R02/Next Academic Year | Sep/Next Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R03/Next Academic Year | Oct/Next Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R04/Next Academic Year | Nov/Next Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R05/Next Academic Year | Dec/Next Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R06/Next Academic Year | Jan/Next Academic Year | 1000         | 0          | 0         | 28.26                      |
        | R07/Next Academic Year | Feb/Next Academic Year | 1000         | 0          | 0         | 28.26                      |
    And only the following provider payments will be recorded
        | Collection Period      | Delivery Period        | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Next Academic Year | Aug/Next Academic Year | 1000          | 0                         | Learning                   |
        | R02/Next Academic Year | Sep/Next Academic Year | 1000          | 0                         | Learning                   |
        | R03/Next Academic Year | Oct/Next Academic Year | 1000          | 0                         | Learning                   |
        | R04/Next Academic Year | Nov/Next Academic Year | 1000          | 0                         | Learning                   |
        | R05/Next Academic Year | Dec/Next Academic Year | 1000          | 0                         | Learning                   |
        | R06/Next Academic Year | Jan/Next Academic Year | 1000          | 0                         | Learning                   |
        | R07/Next Academic Year | Feb/Next Academic Year | 1000          | 0                         | Learning                   |
        | R01/Next Academic Year | Aug/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R02/Next Academic Year | Sep/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R03/Next Academic Year | Oct/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R04/Next Academic Year | Nov/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R05/Next Academic Year | Dec/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R06/Next Academic Year | Jan/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R07/Next Academic Year | Feb/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
	And only the following provider payments will be generated
        | Collection Period      | Delivery Period        | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Next Academic Year | Aug/Next Academic Year | 1000          | 0                         | Learning                   |
        | R02/Next Academic Year | Sep/Next Academic Year | 1000          | 0                         | Learning                   |
        | R03/Next Academic Year | Oct/Next Academic Year | 1000          | 0                         | Learning                   |
        | R04/Next Academic Year | Nov/Next Academic Year | 1000          | 0                         | Learning                   |
        | R05/Next Academic Year | Dec/Next Academic Year | 1000          | 0                         | Learning                   |
        | R06/Next Academic Year | Jan/Next Academic Year | 1000          | 0                         | Learning                   |
        | R07/Next Academic Year | Feb/Next Academic Year | 1000          | 0                         | Learning                   |
        | R01/Next Academic Year | Aug/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R02/Next Academic Year | Sep/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R03/Next Academic Year | Oct/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R04/Next Academic Year | Nov/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R05/Next Academic Year | Dec/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R06/Next Academic Year | Jan/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
        | R07/Next Academic Year | Feb/Next Academic Year | 0             | 28.26                     | OnProgrammeMathsAndEnglish |
Examples: 
        | Collection_Period      | Levy Balance |
        | R01/Next Academic Year | 10500        |
        | R02/Next Academic Year | 9500         |
        | R03/Next Academic Year | 8500         |
        | R04/Next Academic Year | 7500         |
        | R05/Next Academic Year | 6500         |
        | R06/Next Academic Year | 5500         |
        | R07/Next Academic Year | 4500         |