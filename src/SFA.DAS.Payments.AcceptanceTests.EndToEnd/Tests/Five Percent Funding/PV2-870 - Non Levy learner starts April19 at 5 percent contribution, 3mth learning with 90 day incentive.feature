@ignore
#Feature: 5% Contribution from April 2019
#
#Scenario: Non Levy Learner, starts new learning April 2019, 5% contribution, basic day with 3months in learning, demonstrate incentive payments 90 days
#    
#	Given the apprenticeship funding band maximum is 15000
#    
#	When an ILR file is submitted with the following data:
#		| ULN    | learner type                 | agreed price | start date  | planned end date | completion status | framework code | programme type | pathway code |
#		| 123456 | 16-18 programme only non-DAS | 15000        | 06/04/2019  | 09/04/2020       | continuing        | 403            | 2              | 1            |
#
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 04/19 | 05/19 | 06/19 | 07/19 | 08/19 |
#        | Provider Earned Total                   | 1000  | 1000  | 1000  | 2000  | 1000  |
#        | Provider Earned from SFA                | 950   | 950   | 950   | 1950  | 950   |
#        | Provider Earned from Employer           | 50    | 50    | 50    | 50    | 50    |
#        | Provider Paid by SFA                    | 0     | 950   | 950   | 950   | 1950  |
#        | Payment due from Employer               | 0     | 50    | 50    | 50    | 50    |
#        | Levy account debited                    | 0     | 0     | 0     | 0     | 0     |
#        | SFA Levy employer budget                | 0     | 0     | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     |
#		| SFA non-Levy co-funding budget          | 950   | 950   | 950   | 950   | 950   |
#        | SFA non-Levy additional payments budget | 0     | 0     | 0     | 1000  | 0     |
#
#    And the transaction types for the payments are:
#        | Payment type                 | 05/19 | 06/19 | 07/19 | 08/19 |
#        | On-program                   | 950   | 950   | 950   | 950   |
#        | Completion                   | 0     | 0     | 0     | 0     |
#        | Balancing                    | 0     | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 500   |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 500   |
        
 	Feature: 5% Contribution from April 2019 PV2-870

	Scenario Outline: Non Levy Learner, starts new learning April 2019, 5% contribution, basic day with 3months in learning, demonstrate incentive payments 90 days PV2-870

	And the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Apr/Current Academic Year | 12 months        | 15000                | 06/Apr/Current Academic Year        |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 95%                         |

   	When the ILR file is submitted for the learners for collection period <Collection_Period>

    Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive | Second16To18EmployerIncentive | Second16To18ProviderIncentive |
		| Apr/Current Academic Year | 1000         | 0          | 0         | 0                            | 0                            | 0                             | 0                             |
		| May/Current Academic Year | 1000         | 0          | 0         | 0                            | 0                            | 0                             | 0                             |
		| Jun/Current Academic Year | 1000         | 0          | 0         | 0                            | 0                            | 0                             | 0                             |
		| Jul/Current Academic Year | 1000         | 0          | 0         | 500                          | 500                          | 0                             | 0                             |
		| Aug/Next Academic Year    | 1000         | 0          | 0         | 0                            | 0                            | 0                             | 0                             |
		| Sep/Next Academic Year    | 1000         | 0          | 0         | 0                            | 0                            | 0                             | 0                             |
		| Oct/Next Academic Year    | 1000         | 0          | 0         | 0                            | 0                            | 0                             | 0                             |
		| Nov/Next Academic Year    | 1000         | 0          | 0         | 0                            | 0                            | 0                             | 0                             |
		| Dec/Next Academic Year    | 1000         | 0          | 0         | 0                            | 0                            | 0                             | 0                             |
		| Jan/Next Academic Year    | 1000         | 0          | 0         | 0                            | 0                            | 0                             | 0                             |
		| Feb/Next Academic Year    | 1000         | 0          | 0         | 0                            | 0                            | 0                             | 0                             |
		| Mar/Next Academic Year    | 1000         | 0          | 0         | 0                            | 0                            | 500                           | 500                           |

    And at month end only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive |
		| R09/Current Academic Year | Apr/Current Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
		| R10/Current Academic Year | May/Current Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
		| R11/Current Academic Year | Jun/Current Academic Year | 1000         | 0          | 0         | 0                            | 0                            |
		| R12/Current Academic Year | Jul/Current Academic Year | 1000         | 0          | 0         | 500                          | 500                          |
			
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully Funded Payments | Transaction Type             |
		| R09/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | 0                         | Learning                     |
		| R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | 0                         | Learning                     |
		| R11/Current Academic Year | Jun/Current Academic Year | 950                    | 50                          | 0                         | Learning                     |
		| R12/Current Academic Year | Jul/Current Academic Year | 950                    | 50                          | 0                         | Learning                     |
		| R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 500                       | First16To18EmployerIncentive |
		| R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 500                       | First16To18ProviderIncentive |                       																																	

	And only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully Funded Payments | Transaction Type             |
		| R09/Current Academic Year | Apr/Current Academic Year | 950                    | 50                          | 0                         | Learning                     |
		| R10/Current Academic Year | May/Current Academic Year | 950                    | 50                          | 0                         | Learning                     |
		| R11/Current Academic Year | Jun/Current Academic Year | 950                    | 50                          | 0                         | Learning                     |
		| R12/Current Academic Year | Jul/Current Academic Year | 950                    | 50                          | 0                         | Learning                     |
		| R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 500                       | First16To18EmployerIncentive |
		| R12/Current Academic Year | Jul/Current Academic Year | 0                      | 0                           | 500                       | First16To18ProviderIncentive |

   Examples: 
        | Collection_Period         |
		| R09/Current Academic Year | 
		| R10/Current Academic Year |
		| R11/Current Academic Year |
		| R12/Current Academic Year |
        