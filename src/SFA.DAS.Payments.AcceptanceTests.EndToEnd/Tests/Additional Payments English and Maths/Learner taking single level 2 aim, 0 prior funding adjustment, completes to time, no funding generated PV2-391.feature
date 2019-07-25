#For DC Integration
#| Funding Adjustment For Prior Learning |
#| n/a                                   |
#| 0%                                    |

#@supports_dc_e2e
Feature: Learner taking single level 2 aim, 0 prior funding adjustment, completes to time, no funding generated PV2-391
	As a provider,
	I want a payment for a non-levy learner, planned duration is same as programme (assumes both start and finish at same time), to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.
Scenario Outline: Learner taking single level 2 aim, 0 prior funding adjustment, completes to time, no funding generated PV2-391
	Given the following learners
        | Learner Reference Number |      
        | 0fm36391                   |  
	And the following aims
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                   | Completion Status |
		| Programme        | ZPROG001      | 01/Aug/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        |
		| Maths or English | 50093186      | 01/Aug/Last Academic Year | 12 months        |                 | 2                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        |
	And price details as follows		
        | Price details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | 1st price details | 15000                | 01/Aug/Last Academic Year           | 0                      | 01/Aug/Last Academic Year             | Act2          | 1                   | 90%                         |
        | 2nd price details | 0                    | 01/Aug/Last Academic Year           | 0                      | 01/Aug/Last Academic Year             | Act2          | 2                   | 100%                        |
	And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year | 1000         | 0          | 0         |
        | Sep/Last Academic Year | 1000         | 0          | 0         |
        | Oct/Last Academic Year | 1000         | 0          | 0         |
        | Nov/Last Academic Year | 1000         | 0          | 0         |
        | Dec/Last Academic Year | 1000         | 0          | 0         |
        | Jan/Last Academic Year | 1000         | 0          | 0         |
        | Feb/Last Academic Year | 1000         | 0          | 0         |
        | Mar/Last Academic Year | 1000         | 0          | 0         |
        | Apr/Last Academic Year | 1000         | 0          | 0         |
        | May/Last Academic Year | 1000         | 0          | 0         |
        | Jun/Last Academic Year | 1000         | 0          | 0         |
        | Jul/Last Academic Year | 1000         | 0          | 0         |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R01/Last Academic Year | Aug/Last Academic Year | 900                    | 100                         | Learning         |
        | R02/Last Academic Year | Sep/Last Academic Year | 900                    | 100                         | Learning         |
        | R03/Last Academic Year | Oct/Last Academic Year | 900                    | 100                         | Learning         |
        | R04/Last Academic Year | Nov/Last Academic Year | 900                    | 100                         | Learning         |
        | R05/Last Academic Year | Dec/Last Academic Year | 900                    | 100                         | Learning         |
        | R06/Last Academic Year | Jan/Last Academic Year | 900                    | 100                         | Learning         |
        | R07/Last Academic Year | Feb/Last Academic Year | 900                    | 100                         | Learning         |
        | R08/Last Academic Year | Mar/Last Academic Year | 900                    | 100                         | Learning         |
        | R09/Last Academic Year | Apr/Last Academic Year | 900                    | 100                         | Learning         |
        | R10/Last Academic Year | May/Last Academic Year | 900                    | 100                         | Learning         |
        | R11/Last Academic Year | Jun/Last Academic Year | 900                    | 100                         | Learning         |
        | R12/Last Academic Year | Jul/Last Academic Year | 900                    | 100                         | Learning         |
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                   | Completion Status |
		| Programme        | ZPROG001      | 01/Aug/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        |
		| Maths or English | 50093186      | 01/Aug/Last Academic Year | 12 months        | 12 months       | 2                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | completed         |
	#And price details as follows		
 #       | Price details     | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
 #       | 1st price details | 15000                | 01/Aug/Last Academic Year           | 0                      | 01/Aug/Last Academic Year             | Act2          | 1                   | 90%                         |
 #       | 2nd price details | 0                    | 01/Aug/Last Academic Year           | 0                      | 01/Aug/Last Academic Year             | Act2          | 2                   | 100%                        |
	And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 01/Aug/Last Academic Year           | 0                      | 01/Aug/Last Academic Year             | Act2          | 1                   | 90%                         |
        |                  | 0                    | 01/Aug/Last Academic Year           | 0                      | 01/Aug/Last Academic Year             | Act2          | 2                   | 100%                        |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	
	#Then no learner earnings should be generated
	
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier | Contract Type |
		#pe-1																													            		   
		| Aug/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		| Sep/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		| Oct/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		| Nov/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		| Dec/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		| Jan/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		| Feb/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		| Mar/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		| Apr/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		| May/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		| Jun/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		| Jul/Current Academic Year | 300          | 0          | 0         | 0                          | 1                   | pe-1                     | Act2          |
		# Maths/Eng - Level 2
		| Aug/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Sep/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Oct/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Nov/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Dec/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Jan/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Feb/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Mar/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Apr/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| May/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Jun/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		| Jul/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          | Act2          |
		
	And no provider payments will be recorded

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