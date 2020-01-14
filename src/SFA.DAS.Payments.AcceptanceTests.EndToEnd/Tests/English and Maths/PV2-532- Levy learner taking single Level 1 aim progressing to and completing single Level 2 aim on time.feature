Feature: Levy learner taking single Level 1 aim, progressing to and completing single Level 2 aim, completes to time - PV2-532
		As a provider,
		I want a Levy learner with English & Maths aim, where the learner takes and completes a single Level 1 aim, then progresses to and completes a single Level 2 aim
		So that I am accurately paid my apprenticeship provision

Scenario Outline: Levy learner taking single Level 1 aim progressing to and completing single Level 2 aim on time PV2-532
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                | end date                  | agreed price | status | Framework Code | Pathway Code | Programme Type |
        | 06/Aug/Last Academic Year | 08/Aug/Next Academic Year | 15000        | active | 593            | 1            | 20             |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year | 24 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
		| Maths or English | 12345         | 06/Aug/Last Academic Year | 12 months        |                 | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
	And price details as follows	
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act1          | 1                   | 90%                         |
        |                  | 0                    | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act1          | 2                   | 100%                        |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		#p1
        | Aug/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Sep/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Oct/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Nov/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Dec/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jan/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Feb/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Mar/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Apr/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | May/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jun/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
        | Jul/Last Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     |
		#p2
        | Aug/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Sep/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Oct/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Nov/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Dec/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Jan/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Feb/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Mar/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Apr/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | May/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Jun/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
        | Jul/Last Academic Year | 0            | 0          | 0         | 39.25                      | 2                   |                          |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Last Academic Year | Aug/Last Academic Year | 500           | 0                         | Learning                   |
        | R02/Last Academic Year | Sep/Last Academic Year | 500           | 0                         | Learning                   |
        | R03/Last Academic Year | Oct/Last Academic Year | 500           | 0                         | Learning                   |
        | R04/Last Academic Year | Nov/Last Academic Year | 500           | 0                         | Learning                   |
        | R05/Last Academic Year | Dec/Last Academic Year | 500           | 0                         | Learning                   |
        | R06/Last Academic Year | Jan/Last Academic Year | 500           | 0                         | Learning                   |
        | R07/Last Academic Year | Feb/Last Academic Year | 500           | 0                         | Learning                   |
        | R08/Last Academic Year | Mar/Last Academic Year | 500           | 0                         | Learning                   |
        | R09/Last Academic Year | Apr/Last Academic Year | 500           | 0                         | Learning                   |
        | R10/Last Academic Year | May/Last Academic Year | 500           | 0                         | Learning                   |
        | R11/Last Academic Year | Jun/Last Academic Year | 500           | 0                         | Learning                   |
        | R12/Last Academic Year | Jul/Last Academic Year | 500           | 0                         | Learning                   |
        | R01/Last Academic Year | Aug/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R02/Last Academic Year | Sep/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R03/Last Academic Year | Oct/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Last Academic Year | Nov/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Last Academic Year | Dec/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R06/Last Academic Year | Jan/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R07/Last Academic Year | Feb/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Last Academic Year | Mar/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Last Academic Year | Apr/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Last Academic Year | May/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R11/Last Academic Year | Jun/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | Jul/Last Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
	# Updated main aim completion status to completed
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type         | Completion Status |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year    | 24 months        |                 | 1                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
		| Maths or English | 12345         | 06/Aug/Last Academic Year    | 12 months        | 12 months       | 2                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | completed         |
		| Maths or English | 67890         | 09/Aug/Current Academic Year | 12 months        |                 | 3                   | 593            | 1            | 20             | 19-24 Apprenticeship Levy | continuing        |
	And price details are changed as follows
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act1          | 1                   | 90%                         |
		# check if 2nd price details for last year is needed here
        |                  | 0                    | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | Act1          | 2                   | 100%                        |
        |                  | 0                    | 09/Aug/Current Academic Year        | 0                      | 09/Aug/Current Academic Year          | Act1          | 3                   | 100%                        |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier | Contract Type |
		#p1
        | Aug/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
        | Sep/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
        | Oct/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
        | Nov/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
        | Dec/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
        | Jan/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
        | Feb/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
        | Mar/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
        | Apr/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
        | May/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
        | Jun/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
        | Jul/Current Academic Year | 500          | 0          | 0         | 0                          | 1                   | pe-1                     | Act1          |
		#p3
        | Aug/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
        | Sep/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
        | Oct/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
        | Nov/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
        | Jan/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
        | Feb/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
        | Mar/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
        | Apr/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
        | May/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
        | Jun/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
        | Jul/Current Academic Year | 0            | 0          | 0         | 39.25                      | 3                   |                          | Act1          |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
        | R01/Current Academic Year | Aug/Current Academic Year | 500          | 0          | 0         | 39.25                      |
        | R02/Current Academic Year | Sep/Current Academic Year | 500          | 0          | 0         | 39.25                      |
        | R03/Current Academic Year | Oct/Current Academic Year | 500          | 0          | 0         | 39.25                      |
        | R04/Current Academic Year | Nov/Current Academic Year | 500          | 0          | 0         | 39.25                      |
        | R05/Current Academic Year | Dec/Current Academic Year | 500          | 0          | 0         | 39.25                      |
        | R06/Current Academic Year | Jan/Current Academic Year | 500          | 0          | 0         | 39.25                      |
        | R07/Current Academic Year | Feb/Current Academic Year | 500          | 0          | 0         | 39.25                      |
        | R08/Current Academic Year | Mar/Current Academic Year | 500          | 0          | 0         | 39.25                      |
        | R09/Current Academic Year | Apr/Current Academic Year | 500          | 0          | 0         | 39.25                      |
        | R10/Current Academic Year | May/Current Academic Year | 500          | 0          | 0         | 39.25                      |
        | R11/Current Academic Year | Jun/Current Academic Year | 500          | 0          | 0         | 39.25                      |
        | R12/Current Academic Year | Jul/Current Academic Year | 500          | 0          | 0         | 39.25                      |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 500           | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 500           | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 500           | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 500           | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 500           | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 500           | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 500           | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 500           | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 500           | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 500           | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 500           | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 500           | 0                         | Learning                   |
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
        | R11/Current Academic Year | Jun/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Current Academic Year | Jul/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type           |
        | R01/Current Academic Year | Aug/Current Academic Year | 500           | 0                         | Learning                   |
        | R02/Current Academic Year | Sep/Current Academic Year | 500           | 0                         | Learning                   |
        | R03/Current Academic Year | Oct/Current Academic Year | 500           | 0                         | Learning                   |
        | R04/Current Academic Year | Nov/Current Academic Year | 500           | 0                         | Learning                   |
        | R05/Current Academic Year | Dec/Current Academic Year | 500           | 0                         | Learning                   |
        | R06/Current Academic Year | Jan/Current Academic Year | 500           | 0                         | Learning                   |
        | R07/Current Academic Year | Feb/Current Academic Year | 500           | 0                         | Learning                   |
        | R08/Current Academic Year | Mar/Current Academic Year | 500           | 0                         | Learning                   |
        | R09/Current Academic Year | Apr/Current Academic Year | 500           | 0                         | Learning                   |
        | R10/Current Academic Year | May/Current Academic Year | 500           | 0                         | Learning                   |
        | R11/Current Academic Year | Jun/Current Academic Year | 500           | 0                         | Learning                   |
        | R12/Current Academic Year | Jul/Current Academic Year | 500           | 0                         | Learning                   |
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
        | R11/Current Academic Year | Jun/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Current Academic Year | Jul/Current Academic Year | 0             | 39.25                     | OnProgrammeMathsAndEnglish |
