Feature: R01 - First payment, No previous payments

Background:
	Given the current processing period is 1

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 20/08/2018             |                       | continuing       |

	And the following contract type 2 on programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | Learning_1 | SfaContributionPercentage |
	| p1                     | 06/08/2017       | 06/08/2017                   | 9000                 | 600        | 0.90000                   |

@Non-DAS
@Learning_1
@FirstSubmission

Scenario Outline: Contract Type 2 On programme payments

	When an earnings event is received

	Then the payments due component will generate the following contract type 2 transaction type Learning_1 payments due:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   | SfaContributionPercentage |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> | 0.90000                   |
	
	Examples: 
	| transaction_type | amount |
	| Learning_1       | 600    |

Scenario: Contract Type 2 no completion payment

	When an earnings event is received

	Then the payments due component will not generate any contract type 2 transaction type Completion_2 payments due

Scenario: Contract Type 2 no balancing payment

	When an earnings event is received

	Then the payments due component will not generate any contract type 2 transaction type Balancing_3 payments due