#In-progress - Check if this is a valid scenarion and review the scenario
Feature: R04 - No change in duration but start and end months changed after few months
#Start date changed to original start date - 1, end date month also changed to original planned month - 1


Background:
	Given the current processing period is 4

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 20/08/2018             |                       | continuing       |

	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 |
	| p2                     | 06/08/2017       | 06/08/2017                   | 9000                 | 600        |

@Non-DAS
@Historical_Payments
@Start_Date_Earlier

Scenario Outline: Contract Type 2 On programme payments

	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1       | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1       | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p1                     | 1      | 10000 | <transaction_type> | -<amount> |
	| learnref1       | 10000 | p1                     | 2      | 10000 | <transaction_type> | -<amount> |
	| learnref1       | 10000 | p2                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p2                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p2                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref1       | 10000 | p2                     | 4      | 10000 | <transaction_type> | <amount> |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 600    |