Feature: Non-Levy - Basic Day - 2 learners - Both finishes on time
Background:
	Given the current collection period is R02
	And the SFA contribution percentage is 90%
	And following learners are undertaking training with a training provider
	| LearnerId | 
	| L1		|
	| L2		|

	And the payments due component generates the following contract type 2 payments due:	
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| L1		| p2                     | 1				| Learning (TT1)  | 1000   |
	| L1		| p2                     | 2				| Completion (TT2)| 3000   |
	| L2		| p2                     | 1				| Learning (TT1)  | 800    |
	| L2		| p2                     | 2				| Completion (TT2)| 2400   |

@NonDas_BasicDay
@OnTime
Scenario: 2_non_levy_learner_finishes_OnTime
	Given the following historical contract type 2 payments exist:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 1				| Learning (TT1)	| 1000		|
	| L2		| p2                     | 1				| Learning (TT1)	| 800		|

	When a payments due event is received
	Then the required payments component will generate the following contract type 2 payable earnings:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 2 				| Completion (TT2)	| 3000		|
	| L2		| p2                     | 2 				| Completion (TT2)	| 2400		|

@NonDas_BasicDay
@OnTime
@NoHistory

Scenario: 2_non_levy_learner_finishes_OnTime - No history
	When a payments due event is received
	Then the required payments component will generate the following contract type 2 payable earnings:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 1 				| Learning (TT1)	| 1000		|
	| L1		| p2                     | 2 				| Completion (TT2)	| 3000		|
	| L2		| p2                     | 1				| Learning (TT1)	| 800		|
	| L2		| p2                     | 2				| Completion (TT2)	| 2400		|


@NonDas_BasicDay
@OnTime
@PartialHistory
Scenario: 2_non_levy_learner_finishes_OnTime - 1 learner has history
	Given the following historical contract type 2 payments exist:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 1				| Learning (TT1)	| 1000		|

	When a payments due event is received
	Then the required payments component will generate the following contract type 2 payable earnings:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L2		| p2                     | 1				| Learning (TT1)	| 800		|
	| L1		| p2                     | 2 				| Completion (TT2)	| 3000		|
	| L2		| p2                     | 2 				| Completion (TT2)	| 2400		|