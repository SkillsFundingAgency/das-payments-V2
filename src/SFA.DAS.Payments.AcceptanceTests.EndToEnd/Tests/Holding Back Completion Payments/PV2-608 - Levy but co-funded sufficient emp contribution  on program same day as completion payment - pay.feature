﻿@ignore
Feature: Holding back completion payments - PV2-608
	As a provider,
	I want a levy learner with co-funding, where the employer has paid their 10% co-investment for the on-program element, but has not yet paid the employer completion payment element, and the final on program payment is the same day as the completion payment
	So that I am accurately paid the completion payment by SFA

Scenario Outline: Levy learner but co-funded, sufficient employer contribution, on program payment same day as completion payment - pay completion PV2-608
	Given the employer levy account balance in collection period <Collection_Period> is 0
	
	And the following commitments exist
        | start date                | end date                     | agreed price | status |
        | 01/Jun/Last Academic Year | 01/Jun/Current Academic Year | 9000         | active |
	
	And the provider previously submitted the following learner details
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage |
		| 01/Jun/Last Academic Year | 12 months        | 9000                 | 06/Jun/Last Academic Year           | 0                      | 06/Jun/Last Academic Year             |                 | continuing        | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         |
	
	And the following earnings had been generated for the learner
        | Delivery Period        | On-Programme | Completion | Balancing |
        | Aug/Last Academic Year | 0            | 0          | 0         |
        | Sep/Last Academic Year | 0            | 0          | 0         |
        | Oct/Last Academic Year | 0            | 0          | 0         |
        | Nov/Last Academic Year | 0            | 0          | 0         |
        | Dec/Last Academic Year | 0            | 0          | 0         |
        | Jan/Last Academic Year | 0            | 0          | 0         |
        | Feb/Last Academic Year | 0            | 0          | 0         |
        | Mar/Last Academic Year | 0            | 0          | 0         |
        | Apr/Last Academic Year | 0            | 0          | 0         |
        | May/Last Academic Year | 0            | 0          | 0         |
        | Jun/Last Academic Year | 600          | 0          | 0         |
        | Jul/Last Academic Year | 600          | 0          | 0         |
    
	And the following provider payments had been generated 
        | Collection Period      | Delivery Period        | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R11/Last Academic Year | Jun/Last Academic Year | 540                    | 60                          | 0             | Learning         |
        | R12/Last Academic Year | Jul/Last Academic Year | 540                    | 60                          | 0             | Learning         |	
	
	# New field - Employer Contribution
    But the Provider now changes the Learner details as follows
		| Start Date                | Planned Duration | Total Training Price | Total Training Price Effective Date | Total Assessment Price | Total Assessment Price Effective Date | Actual Duration | Completion Status | Contract Type | Aim Sequence Number | Aim Reference | Framework Code | Pathway Code | Programme Type | Funding Line Type                                  | SFA Contribution Percentage | Employer Contribution |
		| 01/Jun/Last Academic Year | 12 months        | 9000                 | 01/Jun/Last Academic Year           | 0                      |                                       | 12 months       | completed         | Act1          | 1                   | ZPROG001      | 593            | 1            | 20             | 16-18 Apprenticeship (From May 2017) Levy Contract | 90%                         | 720                   |
	
	When the amended ILR file is re-submitted for the learners in collection period <Collection_Period>
	
	Then the following learner earnings should be generated
		| Delivery Period           | On-Programme | Completion | Balancing |
		| Aug/Current Academic Year | 600          | 0          | 0         |
		| Sep/Current Academic Year | 600          | 0          | 0         |
		| Oct/Current Academic Year | 600          | 0          | 0         |
		| Nov/Current Academic Year | 600          | 0          | 0         |
		| Dec/Current Academic Year | 600          | 0          | 0         |
		| Jan/Current Academic Year | 600          | 0          | 0         |
		| Feb/Current Academic Year | 600          | 0          | 0         |
		| Mar/Current Academic Year | 600          | 0          | 0         |
		| Apr/Current Academic Year | 600          | 0          | 0         |
		| May/Current Academic Year | 600          | 1800       | 0         |
		| Jun/Current Academic Year | 0            | 0          | 0         |
		| Jul/Current Academic Year | 0            | 0          | 0         |
    
	And at month end only the following payments will be calculated
        | Collection Period         | Delivery Period           | On-Programme | Completion | Balancing |
        | R01/Current Academic Year | Aug/Current Academic Year | 600          | 0          | 0         |
        | R02/Current Academic Year | Sep/Current Academic Year | 600          | 0          | 0         |
        | R03/Current Academic Year | Oct/Current Academic Year | 600          | 0          | 0         |
        | R04/Current Academic Year | Nov/Current Academic Year | 600          | 0          | 0         |
        | R05/Current Academic Year | Dec/Current Academic Year | 600          | 0          | 0         |
        | R06/Current Academic Year | Jan/Current Academic Year | 600          | 0          | 0         |
        | R07/Current Academic Year | Feb/Current Academic Year | 600          | 0          | 0         |
        | R08/Current Academic Year | Mar/Current Academic Year | 600          | 0          | 0         |
        | R09/Current Academic Year | Apr/Current Academic Year | 600          | 0          | 0         |
        | R10/Current Academic Year | May/Current Academic Year | 600          | 1800       | 0         |
	
	And only the following provider payments will be recorded
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 1620                   | 180                         | 0             | Completion       |
	
	And only the following provider payments will be generated
        | Collection Period         | Delivery Period           | SFA Co-Funded Payments | Employer Co-Funded Payments | Levy Payments | Transaction Type |
        | R01/Current Academic Year | Aug/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R02/Current Academic Year | Sep/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R03/Current Academic Year | Oct/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R04/Current Academic Year | Nov/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R05/Current Academic Year | Dec/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R06/Current Academic Year | Jan/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R07/Current Academic Year | Feb/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R08/Current Academic Year | Mar/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R09/Current Academic Year | Apr/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 540                    | 60                          | 0             | Learning         |
        | R10/Current Academic Year | May/Current Academic Year | 1620                   | 180                         | 0             | Completion       |

