

#Feature: Care Leaver Bursary
#
#Scenario: Non Levy learner, receives care leaver payment, but R04 ILR states early withdrawal, refund applied 
#
#	Given the learner is programme only Non Levy
#	And the apprenticeship funding band maximum is 9000
#
#	When an ILR file is submitted with the following data in period R03:
#        | ULN       | learner type           | agreed price | start date | planned end date | actual end date | completion status | LearnDelFAM |
#        | learner a | programme only non-DAS | 7500         | 01/08/2018 | 08/08/2019       |                 | continuing        | EEF4        |		
#
#	Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/18 | 09/18 | 10/18 | 
#        | Provider Earned total                   | 500   | 1500  | 500   | 
#        | Provider Earned from SFA                | 450   | 1450  | 450   | 
#        | Provider Earned from Employer           | 50    | 50    | 50    | 
#        | Provider Paid by SFA                    | 0     | 450   | 1450  | 
#        | Payment due from Employer               | 0     | 50    | 50    | 
#        | Levy account debited                    | 0     | 0     | 0     | 
#        | SFA Levy employer budget                | 0     | 0     | 0     | 
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 
#        | SFA non-Levy co-funding budget          | 450   | 450   | 450   | 
#        | SFA non-Levy additional payments budget | 0     | 1000  | 0     |
#
#	And the transaction types for the payments are:
#        | Payment type                   		  | 09/18 | 10/18 |  
#        | On-program                     		  | 450   | 450   |  
#        | Completion                     		  | 0     | 0     |  
#        | Balancing                      		  | 0     | 0     |  
#        | Employer 16-18 incentive       		  | 0     | 0     |  
#        | Provider 16-18 incentive       		  | 0     | 0     |  
#        | Framework uplift on-program    		  | 0     | 0     |  
#        | Framework uplift completion    		  | 0     | 0     |  
#        | Framework uplift balancing     		  | 0     | 0     |  
#        | Provider disadvantage uplift   		  | 0     | 0     |
#        | Care leaver apprentice payment 		  | 0     | 1000  |
#	
#	
#	And an ILR file is submitted with the following data in period R04:
#        | ULN       | learner type           | agreed price | start date | planned end date | actual end date | completion status | LearnDelFAM |
#        | learner a | programme only non-DAS | 7500         | 01/08/2018 | 08/08/2019       | 05/09/2018      | withdrawn         | EEF4        |
#		
#	
#	Then the provider earnings and payments break down in R04 as follows:
#        | Type                                    | 08/18 | 09/18 | 10/18 | 11/18 | 
#        | Provider Earned total                   | 500   | 1500  | 500   | 0     | 
#        | Provider Earned from SFA                | 450   | 1450  | 450   | 0     | 
#        | Provider Earned from Employer           | 50    | 50    | 50    | 0     | 
#        | Provider Paid by SFA                    | 0     | 450   | 1450  | 450   |
#		| Refunded taken from SFA	          	  |	0     | 0     | 0     | -1900 |
#        | Payment due from Employer               | 0     | 50    | 50    | 50    | 
#		| Levy account debited                    | 0     | 0     | 0     | 0     | 
#		| SFA Levy employer budget                | 0     | 0     | 0     | 0     | 
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     |
#		| SFA Levy additional payments budget     | 0     | 0     | 0     | 0     |				
#        | SFA non-Levy co-funding budget          | 450   | 450   | 450   | 0     | 
#        | SFA non-Levy additional payments budget | 0     | 0     | 0     | 0     |
#
#	And the transaction types for the payments are:
#        | Payment type                   		  | 09/18 | 10/18 | 11/18 | 
#        | On-program                     		  | 450   | 450   | -450  |
#        | Completion                     		  | 0     | 0     | 0     | 
#        | Balancing                      		  | 0     | 0     | 0     | 
#        | Employer 16-18 incentive       		  | 0     | 0     | 0     | 
#        | Provider 16-18 incentive       		  | 0     | 0     | 0     | 
#        | Framework uplift on-program    		  | 0     | 0     | 0     | 
#        | Framework uplift completion    		  | 0     | 0     | 0     | 
#        | Framework uplift balancing     		  | 0     | 0     | 0     |  
#        | Provider disadvantage uplift   		  | 0     | 0     | 0     |
#        | Care leaver apprentice payment 		  | 0     | 1000  | -1000 |
#
#NOTES:                                                                      
## For new starts from 1 August 2018                                        
## Care leaver apprentice payment is triggered after 60 days from start date
## Payment is triggered using EEF code 4


