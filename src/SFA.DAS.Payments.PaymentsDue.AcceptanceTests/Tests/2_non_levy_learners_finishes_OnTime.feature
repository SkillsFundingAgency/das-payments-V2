Feature: Non-Levy learner - Basic Day - 2 learners

@NonDas_BasicDay
@MultipleLearners
Background:
	Given the current collection period is R03
	And the SFA contribution percentage is 90%
	And planned course duration is 12 months
	And learner L1 is undertaking a training with a training provider TP1
	And learner L2 is undertaking a training with a training provider TP2
	And the following course information for Learner L1:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 15000                | completed	    |
	And the following course information for Learner L2:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 7500                 | completed	    |

And the following contract type 2 On Programme earnings are provided in the latest ILR for the current academic year:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| L1		| p2                     | 1				| Learning (TT1)  | 1000   |
	| L1		| p2                     | 2				| Completion (TT2)| 3000   |
	| L2		| p2                     | 1				| Learning (TT1)  | 500    |
	| L2		| p2                     | 2				| Completion (TT2)| 1500   |

Scenario: 2_non_levy_learner_finishes_OnTime
	When an earnings event is received
	Then the payments due component will generate the following contract type 2 payments due:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| L1		| p2                     | 1				| Learning (TT1)  | 1000   |
	| L1		| p2                     | 2				| Completion (TT2)| 3000   |
	| L2		| p2                     | 1				| Learning (TT1)  | 500    |
	| L2		| p2                     | 2				| Completion (TT2)| 1500   |