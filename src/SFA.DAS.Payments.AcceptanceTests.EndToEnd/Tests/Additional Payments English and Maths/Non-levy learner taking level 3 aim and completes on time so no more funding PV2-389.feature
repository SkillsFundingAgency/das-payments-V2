@supports_dc_e2e
Feature: Provider earnings and payments where apprenticeship requires english or maths above level 2 - completes on time. PV2-389

Scenario Outline: Non-levy learner taking level 3 aim and completes on time so no more funding PV2-389
	Given the following learners
        | Learner Reference Number | Uln |
        | abc123                   |     |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                   | Completion Status |
		| Programme        | ZPROG001      | 01/Aug/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        |
		| Maths or English | 50093186      | 01/Aug/Last Academic Year | 12 months        |                 | 2                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        |
	And price details as follows		
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 15000                | 01/Aug/Last Academic Year           |                        |                                       | Act2          | 1                   | 90%                         |
        |                   | 0                    | 01/Aug/Last Academic Year           | 0                      | 01/Aug/Last Academic Year             | Act2          | 2                   | 100%                        |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Price Episode Identifier |
        | Aug/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | Sep/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | Oct/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | Nov/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | Dec/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | Jan/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | Feb/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | Mar/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | Apr/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | May/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | Jun/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | Jul/Last Academic Year | 1000         | 0          | 0         | 0                          | 1st price details        |
        | Aug/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Sep/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Oct/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Nov/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Dec/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Jan/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Feb/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Mar/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Apr/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | May/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Jun/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
        | Jul/Last Academic Year | 0            | 0          | 0         | 39.25                      |                          |
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
        | R01/Last Academic Year | Aug/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R02/Last Academic Year | Sep/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R03/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R05/Last Academic Year | Dec/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R06/Last Academic Year | Jan/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R07/Last Academic Year | Feb/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R08/Last Academic Year | Mar/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R09/Last Academic Year | Apr/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R10/Last Academic Year | May/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R11/Last Academic Year | Jun/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | Jul/Last Academic Year | 0                      | 0                           | 39.25                     | OnProgrammeMathsAndEnglish |
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                | Planned Duration  | Actual Duration   | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                   | Completion Status | Contract Type |
		| Programme        | ZPROG001      | 01/Aug/Last Academic Year | 12 months - 1 day |                   | 1                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        | Act2          |
		| Maths or English | 50093186      | 01/Aug/Last Academic Year | 12 months - 1 day | 12 months - 1 day | 2                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | completed         | Act2          |
	And price details as follows		
        | Price Episode Id  | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 15000                | 01/Aug/Last Academic Year           |                        |                                       | Act2          | 1                   | 90%                         |
        |                   | 0                    | 01/Aug/Last Academic Year           |                        |                                       | Act2          | 2                   | 100%                        |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier | Contract Type |
		# Maths/Eng - Level 2
		| Aug/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Sep/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Oct/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Nov/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Dec/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Jan/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Feb/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Mar/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Apr/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| May/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Jun/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Jul/Current Academic Year | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		#pe-1																													            		   
		| Aug/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
		| Sep/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
		| Oct/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
		| Nov/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
		| Dec/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
		| Jan/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
		| Feb/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
		| Mar/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
		| Apr/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
		| May/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
		| Jun/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
		| Jul/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | 1st price details        | Act2          |
	And only the following payments will be calculated													                     							    
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish |
		| R01/Current Academic Year | Aug/Current Academic Year | 0            | 0          | 0         | 0                          |
		| R02/Current Academic Year | Sep/Current Academic Year | 0            | 0          | 0         | 0                          |
		| R03/Current Academic Year | Oct/Current Academic Year | 0            | 0          | 0         | 0                          |
		| R04/Current Academic Year | Nov/Current Academic Year | 0            | 0          | 0         | 0                          |
		| R05/Current Academic Year | Dec/Current Academic Year | 0            | 0          | 0         | 0                          |
		| R06/Current Academic Year | Jan/Current Academic Year | 0            | 0          | 0         | 0                          |
		| R07/Current Academic Year | Feb/Current Academic Year | 0            | 0          | 0         | 0                          |
		| R08/Current Academic Year | Mar/Current Academic Year | 0            | 0          | 0         | 0                          |
		| R09/Current Academic Year | Apr/Current Academic Year | 0            | 0          | 0         | 0                          |
		| R10/Current Academic Year | May/Current Academic Year | 0            | 0          | 0         | 0                          |
		| R11/Current Academic Year | Jun/Current Academic Year | 0            | 0          | 0         | 0                          |
		| R12/Current Academic Year | Jul/Current Academic Year | 0            | 0          | 0         | 0                          |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type           |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 0                         | Learning                   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R04/Current Academic Year | Nov/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R05/Current Academic Year | Dec/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R06/Current Academic Year | Jan/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R07/Current Academic Year | Feb/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R08/Current Academic Year | Mar/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R09/Current Academic Year | Apr/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R10/Current Academic Year | May/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R11/Current Academic Year | Jun/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |
		| R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 0                         | OnProgrammeMathsAndEnglish |

Examples: 
        | Collection_Period         |
        | R01/Current Academic Year |
        | R02/Current Academic Year |
        | R03/Current Academic Year |
        | R04/Current Academic Year |
        | R05/Current Academic Year |
        | R06/Current Academic Year |
        | R07/Current Academic Year |
        | R08/Current Academic Year |
        | R09/Current Academic Year |
        | R10/Current Academic Year |
        | R11/Current Academic Year |
        | R12/Current Academic Year |