# *************************************
# For DC Integration
# | LearnDelFAM |
# | EEF4        |
# *************************************

Feature: Non-Levy learner in co-funding receives care leaver payment but then withdraws resulted in refund - PV2-930
		As a Provider,
		I want a Non Levy learner, where the learner is a care leaver and receives a care leaver payment at 60 days, but ILR shows learner was withdrawn, refund applied
		So that I am not paid the care leavers bursary by SFA - PV2-930

Scenario Outline: Non-Levy learner in co-funding receives care leaver payment but then withdraws resulted in refund - PV2-930

	Given the provider previously submitted the following learner details
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 7500                 | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |

	# New column - CareLeaverApprenticePayment
    And the following earnings had been generated for the learner
		| Delivery Period           | On-Programme | Completion | Balancing | CareLeaverApprenticePayment |
		| Aug/Current Academic Year | 500          | 0          | 0         | 0                           |
		| Sep/Current Academic Year | 500          | 0          | 0         | 1000                        |
		| Oct/Current Academic Year | 500          | 0          | 0         | 0                           |
		| Nov/Current Academic Year | 500          | 0          | 0         | 0                           |
		| Dec/Current Academic Year | 500          | 0          | 0         | 0                           |
		| Jan/Current Academic Year | 500          | 0          | 0         | 0                           |
		| Feb/Current Academic Year | 500          | 0          | 0         | 0                           |
		| Mar/Current Academic Year | 500          | 0          | 0         | 0                           |
		| Apr/Current Academic Year | 500          | 0          | 0         | 0                           |
		| May/Current Academic Year | 500          | 0          | 0         | 0                           |
		| Jun/Current Academic Year | 500          | 0          | 0         | 0                           |
		| Jul/Current Academic Year | 500          | 0          | 0         | 0                           |

    And the following provider payments had been generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type            |
		| R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | 0                         | Learning                    |
		| R02/Current Academic Year | Sep/Current Academic Year | 450                    | 50                          | 0                         | Learning                    |
		| R02/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | 1000                      | CareLeaverApprenticePayment |


	But the Provider now changes the Learner details as follows
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 01/Aug/Current Academic Year | 12 months        | 7500                 | 01/Aug/Current Academic Year        | 0                      | 01/Aug/Current Academic Year          | 1 month         | withdrawn         | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |

	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>

	# New column - CareLeaverApprenticePayment
	# Refund
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | CareLeaverApprenticePayment |
		| Aug/Current Academic Year | 0            | 0          | 0         | 0                           |
		| Sep/Current Academic Year | 0            | 0          | 0         | 0                           |
		| Oct/Current Academic Year | 0            | 0          | 0         | 0                           |
		| Nov/Current Academic Year | 0            | 0          | 0         | 0                           |
		| Dec/Current Academic Year | 0            | 0          | 0         | 0                           |
		| Jan/Current Academic Year | 0            | 0          | 0         | 0                           |
		| Feb/Current Academic Year | 0            | 0          | 0         | 0                           |
		| Mar/Current Academic Year | 0            | 0          | 0         | 0                           |
		| Apr/Current Academic Year | 0            | 0          | 0         | 0                           |
		| May/Current Academic Year | 0            | 0          | 0         | 0                           |
		| Jun/Current Academic Year | 0            | 0          | 0         | 0                           |
		| Jul/Current Academic Year | 0            | 0          | 0         | 0                           |

	And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | CareLeaverApprenticePayment |
		| R03/Current Academic Year | Aug/Current Academic Year | -500         | 0          | 0         | 0                           |
		| R03/Current Academic Year | Sep/Current Academic Year | -500         | 0          | 0         | -1000                       |

	# New transaction type - CareLeaverApprenticePayment
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type            |
		| R03/Current Academic Year | Aug/Current Academic Year | -450                   | -50                         | 0                         | Learning                    |
		| R03/Current Academic Year | Sep/Current Academic Year | -450                   | -50                         | 0                         | Learning                    |
		| R03/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | -1000                     | CareLeaverApprenticePayment |

	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type            |
		| R03/Current Academic Year | Aug/Current Academic Year | -450                   | -50                         | 0                         | Learning                    |
		| R03/Current Academic Year | Sep/Current Academic Year | -450                   | -50                         | 0                         | Learning                    |
		| R03/Current Academic Year | Sep/Current Academic Year | 0                      | 0                           | -1000                     | CareLeaverApprenticePayment |

Examples:
		| Collection_Period         |
		| R03/Current Academic Year |
		| R04/Current Academic Year |