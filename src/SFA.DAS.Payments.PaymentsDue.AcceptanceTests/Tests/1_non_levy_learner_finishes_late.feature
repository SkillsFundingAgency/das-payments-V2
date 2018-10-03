Feature: Provider earnings and payments where learner completes later than planned
Background:
	Given a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%

@NonDas_BasicDay
@finishes_late

Scenario: A non-DAS learner, learner finishes late
	Given the current collection period is R05
	And planned course duration is 12 months
	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 15000                | completed	    |

	#This needs reviewing - will the previous year earnings are required here?
	#And the following contract type 2 On Programme earnings are provided in the latest ILR for the current-1 academic year:
	#| PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	#| p1                     | 2				| Learning (TT1)  | 1000   |
	#| p1                     | 3				| Learning (TT1)  | 1000   |
	#| p1                     | 4				| Learning (TT1)  | 1000   |
	#| p1                     | 5				| Learning (TT1)  | 1000   |
	#| p1                     | 6				| Learning (TT1)  | 1000   |
	#| p1                     | 7				| Learning (TT1)  | 1000   |
	#| p1                     | 8				| Learning (TT1)  | 1000   |
	#| p1                     | 9				| Learning (TT1)  | 1000   |
	#| p1                     | 10				| Learning (TT1)  | 1000   |
	#| p1                     | 11				| Learning (TT1)  | 1000   |
	#| p1                     | 12				| Learning (TT1)  | 1000   |

	And the following contract type 2 On Programme earnings are provided in the latest ILR for the current academic year:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| p2                     | 1				| Learning (TT1)  | 1000   |
	| p2                     | 5				| Completion (TT2)| 3000   |

	When an earnings event is received
	Then the payments due component will generate the following contract type 2 payments due:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	#| p1                     | 2				| Learning (TT1)	| 1000		|
	#| p1                     | 3				| Learning (TT1)	| 1000		|
	#| p1                     | 4				| Learning (TT1)	| 1000		|
	#| p1                     | 5				| Learning (TT1)	| 1000		|
	#| p1                     | 6				| Learning (TT1)	| 1000		|
	#| p1                     | 7				| Learning (TT1)	| 1000		|
	#| p1                     | 8				| Learning (TT1)	| 1000		|
	#| p1                     | 9				| Learning (TT1)	| 1000		|
	#| p1                     | 10				| Learning (TT1)	| 1000		|
	#| p1                     | 11				| Learning (TT1)	| 1000		|
	#| p1                     | 12				| Learning (TT1)	| 1000		|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 5				| Completion (TT2)	| 3000		|


@NonDas_BasicDay
@finishes_late
@withdrawal

Scenario: A non-DAS learner, learner withdraws after planned end date
	Given the current collection period is R05
	And planned course duration is 12 months
	And the following course information:
	| AimSeqNumber | ProgrammeType | FrameworkCode | PathwayCode | StandardCode | FundingLineType                                                       | LearnAimRef | TotalNegotiatedPrice | CompletionStatus |
	| 1            | 2             | 403           | 1           |              | 16-18 Apprenticeship (From May 2017) Non-Levy Contract (non-procured) | ZPROG001    | 15000                | withdrawn	    |

	And the following contract type 2 On Programme earnings are provided in the latest ILR for the current academic year:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	#| p1                     | 2				| Learning (TT1)	| 1000		|
	#| p1                     | 3				| Learning (TT1)	| 1000		|
	#| p1                     | 4				| Learning (TT1)	| 1000		|
	#| p1                     | 5				| Learning (TT1)	| 1000		|
	#| p1                     | 6				| Learning (TT1)	| 1000		|
	#| p1                     | 7				| Learning (TT1)	| 1000		|
	#| p1                     | 8				| Learning (TT1)	| 1000		|
	#| p1                     | 9				| Learning (TT1)	| 1000		|
	#| p1                     | 10				| Learning (TT1)	| 1000		|
	#| p1                     | 11				| Learning (TT1)	| 1000		|
	#| p1                     | 12				| Learning (TT1)	| 1000		|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 5				| Completion (TT2)	| 0			|

	When an earnings event is received
	Then the payments due component will generate the following contract type 2 payments due:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	#| p1                     | 2				| Learning (TT1)	| 1000		|
	#| p1                     | 3				| Learning (TT1)	| 1000		|
	#| p1                     | 4				| Learning (TT1)	| 1000		|
	#| p1                     | 5				| Learning (TT1)	| 1000		|
	#| p1                     | 6				| Learning (TT1)	| 1000		|
	#| p1                     | 7				| Learning (TT1)	| 1000		|
	#| p1                     | 8				| Learning (TT1)	| 1000		|
	#| p1                     | 9				| Learning (TT1)	| 1000		|
	#| p1                     | 10				| Learning (TT1)	| 1000		|
	#| p1                     | 11				| Learning (TT1)	| 1000		|
	#| p1                     | 12				| Learning (TT1)	| 1000		|
	| p2                     | 1				| Learning (TT1)	| 1000		|