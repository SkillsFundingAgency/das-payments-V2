#@ignore
#Feature: 5% Contribution from April 2019
#
#Scenario: Non Levy Learner, started learning before Apr19, changes Pathway code in Apr19, remains on 10% contribution
#
#Background: The example is demonstrating a learner flagged as 'Non Levy' ACT2 starts learning pre Apr 2019 changes pathway code Apr19
#			and remains at 10% contribution as the learner is existing learner from Apr19
#	
#    Given The learner is programme only Non Levy 
#	And the apprenticeship funding band maximum is 15000
#	
#	When an ILR file is submitted with the following data:
#        | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | framework code | programme type | pathway code |
#        | learner a | 03/01/2019 | 01/01/2020       | 31/03/2019      | withdrawn         | 12000                | 03/01/2019                          | 3000                   | 03/01/2019                            | 403            | 2              | 1            |
#        | learner a | 01/04/2019 | 01/04/2020       |                 | continuing        | 12000                | 01/04/2019                          | 3000                   | 01/04/2019                            | 403            | 2              | 2            |
#       		
#	Then the provider earnings and payments break down as follows:
#		
#        | Type                       		| 01/19 | 02/19 | 03/19 | 04/19 | ... | 12/19 | 01/20 |
#        | Provider Earned Total      		| 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#        | Provider Earned from SFA   		| 900   | 900   | 900   | 900   | ... | 900   | 0     |
#        | Provider Earned from Employer 	| 100   | 100   | 100   | 100   | ... | 100   | 0     |            
#		| Provider Paid by SFA       		| 0     | 900   | 900   | 900   | ... | 900   | 900   |
#        | Payment due from Employer         | 0     | 100   | 100   | 100   | ... | 100   | 100   | 
#		| Levy account debited       		| 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | SFA Levy employer budget   		| 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | SFA Levy co-funding budget 		| 0     | 0     | 0     | 0     | ... | 0     | 0     |
#		| SFA non-Levy co-funding budget	| 900   | 900   | 900   | 900   | ... | 900   | 0     | 
#
#	And the transaction types for the payments are:
#        | Payment type                 | 02/19 | 03/19 | 04/19 | ... | 12/19 | 01/20 |
#        | On-program                   | 900   | 900   | 900   | ... | 900   | 900   |
#        | Completion                   | 0     | 0     | 0     | ... | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | ... | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | ... | 0     | 0     |		

Feature: Non-Levy Learner changes pathway code remains on 10 percent contribution - PV2-900
		As a provider,
		I want a Non Levy learner, starting prior to Apr 2019, where learner changes from Pathway code in Apr 2019 but stays on same price, remains at 10% contribution
		So that I am paid the correct apprenticeship funding by SFA	- PV2-900
Scenario Outline: Non-Levy Learner previously on 10 percent contribution but moves to 5 percent later - PV2-900
	Given the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 03/Jan/Current Academic Year | 12 months        | 15000                | 03/Jan/Current Academic Year        | 0                      |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
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
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 03/Jan/Current Academic Year | 12 months        | 15000                | 03/Jan/Current Academic Year        | 0                      |                                       | 3 months        | withdrawn         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
		| 01/Apr/Current Academic Year | 12 months        | 15000                | 01/Apr/Current Academic Year        | 0                      |                                       |                 | continuing        | Act2          | 2                   | ZPROG001      | 593            | 2            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
	And price details as follows
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 03/Jan/Current Academic Year        |                        |                                       | 0                       |                                        |                           |                                          | Act2          | 1                   | 90%                         |
        | pe-2             | 15000                | 01/Apr/Current Academic Year        |                        |                                       | 0                       |                                        |                           |                                          | Act2          | 2                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Aim Sequence Number | Price Episode Identifier |
		| Aug/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | 2                   | pe-2                     |
		| Jan/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-1                     |
		| Feb/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-1                     |
		| Mar/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-1                     |
		| Apr/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
		| May/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
		| Jun/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
		| Jul/Current Academic Year | 1000         | 0          | 0         | 2                   | pe-2                     |
    And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | LearningSupport |
		| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         | 0               |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         | 0               |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type |
		| R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | 0                         | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | 0                         | Learning         |
	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type |
		| R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | 0                         | Learning         |
		| R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | 0                         | Learning         |
	Examples:
        | Collection_Period         |
        | R09/Current Academic Year |
        | R10/Current Academic Year |