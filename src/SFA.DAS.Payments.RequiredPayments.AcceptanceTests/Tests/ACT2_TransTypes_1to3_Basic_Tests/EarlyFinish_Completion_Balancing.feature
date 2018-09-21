Feature: Provider earnings and payments where learner completes earlier than planned (3 months early)

Background:

	Given the current processing period is 10

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000
	And the collection year is 1718
	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 01/09/2017        | 08/09/2018             | 08/06/2018            | Completed       |

	And the following contract type 2 on programme earnings for periods 1-9 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 | SfaContributionPercentage |
	| p1                     | 01/09/2017       | 01/09/2017                   | 15000                | 1000       | 0.90000                   |

	And the following contract type 2 on programme earnings for period 10 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Completion_2 | Balancing_3 | SfaContributionPercentage |
	| p1                     | 01/09/2017       | 01/09/2017                   | 15000                | 3000         | 3000        | 0.90000                   |

	
@Non-DAS
@Completion_2
@Balancing_3
@FinishedEarly

Scenario Outline: Contract Type 2 On programme payments
	And the following historical contract type 2 on programme payments exist:   
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> | 0.90000                   |

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 7      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 8      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	| learnref1      | 10000 | p1                     | 9      | 10000 | <transaction_type> | <amount> | 0.90000                   |

	Examples: 
	| transaction_type | amount |
	| Learning_1       | 1000    |	
	
Scenario Outline: Contract Type 2 completion payment

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 10     | 10000 | <transaction_type> | <amount> | 0.90000                   |
	
	Examples: 
	| transaction_type | amount |
	| Completion_2     | 3000   |
	
	
Scenario Outline: Contract Type 2 balancing payment

	When a TOBY is received

	Then the payments due component will generate the following contract type 2 payable earnings:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period  | ULN   | TransactionType    | Amount   | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 10      | 10000 | <transaction_type> | <amount> | 0.90000                   |

	Examples: 
	| transaction_type | amount |
	| Balancing_3      | 3000   |	
