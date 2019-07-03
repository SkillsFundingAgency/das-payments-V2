@ignore
# Issue with change from ACT2 to ACT1 non-matching earning events
#Feature: 5% Contribution from April 2019
#
#Scenario: Non Levy Learner, starts new learning April 2019, 5% contribution, changes to ACT1 in the following month, learner is co-funded as not enough levy in the account 
#
#Background: The example is demonstrating an employer flagged at 'Non Levy' ACT2 changes to ACT1, but there are not sufficient levy funds in 
#			their account and the 5% contribution does not revert to 10%
#	
#    Given The learner is programme only Non Levy 
#	And the apprenticeship funding band maximum is 15000
#
#	And the following commitment exist from R10 (May 2019):
#        | ULN       | priority | start date | end date   | agreed price |
#        | learner a | 1        | 01/05/2019 | 01/04/2020 | 15000        |
#
#	When an ILR file is submitted R10 (May 2019) with the following data:
#          
#        | ULN       | learner type           | start date | planned end date | actual end date | Agreed Price  | completion status | aim type      | aim sequence number | framework code | programme type | pathway code |
#		| learner a | programme only non-DAS | 01/04/2019 | 01/04/2020       | 30/04/2019      | 15,000		   | continuing        | programme     | 1                   | 403            | 2              | 1            |
#		| learner a | programme only DAS     | 01/05/2019 | 01/04/2020       |                 | 15,000		   | continuing        | programme     | 1                   | 403            | 2              | 1            |
#
#
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 04/19 | 05/19 | 06/19 | 07/19 | 08/19 |
#        | Provider Earned Total                   | 1000  | 1000  | 1000  | 1000  | 1000  |
#        | Provider Earned from SFA                | 950   | 950   | 950   | 950   | 950   |
#        | Provider Earned from Employer           | 50    | 50    | 50    | 50    | 50    |
#        | Provider Paid by SFA                    | 0     | 950   | 950   | 950   | 950   |
#        | Payment due from Employer               | 0     | 50    | 50    | 50    | 50    |
#        | Levy account debited                    | 0     | 0     | 0     | 0     | 0     |
#        | SFA Levy employer budget                | 0     | 0     | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 0     | 950   | 950   | 950   | 950   |
#		| SFA non-Levy co-funding budget          | 950   | 0     | 0     | 0     | 0     |
#        | SFA non-Levy additional payments budget | 0     | 0     | 0     | 0     | 0     |
#
#    And the transaction types for the payments are:
#        | Payment type                 | 05/19 | 06/19 | 07/19 | 08/19 |
#        | On-program                   | 950   | 950   | 950   | 950   |
#        | Completion                   | 0     | 0     | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     |
 
 Feature: Five percent Contribution from April 2019 PV2-874
 As a provider,
 I want a Non Levy learner, starting in April 2019, at 5% contribution, and learner changes to (ACT1) Levy with co-funding in the following month
 So that I am paid the correct apprenticeship funding by SFA

 Scenario: Non Levy Learner, starts new learning April 2019, five percent contribution, changes to ACT1 in the following month, learner is co-funded as not enough levy in the account PV2-874

	Given the employer levy account balance in collection period R10/Current Academic Year is 0
		
	And the following apprenticeships exist
		| Apprenticeship | Provider   | Learner ID | framework code | programme type | pathway code | agreed price | start date                   | end date                  | status | effective from               |
		| Apprentice a   | Provider a | learner a  | 593            | 20             | 1            | 15000        | 01/May/Current Academic Year | 01/May/Next Academic Year | active | 01/May/Current Academic Year |	
    
	And the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Apr/Current Academic Year | 12 months        | 15000                | 01/Apr/Current Academic Year        |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19-24 Apprenticeship (From May 2017) Levy Contract | 95%                         |

	And the following earnings had been generated for the learner
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 0            | 0          | 0         |
		| Sep/Current Academic Year | 0            | 0          | 0         |
		| Oct/Current Academic Year | 0            | 0          | 0         |
		| Nov/Current Academic Year | 0            | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 0            | 0          | 0         |
		| Feb/Current Academic Year | 0            | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 1000         | 0          | 0         |
		| Jun/Current Academic Year | 1000         | 0          | 0         |
		| Jul/Current Academic Year | 1000         | 0          | 0         |

	And the following provider payments had been generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Contract Type |
		| R09/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | Learning         | Act2          |
       
	But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage | Contract Type |
		| 01/Apr/Current Academic Year | 12 months        | 15000                | 01/Apr/Current Academic Year        |                        |                                       | 1 month         | continuing        | 1                   | ZPROG001      | 593            | 1            | 20             | 19-24 Apprenticeship (From May 2017) Levy Contract | 95%                         | Act1          |
	#New step for changing Contract type 
	#And the Contract Type details as follows 
	#	| Contract Type | Effective From               | Effective To                |
	#	| Act2          | 01/Apr/Current Academic Year | 30/04/Current Academic Year |
	#	| Act1          | 01/May/Current Academic Year |                             |

	And price details as follows
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
        | pe-1             | 15000                | 01/Apr/Current Academic Year        |                        |                                       | 0                       |                                        |                           |                                          | 90%                         | Act2          |
        | pe-2             | 15000                | 01/May/Current Academic Year        |                        |                                       | 0                       |                                        |                           |                                          | 95%                         | Act1          |

	When the amended ILR file is re-submitted for the learners in collection period R10/Current Academic Year

	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | -1000        | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
		| Jun/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
		| Jul/Current Academic Year | 1000         | 0          | 0         | pe-2                     |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Contract Type | Price Episode Identifier |
        | R10/Current Academic Year | Apr/Current Academic Year | -1000        | 0          | 0         | Act2          | pe-1                     |
        | R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         | Act1          | pe-2                     |

	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Contract Type | Price Episode Identifier |
        | R10/Current Academic Year | Apr/Current Academic Year | -950                   | -50                         | Learning         | Act2          | pe-1                     |
        | R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | Learning         | Act1          | pe-2                     |

	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Contract Type |
        | R10/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | Learning         | Act2          |
        | R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | Learning         | Act1          |
