#@SmallEmployerDas       
#Scenario: 1 learner aged 19-24, levy, employed with a small employer at start, is funded using levy for on programme and completion payments (this apprentice does not have a Education Health Care plan and is not a care leaver)
#
#    Given levy balance > agreed price for all months
#    And the following commitments exist:
#        | ULN       | framework code | programme type | pathway code | agreed price | start date | end date   |
#        | learner a | 403            | 2              | 1            | 7500         | 06/08/2018 | 08/08/2019 |
#    
#    When an ILR file is submitted with the following data:
#        | ULN       | learner type             | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code | Employment Status  | Employment Status Applies | Employer Id | Small Employer | LearnDelFAM |
#        | learner a | 19-24 programme only DAS | 7500         | 06/08/2018 | 08/08/2019       | 08/08/2019      | completed         | 403            | 2              | 1            | In paid employment | 05/08/2018                | 12345678    | SEM1           |             |
#	
#    And the employment status in the ILR is:
#        | Employer    | Employment Status      | Employment Status Applies | Small Employer |
#        | employer 1  | in paid employment     | 05/08/2018                | SEM1           |
#	
#    Then the provider earnings and payments break down as follows:
#        | Type                           | 08/18 | 09/18 | 10/18 | ... | 07/19 | 08/19 | 09/19 |
#        | Provider Earned Total          | 500   | 500   | 500   | ... | 500   | 1500  | 0     |
#        | Provider Earned from SFA       | 500   | 500   | 500   | ... | 500   | 1500  | 0     |
#        | Provider Earned from Employer  | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Provider Paid by SFA           | 0     | 500   | 500   | ... | 500   | 500   | 1500  |
#        | Payment due from Employer      | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | Levy account debited           | 0     | 500   | 500   | ... | 500   | 500   | 1500  |
#        | SFA Levy employer budget       | 500   | 500   | 500   | ... | 500   | 1500  | 0     |
#        | SFA Levy co-funding budget     | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA non-Levy co-funding budget | 0     | 0     | 0     | ... | 0     | 0     | 0     |
# 
#    And the transaction types for the payments are:
#        | Payment type                 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 08/19 | 09/19 |
#        | On-program                   | 500   | 500   | 500   | 500   | ... | 500   | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | ... | 0     | 1500  |
#        | Balancing                    | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Employer 19-24 incentive     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Provider 19-24 incentive     | 0     | 0     | 0     | 0     | ... | 0     | 0     |

Feature:  Levy learner 19-24 employed with a small employer at start is funded via Levy PV2-349
		As a provider,
		I want a levy learner, payment for a 19-24 levy learner, small employer at start, to be paid the correct amount
		So that I am accurately paid my apprenticeship provision - PV2-349

# for DC Integration
#    And the employment status in the ILR is:
#        | Employer    | Employment Status      | Employment Status Applies | Small Employer |
#        | employer 1  | in paid employment     | 05/08/2018                | SEM1           |

# 19-24

Scenario Outline: Levy learner 19-24 employed with a small employer at start fully funded PV2-349
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | start date                | end date                     | agreed price | Framework Code | Pathway Code | Programme Type | 
        | 06/Aug/Last Academic Year | 08/Aug/Current Academic Year | 7500         | 593            | 1            | 20             | 
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19-24 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year | 500          | 0          | 0         |
        | Sep/Last Academic Year | 500          | 0          | 0         |
        | Oct/Last Academic Year | 500          | 0          | 0         |
        | Nov/Last Academic Year | 500          | 0          | 0         |
        | Dec/Last Academic Year | 500          | 0          | 0         |
        | Jan/Last Academic Year | 500          | 0          | 0         |
        | Feb/Last Academic Year | 500          | 0          | 0         |
        | Mar/Last Academic Year | 500          | 0          | 0         |
        | Apr/Last Academic Year | 500          | 0          | 0         |
        | May/Last Academic Year | 500          | 0          | 0         |
        | Jun/Last Academic Year | 500          | 0          | 0         |
        | Jul/Last Academic Year | 500          | 0          | 0         |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | Levy Payments | Transaction Type |
        | R01/Last Academic Year | Aug/Last Academic Year | 500           | Learning         |
        | R02/Last Academic Year | Sep/Last Academic Year | 500           | Learning         |
        | R03/Last Academic Year | Oct/Last Academic Year | 500           | Learning         |
        | R04/Last Academic Year | Nov/Last Academic Year | 500           | Learning         |
        | R05/Last Academic Year | Dec/Last Academic Year | 500           | Learning         |
        | R06/Last Academic Year | Jan/Last Academic Year | 500           | Learning         |
        | R07/Last Academic Year | Feb/Last Academic Year | 500           | Learning         |
        | R08/Last Academic Year | Mar/Last Academic Year | 500           | Learning         |
        | R09/Last Academic Year | Apr/Last Academic Year | 500           | Learning         |
        | R10/Last Academic Year | May/Last Academic Year | 500           | Learning         |
        | R11/Last Academic Year | Jun/Last Academic Year | 500           | Learning         |
        | R12/Last Academic Year | Jul/Last Academic Year | 500           | Learning         |      
          
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 7500                 | 06/Aug/Last Academic Year           | 0                      | 06/Aug/Last Academic Year             | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 19-24 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 0            | 1500       | 0         |
		| Sep/Current Academic Year | 0            | 0          | 0         |
		| Oct/Current Academic Year | 0            | 0          | 0         |
		| Nov/Current Academic Year | 0            | 0          | 0         |
		| Dec/Current Academic Year | 0            | 0          | 0         |
		| Jan/Current Academic Year | 0            | 0          | 0         |
		| Feb/Current Academic Year | 0            | 0          | 0         |
		| Mar/Current Academic Year | 0            | 0          | 0         |
		| Apr/Current Academic Year | 0            | 0          | 0         |
		| May/Current Academic Year | 0            | 0          | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |
    And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
		| R01/Current Academic Year | Aug/Current Academic Year | 0            | 1500       | 0         |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 1500          | Completion       |
	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | Levy Payments | Transaction Type |
		| R01/Current Academic Year | Aug/Current Academic Year | 1500          | Completion       |
Examples: 
        | Collection_Period         | Levy Balance |
        | R01/Current Academic Year | 2000         |
        | R02/Current Academic Year | 500          |