Examples: 
        | Collection_Period         |
        | R01/Current Academic Year |
        | R02/Current Academic Year |
        | R03/Current Academic Year |
        | R04/Current Academic Year |
        | R05/Current Academic Year |
        | R06/Current Academic Year |
        | R07/Current Academic Year |
        | R08/Current Academic Year |
        | R09/Current Academic Year |
        | R10/Current Academic Year |











#Feature: Holding back completion payments
# 
#Scenario: AC1 - 1 learner, levy, co-funding has been used, final on program payment is same day as completion payment, provider data shows enough employer contribution – pay completion
#
#	Given the levy balance is 0 for all months
#	
#	And the apprenticeship funding band maximum is 9000
#
#	And the following commitment exist:
#
#        | ULN       | start date | end date   | agreed price | status |
#        | learner a | 01/06/2018 | 01/06/2019 | 9000         | active |
#
#    When an ILR file is submitted for academic year 1718 in period R11 with the following data:
#
#        | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | 
#        | learner a | programme only DAS | 9000         | 06/06/2018 | 08/06/2019       | 	              | continuing        |
#		
#	And an ILR file is submitted for academic year 1819 in period R01 with the following data:
#
#        | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | 
#        | learner a | programme only DAS | 9000         | 06/06/2018 | 08/06/2019       | 	              | continuing        |
#
#	And an ILR file is submitted for academic year 1819 in period R11 with the following data:
#
#        | ULN       | learner type       | agreed price | start date | planned end date | actual end date | completion status | employer contributions |
#        | learner a | programme only DAS | 9000         | 06/06/2018 | 08/06/2019       | 30/05/2019      | completed         | 720					   |
#
#    Then the provider earnings and payments break down as follows:
#
#        | Type                                    | 06/18 | 07/18 | 08/18 | ... | 04/18 | 05/19 | 06/19 |
#        | Provider Earned Total                   | 600   | 600   | 600   | ... | 600   | 2400  | 0     | (Completion Payment £1800 + Final Program Payment £600) in May
#	| Provider Earned from SFA                | 540   | 540   | 540   | ... | 540   | 2160  | 0     | (Completion Payment £1620 + Final Program Payment £540) in May 
#	| Provider Earned from Employer           | 60    | 60    | 60    | ... | 60    | 240   | 0     | (Completion Payment £180 + Final Program Payment £60) in May
#        | Provider Paid by SFA                    | 0     | 540   | 540   | ... | 540   | 540   | 2160  | 
#	| Payment due from Employer               | 0     | 60    | 60    | ... | 60    | 60    | 240   |
#        | Levy account debited                    | 0     | 0     | 0     | ... | 0     | 0     | 0     | 
#        | SFA Levy employer budget                | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 540   | 540   | 540   | ... | 540   | 2160  | 0     |
#        | SFA Levy additional payments budget     | 0     | 0     | 0     | ... | 0     | 0     | 0	    |
#	| SFA non-Levy co-funding budget          | 0     | 0     | 0     | ... | 0     | 0     | 0     | 
#	| SFA non-Levy additional payments budget | 0     | 0     | 0     | ... | 0     | 0     | 0     |
#
#    And the transaction types for the payments are:
#
#	| Payment type                            | 07/18 | 08/18 | 09/18 | ... | 05/19 | 06/19 |
#        | On-program                              | 540   | 540   | 540   | ... | 540   | 540   |
#        | Completion                              | 0     | 0     | 0     | ... | 0     | 1620  |
#        | Balancing                               | 0     | 0     | 0     | ... | 0     | 0     |
#
#Maths.
#Price x 0.20 = £7,200
#£7,200 x 0.90 = £720 = Employer Contribution
#£720/12 = £60
#
#Completion payment workings:
#£9000 x 0.20 = £1,800
#£1800 x 0.90 (for co-funded) = £1620 = SFA, & £180 Employer contribution.
#
#We expect the employer contributions to total 720 in order for the completion payment to be released to the training provider.






