#New name - Additional_Payments_Uplift_Balancing_Completion_Lower_TNP
#Old name - AdditionalPayments_581-AC02
Feature: Additional payments
		581-AC02-Non DAS learner finishes early, price lower than the funding band maximum, earns balancing and completion framework uplift payments. Assumes 15 month apprenticeship and learner completes after 12 months.

Background:
	Given the current processing period is 12

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref6      | 10000 | 10000 |

	And the following course information:
	| LearnRefNumber | Ukprn | ULN   | AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| learnref6      | 10000 | 10000 | 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 09/11/2018             | 09/08/2018            | Completed       |


	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice |
	| learnref6      | 10000 | p1                     | 06/08/2017       | 06/08/2017                   | 7500                 |

#	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
#	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
#	| learnref6      | 10000 | p1                     | 06/08/2017       | 06/08/2017                   | 7500                 | 400        |
#
#	And the following contract type 2 completion earning for period 12 are provided in the latest ILR for the academic year 1718:
#	| LearnRefNumber | Ukprn | Amount |
#	| learnref6      | 10000 | 1500   |  
#
#	#Check following with Dave
#	And the following contract type 2 balancing earning for period 12 are provided in the latest ILR for the academic year 1718:
#	| LearnRefNumber | Ukprn | Amount |
#	| learnref6      | 10000 | 1200   |  

@Non-DAS
@minimum_tests
@additional_payments
@completion
@balancing
@FinishingEarly
@Price_lower_than_FundingBand
#@Framework_uplift -- missing, will require funding band

Scenario Outline: Contract Type 2 On programme payments
	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref6      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 11     | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref6      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 11     | 10000 | <transaction_type> | <amount> |
	| learnref6      | 10000 | p1                     | 12     | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 400    |	
	
Scenario Outline: Contract Type 2 completion payment

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref6      | 10000 | p1                     | 12      | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Completion_2     | 1500   |
	
	
Scenario Outline: Contract Type 2 balancing payment

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref6      | 10000 | p1                     | 12      | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Balancing_3      | 1200   |	
	

#----------------------------------------------------------------------------------------------------------------------------------------
#/*  Payments V1 for reference
#
#@FrameworkUpliftsForNonDasFinishingEarly
#Scenario: 581-AC02-Non DAS learner finishes early, price lower than the funding band maximum, earns balancing and completion framework uplift payments. Assumes 15 month apprenticeship and learner completes after 12 months.
#    Given the apprenticeship funding band maximum is 9000
#    When an ILR file is submitted with the following data:
#		| ULN    | learner type                 | agreed price | start date | planned end date | actual end date | completion status | framework code | programme type | pathway code |
#        | 123456 | 16-18 programme only non-DAS | 7500         | 06/08/2017 | 09/11/2018       | 09/08/2018      | Completed         | 403            | 2              | 1            |
#    Then the provider earnings and payments break down as follows:
#        | Type                                    | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 06/18 | 07/18 | 08/18 | 09/18 |
#        | Provider Earned Total                   | 496   | 496   | 496   | 1496  | 496   | ... | 496   | 496   | 4348  | 0     |
#        | Provider Earned from SFA                | 456   | 456   | 456   | 1456  | 456   | ... | 456   | 456   | 4078  | 0     |
#        | Provider Earned from Employer           | 40    | 40    | 40    | 40    | 40    | ... | 40    | 40    | 270   | 0     |
#        | Provider Paid by SFA                    | 0     | 456   | 456   | 456   | 1456  | ... | 456   | 456   | 456   | 4078  |
#        | Payment due from Employer               | 0     | 40    | 40    | 40    | 40    | ... | 40    | 40    | 40    | 270   |
#        | Levy account debited                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | SFA Levy employer budget                | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#        | SFA Levy co-funding budget              | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 0     |
#		| SFA non-Levy co-funding budget          | 360   | 360   | 360   | 360   | 360   | ... | 360   | 360   | 2430  | 0     |
#        | SFA non-Levy additional payments budget | 96    | 96    | 96    | 1096  | 96    | ... | 96    | 96    | 1648  | 0     |
#    And the transaction types for the payments are:
#        | Payment type                 | 08/17 | 09/17 | 10/17 | 11/17 | 12/17 | ... | 06/18 | 07/18 | 08/18 | 09/18 |
#        | On-program                   | 0     | 360   | 360   | 360   | 360   | ... | 360   | 360   | 360   | 0     |
#        | Completion                   | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 1350  |
#        | Balancing                    | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 1080  |
#        | Employer 16-18 incentive     | 0     | 0     | 0     | 0     | 500   | ... | 0     | 0     | 0     | 500   |
#        | Provider 16-18 incentive     | 0     | 0     | 0     | 0     | 500   | ... | 0     | 0     | 0     | 500   |
#        | Framework uplift on-program  | 0     | 96    | 96    | 96    | 96    | ... | 96    | 96    | 96    | 0     |
#        | Framework uplift completion  | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 360   |
#        | Framework uplift balancing   | 0     | 0     | 0     | 0     | 0     | ... | 0     | 0     | 0     | 288   |
#        | Provider disadvantage uplift | 0     | 0     | 0     | 0     | 0     | ..  | 0     | 0     | 0     | 0     |
#*/        
#----------------------------------------------------------------------------------------------------------------------------------------