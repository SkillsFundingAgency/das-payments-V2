Feature: Two Non-Levy Learners Finishes On Time PV2-197
	Non-Levy - 2 learners - Both finishes on time

Background:
	Given the current collection period is R02
	And the payments are for the current collection year
	And the SFA contribution percentage is 90%
	And following learners are undertaking training with a training provider
	| LearnerId | 
	| L1		|
	| L2		|

	And the payments due component generates the following contract type 2 payments due:	
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType | Amount |
	| L1		| p2                     | 1				| Learning (TT1)  | 1000   |
	| L1		| p2                     | 2				| Completion (TT2)| 3000   |
	| L2		| p3                     | 1				| Learning (TT1)  | 800    |
	| L2		| p3                     | 2				| Completion (TT2)| 2400   |

@NonLevy_BasicDay
@OnTime
Scenario: Completion for both
	Given the following historical contract type 2 payments exist:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 1				| Learning (TT1)	| 1000		|
	| L2		| p3                     | 1				| Learning (TT1)	| 800		|

	When a payments due event is received
	Then the required payments component will only generate contract type 2 required payments
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 2 				| Completion (TT2)	| 3000		|
	| L2		| p3                     | 2 				| Completion (TT2)	| 2400		|

@NonLevy_BasicDay
@OnTime
@NoHistory

Scenario: Learning and Completion for both
	When a payments due event is received
	Then the required payments component will only generate contract type 2 required payments
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 1 				| Learning (TT1)	| 1000		|
	| L1		| p2                     | 2 				| Completion (TT2)	| 3000		|
	| L2		| p2                     | 1				| Learning (TT1)	| 800		|
	| L2		| p2                     | 2				| Completion (TT2)	| 2400		|


@NonLevy_BasicDay
@OnTime
@PartialHistory
Scenario: Learning for 1 and Completion for both - 1 learner has history
	Given the following historical contract type 2 payments exist:
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L1		| p2                     | 1				| Learning (TT1)	| 1000		|

	When a payments due event is received
	Then the required payments component will only generate contract type 2 required payments
	| LearnerId | PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| L2		| p2                     | 1				| Learning (TT1)	| 800		|
	| L1		| p2                     | 2 				| Completion (TT2)	| 3000		|
	| L2		| p2                     | 2 				| Completion (TT2)	| 2400		|