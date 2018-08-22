#New name - Refund_price_change_retrospectively_from_beginning
#Old name - Refund_894-AC02

#Payment v1 test has incorrect refund for employer - Rohan to investigatge

Feature: Refunds - Provider earnings and payments where learner refund payments are due
		 894-AC02 - non DAS standard learner, payments made then price is changed retrospectively from beginning

Background:
	Given the current processing period is 3

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref12     | 10000 | 10000 |

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 04/08/2017        | 20/08/2018             |                       | continuing       |

	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p2                     | 06/08/2017       | 06/08/2017                   | 10                   | 0.6667     |

@Non-DAS
@minimum_tests
@Refunds
@price_reduced_retrospectively

Scenario Outline: Contract Type 2 On programme payments before price change

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref12     | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref12     | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	
	When a TOBY is received

	Then the payments due component will generate the following contract type 1 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount    |
	| learnref12     | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount>  |
	| learnref12     | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount>  |
	| learnref12     | 10000 | p1                     | 1      | 10000 | <transaction_type> | -<amount> |
	| learnref12     | 10000 | p1                     | 2      | 10000 | <transaction_type> | -<amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 750	|

@Non-DAS
@minimum_tests
@Refunds
@price_reduced_retrospectively

Scenario Outline: Contract Type 2 On programme payments after price change
	
	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref12     | 10000 | p2                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref12     | 10000 | p2                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref12     | 10000 | p2                     | 3      | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Learning_1       | 0.6667 |

#-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

#/* V1 - Original Test
#
#@Refunds
#Feature: Provider earnings and payments where learner refund payments are due
#
#@_Minimum_Acceptance_		
# Scenario:894-AC02 - non DAS standard learner, payments made then price is changed retrospectively from beginning
#	Given  the apprenticeship funding band maximum is 27000
#	And levy balance > agreed price for all months
#	
#	And following learning has been recorded for previous payments:
#		| ULN       | employer   | provider   | learner type           | start date | aim sequence number | completion status | programme type | Total training price 1 | Total training price 1 effective date | Total assessment price 1 | Total assessment price 1 effective date |
#		| learner a | employer 0 | provider A | programme only non-DAS | 04/08/2017 | 1                   | continuing        | 25             | 9000                   | 04/08/2017                            | 2250                     | 04/08/2017                              |
#
#	And the following earnings and payments have been made to the provider A for learner a:
#		| Type                           | 08/17 | 09/17 | 10/17 | 11/17 |
#		| Provider Earned Total          | 750   | 750   | 0     | 0     |
#		| Provider Earned from SFA       | 675   | 675   | 0     | 0     |
#		| Provider Earned from Employer  | 75    | 75    | 0     | 0     |
#		| Provider Paid by SFA           | 0     | 675   | 0     | 0     |
#		| Payment due from Employer      | 0     | 75    | 0     | 0     |
#		| Levy account debited           | 0     | 0     | 0     | 0     |
#		| SFA Levy employer budget       | 0     | 0     | 0     | 0     |
#		| SFA Levy co-funding budget     | 0     | 0     | 0     | 0     |
#		| SFA non-Levy co-funding budget | 675   | 675   | 0     | 0     |
#        
#    When an ILR file is submitted for the first time on 10/10/17 with the following data:
#        | ULN       | employer   | provider   | learner type           | start date | planned end date | agreed price | completion status | programme type | Total training price 1 | Total training price 1 effective date | Total assessment price 1 | Total assessment price 1 effective date |
#        | learner a | employer 0 | provider A | programme only non-DAS | 04/08/2017 | 20/08/2018       | 10            | continuing        | 25             | 8                      | 04/08/2017                            | 2                        | 04/08/2017                              |
#	
#    Then the provider earnings and payments break down as follows:
#        | Type                           | 08/17  | 09/17  | 10/17  | 11/17    | 12/17   |
#        | Provider Earned Total          | 0.6667 | 0.6667 | 0.6667 | 0.6667   | 0.6667  |
#        | Provider Earned from SFA       | 0.60   | 0.60   | 0.60   | 0.60     | 0.60    |
#        | Provider Earned from Employer  | 0.0667 | 0.0667 | 0.0667 | 0.0667   | 0.0667  |
#        | Provider Paid by SFA           | 0      | 675    | 675    | 0.60     | 0.60    |
#        | Refund taken by SFA            | 0      | 0      | 0      | -1348.80 | 0       |
#        | Payment due from Employer      | 0      | 75     | 75     | 0.0667   | 0.06667 |
#        | Refund due to employer         | 0      | 0      | 0      | -148.67  | 0       |
#        | Levy account debited           | 0      | 0      | 0      | 0        | 0       |
#        | Levy account credited          | 0      | 0      | 0      | 0        | 0       |
#        | SFA Levy employer budget       | 0      | 0      | 0      | 0        | 0       |
#        | SFA Levy co-funding budget     | 0      | 0      | 0      | 0        | 0       |
#        | SFA non-Levy co-funding budget | 0.60   | 0.60   | 0.60   | 0.60     | 0.60    |
#*/

#-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------