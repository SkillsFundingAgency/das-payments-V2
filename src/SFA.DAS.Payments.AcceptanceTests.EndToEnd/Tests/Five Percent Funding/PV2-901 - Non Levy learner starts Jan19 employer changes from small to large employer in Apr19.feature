@ignore
#Feature: 5% Contribution from April 2019
#
#Scenario: Non Levy Learner, started learning before Apr19, Employer changes from Small Employer to Large Employer in Apr19, remains on 10% contribution
#
#Background: The example is demonstrating a learner flagged as 'Non Levy' ACT2 starts learning pre Apr 2019 employer changes from Small to Large Employer Apr19
#			and moves from fully funded to 10% contribution as the learner is existing learner from Apr19
#	
#    Given The learner is programme only Non Levy 
#	And the apprenticeship funding band maximum is 15000
#	
#	When an ILR file is submitted with the following data:
#        | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | framework code | programme type | pathway code |
#        | learner a | 03/01/2019 | 01/01/2020       | 			      | continuing        | 12000                | 03/01/2019                          | 3000                   | 03/01/2019                            | 403            | 2              | 1            |
#
# 
#    And the employment status in the ILR is:
#		| Employer    | Employment Status  | Employment Status Applies | Small Employer |
#		| employer 1  | in paid employment | 03/01/2019                | SEM1           |
#		| employer 2  | in paid employment | 01/04/2019                |                |
# 
#	Then the provider earnings and payments break down as follows:
#		
#        | Type                       		| 01/19 | 02/19 | 03/19 | 04/19 | 05/19 | ... | 12/19 | 01/20 |
#        | Provider Earned Total      		| 1000  | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#        | Provider Earned from SFA   		| 1000  | 1000  | 1000  | 900   | 900   | ... | 900   | 0     |
#        | Provider Earned from Employer 	| 0     | 0     | 0     | 100   | 100   | ... | 100   | 0     |            
#		| Provider Paid by SFA       		| 0     | 1000  | 1000  | 1000  | 900   | ... | 900   | 900   |
#        | Payment due from Employer         | 0     | 0     | 0     | 0     | 100   | ... | 100   | 100   | 
#		| Levy account debited       		| 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | SFA Levy employer budget   		| 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | SFA Levy co-funding budget 		| 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#		| SFA non-Levy co-funding budget	| 1000  | 1000  | 1000  | 900   | 900   | ... | 900   | 0     | 
#
#	And the transaction types for the payments are:
#        | Payment type                 | 02/19 | 03/19 | 04/19 | 05/19 | ... | 12/19 | 01/20 |
#        | On-program                   | 1000  | 1000  | 1000  | 900   | ... | 900   | 900   |
#        | Completion                   | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     | ... | 0     | 0     |

Feature: 5% Contribution from April 2019 - PV2-901

Scenario Outline: Non Levy Learner, started learning before Apr19, Employer changes from Small Employer to Large Employer in Apr19, remains on 10% contribution
 #	And the employment status in the ILR is
 #       | Employer   | Employment Status  | Employment Status Applies | Small Employer |
 #       | employer 1 | in paid employment | 01/Jan/Current Academic Year | SEM1           |
 #       | employer 2 | in paid employment | 01/Apr/Current Academic Year |                |

		Given the provider previously submitted the following learner details
			| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     |
			| 01/Jan/Current Academic Year | 12 months        | 15000                | 01/Jan/Current Academic Year        |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) |
		
		And the following earnings had been generated for the learner
			| Delivery Period           | On-Programme | Completion | Balancing | SFA Contribution Percentage |
			| Aug/Current Academic Year | 0            | 0          | 0         | 0                           |
			| Sep/Current Academic Year | 0            | 0          | 0         | 0                           |
			| Oct/Current Academic Year | 0            | 0          | 0         | 0                           |
			| Nov/Current Academic Year | 0            | 0          | 0         | 0                           |
			| Dec/Current Academic Year | 0            | 0          | 0         | 0                           |
			| Jan/Current Academic Year | 1000         | 0          | 0         | 100%                        |
			| Feb/Current Academic Year | 1000         | 0          | 0         | 100%                        |
			| Mar/Current Academic Year | 1000         | 0          | 0         | 100%                        |
			| Apr/Current Academic Year | 1000         | 0          | 0         | 100%                        |
			| May/Current Academic Year | 1000         | 0          | 0         | 100%                        |
			| Jun/Current Academic Year | 1000         | 0          | 0         | 100%                        |
			| Jul/Current Academic Year | 1000         | 0          | 0         | 100%                        |
		
		And the following provider payments had been generated
		    | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Transaction Type |
		    | R01/Current Academic Year | Jan/Current Academic Year | 1000                   | Learning         |
		    | R02/Current Academic Year | Feb/Current Academic Year | 1000                   | Learning         |
		    | R03/Current Academic Year | Mar/Current Academic Year | 1000                   | Learning         |

		But the Provider now changes the Learner details as follows
			| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
			| 01/Apr/Current Academic Year | 12 months        | 15000                | 01/Apr/Current Academic Year        |                        |                                       | 12 months       | completed         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
		When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
		Then the following learner earnings should be generated
			| Delivery Period           | On-Programme | Completion | Balancing | SFA Contribution Percentage |
			| Aug/Current Academic Year | 0            | 0          | 0         | 0                           |
			| Sep/Current Academic Year | 0            | 0          | 0         | 0                           |
			| Oct/Current Academic Year | 0            | 0          | 0         | 0                           |
			| Nov/Current Academic Year | 0            | 0          | 0         | 0                           |
			| Dec/Current Academic Year | 0            | 0          | 0         | 0                           |
			| Jan/Current Academic Year | 1000         | 0          | 0         | 100%                        |
			| Feb/Current Academic Year | 1000         | 0          | 0         | 100%                        |
			| Mar/Current Academic Year | 1000         | 0          | 0         | 100%                        |
			| Apr/Current Academic Year | 1000         | 0          | 0         | 90%                         |
			| May/Current Academic Year | 1000         | 0          | 0         | 90%                         |
			| Jun/Current Academic Year | 1000         | 0          | 0         | 90%                         |
			| Jul/Current Academic Year | 1000         | 0          | 0         | 90%                         |

		And at month end only the following payments will be calculated
			| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
			| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
			| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
			| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
			| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |

		And only the following provider payments will be recorded
			| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
			| R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
			| R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
			| R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
			| R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |

		Examples: 
			| Collection_Period         |
			| R09/Current Academic Year | 
			| R10/Current Academic Year |
			| R11/Current Academic Year |
			| R12/Current Academic Year |