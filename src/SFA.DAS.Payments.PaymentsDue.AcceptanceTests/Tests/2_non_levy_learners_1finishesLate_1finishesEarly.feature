Feature: Non-Levy - Basic Day - 2 learners - 1 finishes early and 1 finishes late

Background:
	Given the SFA contribution percentage is 90%
	And following learners are undertaking training with a training provider
	| LearnerId | 
	| L1		|
	| L2		|
	And the following course information for Learners:
	| LearnerId | AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
	| L1		| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 18750                | completed	    |
	| L2		| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 12000                | completed	    |

And the following contract type 2 On Programme earnings are provided:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| L1		| p2                     | 1				| Learning (TT1)  | 1000   |
	| L1		| p2                     | 2				| Completion (TT2)| 3750   |
	| L1		| p2                     | 2				| Balancing (TT3) | 3000   |
	| L2		| p2                     | 1				| Learning (TT1)  | 1000   |
	| L2		| p2                     | 2				| Learning (TT1)  | 0      |
	| L2		| p2                     | 3				| Learning (TT1)  | 0      |
	| L2		| p2                     | 4				| Learning (TT1)  | 0      |
	| L2		| p2                     | 5				| Completion (TT2)| 3000   |

@NonDas_BasicDay
@Finishes_Early

Scenario: 2_non_levy_learners_1finishes_Early
	Given the current collection period is R02
	And planned course duration is 15 months
	When an earnings event is received
	Then the payments due component will generate the following contract type 2 payments due:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| L1		| p2                     | 1				| Learning (TT1)  | 1000   |
	| L1		| p2                     | 2				| Completion (TT2)| 3750   |
	| L1		| p2                     | 2				| Balancing (TT3) | 3000   |

@NonDas_BasicDay
@Finishes_Late

Scenario: 2_non_levy_learners_1finishes_Late
	Given the current collection period is R05
	And planned course duration is 12 months
	When an earnings event is received
	Then the payments due component will generate the following contract type 2 payments due:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| L2		| p2                     | 1				| Learning (TT1)  | 1000   |
	| L2		| p2                     | 5				| Completion (TT2)| 3000   |
