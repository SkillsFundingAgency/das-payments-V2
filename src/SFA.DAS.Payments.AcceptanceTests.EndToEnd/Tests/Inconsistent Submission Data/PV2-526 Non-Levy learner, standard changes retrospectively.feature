@ignore
Feature: Provider earnings and payments where learner changes apprenticeship standard retrospectively and negotiated price remains the same, (remaining with the same employer and provider) PV2-526
		As a provider,
		I want a non-levy learner, changes apprenticeship standard retrospectively and the negotiated price remains the same, generates refund
		So that I am accurately paid my apprenticeship provision
	
Scenario Outline: Changes standard from the beginning and price remains same - PV2-526
	# the provider changes the Standard Type to 52 effective from retrospectively
	Given the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 1000         | 0          | 0         |
        | Sep/Current Academic Year | 1000         | 0          | 0         |
        | Oct/Current Academic Year | 1000         | 0          | 0         |
        | Nov/Current Academic Year | 1000         | 0          | 0         |
        | Dec/Current Academic Year | 1000         | 0          | 0         |
        | Jan/Current Academic Year | 1000         | 0          | 0         |
        | Feb/Current Academic Year | 1000         | 0          | 0         |
        | Mar/Current Academic Year | 1000         | 0          | 0         |
        | Apr/Current Academic Year | 1000         | 0          | 0         |
        | May/Current Academic Year | 1000         | 0          | 0         |
        | Jun/Current Academic Year | 1000         | 0          | 0         |
        | Jul/Current Academic Year | 1000         | 0          | 0         |
    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         |
    But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 12000                | 01/Aug/Current Academic Year        | 3000                   | 01/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 52            | 25             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
    Then the following learner earnings should be generated
        | Delivery Period           | On-Programme | Completion | Balancing |
        | Aug/Current Academic Year | 1000         | 0          | 0         |
        | Sep/Current Academic Year | 1000         | 0          | 0         |
        | Oct/Current Academic Year | 1000         | 0          | 0         |
        | Nov/Current Academic Year | 1000         | 0          | 0         |
        | Dec/Current Academic Year | 1000         | 0          | 0         |
        | Jan/Current Academic Year | 1000         | 0          | 0         |
        | Feb/Current Academic Year | 1000         | 0          | 0         |
        | Mar/Current Academic Year | 1000         | 0          | 0         |
        | Apr/Current Academic Year | 1000         | 0          | 0         |
        | May/Current Academic Year | 1000         | 0          | 0         |
        | Jun/Current Academic Year | 1000         | 0          | 0         |
        | Jul/Current Academic Year | 1000         | 0          | 0         |
    And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R03/Current Academic Year | Aug/Current Academic Year | -1000        | 0          | 0         |
		| R03/Current Academic Year | Sep/Current Academic Year | -1000        | 0          | 0         |
		| R03/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
		| R03/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
		| R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
		| R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
		| R05/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         |
		| R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |
		| R07/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         |
		| R08/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         |
		| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         |
    And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |Standard Code |
        | R03/Current Academic Year | Aug/Current Academic Year | -900                   | -100                        | Learning         | 51           |
        | R03/Current Academic Year | Sep/Current Academic Year | -900                   | -100                        | Learning         | 51           |
        | R03/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         | 52           |
        | R03/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         | 52           |
        | R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         | 52           |
        | R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         | 52           |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type | Standard Code |
		| R03/Current Academic Year | Aug/Current Academic Year | -900                   | -100                        | Learning         | 51            |
		| R03/Current Academic Year | Sep/Current Academic Year | -900                   | -100                        | Learning         | 51            |
		| R03/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R03/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         | 52            |
		| R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         | 52            |
	Examples:
		| Collection_Period         |
		| R03/Current Academic Year |
		| R04/Current Academic Year |




#Feature: Learner Change in Circumstance
#
#Scenario: Non-Levy learner change to standard in ILR with no change in price	
#
#        Given The learner is programme only non-DAS
#        And the apprenticeship funding band maximum is 17000
#		And the total price is 15000
# 
#		When an ILR file is submitted in R01 (Aug 2018) with the following data:
#            | ULN       | standard code | start date | planned end date | completion status | Total training price | Total training price effective date | Total assessment price |
#            | learner a | 51            | 01/08/2018 | 03/08/2019       | continuing        | 12000                | 01/08/2018                          | 3000                   |
#       		
#		Then the provider earnings and payments break down as follows:
#		
#            | Type                       		| 08/18 | 09/18 | 10/18 |
#            | Provider Earned Total      		| 1000  | 1000  | 1000  |
#            | Provider Earned from SFA   		| 900   | 900   | 900   |
#            | Provider Earned from Employer 	| 100   | 100   | 100   |            
#			| Provider Paid by SFA       		| 0     | 900   | 900   |
#			| Refund taken by SFA               | 0     | 0     | 0     | 
#            | Payment due from Employer        | 0     | 100   | 100   |
#			| Refund due to employer            | 0     | 0     | 0     |			
#			| Levy account debited       		| 0     | 0     | 0     |
#            | SFA Levy employer budget   		| 0     | 0     | 0     |
#            | SFA Levy co-funding budget 		| 0     | 0     | 0     |
#			| SFA non-Levy co-funding budget	| 900   | 900   | 900   | 
#			
#        And the transaction types for the payments are:
#            | Payment type                 | 09/18 | 10/18 | 
#            | On-program                   | 900   | 900   | 
#            | Completion                   | 0     | 0     | 
#            | Balancing                    | 0     | 0     | 
#            | Employer 16-18 incentive     | 0     | 0     | 
#            | Provider 16-18 incentive     | 0     | 0     | 
#				
#		
#		
#		When an ILR file is submitted in R03 (Oct 2018) with the following data:
#            | ULN       | standard code | start date | planned end date | completion status | Total training price | Total training price effective date | Total assessment price |
#            | learner a | 52            | 01/08/2018 | 03/08/2019       | continuing        | 12000                | 01/08/2018                          | 3000                   |
#       		
#		Then the provider earnings and payments break down as follows:
#		
#            | Type                       		| 08/18 | 09/18 | 10/18 | 11/18 | 12/18 |
#            | Provider Earned Total      		| 1000  | 1000  | 1000  | 1000  | 1000  |
#            | Provider Earned from SFA   		| 900   | 900   | 900   | 900   | 900   |
#            | Provider Earned from Employer 	| 100   | 100   | 100   | 100   | 100   |            
#			| Provider Paid by SFA       		| 0     | 0     | 0     | 2700  | 900   |
#			| Refund taken by SFA               | 0     | 0     | 0     | -1800 | 0     | 
#            | Payment due from Employer         | 0     | 100   | 100   | 100   | 100   | 
#			| Refund due to employer            | 0     | 0     | 0     | -200  | 0     | 
#			| Levy account debited       		| 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy employer budget   		| 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy co-funding budget 		| 0     | 0     | 0     | 0     | 0     |
#			| SFA non-Levy co-funding budget	| 900   | 900   | 900   | 2700  | 900   | 
#			
#        And the transaction types for the payments are:
#            | Payment type                 | 09/18 | 10/18 | 11/18 | 12/18 | 
#            | On-program                   | 900   | 900   | 900   | 900   | 
#            | Completion                   | 0     | 0     | 0     | 0     | 
#            | Balancing                    | 0     | 0     | 0     | 0     | 
#            | Employer 16-18 incentive     | 0     | 0     | 0     | 0     | 
#            | Provider 16-18 incentive     | 0     | 0     | 0     | 0     | 


