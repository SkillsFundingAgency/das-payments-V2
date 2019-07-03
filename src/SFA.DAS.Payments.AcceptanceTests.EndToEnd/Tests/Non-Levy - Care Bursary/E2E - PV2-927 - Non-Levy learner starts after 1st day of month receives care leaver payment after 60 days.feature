﻿

#Feature: Care Leaver Bursary
#
#Scenario: Non Levy learner, starts learning after first day of month, care leaver payment made after 60 days 
#
#	Given the learner is programme only Non Levy
#	And the apprenticeship funding band maximum is 9000
#
#	When an ILR file is submitted with the following data:
#        | ULN       | learner type                 | total price | start date | planned end date | actual end date | completion status | LearnDelFAM |
#        | learner a | programme only non-DAS       | 7500        | 03/08/2018 | 08/08/2019       |                 | continuing        | EEF4        |
#	
#	Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/18 | 09/18 | 10/18 | 11/18 | 12/18 | 
#        | Provider Earned total                   | 500   | 500   | 1500  | 500   | 500   | 
#        | Provider Earned from SFA                | 450   | 450   | 1450  | 450   | 450   | 
#        | Provider Earned from Employer           | 50    | 50    | 50    | 50    | 50    | 
#        | Provider Paid by SFA                    | 0     | 450   | 450   | 1450  | 450   | 
#        | Payment due from Employer               | 0     | 50    | 50    | 50    | 50    | 
#        | Levy account debited                    | 0     | 0     | 0     | 0     | 0     | 
#        | SFA Levy employer budget                | 0     | 0     | 0     | 0     | 0     | 
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | 
#        | SFA non-Levy co-funding budget          | 450   | 450   | 450   | 450   | 450   | 
#        | SFA non-Levy additional payments budget | 0     | 0     | 1000  | 0     | 0     |
#
#	And the transaction types for the payments are:
#        | Payment type                   		  | 09/18 | 10/18 | 11/18 | 12/18 |  
#        | On-program                     		  | 450   | 450   | 450   | 450   |  
#        | Completion                     		  | 0     | 0     | 0     | 0     |  
#        | Balancing                      		  | 0     | 0     | 0     | 0     |  
#        | Employer 16-18 incentive       		  | 0     | 0     | 0     | 0     |  
#        | Provider 16-18 incentive       		  | 0     | 0     | 0     | 0     |  
#        | Framework uplift on-program    		  | 0     | 0     | 0     | 0     |  
#        | Framework uplift completion    		  | 0     | 0     | 0     | 0     |  
#        | Framework uplift balancing     		  | 0     | 0     | 0     | 0     |  
#        | Provider disadvantage uplift   		  | 0     | 0     | 0     | 0     |
#        | Care leaver apprentice payment 		  | 0     | 0     | 1000  | 0     |
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

Feature: Non-Levy learner starts after 1st day of month receives care leaver payment after 60 days - PV2-927
		As a Provider,
		I want a Non Levy learner, where the learner is a care leaver and starts learning after first day of the month, and receives a care leaver payment after 60 days
		So that I am paid the correct apprenticeship funding by SFA - PV2-927

Scenario Outline: Non-Levy learner starts after 1st day of month receives care leaver payment after 60 days - PV2-927
	Given the provider is providing training for the following learners
		| Start Date                   | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                                     | SFA Contribution Percentage |
		| 03/Aug/Current Academic Year | 12 months        | 7500                 | 03/Aug/Current Academic Year        | 0                      | 03/Aug/Current Academic Year          |                 | continuing        | Act2          | 1                   | ZPROG001      | 593            | 1            | 20             | 19-24 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | 90%                         |	
	And price details as follows
		| Price Episode Id | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Residual Training Price | Residual Training Price Effective Date | Residual Assessment Price | Residual Assessment Price Effective Date | SFA Contribution Percentage | Contract Type | Aim Sequence Number |
		| pe-1             | 7500                 | 03/Aug/Current Academic Year        | 0                      | 03/Aug/Current Academic Year          | 0                       |                                        | 0                         |                                          | 90%                         | Act2          | 1                   |
	When the ILR file is submitted for the learners for collection period <Collection_Period>
	# New column - CareLeaverApprenticePayment
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing | CareLeaverApprenticePayment | Price Episode Identifier |
		| Aug/Current Academic Year | 500          | 0          | 0         | 0                           | pe-1                     |
		| Sep/Current Academic Year | 500          | 0          | 0         | 0                           | pe-1                     |
		| Oct/Current Academic Year | 500          | 0          | 0         | 1000                        | pe-1                     |
		| Nov/Current Academic Year | 500          | 0          | 0         | 0                           | pe-1                     |
		| Dec/Current Academic Year | 500          | 0          | 0         | 0                           | pe-1                     |
		| Jan/Current Academic Year | 500          | 0          | 0         | 0                           | pe-1                     |
		| Feb/Current Academic Year | 500          | 0          | 0         | 0                           | pe-1                     |
		| Mar/Current Academic Year | 500          | 0          | 0         | 0                           | pe-1                     |
		| Apr/Current Academic Year | 500          | 0          | 0         | 0                           | pe-1                     |
		| May/Current Academic Year | 500          | 0          | 0         | 0                           | pe-1                     |
		| Jun/Current Academic Year | 500          | 0          | 0         | 0                           | pe-1                     |
		| Jul/Current Academic Year | 500          | 0          | 0         | 0                           | pe-1                     |
	And only the following payments will be calculated
		| Collection Period         | Delivery Period           | On-Programme | Completion | Balancing | CareLeaverApprenticePayment |
		| R01/Current Academic Year | Aug/Current Academic Year | 500          | 0          | 0         | 0                           |
		| R02/Current Academic Year | Sep/Current Academic Year | 500          | 0          | 0         | 0                           |
		| R03/Current Academic Year | Oct/Current Academic Year | 500          | 0          | 0         | 1000                        |
	# New transaction type - CareLeaverApprenticePayment
	And only the following provider payments will be recorded
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type            |
		| R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | 0                         | Learning                    |
		| R02/Current Academic Year | Sep/Current Academic Year | 450                    | 50                          | 0                         | Learning                    |
		| R03/Current Academic Year | Oct/Current Academic Year | 450                    | 50                          | 0                         | Learning                    |
		| R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 1000                      | CareLeaverApprenticePayment |
	And at month end only the following provider payments will be generated
		| Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | SFA Fully-Funded Payments | Transaction Type            |
		| R01/Current Academic Year | Aug/Current Academic Year | 450                    | 50                          | 0                         | Learning                    |
		| R02/Current Academic Year | Sep/Current Academic Year | 450                    | 50                          | 0                         | Learning                    |
		| R03/Current Academic Year | Oct/Current Academic Year | 450                    | 50                          | 0                         | Learning                    |
		| R03/Current Academic Year | Oct/Current Academic Year | 0                      | 0                           | 1000                      | CareLeaverApprenticePayment |

Examples:
		| Collection_Period         |
		| R01/Current Academic Year |
		| R02/Current Academic Year |
		| R03/Current Academic Year |