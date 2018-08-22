Feature: Clawback - Delay in completion due to change in planned end date in R12.
		 12 months apprenticeship changed to 15 months. 
		 
Background:
	
	Given the current processing period is 12

	And the following learners:
	| LearnRefNumber | Ukprn | ULN   |
	| learnref108    | 10000 | 10000 |

	And the following course information:
	| LearnRefNumber | Ukprn | ULN   | AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| learnref108    | 10000 | 10000 | 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 08/11/2018             |			            | continuing       |

	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| learnref108    | 10000 | p2                     | 06/08/2017       | 06/08/2017                   | 15000                | 800        |


@Non-DAS
@delayed_completion
@Changed_Planned_End_Date
@multiple_price_episodes
@clawback

Scenario Outline: Contract Type 2 On programme payments before change in planned end date

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref108    | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p1                     | 11     | 10000 | <transaction_type> | <amount> |
	
	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount    |
	| learnref108    | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount>  |
	| learnref108    | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount>  |
	| learnref108    | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount>  |
	| learnref108    | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount>  |
	| learnref108    | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount>  |
	| learnref108    | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount>  |
	| learnref108    | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount>  |
	| learnref108    | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount>  |
	| learnref108    | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount>  |
	| learnref108    | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount>  |
	| learnref108    | 10000 | p1                     | 11     | 10000 | <transaction_type> | <amount>  |
	| learnref108    | 10000 | p1                     | 1      | 10000 | <transaction_type> | -<amount> |
	| learnref108    | 10000 | p1                     | 2      | 10000 | <transaction_type> | -<amount> |
	| learnref108    | 10000 | p1                     | 3      | 10000 | <transaction_type> | -<amount> |
	| learnref108    | 10000 | p1                     | 4      | 10000 | <transaction_type> | -<amount> |
	| learnref108    | 10000 | p1                     | 5      | 10000 | <transaction_type> | -<amount> |
	| learnref108    | 10000 | p1                     | 6      | 10000 | <transaction_type> | -<amount> |
	| learnref108    | 10000 | p1                     | 7      | 10000 | <transaction_type> | -<amount> |
	| learnref108    | 10000 | p1                     | 8      | 10000 | <transaction_type> | -<amount> |
	| learnref108    | 10000 | p1                     | 9      | 10000 | <transaction_type> | -<amount> |
	| learnref108    | 10000 | p1                     | 10     | 10000 | <transaction_type> | -<amount> |
	| learnref108    | 10000 | p1                     | 11     | 10000 | <transaction_type> | -<amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 1000	|

@Non-DAS
@delayed_completion
@Changed_Planned_End_Date
@multiple_price_episodes
@clawback

Scenario Outline: Contract Type 2 On programme payments after change in planned end date
	
	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref108    | 10000 | p2                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p2                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p2                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p2                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p2                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p2                     | 6      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p2                     | 7      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p2                     | 8      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p2                     | 9      | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p2                     | 10     | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p2                     | 11     | 10000 | <transaction_type> | <amount> |
	| learnref108    | 10000 | p2                     | 12     | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Learning_1       | 800    |