#@ignore
# issue with refund being created for pe-1 £500 incentives in collection period 5 for which is pe-2. This is because a £0 incentive (TT4) comes through for delivery period 4 and there is a historic payment of £500 for that period so a refund is generated.
Feature: One Learner changes employer - incentives earned in transfer month PV2-373
	As a provider,
	I want 1 learner aged 16-18, levy available, changes employer, earns incentive payment in the commitment transfer month - and the employer transfer is recorded on the ILR in a later month, to be paid the correct amount
	So that I am accurately paid my apprenticeship provision.

	Scenario Outline: One Learner changes employer - incentives earned in transfer month PV2-373
	
	Given the "employer 1" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 1>
	And  the "employer 2" levy account balance in collection period <Collection_Period> is <Levy Balance for employer 2>
	
	And the following commitments exist 
      | Identifier       | Employer   | start date                   | end date                  | agreed price | status    | effective from               | effective to                 | stop effective from          |
      | Apprenticeship 1 | employer 1 | 05/Aug/Current Academic Year | 28/Aug/Next Academic Year | 7500         | cancelled | 05/Aug/Current Academic Year | 14/Nov/Current Academic Year | 15/Nov/Current Academic Year |
      | Apprenticeship 2 | employer 2 | 15/Nov/Current Academic Year | 28/Aug/Next Academic Year | 5625         | active    | 15/Nov/Current Academic Year |                              |                              |

	And the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 6000                 | 01/Aug/Current Academic Year        | 1500                   | 01/Aug/Current Academic Year          |                 | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |

   And price details as follows
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
        | pe-1             | 6000                 | 01/Aug/Current Academic Year        | 1500                   | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |
  
  And the following earnings had been generated for the learner
        | Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | Second16To18EmployerIncentive | First16To18ProviderIncentive | Second16To18ProviderIncentive | Price Episode Identifier |
        | Aug/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
        | Sep/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
        | Oct/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
        | Nov/Current Academic Year | 500          | 0          | 0         | 500                          | 0                             | 500                          | 0                             | pe-1                     |
        | Dec/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
        | Jan/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
        | Feb/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
        | Mar/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
        | Apr/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
        | May/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
        | Jun/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
        | Jul/Current Academic Year | 500          | 1500       | 0         | 0                            | 500                           | 0                            | 500                           | pe-1                     |

    And the following provider payments had been generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully Funded Payments | Transaction Type             | Employer   |
        | R01/Current Academic Year | Aug/Current Academic Year | 500           |                           | Learning                     | employer 1 |
        | R02/Current Academic Year | Sep/Current Academic Year | 500           |                           | Learning                     | employer 1 |
        | R03/Current Academic Year | Oct/Current Academic Year | 500           |                           | Learning                     | employer 1 |
        | R04/Current Academic Year | Nov/Current Academic Year |               | 500                       | First16To18EmployerIncentive | employer 1 |
        | R04/Current Academic Year | Nov/Current Academic Year |               | 500                       | First16To18ProviderIncentive | employer 1 |

    But the Provider now changes the Learner details as follows
		| Employer id | Start Date                   | Planned Duration | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Standard Code | Programme Type | Funding Line Type                                  |
		| employer 1  | 05/Aug/Current Academic Year | 12 months        | 3 months        | withdrawn         | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |
		| employer 2  | 15/Nov/Current Academic Year | 12 months        | 9 months        | continuing        | Act1          | 1                   | ZPROG001      | 51            | 25             | 16-18 Apprenticeship (From May 2017) Levy Contract |

	And price details as follows
        | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
        | pe-1             | 6000                 | 01/Aug/Current Academic Year        | 1500                   | 01/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act1          |
        | pe-2             |                      |                                     |                        |                                       | 5000                    | 15/Dec/Current Academic Year           | 625                       | 15/Dec/Current Academic Year             | 90%                         | Act1          |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>

	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | Second16To18EmployerIncentive | First16To18ProviderIncentive | Second16To18ProviderIncentive | Price Episode Identifier |
		| Aug/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
		| Sep/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
		| Oct/Current Academic Year | 500          | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-1                     |
		| Nov/Current Academic Year | 562.5        | 0          | 0         | 500                          | 0                             | 500                          | 0                             | pe-1                     |
		| Dec/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
		| Jan/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
		| Feb/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
		| Mar/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
		| Apr/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
		| May/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
		| Jun/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             | pe-2                     |
		| Jul/Current Academic Year | 562.5        | 1125       | 0         | 0                            | 500                           | 0                            | 500                           | pe-2                     |
																																																				 
    And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | Second16To18EmployerIncentive | First16To18ProviderIncentive | Second16To18ProviderIncentive |
        | R05/Current Academic Year | Dec/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
        | R06/Current Academic Year | Jan/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
        | R07/Current Academic Year | Feb/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
        | R08/Current Academic Year | Mar/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
        | R09/Current Academic Year | Apr/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
        | R10/Current Academic Year | May/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
        | R11/Current Academic Year | Jun/Current Academic Year | 562.5        | 0          | 0         | 0                            | 0                             | 0                            | 0                             |
        | R12/Current Academic Year | Jul/Current Academic Year | 562.5        | 1125       | 0         | 0                            | 500                           | 0                            | 500                           |

	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully Funded Payments | Transaction Type              | Employer   |
        | R05/Current Academic Year | Dec/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R06/Current Academic Year | Jan/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R07/Current Academic Year | Feb/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R08/Current Academic Year | Mar/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R09/Current Academic Year | Apr/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R10/Current Academic Year | May/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R11/Current Academic Year | Jun/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R12/Current Academic Year | Jul/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R12/Current Academic Year | Jul/Current Academic Year |               | 500                       | Second16To18EmployerIncentive | employer 2 |
        | R12/Current Academic Year | Jul/Current Academic Year |               | 500                       | Second16To18ProviderIncentive | employer 2 |
        | R12/Current Academic Year | Jul/Current Academic Year | 1125          |                           | Completion                    | employer 2 |

	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | Levy Payments | SFA Fully Funded Payments | Transaction Type              | Employer   |
        | R05/Current Academic Year | Dec/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R06/Current Academic Year | Jan/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R07/Current Academic Year | Feb/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R08/Current Academic Year | Mar/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R09/Current Academic Year | Apr/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R10/Current Academic Year | May/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R11/Current Academic Year | Jun/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R12/Current Academic Year | Jul/Current Academic Year | 562.5         |                           | Learning                      | employer 2 |
        | R12/Current Academic Year | Jul/Current Academic Year |               | 500                       | Second16To18EmployerIncentive | employer 2 |
        | R12/Current Academic Year | Jul/Current Academic Year |               | 500                       | Second16To18ProviderIncentive | employer 2 |
        | R12/Current Academic Year | Jul/Current Academic Year | 1125          |                           | Completion                    | employer 2 |
