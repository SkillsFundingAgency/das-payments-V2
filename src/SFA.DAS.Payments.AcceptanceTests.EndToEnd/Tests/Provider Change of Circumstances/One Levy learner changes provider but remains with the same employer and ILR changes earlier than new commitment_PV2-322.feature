@ignore
Feature: One Levy learner changes provider earns incentive in the commitment transfer month and ILR transfer happens earlier than commitment PV2-322
	1 learner aged 16-18, levy available, changes provider, earns incentive payment in the commitment transfer month - and the ILR transfer happens earlier than commitment
	As a provider,
	I want a levy learner, aged 16-18, levy available, changes provider, earns incentive payment in the commitment transfer month and the ILR transfer happens at an earlier point than the commitment, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.

Scenario Outline: One Levy learner changes provider earns incentive in the commitment transfer month and ILR transfer happens earlier than commitment PV2-322
	Given the employer levy account balance in collection period <Collection_Period> is <Levy Balance>
	And the following commitments exist
        | Identifier       |  Provider   | start date                   | end date                  | agreed price | status    | effective from               | effective to                 | stop effective from          |
        | Apprenticeship 1 |  provider a | 01/Aug/Current Academic Year | 01/Aug/Next Academic Year | 7500         | cancelled | 01/Aug/Current Academic Year | 14/Nov/Current Academic Year | 15/Nov/Current Academic Year |
        | Apprenticeship 2 |  provider b | 15/Nov/Current Academic Year | 01/Aug/Next Academic Year | 5625         | active    | 15/Nov/Current Academic Year |                              |                              |
	And the "provider a" previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 6000                 | 06/Aug/Current Academic Year        | 1500                   | 06/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
    And the following earnings had been generated for the learner for "provider a"
        | Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive |
        | Aug/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Sep/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Oct/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Nov/Current Academic Year | 500          | 0          | 0         | 500                          | 500                          |
        | Dec/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Jan/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Feb/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Mar/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Apr/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | May/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Jun/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Jul/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
    And the following "provider a" payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully-Funded Payments | Transaction Type             |
        | R01/Current Academic Year | Aug/Current Academic Year | 500           | 0                         | Learning                     |
        | R02/Current Academic Year | Sep/Current Academic Year | 500           | 0                         | Learning                     |
        | R03/Current Academic Year | Oct/Current Academic Year | 500           | 0                         | Learning                     |
        | R04/Current Academic Year | Nov/Current Academic Year | 0             | 500                       | First16To18EmployerIncentive |
		| R04/Current Academic Year | Nov/Current Academic Year | 0             | 500                       | First16To18ProviderIncentive |
    But the "provider a" now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 06/Aug/Current Academic Year | 12 months        | 6000                 | 06/Aug/Current Academic Year        | 1500                   | 06/Aug/Current Academic Year          | 3 months        | withdrawn         | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	And the "provider b" is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 10/Nov/Current Academic Year | 9 months         | 4000                 | 10/Nov/Current Academic Year        | 1625                   | 10/Nov/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period> by "provider a"
	When the ILR file is submitted for the learners for collection period <Collection_Period> by "provider b"
	Then the following learner earnings should be generated for "provider a"
        | Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive |
        | Aug/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Sep/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Oct/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Nov/Current Academic Year | 0            | 0          | 0         | 500                          | 500                          |
        | Dec/Current Academic Year | 0            | 0          | 0         | 0                            | 0                            |
        | Jan/Current Academic Year | 0            | 0          | 0         | 0                            | 0                            |
        | Feb/Current Academic Year | 0            | 0          | 0         | 0                            | 0                            |
        | Mar/Current Academic Year | 0            | 0          | 0         | 0                            | 0                            |
        | Apr/Current Academic Year | 0            | 0          | 0         | 0                            | 0                            |
        | May/Current Academic Year | 0            | 0          | 0         | 0                            | 0                            |
        | Jun/Current Academic Year | 0            | 0          | 0         | 0                            | 0                            |
        | Jul/Current Academic Year | 0            | 0          | 0         | 0                            | 0                            |
	And the following learner earnings should be generated for "provider b"
        | Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive |
        | Aug/Current Academic Year | 0            | 0          | 0         | 0                            | 0                            |
        | Sep/Current Academic Year | 0            | 0          | 0         | 0                            | 0                            |
        | Oct/Current Academic Year | 0            | 0          | 0         | 0                            | 0                            |
        | Nov/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Dec/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Jan/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Feb/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Mar/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Apr/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | May/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Jun/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |
        | Jul/Current Academic Year | 500          | 0          | 0         | 0                            | 0                            |

    And at month end no payments will be calculated for "provider a"
	And at month end no payments will be calculated for "provider b"

	And Month end is triggered

	And no "provider a" payments will be recorded        
	And no "provider b" payments will be recorded        
	And no "provider a" payments will be generated
	And no "provider b" payments will be generated
Examples: 
        | Collection_Period         | Levy Balance |
        | R01/Current Academic Year | 8000         |
        | R02/Current Academic Year | 7500         |
        | R03/Current Academic Year | 7000         |
        | R04/Current Academic Year | 6500         |
        | R05/Current Academic Year | 6500         |
        | R06/Current Academic Year | 6500         |
        | R07/Current Academic Year | 6500         |
        | R08/Current Academic Year | 6500         |
        | R09/Current Academic Year | 6500         |
        | R10/Current Academic Year | 6500         |
        | R11/Current Academic Year | 6500         |
        | R12/Current Academic Year | 6500         |



