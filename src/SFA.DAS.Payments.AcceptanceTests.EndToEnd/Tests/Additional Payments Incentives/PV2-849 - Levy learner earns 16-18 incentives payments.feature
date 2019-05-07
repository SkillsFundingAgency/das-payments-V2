#Feature: Additional Payments Incentives
#
#Scenario: Payment for a 16-18 Levy learner, levy available, incentives earned
#    
#    Given levy balance > agreed price for all months
#    
#	And the following commitments exist:
#        | ULN       | start date | end date   | agreed price | status |
#        | learner a | 01/08/2018 | 01/08/2019 | 15000        | active |
#
#    When an ILR file is submitted with the following data:
#        | ULN       | learner type             | agreed price | start date | planned end date | actual end date | completion status |
#        | learner a | 16-18 programme only DAS | 15000        | 06/08/2018 | 08/08/2019       |                 | continuing        |
#      
#    Then the provider earnings and payments break down as follows:
#        | Type                                | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 08/19 | 09/19 |
#        | Provider Earned Total               | 1000  | 1000  | 1000  | 2000  | 1000  | ... | 1000  | 0     |
#        | Provider Paid by SFA                | 0     | 1000  | 1000  | 1000  | 2000  | ... | 1000  | 1000  |
#        | Levy account debited                | 0     | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#        | SFA Levy employer budget            | 1000  | 1000  | 1000  | 1000  | 1000  | ... | 0     | 0     |
#        | SFA Levy co-funding budget          | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | SFA Levy additional payments budget | 0     | 0     | 0     | 1000  | 0     | ... | 1000  | 0     |
#
#    And the transaction types for the payments are:
#        | Payment type             | 09/18 | 10/18 | 11/18 | 12/18 | ... | 08/19 | 09/19 |
#        | On-program               | 1000  | 1000  | 1000  | 1000  | ... | 1000  | 0     |
#        | Completion               | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Balancing                | 0     | 0     | 0     | 0     | ... | 0     | 0     |
#        | Employer 16-18 incentive | 0     | 0     | 0     | 500   | ... | 0     | 500   |
#        | Provider 16-18 incentive | 0     | 0     | 0     | 500   | ... | 0     | 500   |

Feature:  Additional Payments - Incentives PV2- 849
		  As a Provider,
		  I want a Levy learner, where the learner is a 16-18 year old learner and earns an incentive payment 
		  So that I am accurately paid my apprenticeship provision


Scenario: Levy learner 16-18 earns incentives PV2-849
	Given the employer levy account balance in collection period R01/Last Academic Year is 16000
	And the following commitments exist
		| start date                | end date                     | agreed price | status |
		| 06/Aug/Last Academic Year | 08/Aug/Current Academic Year | 15000        | active | 
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 12000                | 06/Aug/Last Academic Year           | 3000                   | 06/Aug/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive |
        | Aug/Last Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
        | Sep/Last Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
        | Oct/Last Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
        | Nov/Last Academic Year | 1000         | 0          | 0         | 500                          | 500                          |
        | Dec/Last Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
        | Jan/Last Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
        | Feb/Last Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
        | Mar/Last Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
        | Apr/Last Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
        | May/Last Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
        | Jun/Last Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
        | Jul/Last Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | Levy Payments | SFA Fully-Funded Payments | Transaction Type             |
        | R01/Last Academic Year | Aug/Last Academic Year | 1000          | 0                         | Learning                     |
        | R02/Last Academic Year | Sep/Last Academic Year | 1000          | 0                         | Learning                     |
        | R03/Last Academic Year | Oct/Last Academic Year | 1000          | 0                         | Learning                     |
        | R04/Last Academic Year | Nov/Last Academic Year | 1000          | 0                         | Learning                     |
        | R05/Last Academic Year | Dec/Last Academic Year | 1000          | 0                         | Learning                     |
        | R06/Last Academic Year | Jan/Last Academic Year | 1000          | 0                         | Learning                     |
        | R07/Last Academic Year | Feb/Last Academic Year | 1000          | 0                         | Learning                     |
        | R08/Last Academic Year | Mar/Last Academic Year | 1000          | 0                         | Learning                     |
        | R09/Last Academic Year | Apr/Last Academic Year | 1000          | 0                         | Learning                     |
        | R10/Last Academic Year | May/Last Academic Year | 1000          | 0                         | Learning                     |
        | R11/Last Academic Year | Jun/Last Academic Year | 1000          | 0                         | Learning                     |
        | R12/Last Academic Year | Jul/Last Academic Year | 1000          | 0                         | Learning                     |
        | R04/Last Academic Year | Nov/Last Academic Year | 0             | 500                       | First16To18EmployerIncentive |
        | R04/Last Academic Year | Nov/Last Academic Year | 0             | 500                       | First16To18ProviderIncentive |
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 12000                | 06/Aug/Last Academic Year           | 3000                   | 06/Aug/Last Academic Year             | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 17            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                        |
	When the amended ILR file is re-submitted for the learners in collection period R01/Current Academic Year
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Second16To18EmployerIncentive | Second16To18ProviderIncentive |
		| Aug/Current Academic Year | 0            | 3000       | 0         | 500                           | 500                           |
		| Sep/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             |
		| Oct/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             |
		| Nov/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             |
		| Dec/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             |
		| Jan/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             |
		| Feb/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             |
		| Mar/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             |
		| Apr/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             |
		| May/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             |
		| Jun/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             |
		| Jul/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             |
    And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Second16To18EmployerIncentive | Second16To18ProviderIncentive |
		| R01/Current Academic Year | Aug/Current Academic Year | 0            | 3000       | 0         | 500                           | 500                           |
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R01/Current Academic Year | Aug/Current Academic Year | 3000          | 0                         | Completion                      |
		| R01/Current Academic Year | Aug/Current Academic Year | 0             | 500                       | Second16To18EmployerIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0             | 500                       | Second16To18ProviderIncentive   |
		
	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type                |
		| R01/Current Academic Year | Aug/Current Academic Year | 3000          | 0                         | Completion                      |
		| R01/Current Academic Year | Aug/Current Academic Year | 0             | 500                       | Second16To18EmployerIncentive   |
		| R01/Current Academic Year | Aug/Current Academic Year | 0             | 500                       | Second16To18ProviderIncentive   |
		