Examples: 
        | Collection_Period         | Levy Balance for employer 1 | Levy Balance for employer 2 |
		| R05/Current Academic Year | 13500                       | 7125                        |
		| R06/Current Academic Year | 13500                       | 7125                        |
		| R07/Current Academic Year | 13500                       | 7125                        |
		| R08/Current Academic Year | 13500                       | 7125                        |
		| R09/Current Academic Year | 13500                       | 7125                        |
		| R10/Current Academic Year | 13500                       | 7125                        |
		| R11/Current Academic Year | 13500                       | 7125                        |
		| R12/Current Academic Year | 13500                       | 7125                        | 
  

  #@_Minimum_Acceptance_
#Scenario: 1 learner aged 16-18, levy available, changes employer, earns incentive payment in the commitment transfer month - and the employer transfer is recorded on the ILR in a later month
#    
#        Given The learner is programme only DAS
#        And the employer 1 has a levy balance > agreed price for all months
#        And the employer 2 has a levy balance > agreed price for all months
#        And the learner changes employers
#            | Employer   | Type | ILR employment start date |
#            | employer 1 | DAS  | 06/08/2018                |
#            | employer 2 | DAS  | 15/12/2018                |
#        
#        And the following commitments exist:
#            | Employer   | commitment Id | version Id | Provider   | ULN       | start date | end date   | agreed price | status    | effective from | effective to | stop effective from |
#            | employer 1 | 1             | 1-001      | provider a | learner a | 01/08/2018 | 01/08/2019 | 7500         | cancelled | 01/08/2018     | 14/11/2018   | 15/11/2018   |
#            | employer 2 | 2             | 1-001      | provider a | learner a | 15/11/2018 | 01/08/2019 | 5625         | active    | 15/11/2018     |              |              |
#            
#        When an ILR file is submitted with the following data:
#            | Provider   | learner type             | ULN       | start date | planned end date | actual end date | completion status | Total training price | Total training price effective date | Total assessment price | Total assessment price effective date | Residual training price | Residual training price effective date | Residual assessment price | Residual assessment price effective date |
#            | provider a | 16-18 programme only DAS | learner a | 06/08/2018 | 08/08/2019       |                 | continuing        | 6000                 | 06/08/2018                          | 1500                   | 06/08/2018                            | 4000                    | 15/12/2018                             | 1625                      | 15/12/2018                               |
#
#        #Then the data lock status will be as follows:
#        #    | Payment type             | 08/18               | 09/18               | 10/18               | 11/18               | 12/18               | 01/19               |
#        #    | On-program               | commitment 1 v1-001 | commitment 1 v1-001 | commitment 1 v1-001 |                     | commitment 2 v1-001 | commitment 2 v1-001 |
#        #    | Employer 16-18 incentive |                     |                     |                     | commitment 1 v1-001 |                     |                     |
#        #    | Provider 16-18 incentive |                     |                     |                     | commitment 1 v1-001 |                     |                     |
#        
#        Then the earnings and payments break down for provider a is as follows:
#            | Type                                | 08/18 | 09/18 | 10/18 | 11/18 | 12/18  | 01/19  |
#            | Provider Earned Total               | 500   | 500   | 500   | 1500  | 562.50 | 562.50 |
#            | Provider Earned from SFA            | 500   | 500   | 500   | 1000  | 562.50 | 562.50 |
#            | Provider Earned from Employer 1     | 0     | 0     | 0     | 0     | 0      | 0      |
#            | Provider Earned from Employer 2     | 0     | 0     | 0     | 0     | 0      | 0      |
#            | Provider Paid by SFA                | 0     | 500   | 500   | 500   | 1000   | 562.50 |
#            | Payment due from employer 1         | 0     | 0     | 0     | 0     | 0      | 0      |
#            | Payment due from employer 2         | 0     | 0     | 0     | 0     | 0      | 0      |
#            | Employer 1 Levy account debited     | 0     | 500   | 500   | 500   | 0      | 0      |
#            | Employer 2 Levy account debited     | 0     | 0     | 0     | 0     | 0      | 562.50 |
#            | SFA Levy employer budget            | 500   | 500   | 500   | 0     | 562.50 | 562.50 |
#            | SFA Levy co-funding budget          | 0     | 0     | 0     | 0     | 0      | 0      |
#            | SFA non-Levy co-funding budget      | 0     | 0     | 0     | 0     | 0      | 0      |
#            | SFA Levy additional payments budget | 0     | 0     | 0     | 1000  | 0      | 0      |
#            
#         And the transaction types for the payments for provider a are:
#            | Payment type               | 09/18 | 10/18 | 11/18 | 12/18 | 01/19  |
#            | On-program                 | 500   | 500   | 500   | 0     | 562.50 |
#            | Completion                 | 0     | 0     | 0     | 0     | 0      |
#            | Balancing                  | 0     | 0     | 0     | 0     | 0      |
#            | Employer 1 16-18 incentive | 0     | 0     | 0     | 500   | 0      |
#            | Employer 2 16-18 incentive | 0     | 0     | 0     | 0     | 0      |
#            | Provider 16-18 incentive   | 0     | 0     | 0     | 500   | 0      |
  # For DC Integration
  # And the learner changes employers
  # | Employer   | Type | ILR employment start date |
  # | employer 1 | DAS  | 01/08/2018                |
  # | employer 2 | DAS  | 15/12/2018                |
