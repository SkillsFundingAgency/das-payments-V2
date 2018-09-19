Feature: R06 - missing submission and historical payments after R02

Background:
	Given the current processing period is 6

	And a learner with LearnRefNumber learnref1 and Uln 10000 undertaking training with training provider 10000

	And the SFA contribution percentage is "90%"

	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | LearningStartDate | LearningPlannedEndDate | LearningActualEndDate | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 06/08/2017        | 20/08/2018             |                       | continuing       |

	And the following contract type 2 On Programme earnings for periods 1-12 are provided in the latest ILR for the academic year 1718:
	| PriceEpisodeIdentifier | EpisodeStartDate | EpisodeEffectiveTNPStartDate | TotalNegotiatedPrice | TransactionType | Amount	|
	| p1                     | 06/08/2017       | 06/08/2017                   | 9000                 | Learning (TT1)  | 600       |

@Non-DAS
@MissingSubmission
@Learning (TT1)

Scenario Outline: Contract Type 2 On Programme Learning payments

	When an earnings event is received

	Then the payments due component will generate the following contract type 2 Learning (TT1) payments due:
	| LearnRefNumber | Ukprn | PriceEpisodeIdentifier | Period | ULN   | TransactionType    | Amount   |
	| learnref1      | 10000 | p1                     | 1      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 2      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 3      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 4      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 5      | 10000 | <transaction_type> | <amount> |
	| learnref1      | 10000 | p1                     | 6      | 10000 | <transaction_type> | <amount> |
	
	Examples: 
	| transaction_type | amount |
	| Learning (TT1)   | 600    |

Scenario: Contract Type 2 no On Programme Completion payment

	When an earnings event is received

	Then the payments due component will not generate any contract type 2 Completion (TT2) payments due

Scenario: Contract Type 2 no On Programme Balancing payment

	When an earnings event is received

	Then the payments due component will not generate any contract type 2 Balancing (TT3) payments due