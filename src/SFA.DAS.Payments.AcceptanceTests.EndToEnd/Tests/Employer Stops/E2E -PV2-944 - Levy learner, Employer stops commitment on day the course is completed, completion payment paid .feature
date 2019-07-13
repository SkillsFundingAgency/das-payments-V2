#Feature: Employer Stops
#
#Scenario: Levy learner, Employer stops commitment on the day that the course is completed. Completion payment is paid
#
#	Given the learner is programme only Levy
#	And levy balance > agreed price for all months
#    
#	And the following commitments exist:
#        | commitment Id | version Id | ULN       | start date | end date   | status    | agreed price | effective from | effective to | stop effective from |
#        | 1             | 1          | learner a | 01/08/2018 | 01/09/2019 | cancelled | 15000        | 01/08/2018     | 01/09/2019   | 09/08/2019          |
#       
#	When an ILR file is submitted with the following data:
#        | ULN       | agreed price | learner type       | start date | planned end date | actual end date | completion status |
#        | learner a | 15000        | programme only DAS | 01/08/2018 | 09/08/2019       | 09/08/2019      | completed         |
#       
#	Then the provider earnings and payments break down as follows:
#        | Type                           | 08/18 | 09/18 | ... | 07/19 | 08/19 | 09/19 |
#        | Provider Earned Total          | 1000  | 1000  | ... | 1000  | 3000  | 0     |
#        | Provider Earned from SFA       | 1000  | 1000  | ... | 1000  | 3000  | 0     |
#        | Provider Earned from Employer  | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider Paid by SFA           | 0     | 1000  | ... | 1000  | 1000  | 3000  |
#        | Payment due from Employer      | 0     | 0     | ... | 0     | 0     | 0     |
#        | Levy account debited           | 0     | 1000  | ... | 1000  | 1000  | 3000  |
#        | SFA Levy employer budget       | 1000  | 1000  | ... | 1000  | 3000  | 0     |
#        | SFA Levy co-funding budget     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA non-Levy co-funding budget | 0     | 0     | ... | 0     | 0     | 0     |
#
#    And the transaction types for the payments are:
#        | Payment type                 | 09/18 | ... | 07/19 | 08/19 | 09/19 |
#        | On-program                   | 1000  | ... | 1000  | 1000  | 0     |
#        | Completion                   | 0     | ... | 0     | 0     | 3000  |
#        | Balancing                    | 0     | ... | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | ... | 0     | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | ... | 0     | 0     | 0     |
#        | Framework uplift on-program  | 0     | ... | 0     | 0     | 0     |
#        | Framework uplift completion  | 0     | ... | 0     | 0     | 0     |
#        | Framework uplift balancing   | 0     | ... | 0     | 0     | 0     |
#        | Provider disadvantage uplift | 0     | ... | 0     | 0     | 0     |


Feature: Employer Stops PV2-944

Scenario Outline: Levy learner, Employer stops commitment on the day that the course is completed. Completion payment is paid

Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	# New Commitment line

	And the following apprenticeships exist
		| framework code | programme type | pathway code | agreed price | start date                | end date                     | status  | effective from            | stop effective from          |
		| 593            | 20             | 1            | 15000        | 06/Aug/Last Academic Year | 01/Sep/Current Academic Year | stopped | 06/Aug/Last Academic Year | 06/Aug/Current Academic Year |		

	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 15000                | 06/Aug/Last Academic Year           |                        |                                       |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year | 1000         | 0          | 0         |
        | Sep/Last Academic Year | 1000         | 0          | 0         |
        | Oct/Last Academic Year | 1000         | 0          | 0         |
        | Nov/Last Academic Year | 1000         | 0          | 0         |
        | Dec/Last Academic Year | 1000         | 0          | 0         |
        | Jan/Last Academic Year | 1000         | 0          | 0         |
        | Feb/Last Academic Year | 1000         | 0          | 0         |
        | Mar/Last Academic Year | 1000         | 0          | 0         |
        | Apr/Last Academic Year | 1000         | 0          | 0         |
        | May/Last Academic Year | 1000         | 0          | 0         |
        | Jun/Last Academic Year | 1000         | 0          | 0         |
        | Jul/Last Academic Year | 1000         | 0          | 0         |
	#Levy Payments
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | Levy Payments | Transaction Type |
		| R01/Last Academic Year | Aug/Last Academic Year | 1000          | Learning         |
        | R02/Last Academic Year | Sep/Last Academic Year | 1000          | Learning         |
        | R03/Last Academic Year | Oct/Last Academic Year | 1000          | Learning         |
        | R04/Last Academic Year | Nov/Last Academic Year | 1000          | Learning         |
        | R05/Last Academic Year | Dec/Last Academic Year | 1000          | Learning         |
        | R06/Last Academic Year | Jan/Last Academic Year | 1000          | Learning         |
        | R07/Last Academic Year | Feb/Last Academic Year | 1000          | Learning         |
        | R08/Last Academic Year | Mar/Last Academic Year | 1000          | Learning         |
        | R09/Last Academic Year | Apr/Last Academic Year | 1000          | Learning         |
        | R10/Last Academic Year | May/Last Academic Year | 1000          | Learning         |
        | R11/Last Academic Year | Jun/Last Academic Year | 1000          | Learning         |
        | R12/Last Academic Year | Jul/Last Academic Year | 1000          | Learning         |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 15000                | 06/Aug/Last Academic Year           |                        |                                       | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details are changed as follows        
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
		| pe-1             | 15000                | 01/Aug/Current Academic Year        |                        | 01/Sep/Current Academic Year          | Act1          | 1                   | 90%                         |
	
	
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |Price Episode Identifier |
		| Aug/Current Academic Year | 0            | 3000       | 0         |pe-1                     |
		| Sep/Current Academic Year | 0            | 0          | 0         |pe-1                     |
		| Oct/Current Academic Year | 0            | 0          | 0         |pe-1                     |
		| Nov/Current Academic Year | 0            | 0          | 0         |pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         |pe-1                     |
		| Jan/Current Academic Year | 0            | 0          | 0         |pe-1                     |
		| Feb/Current Academic Year | 0            | 0          | 0         |pe-1                     |
		| Mar/Current Academic Year | 0            | 0          | 0         |pe-1                     |
		| Apr/Current Academic Year | 0            | 0          | 0         |pe-1                     |
		| May/Current Academic Year | 0            | 0          | 0         |pe-1                     |
		| Jun/Current Academic Year | 0            | 0          | 0         |pe-1                     |
		| Jul/Current Academic Year | 0            | 0          | 0         |pe-1                     |
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R01/Current Academic Year | Aug/Current Academic Year | 0            | 3000       | 0         |
	# Levy Payments
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 3000          | Completion       |
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 3000          | Completion       |
# Levy Balance
Examples: 
        | Collection_Period         | Levy Balance |
        | R01/Current Academic Year | 15000        |
