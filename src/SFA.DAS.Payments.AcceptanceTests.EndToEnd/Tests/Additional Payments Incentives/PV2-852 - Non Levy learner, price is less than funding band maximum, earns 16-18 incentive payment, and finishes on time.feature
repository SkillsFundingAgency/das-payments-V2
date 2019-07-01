#Feature: Additional Payment Incentives
#
#Scenario: Non Levy Learner finishes on time, Price is less than Funding Band Maximum of £9,000
#    
#	Given the apprenticeship funding band maximum is 9000
#    
#	When an ILR file is submitted with the following data:
#		| ULN    | learner type                 | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code | employer contribution |
#        | 123455 | 16-18 programme only non-DAS | 8250         | 06/08/2018 | 09/08/2019       | 10/08/2019      | Completed         | 403            | 2              | 1            | 660                   |
#
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 06/19 | 07/19 | 08/19 | 09/19 |
#        | Provider Earned Total                   | 550   | 550   | 550   | 1550  | 550   | ... | 550   | 550   | 2650  | 0     |
#        | Provider Earned from SFA                | 495   | 495   | 495   | 1495  | 495   | ... | 495   | 495   | 2485  | 0     |
#        | Provider Earned from Employer           | 55    | 55    | 55    | 55    | 55    | ... | 55    | 55    | 165   | 0     |
#        | Provider Paid by SFA                    | 0     | 495   | 495   | 495   | 1495  | ... | 495   | 495   | 495   | 2485  |
#        | Payment due from Employer               | 0     | 55    | 55    | 55    | 55    | ... | 55    | 55    | 55    | 165   |
#        | Levy account debited                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | SFA Levy employer budget                | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#		| SFA non-Levy co-funding budget          | 495   | 495   | 495   | 495   | 495   | ... | 495   | 495   | 1485  | 0     |
#        | SFA non-Levy additional payments budget | 0     | 0     | 0     | 1000  | 0     | ... | 0     | 0     | 1000  | 0     |
#
#      And the transaction types for the payments are:
#        | Payment type                 | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | ... | 06/19 | 07/19 | 08/19 | 09/19 |
#        | On-program                   | 0     | 495   | 495   | 495   | 495   | ... | 495   | 495   | 495   | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 1485  |
#        | Balancing                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     | 500   | ... | 0     | 0     | 0     | 500   |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     | 500   | ... | 0     | 0     | 0     | 500   |
#        | Framework uplift on-program  | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | Framework uplift completion  | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | Framework uplift balancing   | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 0     | 0     | ..  | 0     | 0     | 0     | 0     |

	Feature: Additional Payment Incentives PV2-852

	Scenario: Non Levy Learner finishes on time, Price is less than Funding Band Maximum of £9,000

	Given the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 06/Aug/Last Academic Year | 12 months        | 8250                 | 06/Aug/Last Academic Year           |                        |                                       |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |

    And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing | First16To18EmployerIncentive | First16To18ProviderIncentive |
        | Aug/Last Academic Year | 550          | 0          | 0         | 0                            | 0                            |
        | Sep/Last Academic Year | 550          | 0          | 0         | 0                            | 0                            |
        | Oct/Last Academic Year | 550          | 0          | 0         | 0                            | 0                            |
        | Nov/Last Academic Year | 550          | 0          | 0         | 500                          | 500                          |
        | Dec/Last Academic Year | 550          | 0          | 0         | 0                            | 0                            |
        | Jan/Last Academic Year | 550          | 0          | 0         | 0                            | 0                            |
        | Feb/Last Academic Year | 550          | 0          | 0         | 0                            | 0                            |
        | Mar/Last Academic Year | 550          | 0          | 0         | 0                            | 0                            |
        | Apr/Last Academic Year | 550          | 0          | 0         | 0                            | 0                            |
        | May/Last Academic Year | 550          | 0          | 0         | 0                            | 0                            |
        | Jun/Last Academic Year | 550          | 0          | 0         | 0                            | 0                            |
        | Jul/Last Academic Year | 550          | 0          | 0         | 0                            | 0                            |
    And the following provider payments had been generated
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type             |
        | R01/Last Academic Year | Aug/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R02/Last Academic Year | Sep/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R03/Last Academic Year | Oct/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R04/Last Academic Year | Nov/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R05/Last Academic Year | Dec/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R06/Last Academic Year | Jan/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R07/Last Academic Year | Feb/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R08/Last Academic Year | Mar/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R09/Last Academic Year | Apr/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R10/Last Academic Year | May/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R11/Last Academic Year | Jun/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R12/Last Academic Year | Jul/Last Academic Year | 495                    | 55                          | 0                         | Learning                     |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 500                       | First16To18EmployerIncentive |
        | R04/Last Academic Year | Nov/Last Academic Year | 0                      | 0                           | 500                       | First16To18ProviderIncentive |


    But the Provider now changes the Learner details as follows
		| Priority | Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 1        | 06/Aug/Last Academic Year | 12 months        | 8250                 | 06/Aug/Last Academic Year           |                        |                                       | 12 months       | completed         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |

	And price details as follows
    | Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type |
    | pe-1             | 8250                 | 06/Aug/Last Academic Year           |                        |                                       | 0                       |                                        | 0                         |                                          | 90%                         | Act2          |

	When the amended ILR file is re-submitted for the learners in collection period R01/CurrentAcademic Year
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | Second16To18EmployerIncentive | Second16To18ProviderIncentive | Price Episode Identifier |
		| Aug/Current Academic Year | 0            | 1650       | 0         | 500                           | 500                           | pe-1                     |
		| Sep/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | pe-1                     |
		| Oct/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | pe-1                     |
		| Nov/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | pe-1                     |
		| Dec/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | pe-1                     |
		| Jan/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | pe-1                     |
		| Feb/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | pe-1                     |
		| Mar/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | pe-1                     |
		| Apr/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | pe-1                     |
		| May/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | pe-1                     |
		| Jun/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | pe-1                     |
		| Jul/Current Academic Year | 0            | 0          | 0         | 0                             | 0                             | pe-1                     |

    And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | Second16To18EmployerIncentive | Second16To18ProviderIncentive |
		| R01/Current Academic Year | Aug/Current Academic Year | 0            | 1650       | 0         | 500                           | 500                           |
			
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully Funded Payments | Transaction Type              |
		| R01/Current Academic Year | Aug/Current Academic Year | 1485                   | 165                         | 0                         | Completion                    |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500                       | Second16To18EmployerIncentive |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500                       | Second16To18ProviderIncentive |

	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully Funded Payments | Transaction Type              |
		| R01/Current Academic Year | Aug/Current Academic Year | 1485                   | 165                         | 0                         | Completion                    |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500                       | Second16To18EmployerIncentive |
		| R01/Current Academic Year | Aug/Current Academic Year | 0                      | 0                           | 500                       | Second16To18ProviderIncentive |
