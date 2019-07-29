
Feature:  Levy learner requires learning support doing an apprenticeship standard PV2-734
		As a provider,
		I want a Levy learner, with Learning Support, completing an apprenticeship standard
		So that I am paid the correct apprenticeship funding by SFA

# for DC Integration
# | learning support code | learning support date from | learning support date to |
# | 1                     | 06/08/2018                 | 10/08/2019				  |

Scenario: Levy learner requires learning support doing an apprenticeship standard PV2-734	
	Given the employer levy account balance in collection period R01/Current Academic Year is 15000	
	And the following commitments exist
        | start date                | end date                     | agreed price | status |Standard Code | Programme Type |
        | 01/Aug/Last Academic Year | 01/Aug/Current Academic Year | 15000        | active |51            | 25             |
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | LearningSupport |
        | Aug/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Sep/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Oct/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Nov/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Dec/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Jan/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Feb/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Mar/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Apr/Last Academic Year | 1000         | 0          | 0         | 150             |
        | May/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Jun/Last Academic Year | 1000         | 0          | 0         | 150             |
        | Jul/Last Academic Year | 1000         | 0          | 0         | 150             |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | Levy Payments | SFA Fully-Funded Payments | Transaction Type |
        | R01/Last Academic Year | Aug/Last Academic Year | 1000          | 0                         | Learning         |
        | R02/Last Academic Year | Sep/Last Academic Year | 1000          | 0                         | Learning         |
        | R03/Last Academic Year | Oct/Last Academic Year | 1000          | 0                         | Learning         |
        | R04/Last Academic Year | Nov/Last Academic Year | 1000          | 0                         | Learning         |
        | R05/Last Academic Year | Dec/Last Academic Year | 1000          | 0                         | Learning         |
        | R06/Last Academic Year | Jan/Last Academic Year | 1000          | 0                         | Learning         |
        | R07/Last Academic Year | Feb/Last Academic Year | 1000          | 0                         | Learning         |
        | R08/Last Academic Year | Mar/Last Academic Year | 1000          | 0                         | Learning         |
        | R09/Last Academic Year | Apr/Last Academic Year | 1000          | 0                         | Learning         |
        | R10/Last Academic Year | May/Last Academic Year | 1000          | 0                         | Learning         |
        | R11/Last Academic Year | Jun/Last Academic Year | 1000          | 0                         | Learning         |
        | R12/Last Academic Year | Jul/Last Academic Year | 1000          | 0                         | Learning         |
        | R01/Last Academic Year | Aug/Last Academic Year | 0             | 150                       | LearningSupport  |
        | R02/Last Academic Year | Sep/Last Academic Year | 0             | 150                       | LearningSupport  |
        | R03/Last Academic Year | Oct/Last Academic Year | 0             | 150                       | LearningSupport  |
        | R04/Last Academic Year | Nov/Last Academic Year | 0             | 150                       | LearningSupport  |
        | R05/Last Academic Year | Dec/Last Academic Year | 0             | 150                       | LearningSupport  |
        | R06/Last Academic Year | Jan/Last Academic Year | 0             | 150                       | LearningSupport  |
        | R07/Last Academic Year | Feb/Last Academic Year | 0             | 150                       | LearningSupport  |
        | R08/Last Academic Year | Mar/Last Academic Year | 0             | 150                       | LearningSupport  |
        | R09/Last Academic Year | Apr/Last Academic Year | 0             | 150                       | LearningSupport  |
        | R10/Last Academic Year | May/Last Academic Year | 0             | 150                       | LearningSupport  |
        | R11/Last Academic Year | Jun/Last Academic Year | 0             | 150                       | LearningSupport  |
        | R12/Last Academic Year | Jul/Last Academic Year | 0             | 150                       | LearningSupport  |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 15000                | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	And price details as follows		
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Contract Type | Aim Sequence Number | SFA Contribution Percentage |
        | pe-1             | 15000                | 06/Aug/Last Academic Year           |                        | 06/Aug/Last Academic Year             | Act1          | 1                   | 90%                         |
	When the amended ILR file is re-submitted for the learners in collection period R01/Current Academic Year
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Price Episode Identifier |
		| Aug/Current Academic Year | 0            | 3000       | 0         | pe-1                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Apr/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| May/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jun/Current Academic Year | 0            | 0          | 0         | pe-1                     |
		| Jul/Current Academic Year | 0            | 0          | 0         | pe-1                     |
    And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R01/Current Academic Year | Aug/Current Academic Year | 0            | 3000       | 0         |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 3000          | Completion       |
	And only the following provider payments will be generated					  
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 3000          | Completion       |







#Scenario: Payment for a DAS learner, requires learning support, doing an apprenticeship standard
#    
#	Given levy balance > agreed price for all months
#    
#	And the following commitments exist:
#        | commitment Id | version Id | ULN       | start date | end date   | standard code | agreed price | status |
#        | 1             | 1          | learner a | 01/08/2018 | 01/08/2019 | 50            | 15000        | active |
#    
#	When an ILR file is submitted with the following data:
#        | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | standard code | learning support code | learning support date from | learning support date to |
#        | learner a | programme only DAS | 15000        | 06/08/2018 | 08/08/2019       | 10/08/2019      | completed         | 50            | 1                     | 06/08/2018                 | 10/08/2019				  |
#    
#	Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 07/19 | 08/19 | 09/19 |
#        | Provider Earned Total                   | 1150  | 1150  | 1150  | 1150  | 1150  | ... | 1150  | 3000  | 0     |
#        | Provider Earned from SFA                | 1150  | 1150  | 1150  | 1150  | 1150  | ... | 1150  | 3000  | 0     |
#        | Provider Earned from Employer           | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider Paid by SFA                    | 0     | 1150  | 1150  | 1150  | 1150  | ... | 1150  | 1150  | 3000  |
#        | Payment due from Employer               | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Levy account debited                    | 0     | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 1000  | 3000  |
#        | SFA Levy employer budget                | 1000  | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 3000  | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA non-Levy co-funding budget          | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy additional payments budget     | 150   | 150   | 150   | 150   | 150   | ... | 150   | 0     | 0     |
#        | SFA non-Levy additional payments budget | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#    
#	And the transaction types for the payments are:
#        | Payment type                 | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 07/19 | 08/19 | 09/19 |
#        | On-program                   | 0     | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 1000  | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 3000  |
#        | Balancing                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider learning support    | 0     | 150   | 150   | 150   | 150   | ... | 150   | 150   | 0     |



