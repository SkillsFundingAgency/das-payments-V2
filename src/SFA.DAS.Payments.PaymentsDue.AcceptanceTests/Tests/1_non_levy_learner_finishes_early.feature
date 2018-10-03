Feature: Provider earnings and payments where learner completes earlier than planned
Background:
	Given a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%

@NonDas_BasicDay
@finishes_early

Scenario: A non-DAS learner, learner finishes early
	Given the current collection period is R02
	And planned course duration is 15 months
	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 18750                | completed	    |

	And the following contract type 2 On Programme earnings are provided in the latest ILR for the current academic year:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| p2                     | 1				| Learning (TT1)  | 1000   |
	| p2                     | 2				| Completion (TT2)| 3750   |
	| p2                     | 2				| Balancing (TT3) | 3000   |

	When an earnings event is received
	Then the payments due component will generate the following contract type 2 payments due:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 2				| Completion (TT2)	| 3750		|
	| p2                     | 2				| Balancing (TT3)	| 3000		|


@withdrawal

Scenario: A non-DAS learner, learner withdraws after qualifying period
	Given the current collection period is R06
	And planned course duration is 12 months
	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 15000                | withdrawn	    |

	And the following contract type 2 On Programme earnings are provided in the latest ILR for the current academic year:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 2				| Learning (TT1)	| 1000		|
	| p1                     | 3				| Learning (TT1)	| 1000		|
	| p1                     | 4				| Learning (TT1)	| 1000		|
	| p1                     | 5				| Learning (TT1)	| 1000		|

	When an earnings event is received
	Then the payments due component will generate the following contract type 2 payments due:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p1                     | 2				| Learning (TT1)	| 1000		|
	| p1                     | 3				| Learning (TT1)	| 1000		|
	| p1                     | 4				| Learning (TT1)	| 1000		|
	| p1                     | 5				| Learning (TT1)	| 1000		|