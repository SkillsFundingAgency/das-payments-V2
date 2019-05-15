@ignore
#Feature: 5% Contribution from April 2019
#
#Scenario: Levy Learner, started learning before Apr19 on co-funding, changes Provider and changes to ACT2 from April 2019, remains on 10% contribution
#
#Background: The example is demonstrating a learner flagged as 'Levy' starts learning pre Apr 2019 with co-funding, changes Provider and changes to ACT2 in Apr19
#			and remains on 10% contribution from Apr19 as its an existing learner on existing course
#	
#    Given The learner is programme only Levy with co-funding 
#	And the apprenticeship funding band maximum is 15000
#	
#    And the Contract type in the ILR is:
#        | contract type | date from  | date to    |
#        | DAS           | 03/01/2019 | 02/04/2019 |
#        | Non-DAS       | 03/04/2019 | 04/01/2020 |
#	
#    And the following commitments exist:
#        | contract type | commitment Id | version Id | Provider   | ULN       | start date | end date   | agreed price | status    | effective from | effective to | stop effective from |
#        | DAS           | 1             | 1          | provider a | learner a | 03/01/2019 | 02/01/2020 | 15000        | cancelled | 03/01/2019     | 01/04/2019   | 02/04/2019   		 |
#        | Non-DAS       | 2             | 1          | provider b | learner a | 03/04/2019 | 02/01/2020 | 11250        | active    | 03/04/2019     |              |              		 |
#      
#    When the providers submit the following ILR files:
#        | Provider   | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date |
#        | provider a | learner a | 03/01/2019 | 02/01/2020 		 | 01/04/2019      | withdrawn         | 12000                | 03/01/2019                          | 3000                   | 03/01/2019                            |
#        | provider b | learner a | 03/04/2019 | 02/01/2020 		 |                 | continuing        | 9000                 | 03/04/2019                          | 2250                   | 03/04/2019                            |
#        
# 		
#	Then the provider earnings and payments break down as follows:		
#        | Type                       		| 01/19 | 02/19 | 03/19 | 04/19 | 05/19 | 06/19 | ... | 12/19 | 01/20 |
#        | Provider Earned Total      		| 1000  | 1000  | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#        | Provider Earned from SFA   		| 900   | 900   | 900   | 900   | 900   | 900   | ... | 900   | 0     |
#        | Provider Earned from Employer 	| 100   | 100   | 100   | 100   | 100   | 100   | ... | 100   | 0     |            
#		| Provider Paid by SFA       		| 0     | 900   | 900   | 900   | 900   | 900   | ... | 900   | 900   |
#        | Payment due from Employer         | 0     | 100   | 100   | 100   | 100   | 100   | ... | 100   | 100   | 
#		| Levy account debited       		| 0     | 0     | 0     | 0     | 0     | 0     |  ...| 0     | 0     |
#        | SFA Levy employer budget   		| 0     | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | SFA Levy co-funding budget 		| 900   | 900   | 900   | 0     | 0     | 0     | ... | 0     | 0     |
#		| SFA non-Levy co-funding budget	| 0     | 0     | 0     | 900   | 900   | 900   | ... | 900   | 0     | 
#
#	And the transaction types for the payments are:
#        | Payment type                 | 02/19 | 03/19 | 04/19 | 05/19 | 06/19 | ... | 12/19 | 01/20 |
#        | On-program                   | 900   | 900   | 900   | 900   | 900   | ... | 900   | 900   |
#        | Completion                   | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |

Feature: 5% Contribution from April 2019  PV2-908
As a provider,
I want a Levy learner on co-funding, starting prior to Apr 2019, where learner changes Provider and changes to Non Levy in Apr 2019, and remains on 10% contribution
So that I am paid the correct apprenticeship funding by SFA

Scenario Outline: Levy Learner, started learning before Apr19 on co-funding, changes Provider and changes to ACT2 from April 2019, remains on 10% contribution PV2-908

	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
		
	And the following apprenticeships exist
		| Standard code | programme type | agreed price | start date                   | end date                  | status | effective from               |
		| 17            | 25             | 15000        | 01/Jan/Current Academic Year | 01/Jan/Next Academic Year | active | 01/Jan/Current Academic Year |

	And the "provider a" previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Jan/Current Academic Year | 12 months        | 12000                | 01/Jan/Current Academic Year        | 3000                   | 01/Jan/Current Academic Year          | 3 months        | withdrawn         | Act1          | 1                   | ZPROG001      | 17            | 25             | 19-24 Apprenticeship (From May 2017) Levy Contract | 90%                         |

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
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 0            | 0          | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |

	And the following payments had been generated for "provider a"
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
		| R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
		| R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         |
	
	But the Learner has now changed to "provider b" as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                      | SFA Contribution Percentage |
		| 01/Apr/Current Academic Year | 9 months         | 9000                 | 01/Apr/Current Academic Year        | 3000                   | 01/Apr/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 17            | 25             | 19-24 Apprenticeship (From May 2017) Non-Levy Contract | 90%                         |


	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 0            | 0          | 0         |
		| Sep/Current Academic Year | 0            | 0          | 0         |
		| Oct/Current Academic Year | 0            | 0          | 0         |
		| Nov/Current Academic Year | 0            | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 0            | 0          | 0         |
		| Feb/Current Academic Year | 0            | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 1000         | 0          | 0         |
		| May/Current Academic Year | 1000         | 0          | 0         |
		| Jun/Current Academic Year | 1000         | 0          | 0         |
		| Jul/Current Academic Year | 1000         | 0          | 0         |

	And at month end only the following payments will be calculated
		| Provider   | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| provider b | R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
		| provider b | R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
		| provider b | R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
		| provider b | R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
	 
	And only the following provider payments will be recorded
		| Provider   | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| provider b | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
		| provider b | R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
		| provider b | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
		| provider b | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |

	And only the following provider payments will be generated
		| Provider   | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| provider b | R09/Current Academic Year | Apr/Current Academic Year | 900                    | 100                         | Learning         |
		| provider b | R10/Current Academic Year | May/Current Academic Year | 900                    | 100                         | Learning         |
		| provider b | R11/Current Academic Year | Jun/Current Academic Year | 900                    | 100                         | Learning         |
		| provider b | R12/Current Academic Year | Jul/Current Academic Year | 900                    | 100                         | Learning         |
    
	 Examples: 
			| Collection_Period         | Levy Balance |
			| R09/Current Academic Year | 0            |
			| R10/Current Academic Year | 0            |
			| R11/Current Academic Year | 0            | 
			| R12/Current Academic Year | 0            |