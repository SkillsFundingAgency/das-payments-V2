@ignore
#Feature: 5% Contribution from April 2019
#
#Scenario: Non Levy Learner, starts learning in Apr18 and completes in Apr19 with 10% contribution including completion payment
#
#Background: The example is demonstrating an employer flagged as 'Non Levy' ACT2 starts learning pre April 2019 and completes in Apr19
#			and completion payment remains at 10% as the learning is not new learning from Apr19
#	
#    Given The learner is programme only Non Levy 
#	And the apprenticeship funding band maximum is 15000
#     
#    
#	When an ILR file is submitted with the following data:
#          
#        | ULN       | learner type           | start date | planned end date | actual end date | Agreed Price  | completion status | aim type      | aim sequence number | framework code | programme type | pathway code |
#		| learner a | programme only DAS     | 04/04/2018 | 06/04/2019       |                 | 15,000		   | completed         | programme     | 1                   | 403            | 2              | 1            |
#
#
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 04/18 | 05/18 | 06/18 | ... | 03/19 | 04/19 | 05/19 |
#        | Provider Earned Total                   | 1000  | 1000  | 1000  | ... | 1000  | 3000  | 0     |
#        | Provider Earned from SFA                | 900   | 900   | 900   | ... | 900   | 2700  | 0     |
#        | Provider Earned from Employer           | 100   | 100   | 100   | ... | 100   | 300   | 0     |
#        | Provider Paid by SFA                    | 0     | 900   | 900   | ... | 900   | 900   | 2700  |
#        | Payment due from Employer               | 0     | 100   | 100   | ... | 100   | 100   | 300   |
#        | Levy account debited                    | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy employer budget                | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#		| SFA non-Levy co-funding budget          | 900   | 900   | 900   | ... | 900   | 2700  | 0     |
#        | SFA non-Levy additional payments budget | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#
#    And the transaction types for the payments are:
#        | Payment type                 | 05/18 | 06/18 | ... | 03/19 | 04/19 | 05/19 |
#        | On-program                   | 900   | 900   | ... | 900   | 900   | 2700  |
#        | Completion                   | 0     | 0     | ... | 0     | 0     | 0     |
#        | Balancing                    | 0     | 0     | ... | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | ... | 0     | 0     | 0     |

Feature: 5% Contribution from April 2019 PV2-894

Scenario Outline: Non Levy Learner, starts learning in Apr18 and completes in Apr19 with 10% contribution including completion payment

	Given the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                      | SFA Contribution Percentage |
		| 06/Apr/Last Academic Year | 12 months        | 15000                | 06/Apr/Last Academic Year           |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract | 90%                         |
	
	And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year | 0            | 0          | 0         |
        | Sep/Last Academic Year | 0            | 0          | 0         |
        | Oct/Last Academic Year | 0            | 0          | 0         |
        | Nov/Last Academic Year | 0            | 0          | 0         |
        | Dec/Last Academic Year | 0            | 0          | 0         |
        | Jan/Last Academic Year | 0            | 0          | 0         |
        | Feb/Last Academic Year | 0            | 0          | 0         |
        | Mar/Last Academic Year | 0            | 0          | 0         |
        | Apr/Last Academic Year | 1000         | 0          | 0         |
        | May/Last Academic Year | 1000         | 0          | 0         |
        | Jun/Last Academic Year | 1000         | 0          | 0         |
        | Jul/Last Academic Year | 1000         | 0          | 0         |
    
	And the following provider payments had been generated 
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
        | R09/Last Academic Year | Apr/Last Academic Year | 900                    | 100                         | Learning         |
        | R10/Last Academic Year | May/Last Academic Year | 900                    | 100                         | Learning         |
        | R11/Last Academic Year | Jun/Last Academic Year | 900                    | 100                         | Learning         |
        | R12/Last Academic Year | Jul/Last Academic Year | 900                    | 100                         | Learning         |
    
	But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                      | SFA Contribution Percentage | Employer Contribution |
		| 06/Apr/Last Academic Year | 12 months        | 15000                | 06/Apr/Last Academic Year           | 0                      |                                       | 12 months       | completed         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract | 90%                         | 1500                  |
	
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
		| Apr/Current Academic Year | 0            | 3000       | 0         |
		#| May/Current Academic Year | 0            | 0          | 0         |
		#| Jun/Current Academic Year | 0            | 0          | 0         |
		#| Jul/Current Academic Year | 0            | 0          | 0         |
    
	And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R01/Current Academic Year | Aug/Current Academic Year | 1000         | 0          | 0         |
        | R02/Current Academic Year | Sep/Current Academic Year | 1000         | 0          | 0         |
        | R03/Current Academic Year | Oct/Current Academic Year | 1000         | 0          | 0         |
        | R04/Current Academic Year | Nov/Current Academic Year | 1000         | 0          | 0         |
        | R05/Current Academic Year | Dec/Current Academic Year | 1000         | 0          | 0         |
        | R06/Current Academic Year | Jan/Current Academic Year | 1000         | 0          | 0         |
        | R07/Current Academic Year | Feb/Current Academic Year | 1000         | 0          | 0         |
        | R08/Current Academic Year | Mar/Current Academic Year | 1000         | 0          | 0         |
        | R09/Current Academic Year | Apr/Current Academic Year | 0            | 3000       | 0         |

	
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
		| R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         |
		| R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         |
		| R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         |
		| R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | Learning         |
		| R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
		| R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
		| R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         |
		| R09/Current Academic Year | Apr/Current Academic Year | 2700                   | 300                         | Completion       |
		
		
	
	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 900                    | 100                         | Learning         |
		| R02/Current Academic Year | Sep/Current Academic Year | 900                    | 100                         | Learning         |
		| R03/Current Academic Year | Oct/Current Academic Year | 900                    | 100                         | Learning         |
		| R04/Current Academic Year | Nov/Current Academic Year | 900                    | 100                         | Learning         |
		| R05/Current Academic Year | Dec/Current Academic Year | 900                    | 100                         | Learning         |
		| R06/Current Academic Year | Jan/Current Academic Year | 900                    | 100                         | Learning         |
		| R07/Current Academic Year | Feb/Current Academic Year | 900                    | 100                         | Learning         |
		| R08/Current Academic Year | Mar/Current Academic Year | 900                    | 100                         | Learning         |
		| R09/Current Academic Year | Apr/Current Academic Year | 2700                   | 300                         | Completion       |
	
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
	
	