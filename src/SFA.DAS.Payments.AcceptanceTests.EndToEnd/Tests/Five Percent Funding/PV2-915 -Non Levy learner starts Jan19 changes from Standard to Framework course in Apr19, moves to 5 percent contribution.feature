@ignore
#Feature: 5% Contribution from April 2019
#
#Scenario: Non Levy Learner, started learning before Apr19, changes from Standard to Fraemwork course in Apr19, moves to 5% contribution
#
#Background: The example is demonstrating a learner flagged as 'Non Levy' ACT2 starts learning Jan 2019 changes course Apr19
#			and moves to 5% contribution as the learner is existing learner but starting a new course from Apr19
#	
#    Given The learner is programme only Non Levy 
#	And the apprenticeship funding band maximum is 15000
#	
#	When an ILR file is submitted with the following data:
#        | ULN       | framework code | standard code | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#        | learner a |                | 52            | 03/01/2019 | 01/01/2020       | 31/03/2019      | withdrawn         | 12000                | 03/01/2019                          | 3000                   | 03/01/2019                            |
#        | learner a | 403            |               | 01/04/2019 | 01/04/2020       |                 | continuing        | 12000                | 01/04/2019                          | 3000                   | 01/04/2019                            |
#       		
#	Then the provider earnings and payments break down as follows:
#		
#        | Type                       		| 01/19 | 02/19 | 03/19 | 04/19 | 05/19 | ... | 03/20 | 04/20 |
#        | Provider Earned Total      		| 1000  | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#        | Provider Earned from SFA   		| 900   | 900   | 900   | 950   | 950   | ... | 950   | 0     |
#        | Provider Earned from Employer 	| 100   | 100   | 100   | 50    | 50    | ... | 50    | 0     |            
#		| Provider Paid by SFA       		| 0     | 900   | 900   | 900   | 950   | ... | 950   | 950   |
#        | Payment due from Employer         | 0     | 100   | 100   | 100   | 50    | ... | 50    | 50    | 
#		| Levy account debited       		| 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | SFA Levy employer budget   		| 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | SFA Levy co-funding budget 		| 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#		| SFA non-Levy co-funding budget	| 900   | 900   | 900   | 950   | 950   | ... | 950   | 0     | 
#
#	And the transaction types for the payments are:
#        | Payment type                 | 02/19 | 03/19 | 04/19 | 05/19 | ... | 03/20 | 04/20 |
#        | On-program                   | 900   | 900   | 900   | 950   | ... | 950   | 950   |
#        | Completion                   | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     | ... | 0     | 0     |		

Feature: 5% Contribution from April 2019 PV2-915

Scenario Outline: Non Levy Learner, started learning before Apr19, changes from Standard to Fraemwork course in Apr19, moves to 5% contribution

Given the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 01/Jan/Current Academic Year | 12 months        | 15000                | 01/Jan/Current Academic Year        |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 0            | 0          | 0         |
        | Sep/Current Academic Year | 0            | 0          | 0         |
        | Oct/Current Academic Year | 0            | 0          | 0         |
        | Nov/Current Academic Year | 0            | 0          | 0         |
        | Dec/Current Academic Year | 0            | 0          | 0         |
        | Jan/Current Academic Year | 1000         | 0          | 0         |
        | Feb/Current Academic Year | 1000         | 0          | 0         |
        | Mar/Current Academic Year | 1000         | 0          | 0         |
        | Apr/Current Academic Year | 1000         | 0          | 0         |
        | May/Current Academic Year | 1000         | 0          | 0         |
        | Jun/Current Academic Year | 1000         | 0          | 0         |
        | Jul/Current Academic Year | 1000         | 0          | 0         |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         |
    But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 01/Jan/Current Academic Year | 12 months        | 15000                | 01/Jan/Current Academic Year        |                        |                                       | 3 months        | withdrawn         | Act2          | 1                   | ZPROG001      |               | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
		| 01/Apr/Current Academic Year | 12 months        | 12000                | 01/Apr/Current Academic Year        | 3000                   | 01/Apr/Current Academic Year          |                 | continuing        | Act2          | 2                   | ZPROG001      | 17            |                |              | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 95%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 0            | 0          | 0         |
		| Sep/Current Academic Year | 0            | 0          | 0         |
		| Oct/Current Academic Year | 0            | 0          | 0         |
		| Nov/Current Academic Year | 0            | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 1000         | 0          | 0         |
		| Feb/Current Academic Year | 1000         | 0          | 0         |
		| Mar/Current Academic Year | 1000         | 0          | 0         |
		| Apr/Current Academic Year | 1000         | 0          | 0         |
		| May/Current Academic Year | 1000         | 0          | 0         |
		| Jun/Current Academic Year | 1000         | 0          | 0         |
		| Jul/Current Academic Year | 1000         | 0          | 0         |
    And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |

	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R09/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | Learning         |
		| R09/Current Academic Year | Jun/Current Academic Year | 950                    | 50                          | Learning         |
		| R10/Current Academic Year | Jul/Current Academic Year | 950                    | 50                          | Learning         |

	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R09/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | Learning         |
		| R09/Current Academic Year | Jun/Current Academic Year | 950                    | 50                          | Learning         |
		| R10/Current Academic Year | Jul/Current Academic Year | 950                    | 50                          | Learning         |
	Examples:
        | Collection_Period         |
        | R09/Current Academic Year |
        | R10/Current Academic Year |
		| R11/Current Academic Year |
        | R12/Current Academic Year |