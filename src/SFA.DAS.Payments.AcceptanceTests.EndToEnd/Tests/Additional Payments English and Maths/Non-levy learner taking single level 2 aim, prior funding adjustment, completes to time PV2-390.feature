#@supports_dc_e2e
Feature: Non-levy learner taking single level 2 aim, prior funding adjustment, completes to time PV2-390
	As a provider,
	I want a non-levy learner, planned duration is same as programme (assumes both start and finish at same time), to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.
Scenario Outline: Non-levy learner taking single level 2 aim, prior funding adjustment, completes to time PV2-390
	Given the following learners
        | Learner Reference Number |
        | abc123                   |
	And the following aims
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                   | Completion Status | Funding Adjustment For Prior Learning |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        | n/a                                   |
		| Maths or English | 50094695      | 06/Aug/Last Academic Year | 12 months        |                 | 2                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        | 75%                                   |
	And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 06/Aug/Last Academic Year           |                        |                                       | Act2          | 1                   | 90%                         |
        |                  | 0                    | 06/Aug/Last Academic Year           |                        |                                       | Act2          | 2                   | 100%                        |
	And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		# pe-1
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
		# Maths/Eng
        | Aug/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
        | Sep/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
        | Oct/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
        | Nov/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
        | Dec/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
        | Jan/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
        | Feb/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
        | Mar/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
        | Apr/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
        | May/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
        | Jun/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |
        | Jul/Last Academic Year | 0            | 0          | 0         | 29.44                      | 2                   |                          |

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
        | R01/Last Academic Year | Aug/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R02/Last Academic Year | Sep/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R03/Last Academic Year | Oct/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R05/Last Academic Year | Dec/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R06/Last Academic Year | Jan/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R07/Last Academic Year | Feb/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R08/Last Academic Year | Mar/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R09/Last Academic Year | Apr/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R10/Last Academic Year | May/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R11/Last Academic Year | Jun/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
        | R12/Last Academic Year | Jul/Last Academic Year | 0                      | 0                           | 29.44                     | OnProgrammeMathsAndEnglish |
    But aims details are changed as follows
		| Aim Type         | Aim Reference | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                   | Completion Status | Funding Adjustment For Prior Learning |
		| Programme        | ZPROG001      | 06/Aug/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        | n/a                                   |
		| Maths or English | 50094695      | 06/Aug/Last Academic Year | 12 months        | 12 months       | 2                   | 593            | 1            | 20             | 19+ Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | completed         | 75%                                   |
	And price details are changed as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-2             | 15000                | 06/Aug/Last Academic Year           |                        |                                       | Act2          | 1                   | 90%                         |
        |                  | 0                    | 06/Aug/Last Academic Year           |                        |                                       | Act2          | 2                   | 100%                        |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | OnProgrammeMathsAndEnglish | Aim Sequence Number | Price Episode Identifier |
		# pe-2
		| Aug/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		| Apr/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		| May/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		| Jun/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		| Jul/Current Academic Year | 0            | 0          | 0         | 0                          | 1                   | pe-2                     |
		# Maths/Eng
		| Aug/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
		| Sep/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
		| Oct/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
		| Nov/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
		| Dec/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
		| Jan/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
		| Feb/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
		| Mar/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
		| Apr/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
		| May/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
		| Jun/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
		| Jul/Last Academic Year    | 0            | 0          | 0         | 0                          | 2                   |                          |
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