Feature: 5% Contribution from April 2019 - PV2-902
	As a provider,
	I want a Non Levy learner, starting prior to Apr 2019, where learner completes learning and starts a new course on new Pathway code in Apr 2019 on 10% contribution
	So that I am paid the correct apprenticeship funding by SFA	

Scenario Outline: Existing Non Levy Learner, started learning before Apr19, completes learning and starts new course on new pathway code from Apr19, second pathway code on 5% contribution PV2-902
	Given the following learners
		| Learner Reference Number |
		| abc123                   |
	And the following aims
		| Aim Type  | Priority | Start Date                | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | Completion Status | Contract Type | Aim Reference |
		| Programme | 1        | 01/Apr/Last Academic Year | 12 months        |                 | 1                   | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        | Act2          | ZPROG001      |
  	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 15000                | 01/Apr/Last Academic Year           |                        |                                       | Act2          | 1                   | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year    | 0            | 0          | 0         |
        | Sep/Last Academic Year    | 0            | 0          | 0         |
        | Oct/Last Academic Year    | 0            | 0          | 0         |
        | Nov/Last Academic Year    | 0            | 0          | 0         |
        | Dec/Last Academic Year    | 0            | 0          | 0         |
        | Jan/Last Academic Year    | 0            | 0          | 0         |
        | Feb/Last Academic Year    | 0            | 0          | 0         |
        | Mar/Last Academic Year    | 0            | 0          | 0         |
        | Apr/Last Academic Year    | 1000         | 0          | 0         |
        | May/Last Academic Year    | 1000         | 0          | 0         |
        | Jun/Last Academic Year    | 1000         | 0          | 0         |
        | Jul/Last Academic Year    | 1000         | 0          | 0         |
        | Aug/Current Academic Year | 1000         | 0          | 0         |
        | Sep/Current Academic Year | 1000         | 0          | 0         |
        | Oct/Current Academic Year | 1000         | 0          | 0         |
        | Nov/Current Academic Year | 1000         | 0          | 0         |
		| Dec/Current Academic Year | 1000         | 0          | 0         |
		| Jan/Current Academic Year | 1000         | 0          | 0         |
		| Feb/Current Academic Year | 1000         | 0          | 0         |
		| Mar/Current Academic Year | 1000         | 3000       | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 0            | 0          | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |

    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R09/Last Academic Year    | Apr/Last Academic Year    | 900                    | 100                         | Learning         |
        | R10/Last Academic Year    | May/Last Academic Year    | 900                    | 100                         | Learning         |
        | R11/Last Academic Year    | Jun/Last Academic Year    | 900                    | 100                         | Learning         |
        | R12/Last Academic Year    | Jul/Last Academic Year    | 900                    | 100                         | Learning         |
        | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
        | R01/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 2700                   | 300                         | Completion       |
    But aims details are changed as follows
		| Aim Type  | Priority | Start Date                   | Planned Duration | Actual Duration | Aim Sequence Number | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | Completion Status | Contract Type | Aim Reference |
		| Programme | 1        | 01/Apr/Last Academic Year    | 12 months        | 12 months       | 1                   | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | completed         | Act2          | ZPROG001      |
		| Programme | 1        | 01/Apr/Current Academic Year | 12 months        |                 | 2                   | 593            | 2            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | continuing        | Act2          | ZPROG001      |
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| pe-1             | 15000                | 01/Apr/Last Academic Year           |                        |                                       | 0                       |                                        | 0                         |                                          | 90%                         | Act2          | 1                   |
		| pe-2             | 15000                | 01/Apr/Current Academic Year        |                        |                                       | 0                       |                                        | 0                         |                                          | 95%                         | Act2          | 2                   |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Aim Sequence Number | Price Episode Identifier |
		#pe-1
	 	| Aug/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
	 	| Sep/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
	 	| Oct/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
	 	| Nov/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
	 	| Dec/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
	 	| Jan/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
	 	| Feb/Current Academic Year | 1000         | 0          | 0         | 1                   | pe-1                     |
	 	| Mar/Current Academic Year | 1000         | 3000       | 0         | 1                   | pe-1                     |
	 	| Apr/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
	 	| May/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
	 	| Jun/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
	 	| Jul/Current Academic Year | 0            | 0          | 0         | 1                   | pe-1                     |
		#pe-2
		| Aug/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Apr/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
		| May/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
		| Jun/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
		| Jul/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
    And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport |
		| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         | 0               |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         | 0               |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type |
		| R09/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | 0                         | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | 0                         | Learning         |  
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type |
		| R09/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | 0                         | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | 0                         | Learning         |
	Examples:
        | Collection_Period         |
        | R09/Current Academic Year |
        | R10/Current Academic Year |