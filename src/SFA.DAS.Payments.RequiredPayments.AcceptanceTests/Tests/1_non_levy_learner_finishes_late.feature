Feature: Provider earnings and payments where learner completes later than planned
Background:
	Given a learner is undertaking a training with a training provider
	And the SFA contribution percentage is 90%

@NonDas_BasicDay
@finishes_late

Scenario: A non-DAS learner, learner finishes late
	Given the current collection period is R05

	And the payments due component generates the following contract type 2 payments due:	
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 1				| Learning (TT1)	| 1000		|
	| p2                     | 5				| Completion (TT2)	| 3000		|

	And the following historical contract type 2 payments exist:
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

	When a payments due event is received
	Then the required payments component will generate the following contract type 2 payable earnings:
	| PriceEpisodeIdentifier | Delivery Period	| TransactionType   | Amount	|
	| p2                     | 5				| Completion (TT2)	| 3000		|

@NonDas_BasicDay
@finishes_late
@withdrawal

Scenario: A non-DAS learner, learner withdraws after planned end date
	Given the current collection period is R05

	And the payments due component generated no contract type 2 payments due

	And the following historical contract type 2 payments exist:
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

	When a payments due event is received
	Then the required payments component will not generate any contract type 2 payable earnings