Examples: 
        | Collection_Period         | Levy Balance |
        | R01/Current Academic Year | 9500         |
        | R02/Current Academic Year | 9000         |
        | R03/Current Academic Year | 8500         |
        | R04/Current Academic Year | 8000         |
        | R05/Current Academic Year | 7500         |
        | R06/Current Academic Year | 7000         |
        | R07/Current Academic Year | 6500         |
        | R08/Current Academic Year | 6000         |
        | R09/Current Academic Year | 5500         |
        | R10/Current Academic Year | 5000         |
        | R11/Current Academic Year | 4500         |
        | R12/Current Academic Year | 4000         |






#Feature:  Maths and English
#
#Scenario: Levy learner taking single Level 1 aim, progressing to and completing single Level 2 aim, completes to time 
#
#Given levy balance > agreed price for all months
#		
#And the following commitments exist:
#		  | ULN       | start date | end date   | agreed price | status |
#		  | learner a | 06/08/2018 | 08/08/2020 | 15000        | active |
#
#When an ILR file is submitted with the following data:
#		  | ULN       | learner type             | agreed price | start date | planned end date | actual end date | completion status | aim type         |
#		  | learner a | 19-24 programme only DAS | 15000        | 06/08/2018 | 08/08/2020       |                 | continuing        | programme        |
#		  | learner a | 19-24 programme only DAS | 471          | 06/08/2018 | 08/08/2019       | 08/08/2019      | completed         | maths or english |
#		  | learner a | 19-24 programme only DAS | 471          | 09/08/2019 | 08/08/2020       | 08/08/2020      | completed         | maths or english |
#		  
#	
#Then the provider earnings and payments break down as follows:
#		  | Type                                    | 08/18  | 09/18  | 10/18  | 11/18  | 12/18  | ... | 08/19  | 09/19  | ... | 07/20  | 08/20  |
#		  | Provider Earned Total                   | 539.25 | 539.25 | 539.25 | 539.25 | 539.25 | ... | 539.25 | 539.25 | ... | 539.25 | 0      |
#		  | Provider Earned from SFA           	    | 539.25 | 539.25 | 539.25 | 539.25 | 539.25 | ... | 539.25 | 539.25 | ... | 539.25 | 0      |
#		  | Provider Earned from Employer           | 0      | 0      | 0      | 0      | 0      | ... | 0      | 0      | ... | 0      | 0      |
#		  | Provider Paid by SFA                    | 0      | 539.25 | 539.25 | 539.25 | 539.25 | ... | 539.25 | 539.25 | ... | 539.25 | 539.25 |
#		  | Payment due from Employer               | 0      | 0      | 0      | 0      | 0      | ... | 0      | 0      | ... | 0      | 0      |
#		  | Levy account debited                    | 0      | 500    | 500    | 500    | 500    | ... | 500    | 500    | ... | 500    | 500    |
#		  | SFA Levy employer budget                | 500    | 500    | 500    | 500    | 500    | ... | 500    | 500    | ... | 500    | 0      |
#		  | SFA Levy co-funding budget              | 0      | 0      | 0      | 0      | 0      | ... | 0      | 0      | ... | 0      | 0      |
#		  | SFA non-levy co-funding budget          | 0      | 0      | 0      | 0      | 0      | ... | 0      | 0      | ... | 0      | 0      |
#		  | SFA non-Levy additional payments budget | 0      | 0      | 0      | 0      | 0      | ... | 0      | 0      | ... | 0      | 0      |
#		  | SFA levy additional payments budget     | 39.25  | 39.25  | 39.25  | 39.25  | 39.25  | ... | 39.25  | 39.25  | ... | 39.25  | 0      |
#		  
#
#    And the transaction types for the payments are:
#		  | Payment type                   | 09/18 | 10/18 | 11/18 | 12/18 | ... | 08/19 | 09/19 | ... | 07/20 | 08/20 |
#		  | On-program                     | 500   | 500   | 500   | 500   | ... | 500   | 500   | ... | 500   | 500   |
#		  | Completion                     | 0     | 0     | 0     | 0     | ... | 0     | 0     | ... | 0     | 0     |
#		  | Balancing                      | 0     | 0     | 0     | 0     | ... | 0     | 0     | ... | 0     | 0     |
#		  | English and maths on programme | 39.25 | 39.25 | 39.25 | 39.25 | ... | 39.25 | 39.25 | ... | 39.25 | 39.25 |
#		  | English and maths Balancing    | 0     | 0     | 0     | 0     | ... | 0     | 0     | ... | 0     | 0     |
