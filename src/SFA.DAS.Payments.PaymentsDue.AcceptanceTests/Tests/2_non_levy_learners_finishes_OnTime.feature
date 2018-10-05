Feature: Non-Levy - Basic Day - 2 learners - Both finishes on time
Background:
	Given the current collection period is R02
	And the SFA contribution percentage is 90%
	And planned course duration is 12 months
	And following learners are undertaking training with a training provider
	| LearnerId | 
	| L1		|
	| L2		|
	And the following course information for Learners:
	| LearnerId | AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
	| L1		| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 15000                | completed	    |
	| L2		| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 7500                 | completed	    |

And the following contract type 2 On Programme earnings are provided:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| L1		| p2                     | 1				| Learning (TT1)  | 1000   |
	| L1		| p2                     | 2				| Completion (TT2)| 3000   |
	| L2		| p2                     | 1				| Learning (TT1)  | 800    |
	| L2		| p2                     | 2				| Completion (TT2)| 2400   |

@NonDas_BasicDay
@MultipleLearners

Scenario: 2_non_levy_learner_finishes_OnTime
	When an earnings event is received
	Then the payments due component will generate the following contract type 2 payments due:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| L1		| p2                     | 1				| Learning (TT1)  | 1000   |
	| L1		| p2                     | 2				| Completion (TT2)| 3000   |
	| L2		| p2                     | 1				| Learning (TT1)  | 800    |
	| L2		| p2                     | 2				| Completion (TT2)| 2400   |