#Scenario: 1 learner aged 16-18, levy available, changes provider, earns incentive payment in the commitment transfer month - and the ILR transfer happens at an earlier point than the commitment   
#    
#    Given levy balance > agreed price for all months
#	And the apprenticeship funding band maximum is 15000
#    And the following commitments exist:
#            | commitment Id | version Id | Provider   | ULN       | start date | end date   | agreed price | status    | effective from | effective to | stop effective from |
#            | 1             | 1-001      | provider a | learner a | 01/08/2018 | 01/08/2019 | 7500         | cancelled | 01/08/2018     | 14/11/2018   | 15/11/2018   |
#            | 2             | 1-001      | provider b | learner a | 15/11/2018 | 01/08/2019 | 5625         | active    | 15/11/2018     |              |              |
#            
#    When the providers submit the following ILR files:
#            | Provider   | learner type             | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | Residual training price | Residual training price effective date | Residual assessment price | Residual assessment price effective date |
#            | provider a | 16-18 programme only DAS | learner a | 06/08/2018 | 08/08/2019       | 09/11/2018      | withdrawn         | 6000                 | 06/08/2018                          | 1500                   | 06/08/2018                            |                         |                                        |                           |                                          |
#            | provider b | 16-18 programme only DAS | learner a | 10/11/2018 | 08/08/2019       |                 | continuing        |                      |                                     |                        |                                       | 4000                    | 10/11/2018                             | 1625                      | 10/11/2018                               |        
#      
#        #Then the data lock status will be as follows:
#        #    | Payment type             | 08/18               | 09/18               | 10/18               | 11/18               | 12/18 | 01/19 | 02/19 |
#        #    | On-program               | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-001 |                     |       |       |       |
#        #    | Employer 16-18 incentive |                     |                     |                     | commitment 1 v1-001 |       |       |       |
#        #    | Provider 16-18 incentive |                     |                     |                     | commitment 1 v1-001 |       |       |       |
#        
#        Then OBSOLETE - the earnings and payments break down for provider a is as follows:
#            | Type                                | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | 01/19 | 02/19 |
#            | Provider Earned Total               | 500   | 500   | 500   | 1000  | 0     | 0     | 0     |
#            | Provider Earned from SFA            | 500   | 500   | 500   | 1000  | 0     | 0     | 0     |
#            | Provider Earned from Employer       | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#            | Provider Paid by SFA                | 0     | 500   | 500   | 500   | 1000  | 0     | 0     |
#            | Payment due from Employer           | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#            | Levy account debited                | 0     | 500   | 500   | 500   | 0     | 0     | 0     |
#            | SFA Levy employer budget            | 500   | 500   | 500   | 0     | 0     | 0     | 0     |
#            | SFA Levy co-funding budget          | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget      | 0     | 0     | 0     | 0     | 0     | 0     | 0     |
#            | SFA Levy additional payments budget | 0     | 0     | 0     | 1000  | 0     | 0     | 0     |
#            
#         And the transaction types for the payments for provider a are:
#            | Payment type             | 09/18 | 10/18 | 11/18 | 12/18 |
#            | On-program               | 500   | 500   | 500   | 0     |
#            | Completion               | 0     | 0     | 0     | 0     |
#            | Balancing                | 0     | 0     | 0     | 0     |
#            | Employer 16-18 incentive | 0     | 0     | 0     | 500   |
#            | Provider 16-18 incentive | 0     | 0     | 0     | 500   |
#            
#        And OBSOLETE - the earnings and payments break down for provider b is as follows:
#            | Type                                | 08/18 | ... | 11/18 | 12/18 | 01/19 | 02/19 |
#            | Provider Earned Total               | 0     | ... | 500   | 500   | 500   | 500   |
#            | Provider Earned from SFA            | 0     | ... | 0     | 0     | 0     | 0     |
#            | Provider Earned from Employer       | 0     | ... | 0     | 0     | 0     | 0     |
#            | Provider Paid by SFA                | 0     | ... | 0     | 0     | 0     | 0     |
#            | Payment due from Employer           | 0     | ... | 0     | 0     | 0     | 0     |
#            | Levy account debited                | 0     | ... | 0     | 0     | 0     | 0     |
#            | SFA Levy employer budget            | 0     | ... | 0     | 0     | 0     | 0     |
#            | SFA Levy co-funding budget          | 0     | ... | 0     | 0     | 0     | 0     |
#            | SFA non-Levy co-funding budget      | 0     | ... | 0     | 0     | 0     | 0     |
#            | SFA Levy additional payments budget | 0     | ... | 0     | 0     | 0     | 0     |
#            
#         And the transaction types for the payments for provider b are:
#            | Payment type             | 11/18 | 12/18 | 01/19 | 02/19 |
#            | On-program               | 0     | 0     | 0     | 0     |
#            | Completion               | 0     | 0     | 0     | 0     |
#            | Balancing                | 0     | 0     | 0     | 0     |
#            | Employer 16-18 incentive | 0     | 0     | 0     | 0     |
#            | Provider 16-18 incentive | 0     | 0     | 0     | 0     |