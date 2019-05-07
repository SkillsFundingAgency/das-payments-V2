@ignore
#Feature: 5% Contribution from April 2019
#
#Scenario: Levy Learner, starts new learning April 2019, 5% contribution, learner is co-funded as not enough levy in the account
#    
#    Given The learner is programme only Levy with co-funding
#	And the apprenticeship funding band maximum is 15000
#    
#	And the following commitment exist:
#        | ULN       | priority | start date | end date   | agreed price |
#        | learner a | 1        | 01/04/2019 | 01/04/2020 | 15000        |
#    
#	When an ILR file is submitted with the following data:
#		| ULN    | learner type         | agreed price | start date  | planned end date | completion status | framework code | programme type | pathway code |
#		| 123456 | programme only DAS   | 15000        | 06/04/2019  | 09/04/2020       | continuing        | 403            | 2              | 1            |
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
#        | SFA Levy co-funding budget              | 950   | 950   | 950   | 950   | 950   |
#		| SFA non-Levy co-funding budget          | 0     | 0     | 0     | 0     | 0     |
#        | SFA non-Levy additional payments budget | 0     | 0     | 0     | 0     | 0     |
#
#    And the transaction types for the payments are:
#        | Payment type                 | 05/19 | 06/19 | 07/19 | 08/19 |
#        | On-program                   | 950   | 950   | 950   | 950   |
#        | Completion                   | 0     | 0     | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     |



Feature: 5% Contribution from April 2019 PV2-873
As a provider,
I want a Levy learner, starting in April 2019, where learner is co-funded at 5% contribution, as not enough levy in the account
So that I am paid the correct apprenticeship funding by SFA

Scenario: Levy Learner, starts new learning April 2019, 5% contribution, learner is co-funded as not enough levy in the account PV2-873

	Given the employer levy account balance in collection period R10/Current Academic Year is 0
		
	And the following apprenticeships exist
		| framework code | programme type | pathway code | agreed price | start date                   | end date                  | status | effective from               |
		| 593            | 20             | 1            | 15000        | 01/Apr/Current Academic Year | 01/Apr/Next Academic Year | active | 01/Apr/Current Academic Year |	
    
	And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                  | SFA Contribution Percentage |
		| 01/Apr/Current Academic Year | 12 months        | 15000                | 01/Apr/Current Academic Year        |                        |                                       |                 | continuing        | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship Levy Contract | 95%                         |
	#New step for changing Contract type 
	And the Contract Type details are as follows 
		| Contract Type | Effective From               | Effective To                 |
		| Act1          | 01/Apr/Current Academic Year | 30/Apr/Current Academic Year |
		| Act2          | 01/May/Current Academic Year |                              |

	When the ILR file is submitted for the learners for collection period R10/Current Academic Year

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
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Contract Type |
        | R10/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         | Act1          |
        | R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         | Act2          |

	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Contract Type | Transaction Type |
        | R10/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | Act1          | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | Act2          | Learning         |
        
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Contract Type | Transaction Type |
        | R10/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | Act1          | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | Act2          | Learning         |